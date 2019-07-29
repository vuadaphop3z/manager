using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

using Manager.WebApp.Models;
using System.IO;
using Manager.SharedLib.Logging;
using System.Text;
using System.Security.Cryptography;

namespace Manager.WebApp.Helpers
{
    public class Utility
    {
        private static readonly ILog logger = LogProvider.For<Utility>();

        public static string GetRandomPasswordUsingGUID(int length)
        {
            string guidResult = System.Guid.NewGuid().ToString();
            guidResult = guidResult.Replace("-", string.Empty);
            if (length <= 0 || length > guidResult.Length)
                throw new ArgumentException("Length must be between 1 and " + guidResult.Length);
            return guidResult.Substring(0, length);
        }

        public static string Md5HashingData(string rawStr)
        {
            StringBuilder hash = new StringBuilder();
            if (!string.IsNullOrEmpty(rawStr))
            {
                MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
                byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(rawStr));

                for (int i = 0; i < bytes.Length; i++)
                {
                    hash.Append(bytes[i].ToString("x2"));
                }
            }

            return hash.ToString();
        }

        public static string GenerateRedisKeyWithPrefixAndSurfix(string prefix, string surfix)
        {
            var md5Surfix = Md5HashingData(surfix);
            return string.Format("{0}_{1}", prefix, md5Surfix);
        }

        //public string GenerateStringByFormat
        public static string GenerateStringByFormat(string format, params object[] paramList)
        {
            return string.Format(format, paramList);
        }

        //public static string GenerateOTPCode()
        //{
        //    Random random = new Random();
        //    return new string(Enumerable.Repeat(SingleSignOnSettings.CharactersOfOTPCode, SingleSignOnSettings.NumberCharactersOfOTPCode)
        //     .Select(s => s[random.Next(s.Length)]).ToArray());
        //}

        public static string TripleDESEncrypt(string key, string data)
        {
            data = data.Trim();
            byte[] keydata = Encoding.ASCII.GetBytes(key);
            string md5String = BitConverter.ToString(new
            MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-", "").ToLower(); byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24)); TripleDES tripdes = TripleDESCryptoServiceProvider.Create();
            tripdes.Mode = CipherMode.ECB;
            tripdes.Key = tripleDesKey;
            tripdes.GenerateIV();
            MemoryStream ms = new MemoryStream();
            CryptoStream encStream = new CryptoStream(ms, tripdes.CreateEncryptor(), CryptoStreamMode.Write);
            encStream.Write(Encoding.ASCII.GetBytes(data), 0, Encoding.ASCII.GetByteCount(data));
            encStream.FlushFinalBlock();
            byte[] cryptoByte = ms.ToArray();
            ms.Close();
            encStream.Close();
            return Convert.ToBase64String(cryptoByte, 0,
            cryptoByte.GetLength(0)).Trim();
        }

        public static string TripleDESDecrypt(string key, string data)
        {
            byte[] keydata = Encoding.ASCII.GetBytes(key);
            string md5String = BitConverter.ToString(new
            MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-", "").ToLower();
            byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24));
            TripleDES tripdes = TripleDESCryptoServiceProvider.Create();
            tripdes.Mode = CipherMode.ECB;
            tripdes.Key = tripleDesKey;
            byte[] cryptByte = Convert.FromBase64String(data);
            MemoryStream ms = new MemoryStream(cryptByte, 0, cryptByte.Length);
            ICryptoTransform cryptoTransform = tripdes.CreateDecryptor();
            CryptoStream decStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Read);
            StreamReader read = new StreamReader(decStream);
            return (read.ReadToEnd());
        }

        public static string GenTranIdWithPrefix(string prefix)
        {
            var tranId = "{0}{1}";
            var surfix = GetRandomPasswordUsingGUID(5);
            tranId = string.Format(tranId, prefix, surfix);

            return tranId;
        }

        public static string SendSMS(string mobile, string msg, string tranId = "")
        {
            return string.Empty;
            //SMSServices sms = new SMSServices();
            //var result = string.Empty;

            //var wsUrl = SingleSignOnSettings.SoapSMSServiceUrl;
            ////var wsActionSendSMS = string.Format("{0}?op={1}", wsUrl, SingleSignOnSettings.SoapSendSMSOp);

            ////var smsDataStr = new JavaScriptSerializer().Serialize(new SendSMSModel { msisdn = mobile, msg = msg, sendtime = DateTime.Now.ToString(Constant.DATEFORMAT_ddMMyyyyHHmmss), tranid = tranId });
            //var smsDataStr = JsonConvert.SerializeObject(new SendSMSModel { msisdn = mobile, msg = msg, sendtime = DateTime.Now.ToString(Constant.DATEFORMAT_ddMMyyyyHHmmss), tranid = tranId });
            //var smsDataEncrypted = Utility.TripleDESEncrypt(SingleSignOnSettings.SoapSMSAgentKey, smsDataStr);

            //try
            //{
            //    result = sms.SendMT(SingleSignOnSettings.SoapSMSAgentCode, smsDataEncrypted);
            //}
            //catch (Exception ex)
            //{
            //    var strError = string.Format("Failed to SendSMS due to: {0}", ex.Message);
            //    logger.Error(strError);
            //}

            //////XML:<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""><soap:Body><SendMT xmlns=""http://tempuri.org/""><agentCode>string</agentCode><data>string</data></SendMT></soap:Body></soap:Envelope>
            ////var sendSMSXML = @"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""><soap:Body><SendMT xmlns=""http://tempuri.org/""><agentCode>{0}</agentCode><data>{1}</data></SendMT></soap:Body></soap:Envelope>";

            ////sendSMSXML = string.Format(sendSMSXML, SingleSignOnSettings.SoapSMSAgentCode, smsDataEncrypted);

            ////var sendSMSResult = WebserviceCaller.CallWebservice(wsUrl, wsActionSendSMS, sendSMSXML);

            ////return sendSMSResult;
            //return result;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }
}