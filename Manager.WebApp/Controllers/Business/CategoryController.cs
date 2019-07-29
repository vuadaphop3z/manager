//using System;
//using System.Linq;
//using System.Net;
//using System.Web.Mvc;
//using Manager.SharedLib.Logging;
//using Manager.WebApp.Helpers;
//using Manager.WebApp.Models;
//using Manager.WebApp.Settings;
//using MsSql.AspNet.Identity.Entities;
//using Microsoft.AspNet.Identity;
//using MsSql.AspNet.Identity.MsSqlStores;
//using Manager.WebApp.Resources;
//using Manager.WebApp.Services;
//using Manager.WebApp.Helpers;

//namespace Manager.WebApp.Controllers
//{
//    public class CategoryController : BaseAuthedController
//    {
//        private readonly IStoreCategory _mainStore;
//        private readonly ILog logger = LogProvider.For<CategoryController>();

//        public CategoryController(IStoreCategory mainStore)
//        {
//            _mainStore = mainStore;
//        }

//        //[AccessRoleChecker]
//        public ActionResult Index(ManageCategoryModel model)
//        {
//            int currentPage = 1;
//            int pageSize = SystemSettings.DefaultPageSize;

//            if (string.IsNullOrEmpty(model.SearchExec))
//            {
//                model.SearchExec = "Y";
//                if (!ModelState.IsValid)
//                {
//                    ModelState.Clear();
//                }
//            }

//            if (Request["Page"] != null)
//            {
//                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
//            }

//            var filter = new IdentityCategory
//            {
//                Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
//                Status = model.Status == null ? -1 : (int)model.Status
//            };

//            try
//            {
//                model.SearchResults = _mainStore.GetAll(filter, currentPage, SystemSettings.DefaultPageSize);
//                if (model.SearchResults != null && model.SearchResults.Count > 0)
//                {
//                    model.TotalCount = model.SearchResults[0].TotalCount;
//                    model.CurrentPage = currentPage;
//                    model.PageSize = pageSize;
//                }
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed to get data because: " + ex.ToString());

//                return View(model);
//            }

//            return View(model);
//        }

//        //[AccessRoleChecker]
//        public ActionResult Create()
//        {
//            var createModel = new CategoryCreateModel();
//            return View(createModel);
//        }

//        [HttpPost]
//        public ActionResult Create(CategoryCreateModel model)
//        {
//            var newId = 0;
//            if (!ModelState.IsValid)
//            {
//                string messages = string.Join("; ", ModelState.Values
//                                       .SelectMany(x => x.Errors)
//                                       .Select(x => x.ErrorMessage + x.Exception));
//                this.AddNotification(messages, NotificationType.ERROR);
//                return View(model);
//            }

//            try
//            {
//                //Begin db transaction
//                var CategoryInfo = ExtractCreateFormData(model);

//                newId = _mainStore.Insert(CategoryInfo);

//                //Clear cache
//                CategoryServices.ClearCategoryCache();
//                if (newId > 0)
//                {
//                    this.AddNotification(ManagerResource.LB_INSERT_SUCCESS, NotificationType.SUCCESS);
//                }
//                else
//                {
//                    this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);
//                }
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed for Create Category request: " + ex.ToString());

//                return View(model);
//            }

//            return RedirectToAction("Edit/" + newId);
//        }

//        //[AccessRoleChecker]
//        public ActionResult Edit(int id)
//        {
//            if (id == 0)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            try
//            {
//                //Begin db transaction
//                var info = _mainStore.GetDetail(id);

//                if (info == null)
//                    return RedirectToErrorPage();

//                //Render to view model
//                var editModel = RenderEditModel(info);

//                return View(editModel);
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed for Edit Category request: " + ex.ToString());
//            }

//            return View(new CategoryEditModel());
//        }

//        [HttpPost]
//        public ActionResult Edit(CategoryEditModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                string messages = string.Join("; ", ModelState.Values
//                                       .SelectMany(x => x.Errors)
//                                       .Select(x => x.ErrorMessage + x.Exception));
//                this.AddNotification(messages, NotificationType.ERROR);
//                return View(model);
//            }

//            try
//            {
//                //Begin db transaction
//                var CategoryInfo = ExtractEditFormData(model);

//                var isSuccess = _mainStore.Update(CategoryInfo);

//                //Clear cache
//                CategoryServices.ClearCategoryCache();

//                if (isSuccess)
//                {
//                    this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
//                }
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed for Edit Category request: " + ex.ToString());

//                return View(model);
//            }

//            return RedirectToAction("Edit/" + model.Id);
//        }

//        public ActionResult UpdateLang()
//        {
//            CategoryLangModel model = new CategoryLangModel();
//            var id = Utils.ConvertToInt32(Request["Id"]);
//            var categoryId = Utils.ConvertToInt32(Request["CategoryId"]);

//            if (categoryId == 0)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            if (id > 0)
//            {
//                model.IsUpdate = true;
//            }

//            try
//            {
//                model.Languages = CommonHelpers.GetListLanguages();
//                model.CategoryId = categoryId;

//                //Begin db transaction
//                var info = _mainStore.GetLangDetail(id);

