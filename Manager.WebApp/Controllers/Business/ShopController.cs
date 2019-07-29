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
//using Manager.WebApp.Helpers;
//using System.Collections.Generic;
//using Newtonsoft.Json;
//using MsSql.AspNet.Identity.MsSqlStores;
//using Manager.WebApp.Resources;
//using Autofac;
//using Manager.WebApp.Services;

//namespace Manager.WebApp.Controllers
//{
//    public class ShopController : BaseAuthedController
//    {
//        private readonly IStoreShop _mainStore;
//        private readonly ILog logger = LogProvider.For<ShopController>();

//        public ShopController(IStoreShop mainStore)
//        {
//            _mainStore = mainStore;
//        }

//        // GET: Shops
//        [AccessRoleChecker]
//        public ActionResult Index(ManageShopModel model)
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

//                //return View(model);
//            }

//            if (Request["Page"] != null)
//            {
//                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
//            }

//            var filter = new IdentityShop
//            {
//                Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
//                Status = model.Status == null ? -1 : (int)model.Status,
//                CategoryId = model.CategoryId == null ? -1 : (int)model.CategoryId
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
//            var createModel = new ShopCreateModel();

//            try
//            {
//                createModel.Areas = CommonHelpers.GetListArea();
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed for Create Shop request: " + ex.ToString());
//            }

//            return View(createModel);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(ShopCreateModel model)
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
//                var shopInfo = ExtractCreateFormData(model);

//                newId = _mainStore.Insert(shopInfo);
//                if (newId > 0)
//                {
//                    this.AddNotification(NotifSettings.Success_Created, NotificationType.SUCCESS);
//                }
//                else
//                {
//                    this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);
//                }
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed for Create Shop request: " + ex.ToString());

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
//                var info = _mainStore.GetById(id);

//                if (info == null || (info != null && info.Id == 0))
//                    return RedirectToErrorPage();

//                //Render to view model
//                var editModel = RenderEditModel(info);

//                return View(editModel);
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed for Edit Shop request: " + ex.ToString());
//            }

//            return View(new ShopEditModel());
//        }

//        [HttpPost]
//        public ActionResult Edit(ShopEditModel model)
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
//                var shopInfo = ExtractEditFormData(model);

//                var isSuccess = _mainStore.Update(shopInfo);
//                if (isSuccess)
//                {
//                    this.AddNotification(NotifSettings.Success_Updated, NotificationType.SUCCESS);
//                }
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed for Edit Shop POST request: " + ex.ToString());
//            }

//            return RedirectToAction("Edit/" + model.Id);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult GetListShopToAssign()
//        {
//            var strError = string.Empty;
//            try
//            {
//                var keyword = (Request["query"] != null) ? Request["query"].ToString() : string.Empty;
//                var listShop = _mainStore.GetList(keyword);

//                List<ShopItemInListModel> returnList = new List<ShopItemInListModel>();
//                if (listShop.HasData())
//                {
//                    foreach (var shop in listShop)
//                    {
//                        var item = new ShopItemInListModel();
//                        item.Id = shop.Id;
//                        item.Name = shop.Name;

//                        returnList.Add(item);
//                    }
//                }

//                return Json(new { success = true, data = returnList });

//            }
//            catch (Exception ex)
//            {
//                strError = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                logger.Error("Failed to GetListShopToAssign because: " + ex.ToString());

//                return Json(new { success = false, data = string.Empty, message = strError });
//            }
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteAssignedMember(int shopId, int userId)
//        {
//            var strError = string.Empty;
//            try
//            {
//                if (shopId <= 0)
//                    return Json(new { success = false, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT }, JsonRequestBehavior.AllowGet);

//                if (userId <= 0)
//                    return Json(new { success = false, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT }, JsonRequestBehavior.AllowGet);

//                _mainStore.DeleteAssignedUser(shopId, userId);

//                return Json(new { success = true, message = ManagerResource.LB_OPERATION_SUCCESS }, JsonRequestBehavior.AllowGet);
//            }

//            catch (Exception ex)
//            {
//                strError = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                logger.Error("Failed to DeleteAssignedMember because: " + ex.ToString());

//                return Json(new { success = false, message = strError }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        //Show popup        
//        public ActionResult ViewAssignedMembers(int shopId)
//        {
//            if (shopId <= 0)
//            {
//                return Json(new { success = false, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, html = "" }, JsonRequestBehavior.AllowGet);
//            }

//            var model = new ShopAssignToMemberModel();

//            var isSuccess = false;
//            var msg = string.Empty;
//            var returnHtml = string.Empty;
//            try
//            {
//                model.ShopId = shopId;
//                var listAssigned = _mainStore.GetAssignedMembers(shopId);
//                if (listAssigned.HasData())
//                {
//                    var apiModel = new ApiListUserInfoModel();
//                    apiModel.ListUserId = new List<int>();
//                    foreach (var item in listAssigned)
//                    {
//                        apiModel.ListUserId.Add(item.MemberId);
//                    }

//                    var apiReturned = AccountServices.GetListUserProfileAsync(apiModel).Result;
//                    if (apiReturned != null)
//                    {
//                        if (apiReturned.Data != null)
//                        {
//                            model.AssignedMembers = JsonConvert.DeserializeObject<List<IdentityMember>>(apiReturned.Data.ToString());
//                        }
//                    }
//                }

//                if (model.AssignedMembers.HasData())
//                {
//                    foreach (var item in model.AssignedMembers)
//                    {
//                        foreach (var assigned in listAssigned)
//                        {
//                            if (assigned.MemberId == item.Id)
//                            {
//                                item.IsOwner = assigned.IsOwner;
//                                break;
//                            }
//                        }
//                    }
//                }

//                returnHtml = PartialViewAsString("Partials/_AssignedMembers", model);

//                isSuccess = true;
//                return Json(new { success = isSuccess, message = msg, html = returnHtml }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                logger.Error("Failed for ViewAssignedMembers popup action: " + ex.ToString());
//                return Json(new { success = isSuccess, message = msg, html = returnHtml }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        //Show popup        
//        public ActionResult AssignToMember(int shopId)
//        {
//            if (shopId <= 0)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            var model = new ShopAssignToMemberModel();
//            try
//            {
//                model.ShopId = shopId;
//            }
//            catch (Exception ex)
//            {
//                logger.Error("Failed for AssignToMember popup action: " + ex.ToString());
//                return PartialView("../Error/Error");
//            }

//            return PartialView("../Shop/Partials/_AssignToMember", model);
//        }

//        [HttpPost, ActionName("AssignToMember")]
//        [ValidateAntiForgeryToken]
//        public ActionResult AssignToMember_Confirm(ShopAssignToMemberModel model)
//        {
//            var strError = string.Empty;
//            try
//            {
//                if (model.ShopId <= 0)
//                    return Json(new { success = false, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT }, JsonRequestBehavior.AllowGet);

//                if (model.MemberId <= 0)
//                    return Json(new { success = false, message = ManagerResource.ERROR_NOT_SELECT_MEMBER }, JsonRequestBehavior.AllowGet);

//                var shopStore = GlobalContainer.IocContainer.Resolve<IStoreShop>();

