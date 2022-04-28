using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Data;
using System.Reflection;
using System.Web;
using System.Collections;

namespace Cx.Data
{
    public class DataUtils
    {
        /// <summary>
        /// 生成随机字母字符串(数字字母混和) 不推荐
        /// </summary>
        /// <param name="codeCount">待生成的位数</param>
        public static string GetCheckCode(int codeCount)
        {
            string str = string.Empty;
            //int rep = 0;
            //long num2 = DateTime.Now.Ticks + rep;
            //rep++;
            // int seed = ((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep));

            for (int i = 0; i < codeCount; i++)
            {

                char ch;
                int num = Random.Shared.Next(1, 1000);
                ch = (char)(0x30 + ((ushort)(num % 10)));
                //if ((num % 2) == 0)
                //{
                //    ch = (char)(0x30 + ((ushort)(num % 10)));
                //}
                //else
                //{
                //    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                //}
                str = str + ch.ToString();
            }
            return str;
        }

        /// <summary>
        /// 将字符串转换为int类型数组
        /// </summary>
        /// <param name="str">如1,2,3,4,5</param>
        /// <returns></returns>
        public static List<int> StrToListInt(string str)
        {
            var list = new List<int>();
            if (!str.Contains(","))
            {
                list.Add(int.Parse(str));
                return list;
            }
            var slist = str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in slist)
            {
                list.Add(int.Parse(item));
            }
            return list;
        }

