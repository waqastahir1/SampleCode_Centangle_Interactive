using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SOS.OrderTracking.Web.Common.Extenstions
{
    public class CustomLookupProtectorKeyRing : ILookupProtectorKeyRing
    {
        public string this[string keyId]
        {
            get
            {
                return GetAllKeyIds().Where(x => x == keyId).FirstOrDefault();
            }
        }

        public string CurrentKeyId
        {
            get
            {
                byte[] key = { 200, 15, 147, 5, 155, 78, 118, 57, 180, 179, 60, 150, 188, 18, 165, 134 };
                var currentKey = Convert.ToBase64String(key);
                return currentKey;
            }
        }

        public IEnumerable<string> GetAllKeyIds()
        {
            var list = new List<string>();
            byte[] key = { 200, 15, 147, 5, 155, 78, 118, 57, 180, 179, 60, 150, 188, 18, 165, 134 };

            list.Add(Convert.ToBase64String(key));
            return list;
        }
    }
    public class CustomLookupProtector : ILookupProtector
    {
        byte[] iv = { 208, 148, 29, 187, 168, 51, 181, 178, 137, 83, 40, 13, 28, 177, 131, 248 };
        public string Protect(string keyId, string data)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(data);

            string cipherText;
            using (SymmetricAlgorithm algorithm = Aes.Create())
            {
                using (ICryptoTransform encryptor = algorithm.CreateEncryptor(Encoding.UTF8.GetBytes(keyId), iv))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.Close();
                            byte[] chiperTextByte = ms.ToArray();
                            cipherText = Convert.ToBase64String(chiperTextByte);
                        }
                    }
                }
            }

            return cipherText;
        }


        public string Unprotect(string keyId, string data)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(data);
            string plainText;
            using (SymmetricAlgorithm algorithm = Aes.Create())
            {
                using (ICryptoTransform decrypter = algorithm.CreateDecryptor(Encoding.UTF8.GetBytes(keyId), iv))
                {
                    using (MemoryStream ms = new MemoryStream(cipherTextBytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(ms, decrypter, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader(cryptoStream))
                            {
                                plainText = streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }

            return plainText;
        }
    }

    public class PersonalDataProtector : IPersonalDataProtector
    {
        byte[] key = { 200, 15, 147, 5, 155, 78, 118, 57, 180, 179, 60, 150, 188, 18, 165, 134 };
        byte[] iv = { 208, 148, 29, 187, 168, 51, 181, 178, 137, 83, 40, 13, 28, 177, 131, 248 };

        public string Protect(string data)
        {
            var keyId = Convert.ToBase64String(key);

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(data);

            string cipherText;
            using (SymmetricAlgorithm algorithm = Aes.Create())
            {
                using (ICryptoTransform encryptor = algorithm.CreateEncryptor(Encoding.UTF8.GetBytes(keyId), iv))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.Close();
                            byte[] chiperTextByte = ms.ToArray();
                            cipherText = Convert.ToBase64String(chiperTextByte);
                        }
                    }
                }
            }

            return cipherText;
        }

        public string Unprotect(string data)
        {
            var keyId = Convert.ToBase64String(key);

            byte[] cipherTextBytes = Convert.FromBase64String(data);
            string plainText;
            using (SymmetricAlgorithm algorithm = Aes.Create())
            {
                using (ICryptoTransform decrypter = algorithm.CreateDecryptor(Encoding.UTF8.GetBytes(keyId), iv))
                {
                    using (MemoryStream ms = new MemoryStream(cipherTextBytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(ms, decrypter, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader(cryptoStream))
                            {
                                plainText = streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }

            return plainText;
        }
    }
}

// Code to encrypt existingUsers
//var users = dbContext.Users.Where(x => x.UserName == "waqastahir2@gmail.com").ToList();

//foreach (var user in users)
//{
//    var key = keyRing.CurrentKeyId;
//    user.Email = key + ":" + dataProtectionProvider.Protect(key, user.Email);
//    user.NormalizedEmail = dataProtectionProvider.Protect(key, user.UserName.ToUpper());
//    user.UserName = key + ":" + dataProtectionProvider.Protect(key, user.UserName);
//    user.NormalizedUserName = user.NormalizedEmail;

//    user.Firstname = key + ":" + dataProtectionProvider.Protect(key, user.Firstname);
//    user.Lastname = key + ":" + dataProtectionProvider.Protect(key, user.Lastname);
//    // Update other properties as needed
//}

//dbContext.SaveChanges();