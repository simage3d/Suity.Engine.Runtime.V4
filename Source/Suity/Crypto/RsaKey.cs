using System.Security.Cryptography;

namespace Suity.Crypto
{
    public class RsaKey
    {
        public string Private { get; set; }

        public string Public { get; set; }


        public static RsaKey GenerateKeys()
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                var key = new RsaKey
                {
                    Private = rsa.ToXmlString(true),
                    Public = rsa.ToXmlString(false)
                };

                return key;
            }
        }
    }
}