//                var result = shopStore.AssignToUser(model.ShopId, model.MemberId, model.IsOwner);
//                if (result > 0)
//                    return Json(new { success = true, message = ManagerResource.LB_OPERATION_SUCCESS, clientcallback = string.Format("GetAssignedMember({0});", model.ShopId) }, JsonRequestBehavior.AllowGet);
//                else
//                    return Json(new { success = false, message = ManagerResource.ERROR_USER_ASSIGNED_TO_SHOP, clientcallback = string.Format("GetAssignedMember({0});", model.ShopId) }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                strError = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                logger.Error("Failed to AssignToMember because: " + ex.ToString());

//                return Json(new { success = false, message = strError }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        //[HttpPost]
//        //public ActionResult RefreshImageList(int Id)
//        //{
//        //    try
//        //    {
//        //        var myImages = _mainStore.GetListImage(Id);
//        //        var html = PartialViewAsString("Partials/_UploadedImages", myImages);

//        //        return Json(new { success = true, html = html }); // success       
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error("Failed to RefreshImageList because: " + ex.ToString());

//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }
//        //}

//        //[HttpPost]
//        //public ActionResult SaveProperty(int Id, string selectedValues)
//        //{
//        //    try
//        //    {
//        //        _mainStore.UpdateAmenities(Id, selectedValues);

//        //        return Json(new { success = true, message = NotifSettings.Success_Updated }); // success       
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error("Failed to SaveProperty because: " + ex.ToString());

//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }
//        //}

//        //[HttpPost]
//        //public ActionResult UpdateMap(int shopId, string latitude, string longitude)
//        //{
//        //    try
//        //    {
//        //        _mainStore.UpdateMap(shopId, latitude, longitude);

//        //        return Json(new { success = true, message = NotifSettings.Success_Updated }); // success       
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error("Failed to UpdateMap because: " + ex.ToString());

//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }
//        //}

//        //public ActionResult AddPlace(int shopId)
//        //{
//        //    var myModel = new PlaceModel();
//        //    myModel.Id = Utility.GenTranIdWithPrefix(string.Format("{0}{1}", shopId, "_"));
//        //    myModel.ShopId = shopId;
//        //    myModel.Units = CommonHelpers.GetListUnit();

//        //    return PartialView("Partials/_AddPlace", myModel);
//        //}

//        //[HttpPost]
//        //public ActionResult AddPlace(PlaceModel model)
//        //{
//        //    if (!ModelState.IsValid)
//        //    {
//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }

//        //    try
//        //    {
//        //        List<MetaShopNearPlace> myList = null;

//        //        if (model.ShopId <= 0)
//        //        {
//        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//        //        }

//        //        var nearPlacesJson = _mainStore.GetNearPlaces(model.ShopId);

//        //        if (!string.IsNullOrEmpty(nearPlacesJson))
//        //        {
//        //            myList = JsonConvert.DeserializeObject<List<MetaShopNearPlace>>(nearPlacesJson);
//        //        }

//        //        if (myList == null)
//        //        {
//        //            myList = new List<MetaShopNearPlace>();
//        //        }

//        //        //New place
//        //        var newPlace = new MetaShopNearPlace();
//        //        newPlace.Id = model.Id;
//        //        newPlace.Latitude = model.Latitude;
//        //        newPlace.Longitude = model.Longitude;
//        //        newPlace.Name = model.Name;
//        //        newPlace.Range = model.Range;
//        //        newPlace.UnitCode = model.UnitCode;
//        //        newPlace.Icon = model.Icon;
//        //        newPlace.CreatedDate = DateTime.Now;

//        //        //Append to current list
//        //        myList.Add(newPlace);

//        //        var myListJson = JsonConvert.SerializeObject(myList);
//        //        //Update list places
//        //        _mainStore.UpdateNearPlaces(model.ShopId, myListJson);

//        //        var html = PartialViewAsString("Partials/_NearPlaces", myListJson);

//        //        return Json(new { success = true, html = html }); // success       
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error("Failed to AddPlace because: " + ex.ToString());

//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }
//        //}

//        //[HttpPost]
//        //public ActionResult RemovePlace(int shopId, string Id)
//        //{
//        //    try
//        //    {
//        //        List<MetaShopNearPlace> myList = null;

//        //        if (shopId <= 0)
//        //        {
//        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//        //        }

//        //        var nearPlacesJson = _mainStore.GetNearPlaces(shopId);

//        //        if (!string.IsNullOrEmpty(nearPlacesJson))
//        //        {
//        //            myList = JsonConvert.DeserializeObject<List<MetaShopNearPlace>>(nearPlacesJson);
//        //        }

//        //        if (myList == null)
//        //        {
//        //            myList = new List<MetaShopNearPlace>();
//        //        }

//        //        if (myList != null && myList.Count > 0)
//        //        {
//        //            myList.RemoveAll(x => x.Id == Id);
//        //        }

//        //        var myListJson = JsonConvert.SerializeObject(myList);

//        //        //Update list places
//        //        _mainStore.UpdateNearPlaces(shopId, myListJson);

//        //        return Json(new { success = true, message = NotifSettings.Success_Deleted });
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error("Failed to RemovePlace because: " + ex.ToString());

//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }
//        //}

//        //#region Policy

//        //public ActionResult AddPolicy(int shopId, int policyId, string policyCode)
//        //{
//        //    if (policyCode == EnumPolicy.CheckInOut)
//        //    {
//        //        var model = new PolicyCheckInOutModel();
//        //        model.ShopId = shopId;
//        //        model.PolicyCode = policyCode;
//        //        model.PolicyId = policyId;

//        //        return PartialView(string.Format("Partials/AddPolicy/{0}", policyCode), model);
//        //    }
//        //    else if (policyCode == EnumPolicy.ChildAndExtraBeds)
//        //    {
//        //        var model = new PolicyCommonChildModel();
//        //        model.ShopId = shopId;
//        //        model.PolicyCode = policyCode;
//        //        model.PolicyId = policyId;

//        //        return PartialView(string.Format("Partials/AddPolicy/{0}", policyCode), model);
//        //    }
//        //    else if (policyCode == EnumPolicy.Dining)
//        //    {
//        //        var model = new PolicyDinningModel();
//        //        model.ShopId = shopId;
//        //        model.PolicyCode = policyCode;
//        //        model.PolicyId = policyId;

//        //        model.Currencies = CommonHelpers.GetListCurrency();

//        //        return PartialView(string.Format("Partials/AddPolicy/{0}", policyCode), model);
//        //    }
//        //    else if (policyCode == EnumPolicy.Pets)
//        //    {
//        //        var model = new PolicyPetsModel();
//        //        model.ShopId = shopId;
//        //        model.PolicyCode = policyCode;
//        //        model.PolicyId = policyId;

//        //        return PartialView(string.Format("Partials/AddPolicy/{0}", policyCode), model);
//        //    }
//        //    else if (policyCode == EnumPolicy.CreditCards)
//        //    {
//        //        var model = new PolicyCreditCardsModel();
//        //        model.ShopId = shopId;
//        //        model.PolicyCode = policyCode;
//        //        model.PolicyId = policyId;

//        //        model.Credits = CommonHelpers.GetListCredit();

//        //        return PartialView(string.Format("Partials/AddPolicy/{0}", policyCode), model);
//        //    }

//        //    return Content(string.Format("The view name [{0}] is invalid", policyCode));
//        //}

