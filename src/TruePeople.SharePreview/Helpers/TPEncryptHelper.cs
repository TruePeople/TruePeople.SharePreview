using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;

namespace TruePeople.SharePreview.Helpers
{
    internal static class TPEncryptHelper
    {
        public static string DecryptString(string rawData, string privateKey)
        {
            byte[] data = System.Web.HttpServerUtility.UrlTokenDecode(rawData);
            using (MD5CryptoServiceProvider mD5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = mD5.ComputeHash(UTF8Encoding.UTF8.GetBytes(privateKey));
                using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripleDES.CreateDecryptor();
                    byte[] result = transform.TransformFinalBlock(data, 0, data.Length);
                    return UTF8Encoding.UTF8.GetString(result);
                }
            }
        }
        public static string EncryptString(string rawData, string privateKey)
        {
            byte[] data = UTF8Encoding.UTF8.GetBytes(rawData);
            using (MD5CryptoServiceProvider mD5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = mD5.ComputeHash(UTF8Encoding.UTF8.GetBytes(privateKey));
                using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripleDES.CreateEncryptor();
                    byte[] result = transform.TransformFinalBlock(data, 0, data.Length);
                    return System.Web.HttpServerUtility.UrlTokenEncode(result);
                }
            }
        }
    }
}
