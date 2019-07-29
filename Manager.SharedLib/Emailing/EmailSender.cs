using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.IO;
using System.Net.Mail;
using System.Net;
using System.Net.Sockets;
using System.Security;


namespace Manager.SharedLib.Emailing
{
    /// <summary>
    /// Delegate used to handle Completion and failure events
    /// </summary>
    /// <param name="Smtp"></param>
    public delegate void DelegateEmailerEvent(EmailSender sender);

    public class EmailSender : IDisposable
    {
        public MailSmtpSettings SmtpSettings { get; set; }
        public MailSendSettings SendSettings { get; set; }

        /// <summary>
        /// Event fired when sending of a message or multiple messages
        /// is complete and the connection is to be closed. This event
        /// occurs only once per connection as opposed to the MessageSendComplete
        /// event which fires after each message is sent regardless of the
        /// number of SendMessage operations.
        /// </summary>
        public event DelegateEmailerEvent SendComplete;

        /// <summary>
        /// Event fired when an error occurs during processing and before
        /// the connection is closed down.
        /// </summary>
        public event DelegateEmailerEvent SendError;

        

        public EmailSender()
        {
            Headers = new Dictionary<string, string>();
            ErrorMessage = string.Empty;

            SmtpSettings = new MailSmtpSettings();
            SendSettings = new MailSendSettings();
        }

        #region Message

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected internal List<AlternateView> AlternateViews = new List<AlternateView>();

        /// <summary>
        /// Adds a new Alternate view to the request. Passed from FoxPro
        /// which sets up this object.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="contentType"></param>
        /// <param name="contentId"></param>
        public void AddAlternateView(AlternateView view)
        {
            this.AlternateViews.Add(view);
        }


        public void AddAlternateView(string alternateText, string alternateContentType)
        {
            var altView = AlternateView.CreateAlternateViewFromString(alternateText, SendSettings.Encoding, alternateContentType);
            this.AlternateViews.Add(altView);
        }

        /// <summary>
        /// Assigns mail addresses from a string or comma delimited string list.
        /// Facilitates 
        /// </summary> 
        /// <param name="recipients"></param>
        /// <returns></returns>
        private void AssignMailAddresses(MailAddressCollection address, string recipients)
        {
            if (string.IsNullOrEmpty(recipients))
                return;

            string[] recips = recipients.Split(',', ';');

            for (int x = 0; x < recips.Length; x++)
            {
                address.Add(new MailAddress(recips[x]));
            }
        }

        /// <summary>
        /// Configures the message interface
        /// </summary>        
        protected virtual MailMessage BuildMailMessage()
        {
            MailMessage msg = new MailMessage();

            msg.Body = this.SendSettings.Message;
            msg.Subject = this.SendSettings.Subject;

            msg.From = new MailAddress(this.SendSettings.SenderEmail, this.SendSettings.SenderName);

            if (!string.IsNullOrEmpty(this.SendSettings.ReplyTo))
            {
                //msg.ReplyTo = new MailAddress(this.SendSettings.ReplyTo);
                msg.ReplyToList.Add(this.SendSettings.ReplyTo);
            }

            // Send all the different recipients            
            this.AssignMailAddresses(msg.To, this.SendSettings.Recipient);
            this.AssignMailAddresses(msg.CC, this.SendSettings.CC);
            this.AssignMailAddresses(msg.Bcc, this.SendSettings.BCC);

            if (!string.IsNullOrEmpty(this.SendSettings.Attachments))
            {
                string[] files = this.SendSettings.Attachments.Split(new char[2] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string file in files)
                {
                    msg.Attachments.Add(new Attachment(file));
                }
            }

            if (this.SendSettings.ContentType.StartsWith("text/html"))
                msg.IsBodyHtml = true;
            else
                msg.IsBodyHtml = false;

            msg.BodyEncoding = this.SendSettings.Encoding;


            msg.Priority = (MailPriority)Enum.Parse(typeof(MailPriority), this.SendSettings.Priority);

            
            if (this.SendSettings.ReturnReceipt)
                msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;

            if (!string.IsNullOrEmpty(this.SendSettings.UserAgent))
                this.AddHeader("x-mailer", this.SendSettings.UserAgent);


            if (!string.IsNullOrEmpty(this.SendSettings.AlternateText))
            {               
                var alterviewContentType = "text/plain";
                if (!msg.IsBodyHtml)
                    alterviewContentType = "text/html";

                this.AddAlternateView(this.SendSettings.AlternateText, alterviewContentType);
            }

            if (this.AlternateViews.Count > 0)
            {
                foreach (var view in this.AlternateViews)
                {
                    msg.AlternateViews.Add(view);
                }
            }

            foreach (var header in this.Headers)
            {
                msg.Headers[header.Key] = header.Value;
            }

            return msg;
        }


