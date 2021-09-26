using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Crypto
{
    public class DesKey
    {
        /// <summary>
        /// The key, length is 8, generated on https://www.random.org/strings/
        /// You can also use the GenerateKey method in the DESCryptoServiceProvider to generate the key.
        /// </summary>
        public string KeyPart { get; }

        /// <summary>
        /// The iv, length is 8, generated on https://www.random.org/strings/
        /// You can also use the GenerateIV method in the DESCryptoServiceProvider to generate the iv.
        /// </summary>
        public string IvPart { get; }

        /// <summary>
        /// The combine key.
        /// </summary>
        internal readonly byte[] Key;

        internal readonly byte[] Iv;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyPart">The key, length is 8.</param>
        /// <param name="ivPart">The iv, length is 8.</param>
        public DesKey(string keyPart, string ivPart)
        {
            KeyPart = keyPart;
            IvPart = ivPart;

            Key = Encoding.ASCII.GetBytes(KeyPart);
            Iv = Encoding.ASCII.GetBytes(IvPart);
        }
    }
}