        /// <summary>
        /// 将字符串转换为int类型数组
        /// </summary>
        /// <param name="str">如1,2,3,4,5</param>
        /// <returns></returns>
        public static List<string> StrToListString(string str)
        {
            var list = new List<string>();
            if (!str.Contains(","))
            {
                list.Add(str);
                return list;
            }
            var slist = str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in slist)
            {
                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// 将字符串转换为数组
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>字符串数组</returns>
        public static string[] GetStrArray(string str)
        {
            return str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        #region 字符串处理
        /// <summary>
        /// 获取指定字节长度的字符串,汉字占2，其他占1
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="MaxLen"></param>
        /// <returns></returns>
        public static string GetStringLimit(string Source, int MaxLen)
        {
            int Len = 0;
            if (string.IsNullOrEmpty(Source)) return "";

            if (Source.Length * 2 <= MaxLen) return Source;

            string A = string.Empty; string Result = string.Empty;
            for (int i = 0; i < Source.Length; i++)
            {
                A = Source.Substring(i, 1);
                if (RegexHelper.IsZH(A))
                    Len += 2;
                else
                    Len++;

                if (Len > MaxLen)
                    break;
                else
                    Result += A;
            }
            return Result;
        }

        /// <summary>
        /// 取左，增加判断长度，防止错误
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static string GetLeft(string ob, int Length)
        {
            if (string.IsNullOrEmpty(ob)) return "";
            int Len = ob.Length;
            Len = Math.Min(Len, Length);
            return ob.Substring(0, Len);
        }
        /// <summary>
        /// 取右，增加判断长度，防止错误
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static string GetRight(string ob, int Length)
        {
            if (string.IsNullOrEmpty(ob)) return "";
            int Len = ob.Length;
            int i = Math.Min(Len, Length);
            return ob.Substring(Len - i, i);

        }

        /// <summary>
        /// 对获取的字符进行唯一 ' 的替换
        /// </summary>
        /// <param name="Value">要替换的字符</param>
        /// <returns>string</returns>
        public static string DoSqlReplace(string Value)
        {
            if (string.IsNullOrEmpty(Value)) return "";
            StringBuilder Sb = new StringBuilder(Value);
            Sb = Sb.Replace("'", "&#39;");
            return Sb.ToString();
        }

        #endregion

        #region 截取字符串
        public static string GetSubString(string pSrcString, int pLength, string pTailString)
        {
            return GetSubString(pSrcString, 0, pLength, pTailString);
        }
        public static string GetSubString(string pSrcString, int pStartIndex, int pLength, string pTailString)
        {
            string str = pSrcString;
            byte[] bytes = Encoding.UTF8.GetBytes(pSrcString);
            foreach (char ch in Encoding.UTF8.GetChars(bytes))
            {
                if (((ch > 'ࠀ') && (ch < '一')) || ((ch > 0xac00) && (ch < 0xd7a3)))
                {
                    if (pStartIndex >= pSrcString.Length)
                    {
                        return "";
                    }
                    return pSrcString.Substring(pStartIndex, ((pLength + pStartIndex) > pSrcString.Length) ? (pSrcString.Length - pStartIndex) : pLength);
                }
            }
            if (pLength < 0)
            {
                return str;
            }
            byte[] sourceArray = Encoding.Default.GetBytes(pSrcString);
            if (sourceArray.Length <= pStartIndex)
            {
                return str;
            }
            int length = sourceArray.Length;
            if (sourceArray.Length > (pStartIndex + pLength))
            {
                length = pLength + pStartIndex;
            }
            else
            {
                pLength = sourceArray.Length - pStartIndex;
                pTailString = "";
            }
            int num2 = pLength;
            int[] numArray = new int[pLength];
            byte[] destinationArray = null;
            int num3 = 0;
            for (int i = pStartIndex; i < length; i++)
            {
                if (sourceArray[i] > 0x7f)
                {
                    num3++;
                    if (num3 == 3)
                    {
                        num3 = 1;
                    }
                }
                else
                {
                    num3 = 0;
                }
                numArray[i] = num3;
            }
            if ((sourceArray[length - 1] > 0x7f) && (numArray[pLength - 1] == 1))
            {
                num2 = pLength + 1;
            }
            destinationArray = new byte[num2];
            Array.Copy(sourceArray, pStartIndex, destinationArray, 0, num2);
            return (Encoding.Default.GetString(destinationArray) + pTailString);
        }
        #endregion

        #region 截取字符长度
        /// <summary>
        /// 截取字符长度
        /// </summary>
        /// <param name="inputString">字符</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string CutString(string inputString, int len)
        {
            if (string.IsNullOrEmpty(inputString))
                return "";
            inputString = DropHtml(inputString);
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }

                try
                {
                    tempString += inputString.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (tempLen > len)
                    break;
            }
            //如果截过则加上半个省略号 
            byte[] mybyte = System.Text.Encoding.Default.GetBytes(inputString);
            if (mybyte.Length > len)
                tempString += "…";
            return tempString;
        }

        public static string DropHtml(string htmlstring)
        {
            if (string.IsNullOrEmpty(htmlstring)) return "";
            //删除脚本  
            htmlstring = Regex.Replace(htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML  
            htmlstring = Regex.Replace(htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);

            htmlstring = Regex.Replace(htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            htmlstring = htmlstring.Replace("<", "");
            htmlstring = htmlstring.Replace(">", "");
            htmlstring = htmlstring.Replace("\r\n", "");
            //htmlstring = HttpContext.Current.Server.HtmlEncode(htmlstring).Trim(); 
            return htmlstring;
        }
        #endregion

        #region 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母
        /// <summary>
        /// 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母
        /// </summary>
        /// <param name="CnChar">单个汉字</param>
        /// <returns>单个大写字母</returns>
        public static string GetCharSpellCode(string CnChar)
        {
            long iCnChar;
            byte[] ZW = Encoding.Default.GetBytes(CnChar);

            //如果是字母，则直接返回首字母
            if (ZW.Length == 1)
            {
                return CutString(CnChar.ToUpper(), 1);
            }
            else
            {
                int i1 = (short)(ZW[0]);
                int i2 = (short)(ZW[1]);
                iCnChar = i1 * 256 + i2;
            }
            // iCnChar match the constant
            if ((iCnChar >= 45217) && (iCnChar <= 45252))
            {
                return "A";
            }
            else if ((iCnChar >= 45253) && (iCnChar <= 45760))
            {
                return "B";
            }
            else if ((iCnChar >= 45761) && (iCnChar <= 46317))
            {
                return "C";
            }
            else if ((iCnChar >= 46318) && (iCnChar <= 46825))
            {
                return "D";
            }
            else if ((iCnChar >= 46826) && (iCnChar <= 47009))
            {
                return "E";
            }
            else if ((iCnChar >= 47010) && (iCnChar <= 47296))
            {
                return "F";
            }
            else if ((iCnChar >= 47297) && (iCnChar <= 47613))
            {
                return "G";
            }
            else if ((iCnChar >= 47614) && (iCnChar <= 48118))
            {
                return "H";
            }
            else if ((iCnChar >= 48119) && (iCnChar <= 49061))
            {
                return "J";
            }
            else if ((iCnChar >= 49062) && (iCnChar <= 49323))
            {
                return "K";
            }
            else if ((iCnChar >= 49324) && (iCnChar <= 49895))
            {
                return "L";
            }
            else if ((iCnChar >= 49896) && (iCnChar <= 50370))
            {
                return "M";
            }
            else if ((iCnChar >= 50371) && (iCnChar <= 50613))
            {
                return "N";
            }
            else if ((iCnChar >= 50614) && (iCnChar <= 50621))
            {
                return "O";
            }
            else if ((iCnChar >= 50622) && (iCnChar <= 50905))
            {
                return "P";
            }
            else if ((iCnChar >= 50906) && (iCnChar <= 51386))
            {
                return "Q";
            }
            else if ((iCnChar >= 51387) && (iCnChar <= 51445))
            {
                return "R";
            }
            else if ((iCnChar >= 51446) && (iCnChar <= 52217))
            {
                return "S";
            }
            else if ((iCnChar >= 52218) && (iCnChar <= 52697))
            {
                return "T";
            }
            else if ((iCnChar >= 52698) && (iCnChar <= 52979))
            {
                return "W";
            }
            else if ((iCnChar >= 52980) && (iCnChar <= 53640))
            {
                return "X";
            }
            else if ((iCnChar >= 53689) && (iCnChar <= 54480))
            {
                return "Y";
            }
            else if ((iCnChar >= 54481) && (iCnChar <= 55289))
            {
                return "Z";
            }
            else
                return ("?");

        }
        #endregion

        #region 获得IP地址
        /// <summary>
        /// 获得IP地址
        /// </summary>
        /// <returns>字符串数组</returns>
        public static string GetIp()
        {
            HttpContextAccessor _context = new HttpContextAccessor();
            var ip = _context.HttpContext.Request.Headers["X-Forwarded-For"].ToString();
            if (string.IsNullOrEmpty(ip))
            {
                ip = _context.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }
        #endregion

        #region 获得当前访问的URL地址
        /// <summary>
        /// 获得当前访问的URL地址
        /// </summary>
        /// <returns>字符串数组</returns>
        public static string GetUrl()
        {
            HttpContextAccessor _context = new HttpContextAccessor();
            return _context.HttpContext.Request.Path.ToString();
        }
        #endregion

        #region 分割字符串
        public static string[] SplitString(string strContent, char strSplit)
        {
            if (!string.IsNullOrEmpty(strContent))
            {
                return strContent.Split(new char[] { strSplit }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                return new string[0] { };
            }
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!string.IsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit, StringComparison.Ordinal) < 0)
                    return new string[] { strContent };

                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
                return new string[0] { };
        }
        #endregion

        #region 显示错层方法
        /// <summary>
        /// 显示错层方法
        /// </summary>
        public static string LevelName(string name, int? level)
        {
            if (level > 1)
            {
                string nbsp = "";
                for (int i = 0; i < level; i++)
                {
                    nbsp += "　";
                }
                name = nbsp + "|--" + name;
            }
            return name;
        }
        #endregion

        #region 生成随机字母或数字

        private static readonly Random Random = new Random();

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <returns></returns>
        public static string Number(int length)
        {
            return Number(length, false);
        }

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <param name="sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns></returns>
        public static string Number(int length, bool sleep)
        {
            if (sleep)
                System.Threading.Thread.Sleep(2);
            string result = "";

            for (int i = 0; i < length; i++)
            {
                result += Random.Next(10).ToString();
            }
            return result;
        }


        /// <summary>
        /// 根据日期和随机码生成订单号
        /// </summary>
        /// <returns></returns>
        public static string GetOrderNumber()
        {
            string num = DateTime.Now.ToString("yyyyMMddHHmmssms"); //yyyyMMddHHmmssms
            return num + Number(2);
        }

        #endregion

        #region Base64位加密解密
        /// <summary>
        /// 将字符串转换成base64格式,使用UTF8字符集
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <returns></returns>
        public static string Base64Encode(string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// 将base64格式，转换utf8
        /// </summary>
        /// <param name="content">解密内容</param>
        /// <returns></returns>
        public static string Base64Decode(string content)
        {
            byte[] bytes = Convert.FromBase64String(content);
            return Encoding.UTF8.GetString(bytes);
        }
        #endregion

        #region Des加密
        /// <summary>
        /// DES加密方法 指定加密 密钥：setting.config keys:defaultdes
        /// </summary>
        /// <param name="Value">要加密的字符</param>
        /// <param name="Flag">加密是否成功</param>
        /// <returns>string</returns>
        public static string DES_EnCode(string Value)
        {
            if (string.IsNullOrEmpty(Value)) return Value;
            //string Key = Base.GetKeyValue("deskey", "abcdefgh");
            string Key = ConfigSetting.Data.GetSection("keys:defaultdes").Value;

            Key = Key.Substring(0, 8);
            try
            {
                DES DES1 = DES.Create();
                DES1.Mode = CipherMode.ECB;

                byte[] bvs = Encoding.UTF8.GetBytes(Value);
                byte[] bks = Encoding.UTF8.GetBytes(Key);
                DES1.Key = bks;// Encoding.UTF8.GetBytes(Key);
                DES1.IV = bks;// Encoding.UTF8.GetBytes(Key);
                MemoryStream MStream = new MemoryStream();
                CryptoStream CStream = new CryptoStream(MStream, DES1.CreateEncryptor(), CryptoStreamMode.Write);
                CStream.Write(bvs, 0, bvs.Length);
                CStream.FlushFinalBlock();
                CStream.Close(); CStream.Dispose();

                StringBuilder ret = new StringBuilder();
                foreach (byte b in MStream.ToArray())
                {
                    ret.AppendFormat("{0:x2}", b);
                }
                Value = ret.ToString();



                // Value = Convert.ToBase64String(MStream.ToArray());
                MStream.Close(); MStream.Dispose();

                return Value;
            }
            catch { return Value; }
        }
        /// <summary>
        /// DES解密方法 密钥：setting.config keys:defaultdes
        /// </summary>
        /// <param name="Value">要解密的字符</param>
        /// <param name="Flag">解密是否成功</param>
        /// <returns>string</returns>
        public static string DES_DeCode(string Value)
        {
            if (string.IsNullOrEmpty(Value)) return Value;
            // string Key = Base.GetKeyValue("deskey", "abcdefgh");
            string Key = ConfigSetting.Data.GetSection("keys:defaultdes").Value;
            Key = Key.Substring(0, 8);
            try
            {
                byte[] bvs = new byte[Value.Length / 2];
                for (int x = 0; x < Value.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(Value.Substring(x * 2, 2), 16));
                    bvs[x] = (byte)i;
                }

                DES DES = DES.Create();
                //DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
                DES.Mode = CipherMode.ECB;
                byte[] bks = Encoding.UTF8.GetBytes(Key);
                DES.Key = bks;// Encoding.UTF8.GetBytes(Key);
                DES.IV = bks;// Encoding.UTF8.GetBytes(Key);
                MemoryStream MStream = new MemoryStream();
                CryptoStream CStream = new CryptoStream(MStream, DES.CreateDecryptor(), CryptoStreamMode.Write);
                CStream.Write(bvs, 0, bvs.Length);
                CStream.FlushFinalBlock();
                CStream.Close(); CStream.Dispose();
                Value = Encoding.UTF8.GetString(MStream.ToArray());
                MStream.Close(); MStream.Dispose();

                return Value;
            }
            catch { return Value; }
        }
        #endregion

        #region MD5加密
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="Object">要操作的 string  数据</param>
        /// <returns>string</returns>
        public static string MD5(string Object)
        {
            if (string.IsNullOrEmpty(Object)) return "";
            MD5 algorithm = System.Security.Cryptography.MD5.Create();
            byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(Object));
            StringBuilder sh1 = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sh1.Append(data[i].ToString("x2").ToUpperInvariant());
            }
            return sh1.ToString();
        }
        #endregion

        #region sha1
        /// <summary>
        /// 获取SHA1算法加密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetSwcSHA1(string value)
        {
            SHA1 algorithm = SHA1.Create();
            byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            string sh1 = "";
            for (int i = 0; i < data.Length; i++)
            {
                sh1 += data[i].ToString("x2").ToUpperInvariant();
            }
            return sh1;
        }
        /// <summary>
        /// 获取SHA1算法加密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SHA1_Encrypt(string value)
        {
            byte[] StrRes = Encoding.Default.GetBytes(value);
            SHA1 iSHA = SHA1.Create();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString().ToUpper();
        }
        #endregion

        #region Rsa256 --未测试
        public static string SHA2_Encrypt(string contentForSign, string privateKey)
        {
            //转换成适用于.Net的秘钥
            //var netKey = RSAPrivateKeyJava2DotNet(privateKey);
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            //创建一个空对象
            var rsaClear = new RSACryptoServiceProvider();
            var paras = rsa.ExportParameters(true);
            rsaClear.ImportParameters(paras);
            //签名返回
            using (var sha256 = SHA256.Create())
            {
                var signData = rsa.SignData(Encoding.UTF8.GetBytes(contentForSign), sha256);
                return BytesToHex(signData);
            }
        }


        public static string BytesToHex(byte[] data)
        {
            StringBuilder sbRet = new StringBuilder(data.Length * 2);
            for (int i = 0; i < data.Length; i++)
            {
                sbRet.Append(Convert.ToString(data[i], 16).PadLeft(2, '0'));
            }
            return sbRet.ToString();
        }

        /// <summary>
        /// 有问题，待解决
        /// </summary>
        /// <param name="plaintext"></param>
        /// <param name="APIKey"></param>
        /// <returns></returns>
        public static string SHA256_WX(string plaintext, string APIKey)
        {

            byte[] baText2BeHashed = Encoding.UTF8.GetBytes(plaintext),
            baSalt = Encoding.UTF8.GetBytes(APIKey);
            HMACSHA256 hasher = new HMACSHA256(baSalt);

            byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);
            string sign = Convert.ToBase64String(baHashedText);
            return sign;
            //result = string.Join("", baHashedText.ToList().Select(b => b.ToString("x2")).ToArray());
            //return result.ToUpper();
        }
        /// <summary>
        /// 已验证
        /// </summary>
        /// <param name="plaintext"></param>
        /// <param name="APIKey"></param>
        /// <returns></returns>
        public static string SHA256_WX2(string plaintext, string APIKey)
        {
            using (RSACryptoServiceProvider sha256 = new RSACryptoServiceProvider())
            {
                byte[] dataInBytes = Encoding.UTF8.GetBytes(plaintext);
                sha256.FromXmlString(APIKey);
                byte[] inArray = sha256.SignData(dataInBytes, CryptoConfig.MapNameToOID("SHA256") ?? "");
                string sign = Convert.ToBase64String(inArray);
                return sign;
            }
        }
        #endregion

