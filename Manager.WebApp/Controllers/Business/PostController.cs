using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Manager.SharedLib.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Resources;
using Manager.WebApp.Services;
using Manager.WebApp.Settings;
using Microsoft.AspNet.Identity;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Stores;
using Newtonsoft.Json;

namespace Manager.WebApp.Controllers
{
    public class PostController : BaseAuthedController
    {
        private readonly IStorePost _mainStore;
        private readonly ILog logger = LogProvider.For<PostController>();

        public PostController(IStorePost mainStore)
        {
            _mainStore = mainStore;
        }

        [AccessRoleChecker]
        public ActionResult Index(ManagePostModel model)
        {
            return View(model);
        }

        [HttpPost]
        public JsonResult GetListPost()
        {
            var currentPage = 1;
            var status = -1;
            var keyword = string.Empty;
            var pageSize = (Request["pagination[perpage]"] != null) ? Utils.ConvertToInt32(Request["pagination[perpage]"]) : SystemSettings.DefaultPageSize;

            if (Request["pagination[page]"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["pagination[page]"], 1);
            }

            if (Request["query[Status]"] != null)
            {
                status = Utils.ConvertToInt32(Request["query[Status]"], -1);
            }

            if (Request["query[generalSearch]"] != null)
            {
                keyword = Request["query[generalSearch]"].ToString();
            }

            var model = new ManagePostModel();
            model.meta = new CommonMetaPagingModel();

            model.meta.field = Request["sort[field]"];
            model.meta.sort = Request["sort[sort]"];

            var filter = new IdentityPost
            {
                //Name = !string.IsNullOrEmpty(model.Name) ? model.Name.Trim() : null,
                //Phone = !string.IsNullOrEmpty(model.Phone) ? model.Phone.Trim() : null,
                //Code = !string.IsNullOrEmpty(model.Code) ? model.Code.Trim() : null,                
                //FromDate = string.IsNullOrEmpty(model.FromDate) ? null : (DateTime?)DateTime.ParseExact(model.FromDate, ManagePostModel.DATETIME_FORMAT, null),
                //ToDate = string.IsNullOrEmpty(model.ToDate) ? null : (DateTime?)DateTime.ParseExact(model.ToDate, ManagePostModel.DATETIME_FORMAT, null),
                Keyword = keyword,
                PageIndex = currentPage,
                PageSize = pageSize,
                SortField = model.meta.field,
                SortType = model.meta.sort,
                Status = status
            };

            try
            {
                model.data = _mainStore.GetByPage(filter);

                model.meta.perpage = pageSize;
                if (model.data != null && model.data.Count > 0)
                {
                    model.meta.total = model.data[0].TotalCount;
                    model.meta.page = currentPage;
                    model.meta.pages = (int)Math.Ceiling((double)model.meta.total / (double)model.meta.perpage);
                }

                if (model.data.HasData())
                {
                    foreach (var item in model.data)
                    {
                        item.Cover = SocialCdnHelper.GetFullImgPath(item.Cover);
                        item.CreatedDateLabel = item.CreatedDate.DateTimeQuestToString();

                        foreach (var type in Enum.GetValues(typeof(EnumPostType)))
                        {
                            if((int)type == item.PostType)
                                item.PostTypeLabel = EnumExtensions.GetEnumDescription((Enum)type);
                        }
                    }
                }

                return Json(new { data = model.data, meta = model.meta }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to GetListPost because: " + ex.ToString());
            }

            return Json(new { model }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            ModelState.Clear();
            var model = new PostEditModel();
            model.BodyContent = string.Empty;

            return View(model);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Post(PostEditModel model)
        {
            try
            {
                var coverImg = string.Empty;
                if(model.Cover != null)
                {                  
                    if(model.Cover[0] != null)
                    {
                        var apiReturn = CdnServices.UploadImagesAsync(model.Cover, model.PostType.ToString(), "Post/Stories").Result;
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
                
                if(string.IsNullOrEmpty(coverImg))
                    coverImg = model.CurrentCover;

                var postInfo = new IdentityPost();
                postInfo.Title = model.Title;
                postInfo.BodyContent = model.BodyContent;
                postInfo.Description = model.Description;
                postInfo.PostType = model.PostType;
                postInfo.CreatedBy = User.Identity.GetUserId();
                postInfo.IsHighlights = model.IsHighlights;
                postInfo.Cover = coverImg;
                postInfo.Status = model.Status;
                postInfo.UrlFriendly = string.Format("{0}-{1}", UrlFriendly.ConvertToUrlFriendly(model.Title), EpochTime.GetIntDate(DateTime.Now).ToString());

                var newId = _mainStore.Insert(postInfo);

                if(newId > 0)
                {
                    this.AddNotification(ManagerResource.LB_INSERT_SUCCESS, NotificationType.SUCCESS);
                    return RedirectToAction("Edit/" + newId);
                }                
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to CreatePost because: " + ex.ToString());
            }
            return View(model);

        }

        public ActionResult Edit(int id)
        {
            ModelState.Clear();

            var model = new PostEditModel();
            try
            {                
                if(id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var postInfo = _mainStore.GetById(id);
                if(postInfo != null)
                {
                    model.Id = postInfo.Id;
                    model.Title = postInfo.Title;
                    model.BodyContent = postInfo.BodyContent;
                    model.Description = postInfo.Description;
                    model.PostType = postInfo.PostType;
                    model.IsHighlights = postInfo.IsHighlights;
                    model.CurrentCover = postInfo.Cover;
                    model.Status = postInfo.Status;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display EditPost [{0}] because: {1}", id, ex.ToString()));
            }

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Post(PostEditModel model)
        {
            try
            {
                var coverImg = string.Empty;
                if (model.Cover != null)
                {
                    if(model.Cover[0] != null)
                    {
                        var apiReturn = CdnServices.UploadImagesAsync(model.Cover, model.PostType.ToString(), "Post/Stories").Result;
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

                var postInfo = new IdentityPost();
                postInfo.Id = model.Id;
                postInfo.Title = model.Title;
                postInfo.BodyContent = model.BodyContent;
                postInfo.Description = model.Description;
                postInfo.PostType = model.PostType;
                postInfo.CreatedBy = User.Identity.GetUserId();
                postInfo.IsHighlights = model.IsHighlights;
                postInfo.Status = model.Status;
                postInfo.UrlFriendly = string.Format("{0}-{1}", UrlFriendly.ConvertToUrlFriendly(model.Title), EpochTime.GetIntDate(DateTime.Now).ToString());

                if (!string.IsNullOrEmpty(coverImg))
                    postInfo.Cover = coverImg;
                else
                    postInfo.Cover = model.CurrentCover;

                model.CurrentCover = postInfo.Cover;

                _mainStore.Update(postInfo);

                this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to EditPost because: " + ex.ToString());
            }

            return View(model);
        }

        public ActionResult UploadPostImage(List<HttpPostedFileBase> image)
        {
            var apiReturn = CdnServices.UploadImagesAsync(image, "1", "Post/Stories").Result;
            if(apiReturn != null)
            {
                if (apiReturn.Data != null)
                {
                    var resultData = JsonConvert.DeserializeObject<List<string>>(apiReturn.Data.ToString());
                    var returnData = new List<string>();
                    if (resultData.HasData())
                    {
                        foreach (var item in resultData)
                        {
                            returnData.Add(SocialCdnHelper.GetFullImgPath(item));
                        }
                    }

                    return Json(new { success = true, data = returnData });
                }
            }

            return Json(new { success = false, data = "" });
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
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Post because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "AfterDelete()" });
        }

        [HttpGet]
        public ActionResult AssignCategory(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AssignCategoryModel model = new AssignCategoryModel();
            try
            {
                model.Id = id;
                model.Categories = CommonHelpers.GetListCategory();
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying load AssignCategory: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("../Post/_AssignCategory", model);
        }

        [HttpPost, ActionName("AssignCategory")]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptAssignCategory(int id)
        {
            var msg = ManagerResource.LB_OPERATION_SUCCESS;
            var isSuccess = false;
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var catId = Utils.ConvertToInt32(Request["cat"]);
                if (catId > 0 && id > 0)
                {
                    isSuccess = _mainStore.AssignCategory(id, catId);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying AssignCategory: {0}", ex.ToString());
                logger.Error(strError);
            }

            return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = msg, clientcallback = "AfterAssign()" });
        }

        #region Helpers

        #endregion

    }
}