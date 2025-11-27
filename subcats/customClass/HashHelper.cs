using System;
using System.Security.Cryptography;
using System.Text;

namespace subcats.customClass
{
    public static class HashHelper
    {
        /// <summary>
        /// Convierte una cadena de texto a hash MD5
        /// </summary>
        /// <param name="input">Texto a encriptar</param>
        /// <returns>Hash MD5 en formato hexadecimal</returns>
        public static string ToMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
