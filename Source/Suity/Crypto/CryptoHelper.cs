using Suity.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Suity.Crypto
{
    public static class CryptoHelper
    {
        #region Aes
        public static byte[] AesEncrypt(AesKey key, byte[] plainBytes, int offset, int length)
        {
            return AesEncrypt(key, plainBytes, offset, length, CipherMode.CBC, PaddingMode.PKCS7);
        }

        public static byte[] AesDecrypt(AesKey key, byte[] encryptedBytes, int offset, int length)
        {
            return AesDecrypt(key, encryptedBytes, offset, length, CipherMode.CBC, PaddingMode.PKCS7);
        }

        private static byte[] AesEncrypt(AesKey key, byte[] plainBytes, int offset, int length, CipherMode cipher, PaddingMode padding)
        {
            using (var aes = Rijndael.Create())
            {
                aes.Mode = cipher;
                aes.Padding = padding;

                using (var transform = aes.CreateEncryptor(key.Key, key.Iv))
                {
                    var encryptedBytes = transform.TransformFinalBlock(plainBytes, offset, length);
                    return encryptedBytes;
                }
            }
        }

        private static byte[] AesDecrypt(AesKey key, byte[] encryptedBytes, int offset, int length, CipherMode cipher, PaddingMode padding)
        {
            using (var aes = Rijndael.Create())
            {
                aes.Mode = cipher;
                aes.Padding = padding;

                using (var transform = aes.CreateDecryptor(key.Key, key.Iv))
                {
                    var plainBytes = transform.TransformFinalBlock(encryptedBytes, offset, length);
                    return plainBytes;
                }
            }
        }
        #endregion

        #region Des

        public static byte[] DesEncrypt(DesKey key, byte[] plainBytes)
        {
            using (var provider = new DESCryptoServiceProvider())
            {
                provider.Key = key.Key;
                provider.IV = key.Iv;
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, provider.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                        cryptoStream.FlushFinalBlock();
                    }
                    return memoryStream.ToArray();
                }
            }
        }

        public static byte[] DesDecrypt(DesKey key, byte[] encryptedBytes)
        {
            using (var provider = new DESCryptoServiceProvider())
            {
                provider.Key = key.Key;
                provider.IV = key.Iv;
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, provider.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                        cryptoStream.FlushFinalBlock();
                    }
                    return memoryStream.ToArray();
                }
            }
        }
        #endregion

        #region Md5

        public static byte[] Md5Encrypt(byte[] plainBytes)
        {
            using (var md5 = MD5.Create())
            {
                var encryptedBytes = md5.ComputeHash(plainBytes);
                return encryptedBytes;
            }
        }

        #endregion

        #region Rsa

        public static byte[] RsaSign(string privateKey, byte[] bytes)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                var signature = rsa.SignData(bytes, new MD5CryptoServiceProvider());
                return signature;
            }
        }

        public static bool RsaVerify(string publicKey, byte[] bytes, byte[] signature)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                return rsa.VerifyData(bytes, new MD5CryptoServiceProvider(), signature);
            }
        }

        public static byte[] RsaEncrypt(string publicKey, byte[] plainBytes)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                var encryptedBytes = rsa.Encrypt(plainBytes, false);
                return encryptedBytes;
            }
        }

        public static byte[] RsaDecrypt(string privateKey, byte[] encryptedBytes)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                var decryptedBytes = rsa.Decrypt(encryptedBytes, false);
                return decryptedBytes;
            }
        }


        public static byte[] RsaEncryptLarge(string publicKey, byte[] plainBytes)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                var bufferSize = (rsa.KeySize / 8 - 11);
                byte[] buffer = new byte[bufferSize];//待加密块

                using (MemoryStream msInput = new MemoryStream(plainBytes))
                {
                    using (MemoryStream msOutput = new MemoryStream())
                    {
                        int readLen;
                        while ((readLen = msInput.Read(buffer, 0, bufferSize)) > 0)
                        {
                            byte[] dataToEnc = new byte[readLen];
                            Array.Copy(buffer, 0, dataToEnc, 0, readLen);
                            byte[] encData = rsa.Encrypt(dataToEnc, false);
                            msOutput.Write(encData, 0, encData.Length);
                        }

                        byte[] result = msOutput.ToArray();
                        rsa.Clear();
                        return result;
                    }
                }
            }
        }

        public static byte[] RsaDecryptLarge(string privateKey, byte[] encryptedBytes)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                int keySize = rsa.KeySize / 8;
                byte[] buffer = new byte[keySize];
                using (MemoryStream msInput = new MemoryStream(encryptedBytes))
                {
                    using (MemoryStream msOutput = new MemoryStream())
                    {
                        int readLen;

                        while ((readLen = msInput.Read(buffer, 0, keySize)) > 0)
                        {
                            byte[] dataToDec = new byte[readLen];
                            Array.Copy(buffer, 0, dataToDec, 0, readLen);
                            byte[] decData = rsa.Decrypt(dataToDec, false);
                            msOutput.Write(decData, 0, decData.Length);
                        }

                        byte[] result = msOutput.ToArray();
                        rsa.Clear();

                        return result;
                    }
                }
            }
        }


        public static string RsaEncrypt(string publicKey, string plainText)
        {
            byte[] plainBytes = ByteHelpers.FromString(plainText);
            byte[] encryptedBytes = RsaEncrypt(publicKey, plainBytes);
            return ByteHelpers.ToBase64(encryptedBytes);
        }
        public static string RsaDecrypt(string privateKey, string encryptedText)
        {
            byte[] encryptedBytes = ByteHelpers.FromBase64(encryptedText);
            byte[] plainBytes = RsaDecrypt(privateKey, encryptedBytes);
            return ByteHelpers.ToString(plainBytes);
        }
        public static string RsaEncryptLarge(string publicKey, string plainText)
        {
            byte[] plainBytes = ByteHelpers.FromString(plainText);
            byte[] encryptedBytes = RsaEncryptLarge(publicKey, plainBytes);
            return ByteHelpers.ToBase64(encryptedBytes);
        }
        public static string RsaDecryptLarge(string privateKey, string encryptedText)
        {
            byte[] encryptedBytes = ByteHelpers.FromBase64(encryptedText);
            byte[] plainBytes = RsaDecryptLarge(privateKey, encryptedBytes);
            return ByteHelpers.ToString(plainBytes);
        }
        public static string RsaSign(string privateKey, string text)
        {
            byte[] bytes = ByteHelpers.FromString(text);
            byte[] signatureBytes = RsaSign(privateKey, bytes);
            return ByteHelpers.ToBase64(signatureBytes);
        }
        public static bool RsaVerify(string publicKey, string text, string signature)
        {
            byte[] bytes = ByteHelpers.FromString(text);
            byte[] signatureBytes = ByteHelpers.FromBase64(signature);
            return RsaVerify(publicKey, bytes, signatureBytes);
        }

        #endregion

        #region Salt

        public static byte[] GenerateSalt(int size)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var salt = new byte[size];
                rng.GetBytes(salt);
                return salt;
            }
        }

        #endregion

        #region Sha1

        public static byte[] Sha1Encrypt(byte[] plainBytes)
        {
            using (var sha1 = SHA1.Create())
            {
                var encryptedBytes = sha1.ComputeHash(plainBytes);
                return encryptedBytes;
            }
        }

        #endregion
    }
}
