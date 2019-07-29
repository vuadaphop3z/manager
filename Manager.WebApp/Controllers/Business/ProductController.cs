using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Manager.SharedLib.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity.Entities;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.MsSqlStores;
using Manager.WebApp.Models;
using Manager.WebApp.Services;
using System.Collections.Generic;

namespace Manager.WebApp.Controllers
{
    public class ProductController : BaseAuthedController
    {
        private readonly IStoreProduct _mainStore;
        private readonly ILog logger = LogProvider.For<ProductController>();

        public ProductController(IStoreProduct mainStore)
        {
            _mainStore = mainStore;
        }

        [AccessRoleChecker]
        public ActionResult Index(ManageProductModel model)
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

            var filter = new IdentityProduct
            {
                Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                Status = model.Status == null ? -1 : (int)model.Status,
                ProductCategoryId = model.ProductCategoryId == null ? 0 : (int)model.ProductCategoryId,
                ProviderId = model.ProviderId == null ? 0 : (int)model.ProviderId
            };

            try
            {
                model.Providers = CommonHelpers.GetListProvider();
                model.ProductCategories = CommonHelpers.GetListProductCategory();
                model.Units = CommonHelpers.GetListUnit();

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

        [AccessRoleChecker]
        public ActionResult Create()
        {
            var createModel = new ProductCreateModel();
            try
            {
                createModel.MinInventory = "0";
                createModel.Providers = CommonHelpers.GetListProvider();
                createModel.ProductCategories = CommonHelpers.GetListProductCategory();
                createModel.Units = CommonHelpers.GetListUnit();
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed display Create Product page: " + ex.ToString());
            }
            return View(createModel);
        }

        [HttpPost]
        [AccessRoleChecker]
        public ActionResult Create(ProductCreateModel model)
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
                //Extract info
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
                model.Providers = CommonHelpers.GetListProvider();
                model.ProductCategories = CommonHelpers.GetListProductCategory();
                model.Units = CommonHelpers.GetListUnit();

                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Create Product request: " + ex.ToString());

                return View(model);
            }

            return RedirectToAction("Edit/" + newId);
        }

        [AccessRoleChecker]
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

                if (info == null)
                    return RedirectToErrorPage();

                //Render to view model
                var editModel = RenderEditModel(info);

