using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Nop.Plugin.Payments.TelenorEasyPay.Models
{
    public class EasyPaySecurity
    {
        private string _secretKey;
        //private const String secretKey = "698649a1f52a4d259c12358d4fb1097be6843e6e219c416082de4631876c9b2494636a3310264c6fa50a2f58a3c8500788d05a5178fb4ef79cc6544f783686471f5f8b66ddaa4144a63b68342d239c530fd37ab155a34603860da895df4df226a9f35b0e8f9e4120a6522d7762e283f81d99b642c7a34ab69163e23477a0a05a";


        public EasyPaySecurity(string secretKey)
        {
            this._secretKey = secretKey;
        }

        public   String sign(IDictionary<string, string> paramsArray)
        {
            return sign(buildDataToSign(paramsArray), _secretKey);
        }

        public   String sign(Dictionary<string, object> paramsArray)
        {
            return sign(buildDataToSign(paramsArray), _secretKey);
        }

        private static String sign(String data, String secretKey)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secretKey);

            HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);
            byte[] messageBytes = encoding.GetBytes(data);
            return Convert.ToBase64String(hmacsha256.ComputeHash(messageBytes));
        }

        private static String buildDataToSign(IDictionary<string, string> paramsArray)
        {
            String[] signedFieldNames = paramsArray["signed_field_names"].Split(',');
            IList<string> dataToSign = new List<string>();

            foreach (String signedFieldName in signedFieldNames)
            {
                dataToSign.Add(signedFieldName + "=" + paramsArray[signedFieldName]);
            }

            return commaSeparate(dataToSign);
        }

        private static String buildDataToSign(Dictionary<string, object> paramsArray)
        {
            String[] signedFieldNames = paramsArray["signed_field_names"].ToString().Split(',');
            IList<string> dataToSign = new List<string>();

            foreach (String signedFieldName in signedFieldNames)
            {
                dataToSign.Add(signedFieldName + "=" + paramsArray[signedFieldName]);
            }

            return commaSeparate(dataToSign);
        }

        private static String commaSeparate(IList<string> dataToSign)
        {
            return String.Join(",", dataToSign);
        }

        public static String getUUID()
        {
            return System.Guid.NewGuid().ToString();
        }

        public static String getUTCDateTime()
        {
            DateTime time = DateTime.Now.ToUniversalTime();
            return time.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
        }
    }
}