        #endregion


        #region Sending

        /// <summary>
        /// Fully self contained mail sending method. Sends an email message by connecting 
        /// and disconnecting from the email server.
        /// </summary>
        /// <returns>true or false</returns>
        public bool SendMail()
        {
            if (!this.Connect())
                return false;

            try
            {
                // Create and configure the message 
                using (MailMessage msg = this.BuildMailMessage())
                {
                    smtp.Send(msg);

                    if (this.SendComplete != null)
                        this.SendComplete(this);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                    msg = ex.InnerException.Message;

                this.SetError(msg);

                if (this.SendError != null)
                    this.SendError(this);

                return false;
            }
            finally
            {
                // close connection and clear out headers
                this.Close();
            }

            return true;
        }

        /// <summary>
        /// Run mail sending operation on a separate thread and asynchronously
        /// Operation does not return any information about completion.
        /// </summary>
        /// <returns></returns>
        public void SendMailAsync()
        {
            //ThreadStart delSendMail = new ThreadStart(this.SendMailRun);
            //delSendMail.BeginInvoke(null, null);

            Thread mailThread = new Thread(this.SendMailRun);
            mailThread.Start();
        }

        protected void SendMailRun()
        {
            // Create an extra reference to insure GC doesn't collect
            // the reference from the caller
            EmailSender sender = this;
            sender.SendMail();
        }

        #endregion


        #region Session


        /// <summary>
        /// Internal instance of SmtpClient that holds the 'connection'
        /// effectively.
        /// </summary>
        private SmtpClient smtp = null;

        /// <summary>
        /// Starts a new SMTP session. Note this doesn't actually open a connection
        /// but just configures and sets up the SMTP session. The actual connection
        /// is opened only when a message is actually sent
        /// </summary>
        public bool Connect()
        {
            // Allow for server:Port syntax (west-wind.com:1212)
            int serverPort = this.SmtpSettings.SmtpPort;
            string server = this.SmtpSettings.SmtpServer;

            // if there's a port we need to split the address
            string[] parts = server.Split(':');
            if (parts.Length > 1)
            {
                server = parts[0];
                serverPort = int.Parse(parts[1]);
            }

            if (string.IsNullOrEmpty(server))
            {
                this.SetError("No Mail Server specified.");
                this.Headers.Clear();
                return false;
            }

            smtp = null;
            try
            {
                smtp = new SmtpClient(server, serverPort);

                if (this.SmtpSettings.SmtpUseSsl)
                    smtp.EnableSsl = true;
            }
            catch (SecurityException)
            {
                this.SetError("Unable to create SmptClient due to missing permissions. If you are using a port other than 25 for your email server, SmtpPermission has to be explicitly added in Medium Trust.");
                this.Headers.Clear();
                return false;
            }

            // This is a Total Send Timeout not a Connection timeout!
            //smtp.Timeout = this.Timeout * 1000;
            smtp.Timeout = this.SmtpSettings.SmtpTimeout;

            if (!string.IsNullOrEmpty(this.SmtpSettings.SmtpUser))
                smtp.Credentials = new NetworkCredential(this.SmtpSettings.SmtpUser, this.SmtpSettings.SmtpPassword);

            return true;
        }

        /// <summary>
        /// Cleans up and closes the connection
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            this.smtp = null;

            // clear all existing headers
            this.Headers.Clear();

            return true;
        }

