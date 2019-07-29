using ApiHaloSocial.ShareLibs;
using Manager.SharedLib.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Resources;
using Manager.WebApp.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Manager.WebApp.Services
{
    public class CdnServices
    {
        private static readonly ILog logger = LogProvider.For<CdnServices>();

        public static async Task<ResponseApiModel> UploadImagesAsync(List<HttpPostedFileBase> files, string objectId, string subDir)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.SocialContainerServer, "api/upload/images");

            try
            {
                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        if (files.HasData())
                        {
                            foreach (var item in files)
                            {
                                byte[] Bytes = new byte[item.InputStream.Length + 1];
                                item.InputStream.Read(Bytes, 0, Bytes.Length);
                                var fileContent = new ByteArrayContent(Bytes);
                                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = item.FileName };
                                content.Add(fileContent);
                            }
                        }

                        //Extends parameters
                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters.Add("ObjectId", objectId);
                        parameters.Add("SubDir", subDir);
                        HttpContent DictionaryItems = new FormUrlEncodedContent(parameters);
                        content.Add(DictionaryItems, "MyFormData");
                        var response = client.PostAsync(_baseUrl, content).Result;

                        //Trace log
                        HttpStatusCodeTrace(response);

                        // Parsing the returned result                    
                        var responseString = await response.Content.ReadAsStringAsync();

                        result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to UploadImagesAsync - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        private static void HttpStatusCodeTrace(HttpResponseMessage response)
        {
            var statusCode = Utils.ConvertToInt32(response.StatusCode);
            if (statusCode != (int)HttpStatusCode.OK)
            {
                logger.Debug(string.Format("Return code: {0}, message: {1}", response.StatusCode, response.RequestMessage.RequestUri));
            }
        }
    }
}