        #region 条形码解析
        /// <summary>
        /// 分解条形码，并返回数组
        /// </summary>
        /// <param name="code">条形码</param>
        /// <returns>数组0=品牌  1=季节  2=款式  3=批次  4=尺码</returns>
        public static List<string> GetSkuArray(string code)
        {
            var str = new List<string>();
            //品牌，截取0-3
            str.Add(code.Substring(0, 3));
            //季节截取
            str.Add(code.Substring(3, 1));
            //款式截取
            str.Add(code.Substring(4, 1));
            //批次截取
            str.Add(code.Substring(5, 1));
            //尺码截取
            str.Add(code.Substring(6, 1));
            return str;
        }
        #endregion

        #region 返回采购单入库状态
        /// <summary>
        /// 返回采购单入库状态
        /// </summary>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public static string PurchaseStatus(string status)
        {
            var str = "";
            switch (int.Parse(status))
            {
                case 1: str = "未完成入库"; break;
                case 2: str = "未完成付款"; break;
                case 3: str = "未完成到票"; break;
                case 4: str = "完成"; break;
            }
            return str;
        }
        #endregion

        #region 生成采购单编号
        /// <summary>
        /// 生成采购单编号
        /// </summary>
        /// <returns></returns>
        public static string PurchaseNumber(int lastNumber)
        {
            return "CG-" + DateTime.Now.ToString("yyyyMMdd") + "-" + lastNumber.ToString();
        }
        #endregion