//        //public ActionResult EditPolicy(int shopId, int policyId, string policyCode)
//        //{
//        //    if (!ModelState.IsValid)
//        //    {
//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy }, JsonRequestBehavior.AllowGet);
//        //    }

//        //    try
//        //    {
//        //        List<MetaShopPolicy> myList = null;
//        //        MetaShopPolicy currentPolicy = null;

//        //        if (shopId <= 0)
//        //        {
//        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//        //        }

//        //        var metaPolicies = _mainStore.GetPolicies(shopId);

//        //        if (!string.IsNullOrEmpty(metaPolicies))
//        //        {
//        //            myList = JsonConvert.DeserializeObject<List<MetaShopPolicy>>(metaPolicies);
//        //        }

//        //        if (myList == null)
//        //        {
//        //            myList = new List<MetaShopPolicy>();
//        //        }

//        //        if (myList != null && myList.Count > 0)
//        //        {
//        //            //Take one if any
//        //            currentPolicy = myList.Where(x => x.PolicyId == policyId && x.PolicyCode.Equals(policyCode, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
//        //        }

//        //        if (currentPolicy != null)
//        //        {
//        //            return RenderEditPolicyForm(shopId, currentPolicy);
//        //        }
//        //        else
//        //        {
//        //            logger.Error("Failed to EditPolicy due to the current policy could not be identified");

//        //            return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy }, JsonRequestBehavior.AllowGet);
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error("Failed to EditPolicy because: " + ex.ToString());

//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy }, JsonRequestBehavior.AllowGet);
//        //    }
//        //}

//        //private ActionResult RenderEditPolicyForm(int shopId, MetaShopPolicy currentPolicy)
//        //{
//        //    var html = string.Empty;

//        //    if (currentPolicy.PolicyCode == EnumPolicy.CheckInOut)
//        //    {
//        //        var editModel = new PolicyCheckInOutModel();
//        //        editModel.Id = currentPolicy.Id;
//        //        editModel.ShopId = shopId;
//        //        editModel.PolicyId = currentPolicy.PolicyId;
//        //        editModel.PolicyCode = currentPolicy.PolicyCode;

//        //        if (!string.IsNullOrEmpty(currentPolicy.PolicyData))
//        //        {
//        //            var policyData = JsonConvert.DeserializeObject<MetaPolicyDataCheckInOut>(currentPolicy.PolicyData);
//        //            if (policyData != null)
//        //            {
//        //                editModel.CheckIn = policyData.CheckIn;
//        //                editModel.CheckOut = policyData.CheckOut;
//        //                editModel.Note = policyData.Note;
//        //            }
//        //        }

//        //        html = PartialViewAsString(string.Format("Partials/AddPolicy/{0}", currentPolicy.PolicyCode), editModel);
//        //    }
//        //    else if (currentPolicy.PolicyCode == EnumPolicy.ChildAndExtraBeds)
//        //    {
//        //        var editModel = new PolicyCommonChildModel();
//        //        editModel.Id = currentPolicy.Id;
//        //        editModel.ShopId = shopId;
//        //        editModel.PolicyId = currentPolicy.PolicyId;
//        //        editModel.PolicyCode = currentPolicy.PolicyCode;

//        //        if (!string.IsNullOrEmpty(currentPolicy.PolicyData))
//        //        {
//        //            var policyData = JsonConvert.DeserializeObject<MetaChildCommon>(currentPolicy.PolicyData);
//        //            if (policyData != null)
//        //            {
//        //                editModel.AcceptGuest = policyData.AcceptGuest.ToString();
//        //                editModel.ChildrenOver = policyData.ChildrenOver.ToString();
//        //                editModel.PolicyDes = policyData.PolicyDes;
//        //            }
//        //        }

//        //        html = PartialViewAsString(string.Format("Partials/AddPolicy/{0}", currentPolicy.PolicyCode), editModel);
//        //    }
//        //    else if (currentPolicy.PolicyCode == EnumPolicy.Pets)
//        //    {
//        //        var editModel = new PolicyPetsModel();
//        //        editModel.Id = currentPolicy.Id;
//        //        editModel.ShopId = shopId;
//        //        editModel.PolicyId = currentPolicy.PolicyId;
//        //        editModel.PolicyCode = currentPolicy.PolicyCode;

//        //        if (!string.IsNullOrEmpty(currentPolicy.PolicyData))
//        //        {
//        //            var policyData = JsonConvert.DeserializeObject<MetaPolicyDataPets>(currentPolicy.PolicyData);
//        //            if (policyData != null)
//        //            {
//        //                editModel.Accepted = Convert.ToBoolean(policyData.Accepted);
//        //                editModel.Pets = policyData.Pets.ToString();
//        //                editModel.Note = policyData.Note;
//        //            }
//        //        }

//        //        html = PartialViewAsString(string.Format("Partials/AddPolicy/{0}", currentPolicy.PolicyCode), editModel);
//        //    }
//        //    else if (currentPolicy.PolicyCode == EnumPolicy.CreditCards)
//        //    {
//        //        var editModel = new PolicyCreditCardsModel();
//        //        editModel.Id = currentPolicy.Id;
//        //        editModel.ShopId = shopId;
//        //        editModel.PolicyId = currentPolicy.PolicyId;
//        //        editModel.PolicyCode = currentPolicy.PolicyCode;

//        //        if (!string.IsNullOrEmpty(currentPolicy.PolicyData))
//        //        {
//        //            var policyData = JsonConvert.DeserializeObject<MetaPolicyDataCreditCard>(currentPolicy.PolicyData);
//        //            if (policyData != null)
//        //            {
//        //                editModel.CreditCards = policyData.CreditCards;
//        //                editModel.Note = policyData.Note;

//        //                editModel.Credits = CommonHelpers.GetListCredit();
//        //            }
//        //        }

//        //        html = PartialViewAsString(string.Format("Partials/AddPolicy/{0}", currentPolicy.PolicyCode), editModel);
//        //    }

//        //    return Json(new { success = true, html = html }, JsonRequestBehavior.AllowGet); // success
//        //}

//        //public ActionResult UpdatePolicyCheckInOut(PolicyCheckInOutModel model)
//        //{
//        //    if (!ModelState.IsValid)
//        //    {
//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }

//        //    try
//        //    {
//        //        List<MetaShopPolicy> myList = null;

//        //        if (model.ShopId <= 0)
//        //        {
//        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//        //        }

//        //        var metaPolicies = _mainStore.GetPolicies(model.ShopId);

//        //        if (!string.IsNullOrEmpty(metaPolicies))
//        //        {
//        //            myList = JsonConvert.DeserializeObject<List<MetaShopPolicy>>(metaPolicies);
//        //        }

//        //        if (myList == null)
//        //        {
//        //            myList = new List<MetaShopPolicy>();
//        //        }

//        //        //New policy data 
//        //        var newPolicy = new MetaPolicyDataCheckInOut();
//        //        newPolicy.CheckIn = model.CheckIn;
//        //        newPolicy.CheckOut = model.CheckOut;
//        //        newPolicy.Note = model.Note;
//        //        newPolicy.CreatedDate = DateTime.Now;

