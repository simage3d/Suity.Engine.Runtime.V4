using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Crypto
{
    public class AesKey
    {
        /// <summary>
        /// A system key and the length should be 32.
        /// You can use tool to generate the string on https://www.random.org/strings/ or other website.
        /// </summary>
        public string KeyPart { get; } 

        /// <summary>
        /// Please indicate a random string here, and the length must be 16.
        /// You can use tool to generate the string on https://www.random.org/strings/ or other website.
        /// </summary>
        public string IvPart { get; } 

        /// <summary>
        /// The combine key.
        /// </summary>
        internal readonly byte[] Key;

        internal readonly byte[] Iv;

        /// <summary>
        /// 构建AesKey
        /// </summary>
        /// <param name="keyPart">A system key and the length should be 32.</param>
        /// <param name="ivPart">Please indicate a random string here, and the length must be 16.</param>
        public AesKey(string keyPart, string ivPart)
        {
            KeyPart = keyPart;
            IvPart = ivPart;

            Key = Encoding.ASCII.GetBytes(KeyPart);
            Iv = Encoding.ASCII.GetBytes(IvPart);
        }
        /// <summary>
        /// 构建AesKey
        /// </summary>
        /// <param name="systemKeyPart">A system key and the length should be 16</param>
        /// <param name="userKeyPart">A custom key and the lenth should between 4 and 16. You can use the project name as the custom key.</param>
        /// <param name="ivPart">Please indicate a random string here, and the length must be 16.</param>
        public AesKey(string systemKeyPart, string userKeyPart, string ivPart)
        {
            KeyPart = userKeyPart.PadRight(16, '#') + systemKeyPart;
            IvPart = ivPart;

            Key = Encoding.ASCII.GetBytes(KeyPart);
            Iv = Encoding.ASCII.GetBytes(IvPart);
        }

        static Random _rnd = new Random();
        static char[] Chars = {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
        };

        public static AesKey GenerateKeys()
        {
            char[] keyPart = new char[32];
            for (int i = 0; i < keyPart.Length; i++)
            {
                keyPart[i] = Chars[_rnd.Next(0, Chars.Length - 1)];
            }

            char[] ivPart = new char[16];
            for (int i = 0; i < ivPart.Length; i++)
            {
                ivPart[i] = Chars[_rnd.Next(0, Chars.Length - 1)];
            }

            return new AesKey(new string(keyPart), new string(ivPart));
        }
    }
}
