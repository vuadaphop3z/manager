using System.ComponentModel.DataAnnotations;
using Manager.WebApp.Settings;

namespace Manager.Business.Settings
{
    public class MailSettings: SettingsBase
    {        
        [Required]
        [Display(Name = "Smtp Server", Description = "Mail Server to send message through. Should be a domain name (mail.yourserver.net) or IP Address (211.123.123.123)")]
        public string SmtpServer { get; set; }


        [Required]
        [Display(Name = "Smtp Port", Description = "Port on the mail server to send through. Defaults to port 25.")]
        public int SmtpPort { get; set; }

        [Required]
        [Display(Name = "Smtp UseSSL", Description = "Use SSL communication when sending an email. Hack: client.EnableSsl = client.Port == 587 || client.Port == 465; client.EnableSsl = client.Port != 25;")]
        public bool SmtpUseSsl { get; set; }


        //[Required]
        [Display(Name = "Smtp Username", Description = "Username to login to the mail server.")]        
        public string SmtpUsername { get; set; }


        //[Required]
        [Display(Name = "Smtp Password", Description = "Password to login to the mail server.")]             
        public string SmtpPassword { get; set; }


        [Required]
        [Display(Name = "Smtp Timeout In Seconds", Description = "Connection timeouts for the mail server in seconds. If this timeout is exceeded waiting for a connection or for receiving or sending data the request is aborted and fails.")]           
        public int SmtpTimeout { get; set;}

    }
}
