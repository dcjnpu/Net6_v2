using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using XC.RSAUtil;

namespace Cx.Data
{
    /// <summary>
    /// RSA加密解密
    /// https://github.com/stulzq/RSAUtil
    /// </summary>
    public class RSACrypt
    {
        private void Demo()
        {
            string pubkey = string.Empty;
            //RsaKeyConvert.PublicKeyXmlToPem(pubkey);
            //XML->Pkcs1：
            RsaKeyConvert.PrivateKeyXmlToPkcs1(pubkey);
            RsaKeyConvert.PublicKeyXmlToPem(pubkey);
            //XML->Pkcs8：
            //私钥：
            RsaKeyConvert.PrivateKeyXmlToPkcs8(pubkey);
            //公钥：
            RsaKeyConvert.PublicKeyXmlToPem(pubkey);
            //Pkcs1->XML：
            //私钥：
            RsaKeyConvert.PrivateKeyPkcs1ToXml(pubkey);
            //公钥：
            RsaKeyConvert.PublicKeyPemToXml(pubkey);
            //Pkcs1->Pkcs8：
            //私钥：
            RsaKeyConvert.PrivateKeyPkcs1ToPkcs8(pubkey);
            //公钥：不需要转换
            //Pkcs8->XML：
            //私钥：
            RsaKeyConvert.PrivateKeyPkcs8ToXml(pubkey);
            //公钥：
            RsaKeyConvert.PublicKeyPemToXml(pubkey);
            //Pkcs8->Pkcs1：
            //私钥：
            RsaKeyConvert.PrivateKeyPkcs8ToPkcs1(pubkey);
            //公钥：不需要转换
        }


        private readonly RSAUtilBase _RsaUtil;
        private readonly Encoding _encoding;

