using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Manager.SharedLib.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.MsSqlStores;
using Manager.WebApp.Resources;
using Microsoft.AspNet.Identity;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using System.Collections.Generic;
using Manager.WebApp.Services;
using Newtonsoft.Json;

namespace Manager.WebApp.Controllers
{
    public class SlideController : BaseAuthedController
    {
        private readonly IStoreSlide _mainStore;
        private readonly ILog logger = LogProvider.For<SlideController>();

        public SlideController(IStoreSlide mainStore)
        {
            _mainStore = mainStore;
        }

        //[AccessRoleChecker]
        public ActionResult Index(ManageSlideModel model)
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

            var filter = new IdentitySlide
            {
                Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                Status = model.Status == null ? -1 : (int)model.Status
            };

            try
            {
                model.SearchResults = _mainStore.GetByPage(filter, currentPage, SystemSettings.DefaultPageSize);
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
        public ActionResult Create()
        {
            var createModel = new SlideCreateModel();
            return View(createModel);
        }

        [HttpPost]
        public ActionResult Create(SlideCreateModel model)
        {
            var newId = 0;
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
                //Begin db transaction
                var info = ExtractCreateFormData(model);

                newId = _mainStore.Insert(info);

                if (newId > 0)
                {
                    this.AddNotification(ManagerResource.LB_INSERT_SUCCESS, NotificationType.SUCCESS);
                }
                else
                {
                    this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Create Slide request: " + ex.ToString());

                return View(model);
            }

            return RedirectToAction("Edit/" + newId);
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
                var info = _mainStore.GetById(id);
                

                //Render to view model
                var editModel = RenderEditModel(info);

                return View(editModel);
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit Slide request: " + ex.ToString());
            }

            return View(new SlideEditModel());
        }

        [HttpPost]
        public ActionResult Edit(SlideEditModel model)
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
                //if (model.Files != null && model.Files[0] != null)
                //{
                //    var apiResult = CdnServices.UploadSlideCoverAsync(model).Result;
                //    if (apiResult != null)
                //    {
                //        if (apiResult.Code == EnumCommonCode.Success)
                //        {
                //            var imagesList = JsonConvert.DeserializeObject<List<string>>(apiResult.Data.ToString());
                //            if (imagesList != null && imagesList.Count > 0)
                //            {
                //                model.Cover = imagesList[0];
                //            }
                //        }
                //    }
                //}

                //Begin db transaction
                var info = ExtractEditFormData(model);

                var isSuccess = _mainStore.Update(info);

                if (isSuccess)
                {
                    this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit Slide request: " + ex.ToString());

                return View(model);
            }

            return RedirectToAction("Edit/" + model.Id);
        }

        //public ActionResult UpdateLang()
        //{
        //    SlideLangModel model = new SlideLangModel();
        //    var id = Utils.ConvertToInt32(Request["Id"]);
        //    var propertyId = Utils.ConvertToInt32(Request["SlideId"]);

        //    if (propertyId == 0)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    if (id > 0)
        //    {
        //        model.IsUpdate = true;
        //    }

        //    try
        //    {
        //        model.SlideId = propertyId;

        //        //Begin db transaction
        //        var listLangExists = _mainStore.GetLangDetail(propertyId);
        //        if (id == 0)
        //        {
        //            var listItem = new List<string>();
        //            if (listLangExists != null && listLangExists.Count > 0)
        //            {
        //                listItem.AddRange(listLangExists.Select(s => s.LangCode));
        //            }
        //            model.Languages = CommonHelpers.GetListLanguageNotExist(listItem);
        //        }
        //        else
        //        {
        //            model.Languages = CommonHelpers.GetListLanguages();

        //            var info = listLangExists.FirstOrDefault(s => s.Id == id);
        //            if (info != null)
        //            {
        //                model.SlideId = propertyId;
        //                model.Id = info.Id;
        //                model.LangCode = info.LangCode;
        //                model.Name = info.Name;
        //            }
        //        }
              