        #region 上传配置
        /// <summary>
        ///  根据文件类型分配路径
        /// </summary>
        /// <param name="fileExt"></param>
        /// <returns></returns>
        public static string AssigendPath(string fileExt, string path)
        {
            if (IsImage(fileExt))
                return path + "/upload/images/" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "/";
            if (IsVideos(fileExt))
                return path + "/upload/videos/" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "/";
            if (IsDocument(fileExt))
                return "/upload/files/" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "/";
            if (IsMusics(fileExt))
                return "/upload/musics/" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "/";
            return path + "/upload/others/";
        }
        #endregion

        #region 文件格式
        /// <summary>
        /// 是否为图片
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        /// <returns></returns>
        private static bool IsImage(string _fileExt)
        {
            var images = new List<string> { "bmp", "gif", "jpg", "jpeg", "png" };
            if (images.Contains(_fileExt.ToLower())) return true;
            return false;
        }
        /// <summary>
        /// 是否为视频
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        /// <returns></returns>
        private static bool IsVideos(string _fileExt)
        {
            var videos = new List<string> { "rmvb", "mkv", "ts", "wma", "avi", "rm", "mp4", "flv", "mpeg", "mov", "3gp", "mpg" };
            if (videos.Contains(_fileExt.ToLower())) return true;
            return false;
        }
        /// <summary>
        /// 是否为音频
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        /// <returns></returns>
        private static bool IsMusics(string _fileExt)
        {
            var musics = new List<string> { "mp3", "wav" };
            if (musics.Contains(_fileExt.ToLower())) return true;
            return false;
        }
        /// <summary>
        /// 是否为文档
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        /// <returns></returns>
        private static bool IsDocument(string _fileExt)
        {
            var documents = new List<string> { "doc", "docx", "xls", "xlsx", "ppt", "pptx", "txt", "pdf" };
            if (documents.Contains(_fileExt.ToLower())) return true;
            return false;
        }
        #endregion