//        //        //New item
//        //        var newItem = new MetaShopPolicy();
//        //        newItem.Id = Utility.GenTranIdWithPrefix(string.Format("{0}{1}", model.ShopId, "_"));
//        //        newItem.PolicyId = model.PolicyId;
//        //        newItem.PolicyCode = model.PolicyCode;
//        //        newItem.PolicyData = JsonConvert.SerializeObject(newPolicy);

//        //        if (myList != null && myList.Count > 0)
//        //        {
//        //            //Remove if any
//        //            myList.RemoveAll(x => x.PolicyId == model.PolicyId && x.PolicyCode.Equals(model.PolicyCode, StringComparison.CurrentCultureIgnoreCase));
//        //        }

//        //        //Append to current list
//        //        myList.Add(newItem);

//        //        var myListJson = JsonConvert.SerializeObject(myList);

//        //        //Update data
//        //        _mainStore.UpdatePolicies(model.ShopId, myListJson);

//        //        var html = PartialViewAsString(string.Format("Partials/Policies/{0}", model.PolicyCode), newItem);

//        //        return Json(new { success = true, html = html }); // success       
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error("Failed to UpdatePolicyCheckInOut because: " + ex.ToString());

//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }
//        //}

//        //public ActionResult UpdateChildPolicyCommon(PolicyCommonChildModel model)
//        //{
//        //    var html = string.Empty;

//        //    if (!ModelState.IsValid)
//        //    {
//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }

//        //    try
//        //    {
//        //        List<MetaShopPolicy> myList = null;
//        //        MetaShopPolicy currentPolicy = null;

//        //        if (model.ShopId <= 0)
//        //        {
//        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//        //        }

//        //        var metaPolicies = _mainStore.GetPolicies(model.ShopId);

//        //        if (!string.IsNullOrEmpty(metaPolicies))
//        //        {
//        //            myList = JsonConvert.DeserializeObject<List<MetaShopPolicy>>(metaPolicies);
//        //        }

//        //        if (myList == null)
//        //        {
//        //            myList = new List<MetaShopPolicy>();
//        //        }

//        //        //New item
//        //        var newItem = new MetaShopPolicy();
//        //        newItem.Id = Utility.GenTranIdWithPrefix(string.Format("{0}{1}", model.ShopId, "_"));
//        //        newItem.PolicyId = model.PolicyId;
//        //        newItem.PolicyCode = model.PolicyCode;

//        //        //New policy data 
//        //        var newPolicy = new MetaChildCommon();
//        //        newPolicy.AcceptGuest = Utils.ConvertToInt32(model.AcceptGuest);
//        //        newPolicy.ChildrenOver = Utils.ConvertToInt32(model.ChildrenOver);
//        //        newPolicy.PolicyDes = model.PolicyDes;
//        //        newPolicy.CreatedDate = DateTime.Now;

//        //        if (myList != null && myList.Count > 0)
//        //        {
//        //            //Take one if any
//        //            currentPolicy = myList.Where(x => x.PolicyId == model.PolicyId && x.PolicyCode.Equals(model.PolicyCode, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
//        //        }

//        //        if (currentPolicy == null)
//        //        {
//        //            newItem.PolicyData = JsonConvert.SerializeObject(newPolicy);

//        //            //Append to current list
//        //            myList.Add(newItem);

//        //            var myListJson = JsonConvert.SerializeObject(myList);

//        //            //Update data
//        //            _mainStore.UpdatePolicies(model.ShopId, myListJson);

//        //            html = PartialViewAsString(string.Format("Partials/Policies/{0}", model.PolicyCode), newItem);
//        //        }
//        //        else
//        //        {
//        //            if (!string.IsNullOrEmpty(currentPolicy.PolicyData))
//        //            {
//        //                var policyDataCommon = JsonConvert.DeserializeObject<MetaChildCommon>(currentPolicy.PolicyData);

//        //                //Update if existed
//        //                policyDataCommon.AcceptGuest = newPolicy.AcceptGuest;
//        //                policyDataCommon.ChildrenOver = newPolicy.ChildrenOver;
//        //                policyDataCommon.PolicyDes = newPolicy.PolicyDes;
//        //                policyDataCommon.CreatedDate = DateTime.Now;

//        //                //Serialize again
//        //                currentPolicy.PolicyData = JsonConvert.SerializeObject(policyDataCommon);
//        //            }
//        //            else
//        //            {
//        //                //Serialize if not existed
//        //                currentPolicy.PolicyData = JsonConvert.SerializeObject(newPolicy);
//        //            }

//        //            //Remove old data from list
//        //            myList.RemoveAll(x => x.PolicyId == model.PolicyId && x.PolicyCode.Equals(model.PolicyCode, StringComparison.CurrentCultureIgnoreCase));

//        //            //Add new item to list
//        //            myList.Add(currentPolicy);

//        //            var myListJson = JsonConvert.SerializeObject(myList);

//        //            //Update data
//        //            _mainStore.UpdatePolicies(model.ShopId, myListJson);

//        //            html = PartialViewAsString(string.Format("Partials/Policies/{0}", model.PolicyCode), currentPolicy);
//        //        }

//        //        return Json(new { success = true, html = html }); // success       
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error("Failed to UpdateChildPolicyCommon because: " + ex.ToString());

//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }
//        //}

//        //public ActionResult AddExtraBed(int shopId, int policyId, string policyCode)
//        //{
//        //    if (!ModelState.IsValid)
//        //    {
//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy }, JsonRequestBehavior.AllowGet);
//        //    }

//        //    try
//        //    {
//        //        if (shopId <= 0)
//        //        {
//        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//        //        }

//        //        var html = string.Empty;

//        //        var createModel = new PolicyChildAndExtraBedModel();
//        //        createModel.Id = Utility.GenTranIdWithPrefix(string.Format("{0}{1}", shopId, "_"));
//        //        createModel.ShopId = shopId;
//        //        createModel.PolicyId = policyId;
//        //        createModel.PolicyCode = policyCode;

//        //        createModel.Currencies = CommonHelpers.GetListCurrency();

//        //        return PartialView(string.Format("Partials/AddPolicy/{0}", "CHILD_EXTRA_SPECIAL"), createModel);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error("Failed to AddExtraBed because: " + ex.ToString());

//        //        return Content(NotifSettings.Error_SystemBusy);
//        //    }
//        //}

//        //public ActionResult UpdateSpecialChildPolicy(PolicyChildAndExtraBedModel model)
//        //{
//        //    var html = string.Empty;

//        //    if (!ModelState.IsValid)
//        //    {
//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }

//        //    try
//        //    {
//        //        List<MetaShopPolicy> myList = null;
//        //        MetaShopPolicy currentPolicy = null;
//        //        List<MetaPolicyDataChildAndBeds> currentExtraBeds = null;

//        //        if (model.ShopId <= 0)
//        //        {
//        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//        //        }

//        //        var metaPolicies = _mainStore.GetPolicies(model.ShopId);

//        //        if (!string.IsNullOrEmpty(metaPolicies))
//        //        {
//        //            myList = JsonConvert.DeserializeObject<List<MetaShopPolicy>>(metaPolicies);
//        //        }

//        //        if (myList == null)
//        //        {
//        //            myList = new List<MetaShopPolicy>();
//        //        }