        /// <summary>
        /// 获得私钥和公钥
        /// [0]=privateKey  私钥 
        /// [1]=publicKey  公钥
        /// </summary>
        /// <returns></returns>
        public static List<string> GetPkcs1Key()
        {
            return RsaKeyGenerator.Pkcs1Key(2048, true);
        }
        public static List<string> GetPkcs8Key()
        {
            return RsaKeyGenerator.Pkcs8Key(2048, true);
        }
        public static List<string> GetXmlKey()
        {
            return RsaKeyGenerator.XmlKey(2048);
        }


        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="encoding">编码类型</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="publicKey">公钥</param>
        public RSACrypt(E_RsaType RsaType, string privateKey, string publicKey)
        {
            _encoding = Encoding.UTF8;
            switch (RsaType)
            {
                case E_RsaType.Pkcs1: _RsaUtil = new RsaPkcs1Util(_encoding, publicKey, privateKey); break;
                case E_RsaType.Pkcs8: _RsaUtil = new RsaPkcs8Util(_encoding, publicKey, privateKey); break;
                default: _RsaUtil = new RsaXmlUtil(_encoding, publicKey, privateKey); break;
            }

        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="code">加密代码</param>
        /// <returns></returns>
        public string Encrypt(string code)
        {
            return _RsaUtil.RsaEncrypt(code, RSAEncryptionPadding.Pkcs1);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="code">解密代码</param>
        /// <returns></returns>
        public string Decrypt(string code)
        {
            return _RsaUtil.RsaDecrypt(code, RSAEncryptionPadding.Pkcs1);
        }

        public string SignData(string data)
        {
            return _RsaUtil.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        public bool CheckSign(string data, string sign)
        {
            return _RsaUtil.VerifyData(data, sign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }

    public enum E_RsaType
    {
        Pkcs1 = 1,
        Pkcs8 = 2,
        Xml = 3,
    }

    /// <summary>
    /// Rsa，带分段加解密，不介意用了，请用RSACrypt，已验证
    /// </summary>
    public class RSACrypt2
    {
        public const string token_prikey = "<RSAKeyValue><Modulus>s9fBxcgJmBBcH4ZOUmcZAXqh7nCHr+VAKWDVJWH66TC1oxWTgWctICixDdREN1pb109gFkaAHpAIVA11Sqwa3MZynz18CKb8Hm0bS8Pnt2fYqmh5teAOxv1mzOhDj7Xh3z5G3gVA5twZEgQKY4p2qlNDUY/TKYAuoQV681qkQnk=</Modulus><Exponent>AQAB</Exponent><P>4MMkUcU74eztmtrZsRw6Fff1G4Qxj+toirBxFwnZRXLnyrxKDfm5Xp6BG6m1wZjIUZeDNUvdGsOwzYjOhwnhjw==</P><Q>zNZqui006DRlLunYRaStKsakxPQldTfR2jaeYqBNMDtUuVhUN142SGAQ6XDBwKCbRw1XUkW+g+clqC7vLkaHdw==</Q><DP>W3gvHLQG1b/lRqMP+tsA7JXXg1Y+EVL9XYvGylhwuqmzmTcdcCiT/uTEqCxd0qbJxyLA+/yew9il6E9ntV8ZEw==</DP><DQ>kV10/kzIqCVOzT08syfLkHjKQ89zqb0EWxS50Aos3Rat6J7LnUNpyHEMb24KwhX4PDM2jfda3aazLaipKrD4vw==</DQ><InverseQ>y3JbIfefgHIqeVykoIqW5cC8y4JT1Y/ajuUQADjrqtWuj5w0eRfRr1DcFn4pabxmlMjXPLopIbB6kexhOz/OFA==</InverseQ><D>dXDm5C5ePYQpdJizGtKinLmld1za7r2JabwHVNMoFBGv+6DCY5Wp/T81Z+9+4JrMMM1v1PwS/GJYXfMv7iR3oS4XyaBf1uDtrF+s1OEIgJOYTMVUB8p0WarYeC76VaYMUi6/U6wSCSUPehhUOtGimkaR5u9oR6vF8YhyFN0y5WU=</D></RSAKeyValue>";
        public const string token_pubkey = "<RSAKeyValue><Modulus>s9fBxcgJmBBcH4ZOUmcZAXqh7nCHr+VAKWDVJWH66TC1oxWTgWctICixDdREN1pb109gFkaAHpAIVA11Sqwa3MZynz18CKb8Hm0bS8Pnt2fYqmh5teAOxv1mzOhDj7Xh3z5G3gVA5twZEgQKY4p2qlNDUY/TKYAuoQV681qkQnk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        public static string Rsa_Encrypt(string strText, string strPublicKey)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(strPublicKey);

                byte[] data = Encoding.UTF8.GetBytes(strText);
                //byte[] byteEntry = rsa.Encrypt(byteText, false);
                //return Convert.ToBase64String(byteEntry);

                int keySize = rsa.KeySize / 8;
                int bufferSize = keySize - 11;
                byte[] buffer = new byte[bufferSize];

                MemoryStream msInput = new MemoryStream(data);
                MemoryStream msOutput = new MemoryStream();
                int readLen = msInput.Read(buffer, 0, bufferSize);
                while (readLen > 0)
                {
                    byte[] dataToEnc = new byte[readLen];
                    Array.Copy(buffer, 0, dataToEnc, 0, readLen);

                    byte[] encData = rsa.Encrypt(dataToEnc, false);
                    msOutput.Write(encData, 0, encData.Length);

                    readLen = msInput.Read(buffer, 0, bufferSize);
                }
                msInput.Close();

                byte[] result = msOutput.ToArray();
                //得到加密结果
                msOutput.Close();
                rsa.Clear();
                return Convert.ToBase64String(result);
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
        public static string Rsa_Decrypt(string strEntryText, string strPrivateKey)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(strPrivateKey);
                byte[] byteEntry = Convert.FromBase64String(strEntryText);
                //byte[] byteText = rsa.Decrypt(byteEntry, false);
                //return Encoding.UTF8.GetString(byteText);

                int keySize = rsa.KeySize / 8;


                byte[] buffer = new byte[keySize];


                MemoryStream msInput = new MemoryStream(byteEntry);


                MemoryStream msOutput = new MemoryStream();


                int readLen = msInput.Read(buffer, 0, keySize);


                while (readLen > 0)
                {

                    byte[] dataToDec = new byte[readLen];


                    Array.Copy(buffer, 0, dataToDec, 0, readLen);


                    byte[] decData = rsa.Decrypt(dataToDec, false);


                    msOutput.Write(decData, 0, decData.Length);


                    readLen = msInput.Read(buffer, 0, keySize);
                }


                msInput.Close();


                byte[] result = msOutput.ToArray();
                //得到解密结果
                msOutput.Close();
                rsa.Clear();
                return Encoding.UTF8.GetString(result);

            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }



    }
}
