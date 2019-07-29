using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Manager.WebApp.Controllers
{
    public class TemplatesController : Controller
    {
        // GET: Demo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Send()
        {
            string username = "Vũ Lương Bằng";
            string password = "123123";

            //string messageBody = string.Format(body, username, username, password);
            var model = new EmailBookingConfirmModel { Receiver = "Nguyễn Văn Toàn" };
            var messageBody = PartialView("Email/Booking/BookingConfirm", model).RenderToString();
            messageBody = string.Format(messageBody, username, username, password);

            var MailHelper = new MailHelper
            {
                Sender = "registeronly12891@gmail.com", //email.Sender,
                Recipient = "bangkhmt3@gmail.com",
                RecipientCC = null,
                Subject = "Test email",
                Body = messageBody                
            };
            MailHelper.Send();

            return Content("Success");
        }

        public ActionResult SendEmail()
        {
            string body;
            //Read template file from the App_Data folder
            var folderPath = Server.MapPath("\\App_Data\\Templates\\");
            var fullPath = folderPath + "BookingConfirm.html";
            using (var sr = new StreamReader(fullPath))
            {
                body = sr.ReadToEnd();
            }

             try
             {
               //add email logic here to email the customer that their invoice has been voided
            //Username: {1}
               string username = HttpUtility.UrlEncode("bangvl@starcom.vn");
               string password = HttpUtility.UrlEncode("dcnakhmt3");
               string sender = "bangvl@starcom.vn";
               string emailSubject = @"Welcome Email";

               string messageBody = string.Format(body, username, username, password);
               
                var MailHelper = new MailHelper
                            {
                                Sender = sender, //email.Sender,
                                Recipient = "bangkhmt3@gmail.com",
                                RecipientCC = null,
                                Subject = emailSubject,
                                Body = messageBody
                            };
                            MailHelper.Send();

                            //return RedirectToAction("EmailConfirm");
                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("", e.Message);
                        }
            return Content("Success");
        }

        public ActionResult ClearCaches(string prefix)
        {
            MenuHelper.ClearAllMenuCache();

            return Content("Success");
        }
    }
}