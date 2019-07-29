using System;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MsSql.AspNet.Identity;
using Manager.WebApp.Models;
using Manager.WebApp.Helpers;
using System.Net;
using Manager.SharedLib.Logging;
using System.Collections.Generic;
using Newtonsoft.Json;
using Manager.SharedLib.Extensions;
using Manager.WebApp.Resources;
using System.Linq;
using Manager.WebApp.Settings;

namespace Manager.WebApp.Controllers
{
    public class MenuController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<MenuController>();
        private readonly IActivityStore _activityStore;
        private readonly IAccessRolesStore _accessRoleStore;

        public MenuController(IActivityStore activityStore, IAccessRolesStore accessRoleStore)
        {
            _activityStore = activityStore;
            _accessRoleStore = accessRoleStore;
        }

        [AccessRoleChecker]
        public ActionResult Index(MenuViewModels model)
        {
            try
            {
                model.AllMenus = _accessRoleStore.GetAllMenus();
            }
            catch (Exception ex)
            {
                this.AddNotification(string.Format("Could not get all menus because: {0}", ex.ToString()), NotificationType.ERROR);
            }

            return View(model);
        }

        public ActionResult Create(int parentid)
        {
            MenuViewModels model = new MenuViewModels();
            model.ParentId = parentid;
            model.Active = true;
            model.Visible = true;
            try
            {
                model.AllOperation = _accessRoleStore.GetListOperationNotUse();
            }
            catch (Exception ex)
            {
                this.AddNotification("Failed to get data because: " + ex.ToString(), NotificationType.ERROR);
                PartialView("_Create", model);
            }
            return PartialView("_Create", model);
        }
        public ActionResult SaveCreate(MenuViewModels model)
        {
            string Controller = null;
            string Action = null;
            if (model.OperationName != null)
            {
                Controller = model.OperationName.Split('/')[0];
                Action = model.OperationName.Split('/')[1];
            }
            int? ParentId = null;
           if (model.ParentId != 0)
            {
                ParentId = model.ParentId;
            }
            try
            {
                _accessRoleStore.InsertMenu(new IdentityMenu
                {
                    ParentId = model.ParentId,
                    Active = model.Active,
                    Controller = Controller,
                    Action = Action,
                    CssClass = model.CssClass,
                    IconCss = model.IconCss,
                    SortOrder = model.SortOrder,
                    Name = model.Name,
                    Visible = model.Visible,
                    Title = model.Title
                });

                MenuHelper.ClearAllMenuCache();

                return Json(new AjaxResponseModel { Success = true, Message = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                this.AddNotification("Failed to get data because: " + ex.ToString(), NotificationType.ERROR);
                return Json(new AjaxResponseModel { Success = false, Message = "Thêm mới thất bại" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Update(int id)
        {
            MenuViewModels model = new MenuViewModels();
            try
            {
               var menu = _accessRoleStore.GetMenuById(id);
                model.Name = menu.Name;
                model.Active = menu.Active;
                model.CssClass = menu.CssClass;
                model.IconCss = menu.IconCss;
                model.SortOrder = menu.SortOrder??0;
                model.Name = menu.Name;
                model.Visible = menu.Visible;
                model.Title = menu.Title;
                model.ParentId = menu.ParentId??0;
                if (menu.Controller != null&& menu.Controller != "")
                {
                    model.OperationName = menu.Controller + "/" + menu.Action;
                }
                model.AllOperation = _accessRoleStore.GetListOperationNotUse();
            }
            catch (Exception ex)
            {
                this.AddNotification("Failed to get data because: " + ex.ToString(), NotificationType.ERROR);
                PartialView("_Update", model);
            }
            return PartialView("_Update", model);
        }

        public ActionResult SaveUpdate(MenuViewModels model)
        {
            string Controller = null;
            string Action = null;
            if (model.OperationName != null)
            {
                Controller = model.OperationName.Split('/')[0];
                Action = model.OperationName.Split('/')[1];
            }
            int? ParentId = null;
            if (model.ParentId != 0)
            {
                ParentId = model.ParentId;
            }
            try
            {
                _accessRoleStore.UpdateMenu(new IdentityMenu
                {
                    Id=model.Id,
                    ParentId = model.ParentId,
                    Active = model.Active,
                    Controller = Controller,
                    Action = Action,
                    CssClass = model.CssClass,
                    IconCss = model.IconCss,
                    SortOrder = model.SortOrder,
                    Name = model.Name,
                    Visible = model.Visible,
                    Title = model.Title
                });

                MenuHelper.ClearAllMenuCache();

                return Json(new AjaxResponseModel { Success = true, Message = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                this.AddNotification("Failed to get data because: " + ex.ToString(), NotificationType.ERROR);
                return Json(new AjaxResponseModel { Success = false, Message = "Cập nhật thất bại" }, JsonRequestBehavior.AllowGet);
            }
        }

        private void ApplyNewSorting(List<SortingElement> sortList, int parentId = 0)
        {
            if (sortList.HasData())
            {
                var beginOrder = 0;
                foreach (var item in sortList)
                {
                    beginOrder++;
                    item.SortOrder = beginOrder;
                    item.ParentId = parentId;

                    if (item.children.HasData())
                    {
                        ApplyNewSorting(item.children, item.id);
                    }
                }
            }
        }

        public ActionResult UpdateSorting(string data)
        {            
            try
            {
                var sortList = new List<SortingElement>();

                if(!string.IsNullOrEmpty(data))
                    sortList = JsonConvert.DeserializeObject<List<SortingElement>>(data);

                if (sortList != null)
                    ApplyNewSorting(sortList);

                //Update in DB
                if (sortList != null)
                    _accessRoleStore.UpdateSorting(sortList);

                //Clear Cache
                MenuHelper.ClearAllMenuCache();
                
                return Json(new AjaxResponseModel { Success = true, Message = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                this.AddNotification("Failed to get data because: " + ex.ToString(), NotificationType.ERROR);
                return Json(new AjaxResponseModel { Success = false, Message = "Cập nhật thất bại" }, JsonRequestBehavior.AllowGet);
            }
        }

        //Show popup confirm delete        
        public ActionResult PopupDelete(string id)
        {
            if (id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MenuViewModels record = new MenuViewModels();
            record.Id = Convert.ToInt32(id);

            return PartialView("_DeleteInfo", record);
        }

        [HttpPost, ActionName("PopupDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                int Id = Convert.ToInt32(id);
                _accessRoleStore.DeleteMenu(Id);

                //Clear Cache
                MenuHelper.ClearAllMenuCache();
            }
            catch (Exception ex)
            {
                logger.Error("Failed to Delete menu because: " + ex.ToString());
            }
           
            return Json(new { success = true });
        }

        public ActionResult ShowMenuLang(int id)
        {
            ManageMenuLangModel model = new ManageMenuLangModel();
            try
            {
                model.MenuId = id;
                model.Languages = LanguagesProvider.GetListLanguages();
                model.MenuInfo = _accessRoleStore.GetMenuDetail(id);
            }
            catch (Exception ex)
            {
                this.AddNotification("Failed to get data because: " + ex.ToString(), NotificationType.ERROR);
                PartialView("_Detail", model);
            }
            return PartialView("_Detail", model);
        }

        public ActionResult UpdateLang()
        {
            ManageMenuLangModel model = new ManageMenuLangModel();
            var id = Utils.ConvertToInt32(Request["Id"]);
            var groupId = Utils.ConvertToInt32(Request["MenuId"]);

            if (groupId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (id > 0)
            {
                model.IsUpdate = true;
            }

            try
            {
                model.Languages = LanguagesProvider.GetListLanguages();
                model.MenuId = groupId;

                //Begin db transaction
                var info = _accessRoleStore.GetLangDetail(id);

                if (info != null)
                {
                    model.MenuId = groupId;
                    model.Id = info.Id;
                    model.LangCode = info.LangCode;
                    model.Title = info.Title;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Show UpdateLang form request: " + ex.ToString());
            }

            return PartialView("../Menu/_UpdateLang", model);
        }

        [HttpPost]
        public ActionResult UpdateLang(ManageMenuLangModel model)
        {
            var msg = ManagerResource.LB_OPERATION_SUCCESS;
            var isSuccess = false;

            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage + x.Exception));
                this.AddNotification(messages, NotificationType.ERROR);

                return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = messages });
            }

            try
            {
                var code = 0;

                //Begin db transaction
                var data = new IdentityMenuLang();
                data.Id = model.Id;
                data.MenuId = model.MenuId;
                data.Title = model.Title;                
                data.LangCode = model.LangCode;

                if (model.Id > 0)
                {
                    //Update
                    code = _accessRoleStore.UpdateLang(data);                   

                    if (code == EnumCommonCode.Error)
                    {
                        return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = ManagerResource.LB_DUPLICATE_DATA, clientcallback = string.Format(" ShowMenuLang({0})", model.MenuId) });
                    }

                    //Clear cache
                    MenuHelper.ClearAllMenuCache();
                }
                else
                {
                    //Add new
                    code = _accessRoleStore.AddNewLang(data);

                    if (code == EnumCommonCode.Error)
                    {
                        return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = ManagerResource.LB_DUPLICATE_DATA, clientcallback = string.Format(" ShowMenuLang({0})", model.MenuId) });
                    }

                    //Clear cache
                    MenuHelper.ClearAllMenuCache();
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for UpdateLang request: " + ex.ToString());

                return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = NotifSettings.Error_SystemBusy });
            }

            return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = msg, clientcallback = string.Format(" ShowMenuLang({0})", model.MenuId) });
        }

        public ActionResult DeleteLang()
        {
            ManageMenuLangModel model = new ManageMenuLangModel();
            var id = Utils.ConvertToInt32(Request["Id"]);
            var groupId = Utils.ConvertToInt32(Request["MenuId"]);
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            model.MenuId = groupId;
            model.Id = id;

            return PartialView("_DeleteLangInfo", model);
        }

        [HttpPost, ActionName("DeleteLang")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLang_Confirm(ManageMenuLangModel model)
        {
            var strError = string.Empty;
            if (model.Id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                _accessRoleStore.DeleteLang(model.Id);

                //Clear cache
                MenuHelper.ClearAllMenuCache();
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Menu Lang because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format(" ShowMenuLang({0})", model.MenuId) });
        }

        #region Helpers

        private void WriteActivityLog(string activityText, ActivityLogType activityType, string targetId, TargetObjectType targetType)
        {
            var logData = new ActivityLog
            {
                UserId = User.Identity.GetUserId(),
                ActivityText = activityText,
                ActivityType = activityType.ToString(),
                TargetId = targetId,
                TargetType = targetType.ToString(),
                IPAddress = Request.UserHostAddress
            };

            _activityStore.WriteActivityLog(logData);
        }

        #endregion
    }
}