        //    }
        //    catch (Exception ex)
        //    {
        //        this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

        //        logger.Error("Failed for Show UpdateLang frm request: " + ex.ToString());
        //    }

        //    return PartialView("../Slide/_UpdateLang", model);
        //}

        //[HttpPost]
        //public ActionResult UpdateLang(SlideLangModel model)
        //{
        //    var msg = ManagerResource.LB_OPERATION_SUCCESS;
        //    var isSuccess = false;

        //    if (!ModelState.IsValid)
        //    {
        //        string messages = string.Join("; ", ModelState.Values
        //                               .SelectMany(x => x.Errors)
        //                               .Select(x => x.ErrorMessage + x.Exception));
        //        this.AddNotification(messages, NotificationType.ERROR);

        //        return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = messages });
        //    }

        //    try
        //    {
        //        var code = 0;

        //        //Begin db transaction
        //        var data = new IdentitySlideLang();
        //        data.SlideId  = model.SlideId;
        //        data.Id = model.Id;
        //        data.Name = model.Name;
        //        data.LangCode = model.LangCode;

        //        if (model.Id > 0)
        //        {
        //            //Update
        //            _mainStore.UpdateLang(data);
        //        }
        //        else
        //        {
        //            //Add new
        //            code = _mainStore.InsertLang(data);

        //            if (code == EnumCommonCode.Error)
        //            {
        //                return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = ManagerResource.LB_DUPLICATE_DATA, clientcallback = " location.reload()" });
        //            }
        //        }

        //        isSuccess = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

        //        logger.Error("Failed for UpdateLang request: " + ex.ToString());

        //        return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = NotifSettings.Error_SystemBusy });
        //    }

        //    return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = msg, clientcallback = " location.reload()" });
        //}

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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete_Confirm(int id)
        {
            var strError = string.Empty;
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                _mainStore.Delete(id);               
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Slide because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
        }

        #region SlideItem Item

        public ActionResult ManageItems(string id)
        {
            ModelState.Clear();
            ManageSlideItemModel model = new ManageSlideItemModel();
            var slideId = Utils.ConvertToInt32(id);
            if (slideId <= 0)
                return Redirect("/Error/NotFound");

            try
            {
                model.SlideId = slideId;
                model.SearchResults = _mainStore.GetAllSlideItemBySlide(slideId);
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get all slide item because: " + ex.ToString());

                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateSlideItem(SlideItemUpdateModel model)
        {
            var newId = 0;
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage + x.Exception));
                this.AddNotification(messages, NotificationType.ERROR);

                return RedirectToAction("/ManageItems/" + model.SlideId);
            }

            try
            {
                //Begin db transaction
                var info = ExtractUpdateSlideItemFormData(model);

                newId = _mainStore.InsertSlideItem(info);

                if (newId > 0)
                {
                    this.AddNotification(ManagerResource.LB_INSERT_SUCCESS, NotificationType.SUCCESS);
                }
                else
                {
                    this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                    ModelState.Clear();
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Create Slide request: " + ex.ToString());
            }

            return RedirectToAction("/ManageItems/" + model.SlideId);
        }

        //[AccessRoleChecker]
        public ActionResult EditSlideItem(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                //Begin db transaction
                var info = _mainStore.GetSlideItemById(id);


                //Render to view model
                var editModel = RenderEditSlideItemModel(info);

                return View(editModel);
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit Slide Item request: " + ex.ToString());
            }