//        //        //New item
//        //        var newItem = new MetaShopPolicy();
//        //        newItem.Id = Utility.GenTranIdWithPrefix(string.Format("{0}{1}", model.ShopId, "_"));
//        //        newItem.PolicyId = model.PolicyId;
//        //        newItem.PolicyCode = model.PolicyCode;

//        //        //New policy data 
//        //        var newPolicy = new MetaPolicyDataChildAndBeds();
//        //        newPolicy.Id = Utility.GenTranIdWithPrefix(string.Format("{0}_{1}_", model.ShopId, model.PolicyId));
//        //        newPolicy.AgeFrom = Utils.ConvertToInt32(model.AgeFrom);
//        //        newPolicy.AgeTo = Utils.ConvertToInt32(model.AgeTo);
//        //        newPolicy.ExtraInfantBed = (model.ExtraInfantBed) ? 1 : 0;
//        //        newPolicy.ExtraInfantBedSurcharge = Utils.ConvertToDecimal(model.ExtraInfantBedSurcharge);
//        //        newPolicy.BedShare = (model.BedShare) ? 1 : 0;
//        //        newPolicy.BedShareSurcharge = Utils.ConvertToDecimal(model.BedShareSurcharge);
//        //        newPolicy.BreakFast = (model.BreakFast) ? 1 : 0;
//        //        newPolicy.BreakFastSurcharge = Utils.ConvertToDecimal(model.BreakFastSurcharge);
//        //        newPolicy.CurrencyCode = model.CurrencyCode;
//        //        newPolicy.Note = model.Note;

//        //        newPolicy.CreatedDate = DateTime.Now;

//        //        if (myList != null && myList.Count > 0)
//        //        {
//        //            //Take one if any
//        //            currentPolicy = myList.Where(x => x.PolicyId == model.PolicyId && x.PolicyCode.Equals(model.PolicyCode, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
//        //        }

//        //        if (currentPolicy == null)
//        //        {
//        //            newItem.PolicyData = JsonConvert.SerializeObject(newPolicy);

//        //            //Append to current list
//        //            myList.Add(newItem);

//        //            var myListJson = JsonConvert.SerializeObject(myList);

//        //            //Update data
//        //            _mainStore.UpdatePolicies(model.ShopId, myListJson);

//        //            html = PartialViewAsString(string.Format("Partials/Policies/{0}", model.PolicyCode), newItem);
//        //        }
//        //        else
//        //        {
//        //            if (!string.IsNullOrEmpty(currentPolicy.PolicyData))
//        //            {
//        //                var policyDataCommon = JsonConvert.DeserializeObject<MetaChildCommon>(currentPolicy.PolicyData);

//        //                //if existed
//        //                if (policyDataCommon != null)
//        //                {
//        //                    currentExtraBeds = policyDataCommon.ChildrenExtraBeds;
//        //                }

//        //                if (currentExtraBeds == null)
//        //                {
//        //                    currentExtraBeds = new List<MetaPolicyDataChildAndBeds>();
//        //                }

//        //                //Add new policy to list
//        //                currentExtraBeds.Insert(0, newPolicy);

//        //                policyDataCommon.ChildrenExtraBeds = currentExtraBeds;

//        //                //Serialize again
//        //                currentPolicy.PolicyData = JsonConvert.SerializeObject(policyDataCommon);
//        //            }
//        //            else
//        //            {
//        //                //Serialize if not existed
//        //                currentPolicy.PolicyData = JsonConvert.SerializeObject(newPolicy);
//        //            }

//        //            //Remove old data from list
//        //            myList.RemoveAll(x => x.PolicyId == model.PolicyId && x.PolicyCode.Equals(model.PolicyCode, StringComparison.CurrentCultureIgnoreCase));

//        //            //Add new item to list
//        //            myList.Add(currentPolicy);

//        //            var myListJson = JsonConvert.SerializeObject(myList);

//        //            //Update data
//        //            _mainStore.UpdatePolicies(model.ShopId, myListJson);

//        //            html = PartialViewAsString(string.Format("Partials/Policies/{0}", model.PolicyCode), currentPolicy);
//        //        }

//        //        return Json(new { success = true, html = html }); // success       
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error("Failed to UpdateSpecialChildPolicy because: " + ex.ToString());

//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }
//        //}

//        //public ActionResult DeleteChildPolicy(int shopId, string id, int policyId, string policyCode)
//        //{
//        //    if (!ModelState.IsValid)
//        //    {
//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy }, JsonRequestBehavior.AllowGet);
//        //    }

//        //    try
//        //    {
//        //        List<MetaShopPolicy> myList = null;
//        //        MetaShopPolicy currentPolicy = null;

//        //        if (shopId <= 0)
//        //        {
//        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//        //        }

//        //        var metaPolicies = _mainStore.GetPolicies(shopId);

//        //        if (!string.IsNullOrEmpty(metaPolicies))
//        //        {
//        //            myList = JsonConvert.DeserializeObject<List<MetaShopPolicy>>(metaPolicies);
//        //        }

//        //        if (myList == null)
//        //        {
//        //            myList = new List<MetaShopPolicy>();
//        //        }

//        //        if (myList != null && myList.Count > 0)
//        //        {
//        //            //Take one if any
//        //            currentPolicy = myList.Where(x => x.PolicyId == policyId && x.PolicyCode.Equals(policyCode, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
//        //        }

//        //        if (currentPolicy != null)
//        //        {
//        //            if (!string.IsNullOrEmpty(currentPolicy.PolicyData))
//        //            {
//        //                var policyDataCommon = JsonConvert.DeserializeObject<MetaChildCommon>(currentPolicy.PolicyData);
//        //                List<MetaPolicyDataChildAndBeds> currentExtraBeds = null;

//        //                //if existed
//        //                if (policyDataCommon != null)
//        //                {
//        //                    currentExtraBeds = policyDataCommon.ChildrenExtraBeds;
//        //                }

//        //                if (currentExtraBeds == null)
//        //                {
//        //                    currentExtraBeds = new List<MetaPolicyDataChildAndBeds>();
//        //                }

//        //                if (currentExtraBeds.Count > 0)
//        //                {
//        //                    //Delete policy from list
//        //                    currentExtraBeds.RemoveAll(x => x.Id == id);
//        //                }

//        //                policyDataCommon.ChildrenExtraBeds = currentExtraBeds;

//        //                //Serialize again
//        //                currentPolicy.PolicyData = JsonConvert.SerializeObject(policyDataCommon);
//        //            }

//        //            //Remove old data from list
//        //            myList.RemoveAll(x => x.PolicyId == policyId && x.PolicyCode.Equals(policyCode, StringComparison.CurrentCultureIgnoreCase));

//        //            //Add new item to list
//        //            myList.Add(currentPolicy);

//        //            var myListJson = JsonConvert.SerializeObject(myList);

//        //            //Update data
//        //            _mainStore.UpdatePolicies(shopId, myListJson);

//        //            var html = PartialViewAsString(string.Format("Partials/Policies/{0}", policyCode), currentPolicy);

//        //            return Json(new { success = true, html = html }, JsonRequestBehavior.AllowGet); // success
//        //        }
//        //        else
//        //        {
//        //            logger.Error("Failed to DeleteChildPolicy due to the current policy could not be identified");

//        //            return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy }, JsonRequestBehavior.AllowGet);
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error("Failed to DeleteChildPolicy because: " + ex.ToString());

