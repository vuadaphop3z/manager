using Manager.WebApp.Settings;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Manager.WebApp.Services
{
    public static class CommonServices
    {
        public static HttpClient AuthorizationHttpClientCore(this HttpClient client)
        {
            //Set Basic Auth
            var user = (!string.IsNullOrEmpty(AuthorizationCoreSettings.Username)) ? AuthorizationCoreSettings.Username : "username";
            var password = (!string.IsNullOrEmpty(AuthorizationCoreSettings.Password)) ? AuthorizationCoreSettings.Password : "password";
            var authorizeHeaderKey = (!string.IsNullOrEmpty(AuthorizationCoreSettings.HeaderKey)) ? AuthorizationCoreSettings.HeaderKey : "key";
            var authorizeHeaderValue = (!string.IsNullOrEmpty(AuthorizationCoreSettings.HeaderValue)) ? AuthorizationCoreSettings.HeaderValue : "value";

            var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);

            client.DefaultRequestHeaders.Add(authorizeHeaderKey, authorizeHeaderValue);

            return client;
        }

        public static HttpClient AuthorizationHttpClientSocial(this HttpClient client)
        {
            //Set Basic Auth
            var user = (!string.IsNullOrEmpty(AuthorizationSocialSettings.Username)) ? AuthorizationSocialSettings.Username : "username";
            var password = (!string.IsNullOrEmpty(AuthorizationSocialSettings.Password)) ? AuthorizationSocialSettings.Password : "password";
            var authorizeHeaderKey = (!string.IsNullOrEmpty(AuthorizationSocialSettings.HeaderKey)) ? AuthorizationSocialSettings.HeaderKey : "key";
            var authorizeHeaderValue = (!string.IsNullOrEmpty(AuthorizationSocialSettings.HeaderValue)) ? AuthorizationSocialSettings.HeaderValue : "value";

            var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);

            client.DefaultRequestHeaders.Add(authorizeHeaderKey, authorizeHeaderValue);

            return client;
        }
    }
}