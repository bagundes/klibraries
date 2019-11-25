using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace k
{
    public static class Security
    {
        private static string masterKey => k.R.Security.MasterKey;
        private static string keyValidate => "@";

        public static string Encrypt(string message, string key)
        {
            message = Encrypt1(message, MyKey(key));
            AddTokenValidation(ref message);
            return message;
        }

        public static string Decrypt(string token, string key)
        {
            if (!IsValidsToken(ref token))
                throw new FormatException("Token is not currect format");

            return Decrypt1(token, MyKey(key));
        }

        public static string Hash(params object[] values)
        {
            var input = String.Join("", values);
            return MD5(input);
        }

        public static string Token(params object[] values)
        {
            var input = String.Join("", values);
            return SHA512(input);
        }

        public static string RandomChars(int size, bool special = false)
        {
            var chars = System.String.Empty;

            if (special)
                chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!£$%^&*(){}-_+=[]:;@~#";
            else
                chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, size)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
        #region Validation
        private static string AddTokenValidation(ref string token)
        {
            int sum = 0, key = 0;
            
            foreach (var c in token.ToCharArray())
                sum += (int)c;

            var sumstr = sum.ToString();
            for (int i = 0; i < sumstr.Length; i++)
                key += int.Parse(sumstr[i].ToString());

            var hex = key.ToString("X");
            token = $"{keyValidate}{token}{hex}{hex.Length}";

            return hex;
        }

        /// <summary>
        /// Valid it is a token and remove the key
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>Valid or invalid</returns>
        private static bool IsValidsToken(ref string token)
        {
            if (!token.StartsWith(keyValidate))
                return false;

            var keyQtty = int.Parse(token.Substring(token.Length - 1));
            var keyVerify = token.Substring(token.Length - keyQtty - 1, keyQtty);

            var fooToken = token.Substring(token.IndexOf(keyValidate) + 1);
            fooToken = fooToken.Substring(0, fooToken.Length - keyVerify.Length - 1);
            var bar = (string)fooToken.Clone();
            if (AddTokenValidation(ref bar) == keyVerify)
            {
                token = fooToken;
                return true;
            } else
            {
                return false;
            }
        }

        #endregion

        #region Crypto version 1
        private static string Encrypt1(object input, string key)
        {
            if (input == null)
                return null;

            key = key ?? R.Security.MasterKey;
            var byte24 = MD5(key).Substring(0, 24);
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input.ToString());
            var tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(byte24);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            var cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private static string Decrypt1(string input, string key)
        {
            key = key ?? R.Security.MasterKey;
            var byte24 = MD5(key).Substring(0, 24);
            byte[] inputArray = Convert.FromBase64String(input.ToString());
            var tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(byte24);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            var cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        #endregion

        #region Hashs
        private static string MD5(string input)
        {
            
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private static string SHA512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }
        #endregion

        #region Others
        private static string MyKey(string key)
        {
            return Security.MD5($"{key}{masterKey}{key}");
        }
        #endregion


    }
}
