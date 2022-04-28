using System;
using System.Collections.Generic;
using System.DrawingCore.Drawing2D;
using System.DrawingCore.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DrawingCore;

namespace Cx.Data
{
    /// <summary>
    /// 图片压缩
    /// </summary>
    public class ImageCompress
    {
        #region GetPicThumbnail

        /// <summary>
        /// 无损压缩图片
        /// </summary>
        public static Task Compress_v1_Asyn(string sFile)
        {
            //扩展名
            var fileext = Path.GetExtension(sFile).ToLower();
            string dFile = sFile.Substring(0, sFile.LastIndexOf('.')) + "_spr" + fileext;

            return Compress_v1_Asyn(sFile, dFile, 1000, 1000, 50);
        }

        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="dFile">压缩后保存位置</param>
        /// <param name="dHeight">高度</param>
        /// <param name="dWidth">宽度</param>
        /// <param name="flag">压缩质量 1-100</param>
        /// <returns></returns>
        public static Task Compress_v1_Asyn(string sFile, string dFile, int dHeight, int dWidth, int flag)
        {
            return Task.Run(() =>
            {
                _Compress_v1(sFile, dFile, dHeight, dWidth, flag);
            });
        }

        private static void _Compress_v1(string sFile, string dFile, int dHeight, int dWidth, int flag)
        {
            sFile = FileHelperCore.MapPath(sFile);
            dFile = FileHelperCore.MapPath(dFile);
            Image iSource = Image.FromFile(sFile);
            ImageFormat tFormat = iSource.RawFormat;
            int sW = 0, sH = 0;
            //按比例缩放
            Size tem_size = new Size(iSource.Width, iSource.Height);
            if (tem_size.Width > dHeight || tem_size.Width > dWidth) //将**改成c#中的或者操作符号
            {
                if ((tem_size.Width * dHeight) > (tem_size.Height * dWidth))
                {
                    sW = dWidth;
                    sH = (dWidth * tem_size.Height) / tem_size.Width;
                }
                else
                {
                    sH = dHeight;
                    sW = (tem_size.Width * dHeight) / tem_size.Height;
                }
            }
            else
            {
                sW = tem_size.Width;
                sH = tem_size.Height;
            }

            Bitmap ob = new Bitmap(sW, sH); //Bitmap ob = new Bitmap(dWidth, dHeight);

            Graphics g = Graphics.FromImage(ob);
            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //g.DrawImage(iSource, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
            g.DrawImage(iSource, new Rectangle(0, 0, sW, sH), new Rectangle(0, 0, iSource.Width, iSource.Height), GraphicsUnit.Pixel);// g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), new Rectangle(0, 0, iSource.Width, iSource.Height), GraphicsUnit.Pixel);
            g.Dispose();

            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(System.DrawingCore.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();

                ImageCodecInfo jpegICIinfo = null;

                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                }
                else
                {
                    ob.Save(dFile, tFormat);
                }
                return;
            }
            catch
            {
                return;
            }
            finally
            {
                iSource.Dispose();
                //iSource.Dispose();
                ob.Dispose();
            }
        }

        #endregion GetPicThumbnail

        /// <summary>
        /// 压缩图片,第二版方法
        /// </summary>
        public static Task Compress_V2_Asyn(string filename)
        {
            return Task.Run(() => { _Compress_V2(filename); });
        }

        /// <summary>
        /// 压缩图片,第二版方法
        /// </summary>
        private static void _Compress_V2(string filename)
        {
            //string filename = @"E:\CoreWork\backone.jpg";
            //string targetname = filename;
            filename = FileHelperCore.MapPath(filename);
            Bitmap bitmap = new Bitmap(filename);

            if (bitmap.Width <= 1000 && bitmap.Height <= 1000) return;
            //保持图片的比例不变，缩放图片
            int width = 1000, height = 1000;

            if (bitmap.Width > bitmap.Height)
            {
                //宽度为大,计算高度
                height = Convert.ToInt32(width * (bitmap.Height * 1.0 / bitmap.Width));
            }
            else
            {
                //高度为大，计算宽度
                width = Convert.ToInt32(height * (bitmap.Width * 1.0 / bitmap.Height));
            }
            Bitmap result = ResizeImage(bitmap, width, height);
            bitmap.Dispose();
            //删除源文件
            //Cx.Data.FileHelperCore.DeleteFiles(filename);

            if (result == null) return;
            //var fileext = Path.GetExtension(sFile).ToLower();
            //filename = filename.Substring(0, filename.LastIndexOf('.'))+".jpg";
            //string targetname = filename.Substring(0, filename.LastIndexOf('.'))+"_spr.jpg";

            //保存图片,指定保存 格式为Jpeg，占用空间会比较小
            result.Save(filename, ImageFormat.Jpeg);
            result.Dispose();
        }

        /// <summary>
        ///  Resize图片
        /// </summary>
        /// <param name="bmp">原始Bitmap </param>
        /// <param name="newW">新的宽度</param>
        /// <param name="newH">新的高度</param>
        /// <returns>处理以后的图片</returns>
        private static Bitmap ResizeImage(Bitmap bmp, int newW, int newH)
        {
            try
            {
                Bitmap b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量
                //g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}