//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy }, JsonRequestBehavior.AllowGet);
//        //    }
//        //}

//        //public ActionResult UpdateDinningPolicy(PolicyDinningModel model)
//        //{
//        //    var html = string.Empty;

//        //    if (!ModelState.IsValid)
//        //    {
//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }

//        //    try
//        //    {
//        //        List<MetaShopPolicy> myList = null;
//        //        MetaShopPolicy currentPolicy = null;
//        //        List<MetaPolicyDataDinning> currentDinningList = null;

//        //        if (model.ShopId <= 0)
//        //        {
//        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//        //        }

//        //        var metaPolicies = _mainStore.GetPolicies(model.ShopId);

//        //        if (!string.IsNullOrEmpty(metaPolicies))
//        //        {
//        //            myList = JsonConvert.DeserializeObject<List<MetaShopPolicy>>(metaPolicies);
//        //        }

//        //        if (myList == null)
//        //        {
//        //            myList = new List<MetaShopPolicy>();
//        //        }

//        //        //New item
//        //        var newItem = new MetaShopPolicy();
//        //        newItem.Id = Utility.GenTranIdWithPrefix(string.Format("{0}{1}", model.ShopId, "_"));
//        //        newItem.PolicyId = model.PolicyId;
//        //        newItem.PolicyCode = model.PolicyCode;

//        //        //New policy data 
//        //        var newPolicy = new MetaPolicyDataDinning();
//        //        newPolicy.Id = Utility.GenTranIdWithPrefix(string.Format("{0}_{1}_", model.ShopId, model.PolicyId));
//        //        newPolicy.DinningType = model.DinningType;
//        //        newPolicy.IsBuffet = (model.IsBuffet) ? 1 : 0;
//        //        newPolicy.Surcharge = Utils.ConvertToDecimal(model.Surcharge);
//        //        newPolicy.CurrencyCode = model.CurrencyCode;
//        //        newPolicy.Note = model.Note;
//        //        newPolicy.CreatedDate = DateTime.Now;

//        //        if (myList != null && myList.Count > 0)
//        //        {
//        //            //Take one if any
//        //            currentPolicy = myList.Where(x => x.PolicyId == model.PolicyId && x.PolicyCode.Equals(model.PolicyCode, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
//        //        }

//        //        if (currentPolicy == null)
//        //        {
//        //            currentDinningList = new List<MetaPolicyDataDinning>();

//        //            //Add new policy to list
//        //            currentDinningList.Add(newPolicy);

//        //            newItem.PolicyData = JsonConvert.SerializeObject(currentDinningList);

//        //            //Append to current list
//        //            myList.Add(newItem);

//        //            var myListJson = JsonConvert.SerializeObject(myList);

//        //            //Update data
//        //            _mainStore.UpdatePolicies(model.ShopId, myListJson);

//        //            html = PartialViewAsString(string.Format("Partials/Policies/{0}", model.PolicyCode), newItem);
//        //        }
//        //        else
//        //        {
//        //            if (!string.IsNullOrEmpty(currentPolicy.PolicyData))
//        //            {
//        //                currentDinningList = JsonConvert.DeserializeObject<List<MetaPolicyDataDinning>>(currentPolicy.PolicyData);

//        //                if (currentDinningList == null)
//        //                {
//        //                    currentDinningList = new List<MetaPolicyDataDinning>();
//        //                }

//        //                //Add new policy to list
//        //                currentDinningList.Insert(0, newPolicy);

//        //                //Serialize again
//        //                currentPolicy.PolicyData = JsonConvert.SerializeObject(currentDinningList);
//        //            }
//        //            else
//        //            {
//        //                currentDinningList = new List<MetaPolicyDataDinning>();

//        //                //Add new policy to list
//        //                currentDinningList.Add(newPolicy);

//        //                //Serialize if not existed
//        //                currentPolicy.PolicyData = JsonConvert.SerializeObject(currentDinningList);
//        //            }

//        //            //Remove old data from list
//        //            myList.RemoveAll(x => x.PolicyId == model.PolicyId && x.PolicyCode.Equals(model.PolicyCode, StringComparison.CurrentCultureIgnoreCase));

//        //            //Add new item to list
//        //            myList.Add(currentPolicy);

//        //            var myListJson = JsonConvert.SerializeObject(myList);

//        //            //Update data
//        //            _mainStore.UpdatePolicies(model.ShopId, myListJson);

//        //            html = PartialViewAsString(string.Format("Partials/Policies/{0}", model.PolicyCode), currentPolicy);
//        //        }

//        //        return Json(new { success = true, html = html }); // success       
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error("Failed to UpdateDinningPolicy because: " + ex.ToString());

//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }
//        //}

//        //public ActionResult DeleteDinningPolicy(int shopId, string id, int policyId, string policyCode)
//        //{
//        //    if (!ModelState.IsValid)
//        //    {
//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy }, JsonRequestBehavior.AllowGet);
//        //    }

//        //    try
//        //    {
//        //        List<MetaShopPolicy> myList = null;
//        //        MetaShopPolicy currentPolicy = null;

//        //        if (shopId <= 0)
//        //        {
//        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//        //        }

//        //        var metaPolicies = _mainStore.GetPolicies(shopId);

//        //        if (!string.IsNullOrEmpty(metaPolicies))
//        //        {
//        //            myList = JsonConvert.DeserializeObject<List<MetaShopPolicy>>(metaPolicies);
//        //        }

//        //        if (myList == null)
//        //        {
//        //            myList = new List<MetaShopPolicy>();
//        //        }

//        //        if (myList != null && myList.Count > 0)
//        //        {
//        //            //Take one if any
//        //            currentPolicy = myList.Where(x => x.PolicyId == policyId && x.PolicyCode.Equals(policyCode, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
//        //        }

//        //        if (currentPolicy != null)
//        //        {
//        //            if (!string.IsNullOrEmpty(currentPolicy.PolicyData))
//        //            {
//        //                var policyDataCommon = JsonConvert.DeserializeObject<List<MetaPolicyDataDinning>>(currentPolicy.PolicyData);

//        //                if (policyDataCommon.Count > 0)
//        //                {
//        //                    //Delete policy from list
//        //                    policyDataCommon.RemoveAll(x => x.Id == id);
//        //                }

//        //                //Serialize again
//        //                currentPolicy.PolicyData = JsonConvert.SerializeObject(policyDataCommon);
//        //            }

//        //            //Remove old data from list
//        //            myList.RemoveAll(x => x.PolicyId == policyId && x.PolicyCode.Equals(policyCode, StringComparison.CurrentCultureIgnoreCase));

//        //            //Add new item to list
//        //            myList.Add(currentPolicy);

//        //            var myListJson = JsonConvert.SerializeObject(myList);

//        //            //Update data
//        //            _mainStore.UpdatePolicies(shopId, myListJson);

//        //            var html = PartialViewAsString(string.Format("Partials/Policies/{0}", policyCode), currentPolicy);

//        //            return Json(new { success = true, html = html }, JsonRequestBehavior.AllowGet); // success
//        //        }
//        //        else
//        //        {
//        //            logger.Error("Failed to DeleteDinningPolicy due to the current policy could not be identified");