        #region 返回活动名称
        /// <summary>
        /// 返回活动名称
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string GetActivityMethod(int method)
        {
            var str = "";
            switch (method)
            {
                case 1: str = "打折"; break;
                case 2: str = "满减"; break;
                case 3: str = "买一赠一"; break;
                default: str = "无"; break;
            }
            return str;
        }
        /// <summary>
        /// 根据方式返回参加活动类型
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static byte GetActivityTypes(int method)
        {
            byte str = 1;
            switch (method)
            {
                case 1: str = 2; break;
                case 2: str = 3; break;
                case 3: str = 4; break;
            }
            return str;
        }
        #endregion

        #region 得到一周的周一和周日的日期
        /// <summary> 
        /// 计算本周的周一日期 
        /// </summary> 
        /// <returns></returns> 
        public static DateTime GetMondayDate()
        {
            return GetMondayDate(DateTime.Now);
        }
        /// <summary> 
        /// 计算本周周日的日期 
        /// </summary> 
        /// <returns></returns> 
        public static DateTime GetSundayDate()
        {
            return GetSundayDate(DateTime.Now);
        }
        /// <summary> 
        /// 计算某日起始日期（礼拜一的日期） 
        /// </summary> 
        /// <param name="someDate">该周中任意一天</param> 
        /// <returns>返回礼拜一日期，后面的具体时、分、秒和传入值相等</returns> 
        public static DateTime GetMondayDate(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Monday;
            if (i == -1) i = 6;// i值 > = 0 ，因为枚举原因，Sunday排在最前，此时Sunday-Monday=-1，必须+7=6。 
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Subtract(ts);
        }
        /// <summary> 
        /// 计算某日结束日期（礼拜日的日期） 
        /// </summary> 
        /// <param name="someDate">该周中任意一天</param> 
        /// <returns>返回礼拜日日期，后面的具体时、分、秒和传入值相等</returns> 
        public static DateTime GetSundayDate(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Sunday;
            if (i != 0) i = 7 - i;// 因为枚举原因，Sunday排在最前，相减间隔要被7减。 
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Add(ts);
        }