            return View(new SlideItemUpdateModel());
        }

        [HttpPost]
        public ActionResult EditSlideItem(SlideItemUpdateModel model)
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
                //Begin db transaction
                var info = ExtractUpdateSlideItemFormData(model);

                var isSuccess = _mainStore.UpdateSlideItem(info);

                if (isSuccess)
                {
                    this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit Slide request: " + ex.ToString());

                return View(model);
            }

            return RedirectToAction("ManageItems/" + model.SlideId);
        }

        //Show popup confirm delete        
        //[AccessRoleChecker]
        public ActionResult DeleteSlideItem(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return PartialView("_PopupDelete", id);
        }

        [HttpPost, ActionName("DeleteSlideItem")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSlideItem_Confirm(int id)
        {
            var strError = string.Empty;
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                _mainStore.DeleteSlideItem(id);
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Slide Item because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "location.reload();" });
        }

        #endregion

        #region Helpers

        private IdentitySlide ExtractCreateFormData(SlideCreateModel formData)
        {
            var myIdetity = new IdentitySlide();
            myIdetity.Name = formData.Name;
            myIdetity.Code = formData.Code;
            myIdetity.SlideType = formData.SlideType;
            myIdetity.CssClass = formData.CssClass;
            myIdetity.DelayTime = formData.DelayTime;
            myIdetity.Status = formData.Status;

            return myIdetity;
        }

        private IdentitySlideItem ExtractUpdateSlideItemFormData(SlideItemUpdateModel formData)
        {
            var coverImg = string.Empty;
            if (formData.ImageUpload != null)
            {
                if (formData.ImageUpload[0] != null)
                {
                    var apiReturn = CdnServices.UploadImagesAsync(formData.ImageUpload, formData.SlideId.ToString(), "Slide").Result;
                    if (apiReturn != null)
                    {
                        if (apiReturn.Data != null)
                        {
                            var resultData = JsonConvert.DeserializeObject<List<string>>(apiReturn.Data.ToString());
                            var returnData = new List<string>();
                            if (resultData.HasData())
                            {
                                coverImg = resultData[0];
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(coverImg))
                coverImg = formData.CurrentImageUrl;

            var myIdetity = new IdentitySlideItem();

            myIdetity.Id = formData.Id;
            myIdetity.Title = formData.Title;
            myIdetity.SlideId = formData.SlideId;
            myIdetity.Description = formData.Description;
            myIdetity.ImageUrl = coverImg;
            myIdetity.Link = formData.Link;
            myIdetity.LinkAction = formData.LinkAction;
            myIdetity.SortOrder = formData.SortOrder;
            myIdetity.Status = formData.Status;

            return myIdetity;
        }

        private IdentitySlide ExtractEditFormData(SlideEditModel formData)
        {
            var myIdetity = new IdentitySlide();
            myIdetity.Id = formData.Id;
            myIdetity.Name = formData.Name;
            myIdetity.Code = formData.Code;
            myIdetity.SlideType = formData.SlideType;
            myIdetity.CssClass = formData.CssClass;
            myIdetity.DelayTime = formData.DelayTime;
            myIdetity.Status = formData.Status;

            return myIdetity;
        }

        private SlideEditModel RenderEditModel(IdentitySlide identity)
        {
            var editModel = new SlideEditModel();

            editModel.Id = identity.Id;
            editModel.Name = identity.Name;
            editModel.Code = identity.Code;
            editModel.SlideType = identity.SlideType;
            editModel.CssClass = identity.CssClass;
            editModel.DelayTime = identity.DelayTime;
            editModel.Status = identity.Status;

            return editModel;
        }

        private SlideItemUpdateModel RenderEditSlideItemModel(IdentitySlideItem identity)
        {
            var editModel = new SlideItemUpdateModel();

            editModel.Id = identity.Id;
            editModel.Title = identity.Title;
            editModel.SlideId = identity.SlideId;
            editModel.Description = identity.Description;
            editModel.CurrentImageUrl = identity.ImageUrl;
            editModel.Link = identity.Link;
            editModel.LinkAction = identity.LinkAction;
            editModel.SortOrder = identity.SortOrder;
            editModel.Status = identity.Status;

            return editModel;
        }

        #endregion

    }
}