//using ApiHaloSocial.ShareLibs;
//using Manager.SharedLib.Logging;
//using Manager.WebApp.Models;
//using Manager.WebApp.Resources;
//using Manager.WebApp.Settings;
//using Newtonsoft.Json;
//using System;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;

//namespace Manager.WebApp.Services
//{
//    public class DiscoverServices
//    {
//        private static readonly ILog logger = LogProvider.For<DiscoverServices>();

//        public static async Task<ResponseApiModel> ClearCategoryCacheAsync()
//        {
//            var strError = string.Empty;
//            var result = new ResponseApiModel();
//            var _baseUrl = string.Format("{0}/{1}", SystemSettings.HaloSocialApi, "api/discover/clearcategorycache");
//            try
//            {
//                var client = new HttpClient();
//                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExtenalSeviceTimeOutInSeconds);

//                // We want the response to be JSON.
//                client.DefaultRequestHeaders.Accept.Clear();
//                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//                //Authorization
//                client.AuthorizationHttpClientCore();

//                //Begin calling
//                var response = new HttpResponseMessage();

//                //StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

//                // Post to the Server and parse the response.
//                response = client.GetAsync(_baseUrl).Result;

//                // Parsing the returned result                    
//                var responseString = await response.Content.ReadAsStringAsync();
//                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

//                //Trace log
//                HttpStatusCodeTrace(response);

//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed when calling api to ClearCategoryCacheAsync - {0} because: {1}", _baseUrl, ex.ToString());
//                logger.Error(strError);

//                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
//            }

//            return result;
//        }

//        private static void HttpStatusCodeTrace(HttpResponseMessage response)
//        {
//            var statusCode = Utils.ConvertToInt32(response.StatusCode);
//            if (statusCode != (int)HttpStatusCode.OK)
//            {
//                logger.Debug(string.Format("Return code: {0}, message: {1}", response.StatusCode, response.RequestMessage.RequestUri));
//            }
//        }
//    }
//}