using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

namespace TruePeople.SharePreview.Helpers
{
    internal static class TPEncryptHelper
    {
        public static string DecryptString(string rawData, string privateKey)
        {
            byte[] data = WebEncoders.Base64UrlDecode(rawData);
            var mD5 = MD5.Create();
            byte[] keys = mD5.ComputeHash(Encoding.UTF8.GetBytes(privateKey));
            var tripleDES = GetTripleDES(keys);
            ICryptoTransform transform = tripleDES.CreateDecryptor();
            byte[] result = transform.TransformFinalBlock(data, 0, data.Length);
            return Encoding.UTF8.GetString(result);
        }

        public static string EncryptString(string rawData, string privateKey)
        {
            byte[] data = Encoding.UTF8.GetBytes(rawData);
            var mD5 = MD5.Create();
            byte[] keys = mD5.ComputeHash(Encoding.UTF8.GetBytes(privateKey));
            var tripleDES = GetTripleDES(keys);
            ICryptoTransform transform = tripleDES.CreateEncryptor();
            byte[] result = transform.TransformFinalBlock(data, 0, data.Length);
            return WebEncoders.Base64UrlEncode(result);
        }

        private static TripleDES GetTripleDES(byte[] keys)
        {
            var tripleDES = TripleDES.Create();
            tripleDES.Key = keys;
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            return tripleDES;
        }
    }
}