//        //            return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy }, JsonRequestBehavior.AllowGet);
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error("Failed to DeleteDinningPolicy because: " + ex.ToString());

//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy }, JsonRequestBehavior.AllowGet);
//        //    }
//        //}

//        //public ActionResult UpdatePolicyPets(PolicyPetsModel model)
//        //{
//        //    if (!ModelState.IsValid)
//        //    {
//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }

//        //    try
//        //    {
//        //        List<MetaShopPolicy> myList = null;

//        //        if (model.ShopId <= 0)
//        //        {
//        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//        //        }

//        //        var metaPolicies = _mainStore.GetPolicies(model.ShopId);

//        //        if (!string.IsNullOrEmpty(metaPolicies))
//        //        {
//        //            myList = JsonConvert.DeserializeObject<List<MetaShopPolicy>>(metaPolicies);
//        //        }

//        //        if (myList == null)
//        //        {
//        //            myList = new List<MetaShopPolicy>();
//        //        }

//        //        //New policy data 
//        //        var newPolicy = new MetaPolicyDataPets();
//        //        newPolicy.Accepted = (model.Accepted) ? 1 : 0;
//        //        newPolicy.Pets = Utils.ConvertToInt32(model.Pets);
//        //        newPolicy.Note = model.Note;

//        //        //New item
//        //        var newItem = new MetaShopPolicy();
//        //        newItem.Id = Utility.GenTranIdWithPrefix(string.Format("{0}{1}", model.ShopId, "_"));
//        //        newItem.PolicyId = model.PolicyId;
//        //        newItem.PolicyCode = model.PolicyCode;
//        //        newItem.PolicyData = JsonConvert.SerializeObject(newPolicy);

//        //        if (myList != null && myList.Count > 0)
//        //        {
//        //            //Remove if any
//        //            myList.RemoveAll(x => x.PolicyId == model.PolicyId && x.PolicyCode.Equals(model.PolicyCode, StringComparison.CurrentCultureIgnoreCase));
//        //        }

//        //        //Append to current list
//        //        myList.Add(newItem);

//        //        var myListJson = JsonConvert.SerializeObject(myList);

//        //        //Update data
//        //        _mainStore.UpdatePolicies(model.ShopId, myListJson);

//        //        var html = PartialViewAsString(string.Format("Partials/Policies/{0}", model.PolicyCode), newItem);

//        //        return Json(new { success = true, html = html }); // success       
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error("Failed to UpdatePolicyPets because: " + ex.ToString());

//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }
//        //}

//        //public ActionResult UpdateCreditPolicy(PolicyCreditCardsModel model)
//        //{
//        //    if (!ModelState.IsValid)
//        //    {
//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }

//        //    try
//        //    {
//        //        List<MetaShopPolicy> myList = null;

//        //        if (model.ShopId <= 0)
//        //        {
//        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//        //        }

//        //        var metaPolicies = _mainStore.GetPolicies(model.ShopId);

//        //        if (!string.IsNullOrEmpty(metaPolicies))
//        //        {
//        //            myList = JsonConvert.DeserializeObject<List<MetaShopPolicy>>(metaPolicies);
//        //        }

//        //        if (myList == null)
//        //        {
//        //            myList = new List<MetaShopPolicy>();
//        //        }

//        //        //New policy data 
//        //        var newPolicy = new MetaPolicyDataCreditCard();
//        //        newPolicy.CreditCards = model.CreditCards;
//        //        newPolicy.Note = model.Note;

//        //        //New item
//        //        var newItem = new MetaShopPolicy();
//        //        newItem.Id = Utility.GenTranIdWithPrefix(string.Format("{0}{1}", model.ShopId, "_"));
//        //        newItem.PolicyId = model.PolicyId;
//        //        newItem.PolicyCode = model.PolicyCode;
//        //        newItem.PolicyData = JsonConvert.SerializeObject(newPolicy);

//        //        if (myList != null && myList.Count > 0)
//        //        {
//        //            //Remove if any
//        //            myList.RemoveAll(x => x.PolicyId == model.PolicyId && x.PolicyCode.Equals(model.PolicyCode, StringComparison.CurrentCultureIgnoreCase));
//        //        }

//        //        //Append to current list
//        //        myList.Add(newItem);

//        //        var myListJson = JsonConvert.SerializeObject(myList);

//        //        //Update data
//        //        _mainStore.UpdatePolicies(model.ShopId, myListJson);

//        //        var html = PartialViewAsString(string.Format("Partials/Policies/{0}", model.PolicyCode), newItem);

//        //        return Json(new { success = true, html = html }); // success       
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error("Failed to UpdateCreditPolicy because: " + ex.ToString());

//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }
//        //}

//        //[HttpPost]
//        //public ActionResult SavePayment(int Id, string selectedValues)
//        //{
//        //    try
//        //    {
//        //        _mainStore.UpdatePayments(Id, selectedValues);

//        //        return Json(new { success = true, message = NotifSettings.Success_Updated }); // success       
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error("Failed to SavePayment because: " + ex.ToString());

//        //        return Json(new { success = false, errorMessage = NotifSettings.Error_SystemBusy });
//        //    }
//        //}

//        //#endregion


//        //Show popup confirm delete        
//        //[AccessRoleChecker]
//        public ActionResult Delete(int id)
//        {
//            if (id <= 0)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            return PartialView("_DeleteInfo", id);
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
//                strError = NotifSettings.Error_SystemBusy;

//                logger.Error("Failed to get Delete Shop because: " + ex.ToString());

//                return Json(new { success = true, message = strError });
//            }

//            return Json(new { success = true, message = NotifSettings.Success_Deleted });
//        }

//        [HttpGet]
//        public ActionResult AssignCategory(int shopId)
//        {
//            if (shopId <= 0)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            ShopAssignCategoryModel model = new ShopAssignCategoryModel();
//            try
//            {
//                model.Id = shopId;
//                var shopInfo = _mainStore.GetById(shopId);
//                if (shopInfo != null)
//                    model.ShopCategory = shopInfo.CategoryId;
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying load AssignCategory: {0}", ex.ToString());
//                logger.Error(strError);
//            }

//            return PartialView("../Shop/Partials/_AssignCategory", model);
//        }

//        [HttpPost, ActionName("AssignCategory")]
//        [ValidateAntiForgeryToken]
//        public ActionResult AcceptAssignCategory(ShopAssignCategoryModel model)
//        {
//            var msg = ManagerResource.LB_OPERATION_SUCCESS;
//            var isSuccess = false;
//            if (model.Id <= 0)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            try
//            {
//                var catId = Utils.ConvertToInt32(Request["cat"]);
//                if (catId > 0 && model.Id > 0)
//                {
//                    _mainStore.AssignCategory(model.Id, catId);
//                    isSuccess = true;
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying AssignCategory: {0}", ex.ToString());
//                logger.Error(strError);
//            }

//            return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = msg, clientcallback = "location.reload()" });
//        }

//        //Show popup        
//        //public ActionResult AssignCleaner(int orderId, int roomId)
//        //{
//        //    if (orderId <= 0 || roomId <= 0)
//        //    {
//        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//        //    }

