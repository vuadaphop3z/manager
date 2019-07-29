//using System;
//using System.Linq;
//using System.Net;
//using System.Web.Mvc;
//using Manager.SharedLib.Logging;
//using Manager.WebApp.Helpers;
//using Manager.WebApp.Settings;
//using MsSql.AspNet.Identity.Entities;
//using MsSql.AspNet.Identity.MsSqlStores;
//using Manager.WebApp.Resources;
//using Microsoft.AspNet.Identity;
//using Manager.WebApp.Helpers;
//using Manager.WebApp.Models;
//using Autofac;
//using System.Collections.Generic;

//namespace Manager.WebApp.Controllers
//{
//    public class MemberController : BaseAuthedController
//    {
//        private readonly IStoreMember _mainStore;
//        private readonly ILog logger = LogProvider.For<MemberController>();

//        public MemberController(IStoreMember mainStore)
//        {
//            _mainStore = mainStore;
//        }

//        [AccessRoleChecker]
//        public ActionResult Index(ManageMemberModel model)
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

//            var filter = new IdentityMember
//            {
//                Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
//                Status = model.Status == null ? -1 : (int)model.Status
//            };

//            try
//            {
//                model.SearchResults = _mainStore.GetByPage(filter, currentPage, SystemSettings.DefaultPageSize);
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
//            var createModel = new MemberCreateModel();
//            return View(createModel);
//        }

//        [HttpPost]
//        public ActionResult Create(MemberCreateModel model)
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
//                var code = 1;
//                //Begin db transaction
//                var info = ExtractCreateFormData(model);

//                newId = _mainStore.Insert(info, out code);

//                if (code != 1)
//                {
//                    if (code == 101)
//                    {
//                        this.AddNotification(ManagerResource.ERROR_USERNAME_EXISTED, NotificationType.ERROR);
//                        return View(model);
//                    }
//                }

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

//                logger.Error("Failed for Create Member request: " + ex.ToString());

//                return View(model);
//            }

//            return RedirectToAction("Index");
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
//                var info = _mainStore.GetById(id);

//                //Render to view model
//                var editModel = RenderEditModel(info);

//                return View(editModel);
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed for Edit Member request: " + ex.ToString());
//            }

//            return View(new MemberEditModel());
//        }

//        [HttpPost]
//        public ActionResult Edit(MemberEditModel model)
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
//                var code = 1;

//                //Begin db transaction
//                var info = ExtractEditFormData(model);

//                var isSuccess = _mainStore.Update(info, out code);

//                if (code != 1)
//                {
//                    if (code == 101)
//                    {
//                        this.AddNotification(ManagerResource.ERROR_USERNAME_EXISTED, NotificationType.ERROR);
//                        return View(model);
//                    }
//                }

//                if (isSuccess)
//                {
//                    this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
//                }
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed for Edit Member request: " + ex.ToString());

//                return View(model);
//            }

//            return RedirectToAction("Edit/" + model.Id);
//        }

//        //Show popup        
//        public ActionResult ViewAssignedShops(int memberId)
//        {
//            if (memberId <= 0)
//            {
//                return Json(new { success = false, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, html = "" }, JsonRequestBehavior.AllowGet);
//            }

//            var model = new MemberAssignToShopModel();

//            var isSuccess = false;
//            var msg = string.Empty;
//            var returnHtml = string.Empty;
//            try
//            {
//                model.MemberId = memberId;
//                var shopStore = GlobalContainer.IocContainer.Resolve<IStoreShop>();
//                model.AssignedShops = shopStore.GetAssigned(model.MemberId);

//                returnHtml = PartialViewAsString("_AssignedShops", model);

//                isSuccess = true;
//                return Json(new { success = isSuccess, message = msg, html = returnHtml }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                logger.Error("Failed for ViewAssignedShops popup action: " + ex.ToString());
//                return Json(new { success = isSuccess, message = msg, html = returnHtml }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        //Show popup        
//        public ActionResult AssignToShop(int memberId)
//        {
//            if (memberId <= 0)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            var model = new MemberAssignToShopModel();
//            try
//            {
//                model.MemberId = memberId;
//            }
//            catch (Exception ex)
//            {
//                logger.Error("Failed for AssignToShop popup action: " + ex.ToString());
//                return PartialView("../Error/Error");
//            }