                return View(editModel);
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit Product request: " + ex.ToString());
            }

            return View(new ProductEditModel());
        }

        [HttpPost]
        public ActionResult Edit(ProductEditModel model)
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
                //Extract data
                var info = ExtractEditFormData(model);

                var isSuccess = _mainStore.Update(info);

                //Clear cache
                CachingHelpers.ClearProductCache(info.Id);

                if (isSuccess)
                {
                    this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
                }
            }
            catch (Exception ex)
            {
                model.Providers = CommonHelpers.GetListProvider();
                model.ProductCategories = CommonHelpers.GetListProductCategory();
                model.Units = CommonHelpers.GetListUnit();

                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit Product request: " + ex.ToString());

                return View(model);
            }

            return RedirectToAction("Edit/" + model.Id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetListProductInDropdown()
        {
            var strError = string.Empty;
            try
            {
                var keyword = (Request["query"] != null) ? Request["query"].ToString() : string.Empty;
                int currentPage = 1;
                int pageSize = SystemSettings.DefaultPageSize;

                var filter = new IdentityProduct
                {
                    Keyword = !string.IsNullOrEmpty(keyword) ? keyword.Trim() : null
                };

                var listProducts = _mainStore.GetActiveForChoosen(filter, currentPage, pageSize);

                List<ProductItemInDropdownListModel> returnList = new List<ProductItemInDropdownListModel>();

                var listUnits = CommonHelpers.GetListUnit();
                if (listProducts.HasData())
                {
                    foreach (var prd in listProducts)
                    {
                        var item = new ProductItemInDropdownListModel();
                        item.Id = prd.Id;
                        item.Name = prd.Name;
                        item.Code = prd.Code;
                        item.WarehouseNum = Utils.DoubleToStringFormat(prd.WarehouseNum);

                        var currentUnit = listUnits.Where(x => x.Id == prd.UnitId).FirstOrDefault();

                        if (currentUnit != null)
                            item.UnitName = currentUnit.Name;

                        returnList.Add(item);
                    }
                }

                return Json(new { success = true, data = returnList });

            }
            catch (Exception ex)
            {
                strError = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to GetListProductInDropdown because: " + ex.ToString());

                return Json(new { success = false, data = string.Empty, message = strError });
            }
        }

        public ActionResult ChoosenProduct(ProductChoosenModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (Request["Page"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
            }

            var filter = new IdentityProduct
            {
                Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                Status = model.Status == null ? -1 : (int)model.Status,
                ProductCategoryId = model.ProductCategoryId == null ? 0 : (int)model.ProductCategoryId,
                ProviderId = model.ProviderId == null ? 0 : (int)model.ProviderId
            };

            try
            {
                model.Providers = CommonHelpers.GetListProvider();
                model.ProductCategories = CommonHelpers.GetListProductCategory();
                model.Units = CommonHelpers.GetListUnit();

                model.SearchResults = _mainStore.GetActiveForChoosen(filter, currentPage, SystemSettings.DefaultPageSize);
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

                logger.Error("Failed to show ChoosenProduct form: " + ex.ToString());
            }

            return PartialView("_ChoosenProduct", model);           
        }

        [HttpPost]
        public ActionResult ChoosenProductSearch(ProductChoosenModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (Request["Page"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
            }

            if (Request["CurrentPage"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["CurrentPage"], 1);
            }

            var filter = new IdentityProduct
            {
                Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                ProductCategoryId = model.ProductCategoryId == null ? 0 : (int)model.ProductCategoryId,
                ProviderId = model.ProviderId == null ? 0 : (int)model.ProviderId
            };

            try
            {
                model.Providers = CommonHelpers.GetListProvider();
                model.ProductCategories = CommonHelpers.GetListProductCategory();
                model.Units = CommonHelpers.GetListUnit();

                model.SearchResults = _mainStore.GetActiveForChoosen(filter, currentPage, SystemSettings.DefaultPageSize);
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

                logger.Error("Failed to show _ChoosenProductList form: " + ex.ToString());
            }

            return PartialView("_ChoosenProductList", model);
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

                //Clear cache
                CachingHelpers.ClearProductCache(id);
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Product because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
        }

        #region Helpers

        private IdentityProduct ExtractCreateFormData(ProductCreateModel formData)
        {
            var myIdetity = new IdentityProduct();
            myIdetity.Name = formData.Name;
            myIdetity.UnitId = formData.UnitId;
            myIdetity.ProviderId = formData.ProviderId;
            myIdetity.ProductCategoryId = formData.ProductCategoryId;
            myIdetity.MinInventory = Utils.ConvertToDouble(formData.MinInventory);
            myIdetity.Code = formData.Code;
            myIdetity.CreatedBy = GetCurrentStaffId();
            myIdetity.Status = formData.Status;

            return myIdetity;
        }

        private IdentityProduct ExtractEditFormData(ProductEditModel formData)
        {
            var myIdetity = new IdentityProduct();
            myIdetity.Id = formData.Id;

            myIdetity.Name = formData.Name;
            myIdetity.UnitId = formData.UnitId;
            myIdetity.ProviderId = formData.ProviderId;
            myIdetity.ProductCategoryId = formData.ProductCategoryId;
            myIdetity.Code = formData.Code;
            myIdetity.MinInventory = Utils.ConvertToDouble(formData.MinInventory);
            myIdetity.LastUpdatedBy = GetCurrentStaffId();
            myIdetity.Status = formData.Status;

            return myIdetity;
        }

        private ProductEditModel RenderEditModel(IdentityProduct identity)
        {
            var editModel = new ProductEditModel();

            editModel.Id = identity.Id;
            editModel.Name = identity.Name;
            editModel.UnitId = identity.UnitId;
            editModel.ProviderId = identity.ProviderId;
            editModel.ProductCategoryId = identity.ProductCategoryId;
            editModel.Code = identity.Code;
            editModel.MinInventory = Utils.DoubleToStringFormat(identity.MinInventory);
            editModel.Status = identity.Status;
            editModel.Providers = CommonHelpers.GetListProvider();
            editModel.ProductCategories = CommonHelpers.GetListProductCategory();
            editModel.Units = CommonHelpers.GetListUnit();

            return editModel;
        }

        #endregion

    }
}