//                if(info != null)
//                {
//                    model.CategoryId = categoryId;
//                    model.Id = info.Id;
//                    model.LangCode = info.LangCode;
//                    model.Name = info.Name;
//                }
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed for Show UpdateLang frm request: " + ex.ToString());
//            }

//            return PartialView("../Category/_UpdateLang", model);
//        }

//        [HttpPost]
//        public ActionResult UpdateLang(CategoryLangModel model)
//        {
//            var msg = ManagerResource.LB_OPERATION_SUCCESS;
//            var isSuccess = false;              
            
//            if (!ModelState.IsValid)
//            {
//                string messages = string.Join("; ", ModelState.Values
//                                       .SelectMany(x => x.Errors)
//                                       .Select(x => x.ErrorMessage + x.Exception));
//                this.AddNotification(messages, NotificationType.ERROR);

//                return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = messages });
//            }

//            try
//            {
//                var code = 0;

//                //Begin db transaction
//                var data = new IdentityCategoryLang();
//                data.CategoryId = model.CategoryId;
//                data.Id = model.Id;
//                data.LangCode = model.LangCode;
//                data.Name = model.Name;
//                data.UrlFriendly = UrlFriendly.ConvertToUrlFriendly(model.Name);

//                if (model.Id > 0)
//                {
//                    //Update
//                    _mainStore.UpdateLang(data);
//                }
//                else
//                {
//                    //Add new
//                    code = _mainStore.AddNewLang(data);

//                    if(code == EnumCommonCode.Error)
//                    {
//                        return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = ManagerResource.LB_DUPLICATE_DATA, clientcallback = " location.reload()" });
//                    }
//                }

//                isSuccess = true;

//                //Clear cache
//                CategoryServices.ClearCategoryCache();
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed for UpdateLang request: " + ex.ToString());

//                return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = NotifSettings.Error_SystemBusy });
//            }

//            return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = msg, clientcallback = " location.reload()" });
//        }

//        //Show popup confirm delete        
//        //[AccessRoleChecker]
//        public ActionResult Delete(int id)
//        {
//            if (id <= 0)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            return PartialView("_PopupDelete", id);
//        }

//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult Delete_Confirm(int id)
//        {
//            var strError = string.Empty;
//            if (id <= 0)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            try
//            {
//                _mainStore.Delete(id);
//                //Clear cache
//                CategoryServices.ClearCategoryCache();
//            }
//            catch (Exception ex)
//            {
//                strError = ManagerResource.LB_SYSTEM_BUSY;

//                logger.Error("Failed to get Delete Category because: " + ex.ToString());

//                return Json(new { success = false, message = strError });
//            }

//            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
//        }

//        public ActionResult DeleteLang(int id)
//        {
//            if (id <= 0)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            return PartialView("_PopupDelete", id);
//        }

//        [HttpPost, ActionName("DeleteLang")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteLang_Confirm(int id)
//        {
//            var strError = string.Empty;
//            if (id <= 0)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            try
//            {
//                _mainStore.DeleteLang(id);

//                //Clear cache
//                CategoryServices.ClearCategoryCache();
//            }
//            catch (Exception ex)
//            {
//                strError = ManagerResource.LB_SYSTEM_BUSY;

//                logger.Error("Failed to get Delete Category because: " + ex.ToString());

//                return Json(new { success = false, message = strError });
//            }

//            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "location.reload()" });
//        }

//        #region Helpers

//        private IdentityCategory ExtractCreateFormData(CategoryCreateModel formData)
//        {
//            var myIdetity = new IdentityCategory();
//            myIdetity.Name = formData.Name;
//            myIdetity.Code = formData.Code;
//            myIdetity.UrlFriendly = UrlFriendly.ConvertToUrlFriendly(formData.Name);
//            myIdetity.Icon = formData.Icon;
//            myIdetity.Status = Utils.ConvertToInt32(formData.Status);
//            myIdetity.CreatedBy = User.Identity.GetUserId();

//            return myIdetity;
//        }

//        private IdentityCategory ExtractEditFormData(CategoryEditModel formData)
//        {
//            var myIdetity = new IdentityCategory();
//            myIdetity.Id = formData.Id;

//            myIdetity.Name = formData.Name;
//            myIdetity.Code = formData.Code;
//            myIdetity.Icon = formData.Icon;
//            myIdetity.UrlFriendly = UrlFriendly.ConvertToUrlFriendly(formData.Name);
//            myIdetity.Status = Utils.ConvertToInt32(formData.Status);
//            myIdetity.LastUpdatedBy = User.Identity.GetUserId();

//            return myIdetity;
//        }

//        private CategoryEditModel RenderEditModel(IdentityCategory identity)
//        {
//            var editModel = new CategoryEditModel();

//            editModel.Id = identity.Id;
//            editModel.Name = identity.Name;
//            editModel.Code = identity.Code;
//            editModel.Icon = identity.Icon;
//            editModel.Status = identity.Status;
//            editModel.UrlFriendly = identity.UrlFriendly;
//            editModel.LangList = identity.LangList;

//            return editModel;
//        }

//        #endregion

//    }
//}