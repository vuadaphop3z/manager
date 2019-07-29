using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Manager.SharedLib.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.MsSqlStores;
using Manager.WebApp.Models;
using Manager.WebApp.Resources;
using System.Web;
using System.Net.Http;
using System.Collections.Generic;
using Manager.WebApp.Services;
using Newtonsoft.Json;

namespace Manager.WebApp.Controllers
{
    public class PlaceController : BaseAuthedController
    {
        private readonly IStorePlace _mainStore;
        private readonly ILog logger = LogProvider.For<PlaceController>();

        public PlaceController(IStorePlace mainStore)
        {
            _mainStore = mainStore;
        }

        //[AccessRoleChecker]
        public ActionResult Index(ManagePlaceModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.SearchExec = "Y";
                if (!ModelState.IsValid)
                {
                    ModelState.Clear();
                }
            }

            if (Request["Page"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
            }

            var filter = new IdentityPlace
            {
                Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                Status = model.Status == null ? -1 : (int)model.Status
            };

            try
            {
                model.SearchResults = _mainStore.GetAll(filter, currentPage, SystemSettings.DefaultPageSize);
                if (model.SearchResults != null && model.SearchResults.Count > 0)
                {
                    model.TotalCount = model.SearchResults[0].TotalCount;
                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get data because: " + ex.ToString());

                return View(model);
            }

            return View(model);
        }

        //[AccessRoleChecker]
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                //Begin db transaction
                var info = _mainStore.GetPlaceById(id);

                //Render to view model
                var editModel = RenderEditModel(info);

                return View(editModel);
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit Place request: " + ex.ToString());
            }

            return View(new PlaceEditModel());
        }

        [HttpPost]
        public ActionResult Edit(PlaceEditModel model)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage + x.Exception));
                this.AddNotification(messages, NotificationType.ERROR);
                return View(model);
            }

            try
            {
                //Upload file
                if(model.Files != null && model.Files[0] != null)
                {
                    //var apiResult = CdnServices.UploadPlaceCoverAsync(model).Result;
                    //if(apiResult != null)
                    //{
                    //    if(apiResult.Code == EnumCommonCode.Success)
                    //    {
                    //        var imagesList = JsonConvert.DeserializeObject<List<string>>(apiResult.Data.ToString());
                    //        if(imagesList != null && imagesList.Count > 0)
                    //        {
                    //            model.Cover = imagesList[0];
                    //        }
                    //    }
                    //}
                }                

                //Begin db transaction
                var PlaceInfo = ExtractEditFormData(model);

                var isSuccess = _mainStore.Update(PlaceInfo);

                //Clear cache
                //PlaceServices.ClearPlaceCache();

                if (isSuccess)
                {
                    this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit Place request: " + ex.ToString());

                return View(model);
            }

            return RedirectToAction("Edit/" + model.Id);
        }

        //Show popup confirm delete        
        //[AccessRoleChecker]

        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return PartialView("_PopupDelete", id);
        }

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete_Confirm(int id)
        //{
        //    var strError = string.Empty;
        //    if (id <= 0)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    try
        //    {
        //        _mainStore.Delete(id);
        //        //Clear cache
        //        PlaceServices.ClearPlaceCache();
        //    }
        //    catch (Exception ex)
        //    {
        //        strError = ManagerResource.LB_SYSTEM_BUSY;

        //        logger.Error("Failed to get Delete Place because: " + ex.ToString());

        //        return Json(new { success = false, message = strError });
        //    }

        //    return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
        //}

        #region Helpers

        private IdentityPlace ExtractCreateFormData(PlaceCreateModel formData)
        {
            var myIdetity = new IdentityPlace();
            //myIdetity.Name = formData.Name;
            //myIdetity.Code = formData.Code;
            //myIdetity.UrlFriendly = UrlFriendly.ConvertToUrlFriendly(formData.Name);
            //myIdetity.Icon = formData.Icon;
            //myIdetity.Status = Utils.ConvertToInt32(formData.Status);
            //myIdetity.CreatedBy = User.Identity.GetUserId();

            return myIdetity;
        }

        private IdentityPlace ExtractEditFormData(PlaceEditModel formData)
        {
            var myIdetity = new IdentityPlace();
            myIdetity.Id = formData.Id;
            myIdetity.GName = formData.Name;
            myIdetity.Cover = formData.Cover;
            myIdetity.Status = formData.Status;

            return myIdetity;
        }

        private PlaceEditModel RenderEditModel(IdentityPlace identity)
        {
            var editModel = new PlaceEditModel();

            editModel.Id = (int)identity.Id;
            editModel.Name = identity.GName;
            editModel.Cover = identity.Cover;
            editModel.Status = identity.Status;

            return editModel;
        }

        #endregion

    }
}