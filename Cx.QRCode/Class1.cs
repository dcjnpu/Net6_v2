//using System.Drawing;
using System.DrawingCore;
using ZXing;
using ZXing.Common;

namespace Cx.QRCode;

public class QRCodeHelper
{
    /// <summary>
    /// 生成二维码图片
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static Bitmap CreatQRCodeImg(I_QR_DATA model)
    {
        string imgcode = string.Empty;
        ///二维码图片
        Bitmap bew = Pv_GetQr(false, 360, 360, model.Content)!;
        // if (string.IsNullOrEmpty(model.FootTitle)) imgcode = ToBase64(bew);
        if (string.IsNullOrEmpty(model.FootTitle)) return bew;
        else
        {
            //空白图片
            Bitmap upperPic = new Bitmap(bew.Width, bew.Height + 40);
            var img2 = BitmapToImg(bew);
            var img = BitmapToImg(upperPic);
            var a = OverlayText(img, img2, model.FootTitle);
            bew.Dispose();
            upperPic.Dispose(); img.Dispose(); img2.Dispose();
            return a;
            //imgcode = ToBase64(a);
        }

        //var back = FileUpload.UploadByImgCode(model.filefolder, imgcode, false);
        //return back;
    }

    private static Bitmap? Pv_GetQr(bool istm, int width, int height, string txt)
    {
        EncodingOptions options = new EncodingOptions { Margin = 0, Width = width, Height = height, PureBarcode = true };
        BarcodeWriter<Bitmap> writer = new BarcodeWriter<Bitmap>();
        writer.Format = istm ? BarcodeFormat.CODE_128 : BarcodeFormat.QR_CODE;
        writer.Options = options;
        //
        try { return writer.Write(txt); }
        catch { return null; }
    }

    protected static Bitmap OverlayText(Image image, Image imageBody, string name)
    {
        using (Graphics g = Graphics.FromImage(image))
        {
            g.DrawImage(imageBody, 0, 0, imageBody.Width, imageBody.Height);
            using (Font f = new Font("宋体", 20))
            {
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center; //居中
                using (Brush b = new SolidBrush(Color.Black))
                {
                    g.DrawString(name, f, b, 360 / 2, 370, format);
                }
            }
        }
        return ImgToBitmap(image);
    }

    private static Image BitmapToImg(Bitmap Bitmap)
    {
        try
        {
            Bitmap map = new Bitmap(Bitmap);
            Image img = map;
            return img;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    private static Bitmap ImgToBitmap(Image image)
    {
        try
        {
            Bitmap img = new Bitmap(image);
            return img;
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// 转为base64
    /// </summary>
    /// <param name="bmp"></param>
    /// <returns></returns>
    public static string ToBase64(Bitmap bmp)
    {
        try
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.DrawingCore.Imaging.ImageFormat.Png);
            byte[] arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();
            String strbaser64 = Convert.ToBase64String(arr);
            return strbaser64;
        }
        catch (Exception ex)
        {
            return "";
        }
    }
}