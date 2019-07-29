using Manager.SharedLib.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Resources;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.MsSqlStores;
using MsSql.AspNet.Identity.MsSqlStores.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Manager.WebApp.Controllers
{
    public class CompanyController : BaseAuthedController
    {
        // GET: Company
        private readonly IStoreCompany _mainStore;
        private readonly ILog logger = LogProvider.For<CompanyController>();

        public CompanyController(IStoreCompany mainStore)
        {
            _mainStore = mainStore;
        }

        //[AccessRoleChecker]
        public ActionResult Index(ManageCompanyModel model)
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

            var filter = new IdentityCompany
            {
                Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                //Status = model.Status == null ? -1 : (int)model.Status,
                //ProductCategoryId = model.ProductCategoryId == null ? 0 : (int)model.ProductCategoryId,
                //ProviderId = model.ProviderId == null ? 0 : (int)model.ProviderId
                Address = !string.IsNullOrEmpty(model.Address) ? model.Address.Trim() : null,
            };

            try
            {
                model.Countrys = CommonHelpers.GetListCountry();
                model.Provinces = CommonHelpers.GetListProvince();
                model.Districts = CommonHelpers.GetListDistrict();
                //model.Provinces = CommonHelpers.GetProvinceByCountry(model.ProvinceId);
                //model.Districts = CommonHelpers.GetDistrictByProvince(model.DistrictId);

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

        //public ActionResult Create()
        //{
        //    var createModel = new CompanyCreateModel();
        //    return View(createModel);
        //}

        public ActionResult Create()
        {
            var createModel = new CompanyCreateModel();

            try
            {
                //createModel.MinInventory = "0";
                createModel.Countrys = CommonHelpers.GetListCountry();
                //createModel.ProvinceId = CommonHelpers.GetProvinceByCountry(listCountry.Id);
                //createModel.DistrictId = CommonHelpers.GetDistrictByProvince(listProvince.Id);
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed display Create Product page: " + ex.ToString());
            }
            return View(createModel);
        }


        [HttpPost]
        public ActionResult Create(CompanyCreateModel model)
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

                logger.Error("Failed for Create Product request: " + ex.ToString());

                return View(model);
            }

            //return RedirectToAction("Edit/" + newId);
            return RedirectToAction("Create");
        }

        public ActionResult Edit(int id)
        {
            if (id ==0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                var info = _mainStore.GetById(id);
                if (info == null)
                    return RedirectToErrorPage();


                var editModel = RenderEditModel(info);

                return View(editModel);

            }
            catch (Exception ex)
            {

                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit Product request: " + ex.ToString());
            }

            return View(new CompanyEditModel());

        }

        [HttpPost]
        public ActionResult Test(int id, string name)
        {

            return Json(new { success = true, message = "Thanh cong" });

        }


        [HttpPost]
        public ActionResult GetDetail(int id)
        {
            var data = _mainStore.GetById(id);

            return PartialView("_Detail", data);
            //return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
        }

        [HttpPost]
        public ActionResult Edit(CompanyEditModel model)
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

                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit Product request: " + ex.ToString());

                return View(model);
            }

            return RedirectToAction("Edit/" + model.Id);

        }

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

        public ActionResult Delete_comfirm(int id)
        {
            var strError = string.Empty;
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                _mainStore.Delete(id);

                CachingHelpers.ClearProductCache(id);
            }
            catch (Exception ex )
            {

                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Company because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
        }

        //  
        public IdentityCompany ExtractCreateFormData(CompanyCreateModel formData)
        {
            var myIdentity = new IdentityCompany();

            myIdentity.Code = formData.Code;
            myIdentity.Name = formData.Name;
            myIdentity.CountryId = formData.CountryId;
            myIdentity.ProvinceId = formData.ProvinceId;
            myIdentity.DistrictId = formData.DistrictId;
            myIdentity.Address = formData.Address;
            myIdentity.Email = formData.Email;
            myIdentity.Phone = formData.Phone;
            myIdentity.IsEPE = formData.IsEPE;
            myIdentity.Status = formData.Status;

            return myIdentity;
        }

        public IdentityCompany ExtractEditFormData(CompanyEditModel formData)
        {
            var myIdentity = new IdentityCompany();

            myIdentity.Id = formData.Id;
            myIdentity.Code = formData.Code;
            myIdentity.Name = formData.Name;
            myIdentity.CountryId = formData.CountryId;
            myIdentity.ProvinceId = formData.ProvinceId;
            myIdentity.DistrictId = formData.DistrictId;
            myIdentity.Address = formData.Address;
            myIdentity.Email = formData.Email;
            myIdentity.Phone = formData.Phone;
            myIdentity.IsEPE = formData.IsEPE;
            myIdentity.Status = formData.Status;

            return myIdentity;
        }


        public CompanyEditModel RenderEditModel(IdentityCompany identity)
        {
            var editModel = new CompanyEditModel();

            editModel.Code = identity.Code;
            editModel.Name = identity.Name;
            editModel.CountryId = identity.CountryId;
            editModel.ProvinceId = identity.ProvinceId;
            editModel.DistrictId = identity.DistrictId;
            editModel.Countrys = CommonHelpers.GetListCountry();
            editModel.Provinces = CommonHelpers.GetProvinceByCountry(identity.CountryId);
            editModel.Districts = CommonHelpers.GetDistrictByProvince(identity.ProvinceId);
            editModel.Address = identity.Address;
            editModel.Email = identity.Email;
            editModel.Phone = identity.Phone;
            editModel.IsEPE = identity.IsEPE;
            editModel.Status = identity.Status;

            return editModel;

        }
    }
}