        #endregion


        #region Header

        /// <summary>
        /// SMTP headers for this email request
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Adds an Smtp header to this email request. Headers are 
        /// always cleared after a message has been sent or failed.
        /// </summary>
        /// <param name="headerName"></param>
        /// <param name="value"></param>
        public void AddHeader(string headerName, string value)
        {
            if (headerName.ToLower() == "clear" || headerName.ToLower() == "reset")
                this.Headers.Clear();
            else
            {
                if (!Headers.ContainsKey(headerName))
                    this.Headers.Add(headerName, value);
                else
                    this.Headers[headerName] = value;
            }
        }

        /// <summary>
        /// Adds headers from a CR/LF separate string that has key:value header pairs 
        /// defined.
        /// </summary>
        /// <param name="headers"></param>
        public void AddHeadersFromString(string headers)
        {
            string[] lines = headers.Split(new char[2] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] tokens = line.Split(':');
                if (tokens.Length != 2)
                    continue;

                this.AddHeader(tokens[0].Trim(), tokens[1].Trim());
            }
        }

        #endregion


        #region Error

        /// <summary>
        /// An Error Message if the result is negative or Error is set to true;
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Internally used to set errors
        /// </summary>
        /// <param name="errorMessage"></param>
        private void SetError(string errorMessage)
        {
            if (errorMessage == null || errorMessage.Length == 0)
            {
                this.ErrorMessage = string.Empty;                
                return;
            }

            ErrorMessage = errorMessage;            
        }

        #endregion


        #region IDisposable

        public void Dispose()
        {
            if (this.smtp != null)
                this.smtp = null;
        }

        #endregion