//        //    var model = new ManageAddShopOwnerModel();
//        //    try
//        //    {
//        //        if (currentShopId > 0)
//        //        {
//        //            model.ListEmployees = AccountHelper.GetListEmployeeInShop(currentShopId);
//        //        }
//        //        else
//        //        {
//        //            logger.Error(string.Format("Could not get current shop"));
//        //            return PartialView("../Error/Error");
//        //        }

//        //        var roomInfo = _mainStore.GetById(roomId);

//        //        if (roomInfo != null)
//        //        {
//        //            if (roomInfo.ShopId != currentShopId)
//        //            {
//        //                logger.Error(string.Format("Room [{0}] is not exists on Shop [{1}]", roomId, currentShopId));
//        //                return PartialView("../Error/Error");
//        //            }
//        //        }
//        //        else
//        //        {
//        //            logger.Error(string.Format("Room [{0}] is not exists on Shop [{1}]", roomId, currentShopId));
//        //            return PartialView("../Error/Error");
//        //        }
//        //        model.RoomId = roomId;
//        //        model.OrderId = orderId;

//        //        var currentUserId = AccountHelper.GetCurrentUserId();
//        //        model.ListEmployees = AccountHelper.GetListEmployeeInShop(currentShopId);

//        //        //var returnListEmployess = new List<IdentityUser>();

//        //        //if (model.ListEmployees.HasData())
//        //        //{
//        //        //    foreach (var item in model.ListEmployees)
//        //        //    {
//        //        //        if (currentUserId != item.Id)
//        //        //            returnListEmployess.Add(item);
//        //        //    }
//        //        //}

//        //        //model.ListEmployees = returnListEmployess;

//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error("Failed for AssignCleaner popup action: " + ex.ToString());
//        //        return PartialView("../Error/Error");
//        //    }

//        //    return PartialView("../Room/Manage/Partials/_AssignCleaner", model);
//        //}

//        //[HttpPost, ActionName("AssignCleaner")]
//        //[ValidateAntiForgeryToken]
//        //public ActionResult AssignCleaner_Confirm(ManageAssignCleanerModel model)
//        //{
//        //    var strError = string.Empty;
//        //    if (!ModelState.IsValid)
//        //    {
//        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//        //    }

//        //    try
//        //    {
//        //        _mainStore.AssignCleaner(model.OrderId, model.EmployeeId);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        strError = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//        //        logger.Error("Failed to AssignCleaner because: " + ex.ToString());

//        //        return Json(new { success = false, message = strError });
//        //    }

//        //    return Json(new { success = true, message = UserWebResource.LB_ASSIGNED_SUCCESS, clientcallback = "location.reload();" }, JsonRequestBehavior.AllowGet);
//        //}

//        #region Helpers

//        private void DeleteImageByUrl(string url)
//        {
//            try
//            {
//                string fullPath = Request.MapPath(url);
//                if (System.IO.File.Exists(fullPath))
//                {
//                    System.IO.File.Delete(fullPath);
//                }
//            }
//            catch
//            {
//                // Deliberately empty.
//            }
//        }

//        private IdentityShop ExtractCreateFormData(ShopCreateModel formData)
//        {
//            var myIdetity = new IdentityShop();
//            myIdetity.IsInternal = (formData.IsInternal) ? 1 : 0;
//            myIdetity.Name = formData.Name;
//            myIdetity.Code = formData.Code;
//            myIdetity.ProviderId = formData.ProviderId;
//            myIdetity.Phone = formData.Phone;
//            myIdetity.Address = formData.Address;
//            myIdetity.ContactInfo = formData.ContactInfo;
//            myIdetity.AreaId = formData.AreaId;
//            myIdetity.CountryId = formData.CountryId;
//            myIdetity.ProvinceId = formData.ProvinceId;
//            myIdetity.DistrictId = formData.DistrictId;
//            myIdetity.Openned = formData.Openned;
//            myIdetity.PostCode = formData.PostCode;
//            myIdetity.Description = formData.Description;
//            myIdetity.CreatedBy = User.Identity.GetUserId();
//            myIdetity.Status = formData.Status.Value;
//            myIdetity.Email = formData.Email;

//            return myIdetity;
//        }

//        private IdentityShop ExtractEditFormData(ShopEditModel formData)
//        {
//            var myIdetity = new IdentityShop();
//            myIdetity.Id = formData.Id;

//            myIdetity.IsInternal = (formData.IsInternal) ? 1 : 0;
//            myIdetity.Name = formData.Name;
//            myIdetity.Code = formData.Code;
//            myIdetity.ProviderId = formData.ProviderId;
//            myIdetity.Phone = formData.Phone;
//            myIdetity.Address = formData.Address;
//            myIdetity.ContactInfo = formData.ContactInfo;
//            myIdetity.AreaId = formData.AreaId;
//            myIdetity.CountryId = formData.CountryId;
//            myIdetity.ProvinceId = formData.ProvinceId;
//            myIdetity.DistrictId = formData.DistrictId;
//            myIdetity.Openned = formData.Openned;
//            myIdetity.PostCode = formData.PostCode;
//            myIdetity.Description = formData.Description;
//            myIdetity.LastUpdatedBy = User.Identity.GetUserId();
//            myIdetity.Status = formData.Status.Value;
//            myIdetity.Email = formData.Email;

//            return myIdetity;
//        }

//        private ShopEditModel RenderEditModel(IdentityShop identity)
//        {
//            var editModel = new ShopEditModel();

//            editModel.Id = identity.Id;

//            editModel.IsInternal = Convert.ToBoolean(identity.IsInternal);
//            editModel.Name = identity.Name;
//            editModel.Code = identity.Code;
//            editModel.ProviderId = identity.ProviderId;
//            editModel.Phone = identity.Phone;
//            editModel.Address = identity.Address;
//            editModel.ContactInfo = identity.ContactInfo;
//            editModel.AreaId = identity.AreaId;
//            editModel.CountryId = identity.CountryId;
//            editModel.ProvinceId = identity.ProvinceId;
//            editModel.DistrictId = identity.DistrictId;
//            editModel.Openned = identity.Openned;
//            editModel.PostCode = identity.PostCode;
//            editModel.Description = identity.Description;
//            editModel.Longitude = identity.Longitude;
//            editModel.Latitude = identity.Latitude;

//            if (string.IsNullOrEmpty(editModel.Latitude) || editModel.Latitude == "0")
//            {
//                editModel.Latitude = MapSettings.DefaultLatitude;
//            }

//            if (string.IsNullOrEmpty(editModel.Longitude) || editModel.Longitude == "0")
//            {
//                editModel.Longitude = MapSettings.DefaultLongitude;
//            }

//            editModel.Status = identity.Status;
//            editModel.Email = identity.Email;

//            //Images
//            //editModel.Images = identity.Images;

//            //Meta data
//            //editModel.MetaData = identity.MetaData;

//            //Common list info
//            editModel.Areas = CommonHelpers.GetListArea();
//            //editModel.Providers = CommonHelpers.GetListProvider();
//            //editModel.Policies = CommonHelpers.GetListPolicy();

//            //Area info 
//            editModel.Countries = CommonHelpers.GetCountryByArea(editModel.AreaId);
//            editModel.Provinces = CommonHelpers.GetProvinceByCountry(editModel.CountryId);
//            editModel.Districts = CommonHelpers.GetDistrictByProvince(editModel.ProvinceId);

//            return editModel;
//        }

//        #endregion

//    }
//}