        /// <summary>
        /// 根据星期几获得数字的星期几
        /// </summary>
        /// <param name="weekName">例如周一：Monday</param>
        /// <returns></returns>
        public static int GetWeekByWeekName(string weekName)
        {
            var week = 1;
            switch (weekName)
            {
                case "Monday":
                    week = 1;
                    break;
                case "Tuesday":
                    week = 2;
                    break;
                case "Wednesday":
                    week = 3;
                    break;
                case "Thursday":
                    week = 4;
                    break;
                case "Friday":
                    week = 5;
                    break;
                case "Saturday":
                    week = 6;
                    break;
                case "Sunday":
                    week = 7;
                    break;
            }
            return week;
        }
        #endregion

        #region 时间戳
        /// <summary>
        /// 时间戳转换为日期（时间戳单位秒）
        /// </summary>
        /// <param name="TimeStamp"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(long timeStamp)
        {
            var dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = new TimeSpan(timeStamp);
            return dtStart.Add(toNow);
            //var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //return start.AddMilliseconds(timeStamp).AddHours(8);
        }
        /// <summary>
        /// 日期转换为时间戳（时间戳单位秒）
        /// </summary>
        /// <param name="TimeStamp"></param>
        /// <returns></returns>
        public static long ConvertToTimeStamp(DateTime time)
        {
            DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(time.AddHours(-8) - Jan1st1970).TotalMilliseconds;
        }
        #endregion