//            return PartialView("../Member/_AssignToShop", model);
//        }

//        [HttpPost, ActionName("AssignToShop")]
//        [ValidateAntiForgeryToken]
//        public ActionResult AssignToShop_Confirm(MemberAssignToShopModel model)
//        {
//            var strError = string.Empty;
//            try
//            {
//                if (model.MemberId <= 0)
//                    return Json(new { success = false, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT }, JsonRequestBehavior.AllowGet);

//                if (model.ShopId <= 0)
//                    return Json(new { success = false, message = ManagerResource.ERROR_NOT_SELECT_SHOP }, JsonRequestBehavior.AllowGet);

//                var shopStore = GlobalContainer.IocContainer.Resolve<IStoreShop>();

//                var result = shopStore.AssignToUser(model.ShopId, model.MemberId, model.IsOwner);
//                if (result > 0)
//                    return Json(new { success = true, message = ManagerResource.LB_OPERATION_SUCCESS, clientcallback = string.Format("GetAssignedShop({0});", model.MemberId) }, JsonRequestBehavior.AllowGet);
//                else
//                    return Json(new { success = false, message = ManagerResource.ERROR_USER_ASSIGNED_TO_SHOP, clientcallback = string.Format("GetAssignedShop({0});", model.MemberId) }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                strError = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                logger.Error("Failed to AssignToShop because: " + ex.ToString());

//                return Json(new { success = false, message = strError }, JsonRequestBehavior.AllowGet);
//            }
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
//            }
//            catch (Exception ex)
//            {
//                strError = ManagerResource.LB_SYSTEM_BUSY;

//                logger.Error("Failed to get Delete Member because: " + ex.ToString());

//                return Json(new { success = false, message = strError });
//            }

//            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult GetListMember()
//        {
//            var strError = string.Empty;
//            try
//            {
//                var keyword = (Request["query"] != null) ? Request["query"].ToString() : string.Empty;
//                var listUser = _mainStore.GetList(keyword);

//                List<MemberItemInListModel> returnList = new List<MemberItemInListModel>();
//                if (listUser.HasData())
//                {
//                    foreach (var user in listUser)
//                    {
//                        var item = new MemberItemInListModel();
//                        item.Id = user.Id;
//                        item.DisplayName = user.DisplayName;
//                        item.UserName = user.UserName;

//                        returnList.Add(item);
//                    }
//                }

//                return Json(new { success = true, data = returnList });

//            }
//            catch (Exception ex)
//            {
//                strError = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                logger.Error("Failed to GetListMember because: " + ex.ToString());

//                return Json(new { success = false, data = string.Empty, message = strError });
//            }
//        }


//        #region Helpers

//        private IdentityMember ExtractCreateFormData(MemberCreateModel formData)
//        {
//            var myIdetity = new IdentityMember();
//            myIdetity.UserName = formData.UserName;
//            myIdetity.Email = formData.Email;
//            myIdetity.PhoneNumber = formData.PhoneNumber;
//            myIdetity.DisplayName = formData.DisplayName;
//            myIdetity.Status = formData.Status;

//            if (string.IsNullOrEmpty(formData.Password))
//                myIdetity.PasswordHash = Utility.Md5HashingData(formData.Password);
//            else
//                myIdetity.PasswordHash = Utility.Md5HashingData("123456");

//            return myIdetity;
//        }

//        private IdentityMember ExtractEditFormData(MemberEditModel formData)
//        {
//            var myIdetity = new IdentityMember();
//            myIdetity.Id = formData.Id;
//            myIdetity.UserName = formData.UserName;
//            myIdetity.Email = formData.Email;
//            myIdetity.PhoneNumber = formData.PhoneNumber;
//            myIdetity.DisplayName = formData.DisplayName;
//            myIdetity.Status = formData.Status;

//            return myIdetity;
//        }

//        private MemberEditModel RenderEditModel(IdentityMember identity)
//        {
//            var editModel = new MemberEditModel();

//            editModel.UserName = identity.UserName;
//            editModel.Email = identity.Email;
//            editModel.PhoneNumber = identity.PhoneNumber;
//            editModel.DisplayName = identity.DisplayName;
//            editModel.Status = identity.Status;

//            return editModel;
//        }

//        #endregion

//    }
//}