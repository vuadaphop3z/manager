using Manager.WebApp.Caching;
using Manager.WebApp.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Manager.WebApp
{    
    public class LanguageMessageHandler : DelegatingHandler
    {
        //private readonly List<string> _supportedLanguages = LanguagesProvider._supportedLanguages;
        public static bool IsSupported(string langCode)
        {
            //if (LanguagesProvider._supportedLanguages.Contains(langCode))
            //    return true;
            if (LanguagesProvider.GetListLanguages().Any(x => x.LanguageCultureName.Contains(langCode)))
                return true;

            return false;
        }

        private bool SetHeaderIfAcceptLanguageMatchesSupportedLanguage(HttpRequestMessage request)
        {
            foreach (var lang in request.Headers.AcceptLanguage)
            {
                if (IsSupported(lang.Value))
                {
                    SetCulture(request, lang.Value);
                    return true;
                }
            }

            return false;
        }

        private bool SetHeaderIfGlobalAcceptLanguageMatchesSupportedLanguage(HttpRequestMessage request)
        {
            var listLangs = LanguagesProvider.GetListLanguages();
            foreach (var lang in request.Headers.AcceptLanguage)
            {
                var globalLang = lang.Value.Substring(0, 2);
                if (LanguagesProvider.GetListLanguages().Any(t => t.LanguageCultureName.StartsWith(globalLang)))
                {
                    //SetCulture(request, _supportedLanguages.FirstOrDefault(i => i.StartsWith(globalLang)));
                    SetCulture(request, listLangs.FirstOrDefault(i => i.LanguageCultureName.StartsWith(globalLang)).LanguageCultureName);
                    return true;
                }
            }

            return false;
        }

        private void SetCulture(HttpRequestMessage request, string lang)
        {
            request.Headers.AcceptLanguage.Clear();
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(lang));
            Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!SetHeaderIfAcceptLanguageMatchesSupportedLanguage(request))
            {
                // Whoops no localization found. Lets try Globalisation
                if (!SetHeaderIfGlobalAcceptLanguageMatchesSupportedLanguage(request))
                {
                    // no global or localization found
                    SetCulture(request, LanguagesProvider._defaultLang);
                }
            }

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }

        public void SetLanguage(string lang)
        {
            try
            {
                if (!IsSupported(lang)) lang = GetDefaultLanguage();
                var cultureInfo = new CultureInfo(lang);
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
                HttpCookie langCookie = new HttpCookie(SystemSettings.CultureKey, lang);
                langCookie.Expires = DateTime.Now.AddYears(1);
                HttpContext.Current.Response.Cookies.Add(langCookie);                             
            }
            catch{

            }
        }

        public static string GetDefaultLanguage()
        {
            return LanguagesProvider._defaultLang;
        }
    }
}