        #region 返回时间差
        public static string DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;
            try
            {
                //TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                //TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                //TimeSpan ts = ts1.Subtract(ts2).Duration();
                TimeSpan ts = DateTime2 - DateTime1;
                if (ts.Days >= 1)
                {
                    dateDiff = DateTime1.Month.ToString() + "月" + DateTime1.Day.ToString() + "日";
                }
                else
                {
                    if (ts.Hours > 1)
                    {
                        dateDiff = ts.Hours.ToString() + "小时前";
                    }
                    else
                    {
                        dateDiff = ts.Minutes.ToString() + "分钟前";
                    }
                }
            }
            catch
            { }
            return dateDiff;
        }
        #endregion

        #region 社区返回用户等级颜色标识
        /// <summary>
        /// 社区返回用户等级颜色标识
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static string GetAskGroupColor(string groupName)
        {
            var str = "";
            switch (groupName)
            {
                case "普通用户":
                    str = "04aad4"; break;
                case "专家":
                    str = "ffbf00"; break;
                case "社区管理员":
                    str = "a35429"; break;
                case "创始人":
                    str = "780978"; break;
                default:
                    str = "04aad4"; break;
            }
            return str;
        }
        #endregion

        #region 字符串-2进制，16进制转化
        /// <summary>
        /// 16进制byte转字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string byte2HexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 字符转16进制byte
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] hexString2Byte(string hex)
        {
            int len = hex.Length / 2;
            byte[] bytes = new byte[len];
            for (int i = 0; i < len; i++)
            {
                bytes[i] = (byte)(Convert.ToInt32(hex.Substring(i * 2, 2), 16));
            }
            return bytes;
        }
        /// <summary>
        /// 字符串转byte
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] string2Byte(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
        /// <summary>
        /// byte转字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string byte2String(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
        #endregion

        #region 获取随机值
        /// <summary>
        /// 获取DateTime.Now.Ticks的后几位
        /// </summary>
        /// <param name="MaxLen"></param>
        /// <returns></returns>
        public static int GetTickNum(int MaxLen)
        {
            return GetTickStr(MaxLen).ToInt32();
        }

        private static string GetTickStr(int MaxLen)
        {
            if (MaxLen > 10) MaxLen = 10;
            string a = DateTime.Now.Ticks.ToString();
            return a.Substring(a.Length - MaxLen, MaxLen);
        }

        private static string GetRandom_Num(int seed)
        {
            Random r = new Random(seed);
            return r.Next(0, 10).ToString();
        }

        private static string GetRandom_C(int seed)
        {
            Random r = new Random(seed);
            int assNum_10 = r.Next(97, 123);
            return ((char)assNum_10).ToString();
        }

        /// <summary>
        /// 获取随机产生的中文
        /// </summary>
        /// <param name="Encode">产生随机中文的编码</param>
        /// <param name="MaxLength">最大长度</param>
        /// <returns>string</returns>
        private static string GetRandom_ZH(string Encode, int seed)
        {
            //定义返回的字符串
            string Chinese = "";
            //定义中文编码
            Encoding Ecode = Encoding.GetEncoding(Encode);
            Random Rnd = null;// new Random(GetIntRand(4));
            //int Rint = Rnd.Next(1, 100);
            //定义位码、区码的范围数
            int Wint, Qint;

            int Rint = 0;
            //获取汉字区位
            Rnd = new Random(seed);
            Rint = Rnd.Next(16, 56);//只获取常用汉字 16-55之间
            Wint = Rint;
            //55区只89个汉字 其他 94个汉字
            Rint = (Wint == 55) ? 90 : 95;
            //定义新种子
            Rnd = new Random(seed + 1000);
            Rint = Rnd.Next(1, Rint);
            Qint = Rint;
            //两个字节变量存储产生的随机汉字区位码
            byte Fbyte = System.Convert.ToByte((Wint + 160).ToString("x"), 16);
            byte Lbyte = System.Convert.ToByte((Qint + 160).ToString("x"), 16);
            //将两个字节变量存储在字节数组中
            byte[] Nbytes = new byte[] { Fbyte, Lbyte };
            Chinese = Ecode.GetString(Nbytes);
            Rnd = null;
            Ecode = null;
            return Chinese;
        }
        /// <summary>
        /// 获取一组随机数
        /// </summary>
        /// <param name="rt">RandType</param>
        /// <param name="MaxLen"></param>
        /// <returns></returns>
        public static string GetRandom(int rt, int MaxLen)
        {
            Random ran = null; int seed = GetTickNum(9); string result = string.Empty; List<int> rt_s = null;
            rt_s = new List<int>();
            if ((rt & (int)RandType.Num) == (int)RandType.Num) rt_s.Add((int)RandType.Num);
            if ((rt & (int)RandType.Character) == (int)RandType.Character) rt_s.Add((int)RandType.Character);
            if ((rt & (int)RandType.ZH) == (int)RandType.ZH) rt_s.Add((int)RandType.ZH);

            int rts_len = rt_s.Count; int rt_index = 0; int rt_i = rt;
            for (int i = 0; i < MaxLen; i++)
            {
                if (rts_len > 1)
                {
                    ran = new Random(seed + i * 1000);
                    rt_index = ran.Next(0, rts_len);
                    rt_i = rt_s[rt_index];
                }

                switch (rt_i)
                {
                    case (int)RandType.Num: result += GetRandom_Num(seed + i * 2000); break;
                    case (int)RandType.Character: result += GetRandom_C(seed + i * 3000); break;
                    case (int)RandType.ZH: result += GetRandom_ZH("GB2312", seed + i * 4000); break;
                    default: result += GetRandom_Num(seed + i * 2000); break;
                }
            }
            rt_s = null;
            return result;
        }
        #endregion

        #region 通用 生成排序的key=value&字符串

        internal static string GetContentString(IDictionary<string, string> parameters)
        {
            return GetContentString(parameters, 0);
        }
        internal static string GetContentStringEncode(IDictionary<string, string> parameters)
        {
            return GetContentString(parameters, 1);
        }
        internal static string GetContentStringDecode(IDictionary<string, string> parameters)
        {
            return GetContentString(parameters, 2);
        }


        /// <summary>
        /// 更具参数拼接正续字符串 （KEY=VALUE&）的形式
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="Encode">1:urlencode,2urldecode,3nothing</param>
        /// <returns></returns>
        private static string GetContentString(IDictionary<string, string> parameters, int Encode)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder("");
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    if (Encode == 1)
                        query.Append(key).Append("=").Append(HttpUtility.UrlEncode(value)).Append("&");
                    else if (Encode == 2)
                        query.Append(key).Append("=").Append(HttpUtility.UrlDecode(value)).Append("&");
                    else
                        query.Append(key).Append("=").Append(value).Append("&");
                }
            }
            // 取出尾巴
            string content = query.ToString().Substring(0, query.Length - 1);
            return content;
        }
        #endregion

        #region table<->list
        /// <summary>
        /// DataTable=>List 方法2 最好用这个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IList<T> ConvertToList<T>(DataTable dt) where T : new()
        {
            // 定义集合    
            IList<T> ts = new List<T>();
            string tempName = "";
            T t = new T();
            // 获得此模型的公共属性      
            PropertyInfo[] propertys = t.GetType().GetProperties();

            foreach (DataRow dr in dt.Rows)
            {
                t = new T();
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;  // 检查DataTable是否包含此列    
                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter      
                        if (!pi.CanWrite) continue;

                        object value = dr[tempName];
                        if (value != DBNull.Value)
                        {
                            var pit = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                            var val = Convert.ChangeType(value, pit);
                            pi.SetValue(t, val, null);
                        }
                            
                    }
                }
                ts.Add(t);
            }
            return ts;
        }

        /// <summary>
        /// DataTable=>List 方法2 最好用这个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static T? ConvertToModel<T>(DataTable dt)
            where T : new()
        {
            // 定义集合    
            IList<T> ts = ConvertToList<T>(dt);
            if (ts == null || ts.Count == 0) return default(T);
            return ts[0];
        }

        #endregion

        #region List=>DataTable 方法1

        /// <summary>
        /// 方法一，可以用
        /// </summary>
        public static DataTable ToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(h => h.CanRead&&h.CanWrite).ToArray();

            foreach (PropertyInfo prop in props)
            {
                //if (!prop.CanWrite) continue;
                Type? t = GetCoreType(prop.PropertyType);
                if (t!=null) tb.Columns.Add(prop.Name, t);
            }

            foreach (T item in items)
            {
                var values = new object?[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>
        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Return underlying type if type is Nullable otherwise return the type
        /// </summary>
        public static Type? GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }


        #endregion

        #region List=>DataTable 方法2
        /// <summary>
        /// 方法二，可以用
        /// </summary>
        public static DataTable ToDataTable2<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(h => h.CanRead&&h.CanWrite).ToArray();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object? obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray()!;
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }

        #endregion

        #region 隐藏中间数据
        /// <summary>
        /// 获取姓名--中间隐藏
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Hid_RealName(string name) =>
             string.IsNullOrEmpty(name) ? ""
                : name.Length<=2 ? name.Substring(0, 1)+"*"
                : name.FirstOrDefault()+"*"+name.LastOrDefault();


        /// <summary>
        /// 获取姓名--中间隐藏
        /// </summary>
        /// <param name="CardID"></param>
        /// <returns></returns>
        public static string Hid_CardID(string CardID) =>
             string.IsNullOrEmpty(CardID) ? ""
                : CardID.Length==15 ? DataUtils.GetLeft(CardID, 6)+ "****"+DataUtils.GetRight(CardID, 1)
                : CardID;


        /// <summary>
        /// 获取姓名--中间隐藏
        /// </summary>
        /// <param name="Tel"></param>
        /// <returns></returns>
        public static string Hid_Tel(string Tel) => string.IsNullOrEmpty(Tel) ? ""
                : Tel.Length==11 ? DataUtils.GetLeft(Tel, 3)+ "****"+DataUtils.GetRight(Tel, 4)
                : Tel;
        #endregion
    }

    /// <summary>
    /// 生成唯一数
    /// </summary>
    public class UniqueData
    {
        private static object obj = new object();
        private static int _sn = 0;
        public static string Gener()
        {
            lock (obj)
            {
                if (_sn == int.MaxValue)
                {
                    _sn = 0;
                }
                else
                {
                    _sn++;
                }
                //Thread.Sleep(100);
                return DateTime.Now.ToString("yyyyMMdd") + _sn.ToString().PadLeft(5, '0');
            }
        }
    }
}