        /// <summary>
        /// Thuc hien cong viec gui mot Email
        /// </summary>
        /// <param name="smtpSettings">Thong so thiet lap truoc khi gui mail</param>
        /// <param name="from">Dia chi email gui</param>
        /// <param name="to">Dia chi email nhan</param>
        /// <param name="cc">Cac dia chi cung tham gia</param>
        /// <param name="bcc">Cac email khac can biet</param>
        /// <param name="subject">Chu de</param>
        /// <param name="messageBody">Noi dung email</param>
        /// <param name="html">Dinh dang HTML hay khong</param>
        /// <param name="priority">Muc do uu tien</param>
        /// <param name="filePath">Duong dan file gui kem</param>        
        public static bool SendEmail(MailSmtpSettings smtpSettings, 
            string fromEmail, string fromName, 
            string toEmails,  string ccEmails, string bccEmails, 
            string emailSubject, string emailBody, 
            bool isHtml, string priority, string filePaths, 
            out string errorMessage)
        {
            bool result = false;
            errorMessage = string.Empty;

            using (EmailSender emailEngine = new EmailSender())
            {
                emailEngine.SmtpSettings = smtpSettings;

                emailEngine.SendSettings.SenderEmail = fromEmail;
                emailEngine.SendSettings.SenderName = fromName;
                emailEngine.SendSettings.Recipient = toEmails;
                emailEngine.SendSettings.CC = ccEmails;
                emailEngine.SendSettings.BCC = bccEmails;
                emailEngine.SendSettings.Subject = emailSubject;
                emailEngine.SendSettings.Message = emailBody;
                emailEngine.SendSettings.Priority = priority;
                emailEngine.SendSettings.Encoding = Encoding.UTF8;

                emailEngine.SendSettings.Attachments = filePaths;
                if (isHtml)
                    emailEngine.SendSettings.ContentType = "text/html";

                result = emailEngine.SendMail();
                errorMessage = emailEngine.ErrorMessage;
            }

            return result;
        }
    }

    public class MailSendSettings
    {
        /// <summary>
        /// Email address or addresses of the Recipient. Comma delimit multiple addresses. 
        /// To have formatted names use "Rick Strahl" &lt;rstrahl@west-wind.com&gt;
        /// </summary>
        public string Recipient = string.Empty;

        /// <summary>
        /// Email address of the sender
        /// </summary>
        public string SenderEmail { get; set; }


        /// <summary>
        /// Display name of the sender (optional)
        /// </summary>
        public string SenderName {get; set;}


        /// <summary>
        /// The ReplyTo address
        /// </summary>
        public string ReplyTo {get; set;}

        /// <summary>
        /// Carbon Copy Recipients
        /// </summary>
        public string CC {get;set;}

        /// <summary>
        /// Blind Copy Recipients
        /// </summary>
        public string BCC {get; set;}


        /// <summary>
        /// Message Subject.
        /// </summary>
        public string Subject {get; set;}

        /// <summary>
        /// The body of the message.
        /// </summary>
        public string Message {get; set;}


        /// <summary>
        /// Any attachments you'd like to send
        /// </summary>
        public string Attachments {get; set;}


        /// <summary>
        /// The content type of the message. text/plain default or you can set to any other type like text/html
        /// </summary>
        public string ContentType {get; set;}


        /// <summary>
        /// The character Encoding used to write the stream out to disk
        /// Defaults to the default Locale used on the server.
        /// </summary>
        public System.Text.Encoding Encoding { get; set; }


        /// <summary>        
        /// Determines the priority of the message
        /// Normal = The email has normal priority.
        /// Low = The email has low priority.
        /// High = The email has high priority
        /// </summary>
        public string Priority { get; set; }


        /// <summary>
        /// The content of alternate view: Html View + Plain View auto switching
        /// </summary>
        public string AlternateText { get; set; }      

        /// <summary>
        /// The user agent for the x-mailer
        /// </summary>
        public string UserAgent { get; set; }


        /// <summary>
        /// Determines whether a return receipt is sent
        /// </summary>
        public bool ReturnReceipt {get; set;}


        public MailSendSettings ()
        {
            SenderEmail = string.Empty;
            SenderName = string.Empty;
            ReplyTo = string.Empty;

            CC = string.Empty;
            BCC = string.Empty;

            Subject = string.Empty;
            Message = string.Empty;
            AlternateText = string.Empty;

            Attachments = string.Empty;
            ContentType = "text/plain";

            Encoding = Encoding.Default;
            Priority = "Normal"; //Reference: MailPriority

            UserAgent = string.Empty;
            ReturnReceipt = false;
        }
    }

    public class MailSmtpSettings
    {
        /// <summary>
        /// Mail Server to send message through. Should be a domain name (mail.yourserver.net) or IP Address (211.123.123.123).
        /// </summary>
        public string SmtpServer { get; set; }

        /// <summary>
        /// Port on the mail server to send through. Defaults to port 25.
        /// </summary>
        public int SmtpPort { get; set; }        

        public bool SmtpUseSsl { get; set; }

        /// <summary>
        /// Username to login to the mail server.
        /// </summary>
        public string SmtpUser { get; set; }

        /// <summary>
        /// Password to login to the mail server.
        /// </summary>
        public string SmtpPassword { get; set; }

        /// <summary>
        /// Connection timeouts for the mail server in seconds. If this timeout is exceeded waiting for a connection
        /// or for receiving or sending data the request is aborted and fails.
        /// </summary>
        public int SmtpTimeout = 30;

        /// <summary>
        /// Default constructor with default properties
        /// </summary>
        public MailSmtpSettings()
        {
            SmtpServer = "localhost";
            SmtpPort = 25;            
            SmtpUseSsl = false;
            SmtpUser = string.Empty;
            SmtpPassword = string.Empty;
            SmtpTimeout = 30;
        }
    }
}
