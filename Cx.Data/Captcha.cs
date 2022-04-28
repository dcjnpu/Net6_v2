using SkiaSharp;

namespace Cx.Data
{
    public class ImageCaptcha: IImageCaptcha
    {
        /// <summary>
        /// 干扰线的颜色集合
        /// </summary>
        private List<SKColor> colors { get; set; }
        public ImageCaptcha()
        {
            colors = new List<SKColor>();
            colors.Add(SKColors.AliceBlue);
            colors.Add(SKColors.PaleGreen);
            colors.Add(SKColors.PaleGoldenrod);
            colors.Add(SKColors.Orchid);
            colors.Add(SKColors.OrangeRed);
            colors.Add(SKColors.Orange);
            colors.Add(SKColors.OliveDrab);
            colors.Add(SKColors.Olive);
            colors.Add(SKColors.OldLace);
            colors.Add(SKColors.Navy);
            colors.Add(SKColors.NavajoWhite);
            colors.Add(SKColors.Moccasin);
            colors.Add(SKColors.MistyRose);
            colors.Add(SKColors.MintCream);
            colors.Add(SKColors.MidnightBlue);
            colors.Add(SKColors.MediumVioletRed);
            colors.Add(SKColors.MediumTurquoise);
            colors.Add(SKColors.MediumSpringGreen);
            colors.Add(SKColors.LightSlateGray);
            colors.Add(SKColors.LightSteelBlue);
            colors.Add(SKColors.LightYellow);
            colors.Add(SKColors.Lime);
            colors.Add(SKColors.LimeGreen);
            colors.Add(SKColors.Linen);
            colors.Add(SKColors.PaleTurquoise);
            colors.Add(SKColors.Magenta);
            colors.Add(SKColors.MediumAquamarine);
            colors.Add(SKColors.MediumBlue);
            colors.Add(SKColors.MediumOrchid);
            colors.Add(SKColors.MediumPurple);
            colors.Add(SKColors.MediumSeaGreen);
            colors.Add(SKColors.MediumSlateBlue);
            colors.Add(SKColors.Maroon);
            colors.Add(SKColors.PaleVioletRed);
            colors.Add(SKColors.PapayaWhip);
            colors.Add(SKColors.PeachPuff);
            colors.Add(SKColors.Snow);
            colors.Add(SKColors.SpringGreen);
            colors.Add(SKColors.SteelBlue);
            colors.Add(SKColors.Tan);
            colors.Add(SKColors.Teal);
            colors.Add(SKColors.Thistle);
            colors.Add(SKColors.SlateGray);
            colors.Add(SKColors.Tomato);
            colors.Add(SKColors.Violet);
            colors.Add(SKColors.Wheat);
            colors.Add(SKColors.White);
            colors.Add(SKColors.WhiteSmoke);
            colors.Add(SKColors.Yellow);
            colors.Add(SKColors.YellowGreen);
            colors.Add(SKColors.Turquoise);
            colors.Add(SKColors.LightSkyBlue);
            colors.Add(SKColors.SlateBlue);
            colors.Add(SKColors.Silver);
            colors.Add(SKColors.Peru);
            colors.Add(SKColors.Pink);
            colors.Add(SKColors.Plum);
            colors.Add(SKColors.PowderBlue);
            colors.Add(SKColors.Purple);
            colors.Add(SKColors.Red);
            colors.Add(SKColors.SkyBlue);
            colors.Add(SKColors.RosyBrown);
            colors.Add(SKColors.SaddleBrown);
            colors.Add(SKColors.Salmon);
            colors.Add(SKColors.SandyBrown);
            colors.Add(SKColors.SeaGreen);
            colors.Add(SKColors.SeaShell);
            colors.Add(SKColors.Sienna);
            colors.Add(SKColors.RoyalBlue);
            colors.Add(SKColors.LightSeaGreen);
            colors.Add(SKColors.LightSalmon);
            colors.Add(SKColors.LightPink);
            colors.Add(SKColors.Crimson);
            colors.Add(SKColors.Cyan);
            colors.Add(SKColors.DarkBlue);
            colors.Add(SKColors.DarkCyan);
            colors.Add(SKColors.DarkGoldenrod);
            colors.Add(SKColors.DarkGray);
            colors.Add(SKColors.Cornsilk);
            colors.Add(SKColors.DarkGreen);
            colors.Add(SKColors.DarkMagenta);
            colors.Add(SKColors.DarkOliveGreen);
            colors.Add(SKColors.DarkOrange);
            colors.Add(SKColors.DarkOrchid);
            colors.Add(SKColors.DarkRed);
            colors.Add(SKColors.DarkSalmon);
            colors.Add(SKColors.DarkKhaki);
            colors.Add(SKColors.DarkSeaGreen);
            colors.Add(SKColors.CornflowerBlue);
            colors.Add(SKColors.Chocolate);
            colors.Add(SKColors.AntiqueWhite);
            colors.Add(SKColors.Aqua);
            colors.Add(SKColors.Aquamarine);
            colors.Add(SKColors.Azure);
            colors.Add(SKColors.Beige);
            colors.Add(SKColors.Bisque);
            colors.Add(SKColors.Coral);
            colors.Add(SKColors.Black);
            colors.Add(SKColors.Blue);
            colors.Add(SKColors.BlueViolet);
            colors.Add(SKColors.Brown);
            colors.Add(SKColors.BurlyWood);
            colors.Add(SKColors.CadetBlue);
            colors.Add(SKColors.Chartreuse);
            colors.Add(SKColors.BlanchedAlmond);
            colors.Add(SKColors.Transparent);
            colors.Add(SKColors.DarkSlateBlue);
            colors.Add(SKColors.DarkTurquoise);
            colors.Add(SKColors.IndianRed);
            colors.Add(SKColors.Indigo);
            colors.Add(SKColors.Ivory);
            colors.Add(SKColors.Khaki);
            colors.Add(SKColors.Lavender);
            colors.Add(SKColors.LavenderBlush);
            colors.Add(SKColors.HotPink);
            colors.Add(SKColors.LawnGreen);
            colors.Add(SKColors.LightBlue);
            colors.Add(SKColors.LightCoral);
            colors.Add(SKColors.LightCyan);
            colors.Add(SKColors.LightGoldenrodYellow);
            colors.Add(SKColors.LightGray);
            colors.Add(SKColors.LightGreen);
            colors.Add(SKColors.LemonChiffon);
            colors.Add(SKColors.DarkSlateGray);
            colors.Add(SKColors.Honeydew);
            colors.Add(SKColors.Green);
            colors.Add(SKColors.DarkViolet);
            colors.Add(SKColors.DeepPink);
            colors.Add(SKColors.DeepSkyBlue);
            colors.Add(SKColors.DimGray);
            colors.Add(SKColors.DodgerBlue);
            colors.Add(SKColors.Firebrick);
            colors.Add(SKColors.GreenYellow);
            colors.Add(SKColors.FloralWhite);
            colors.Add(SKColors.Fuchsia);
            colors.Add(SKColors.Gainsboro);
            colors.Add(SKColors.GhostWhite);
            colors.Add(SKColors.Gold);
            colors.Add(SKColors.Goldenrod);
            colors.Add(SKColors.Gray);
            colors.Add(SKColors.ForestGreen);
        }
        /// <summary>
        /// 创建画笔
        /// </summary>
        /// <param name="color"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        private SKPaint CreatePaint(SKColor color, float fontSize)
        {
            SkiaSharp.SKTypeface font = SKTypeface.FromFamilyName(null, SKFontStyleWeight.SemiBold, SKFontStyleWidth.ExtraCondensed, SKFontStyleSlant.Upright);
            SKPaint paint = new SKPaint();
            paint.IsAntialias = true;
            paint.Color = color;
            paint.Typeface = font;
            paint.TextSize = fontSize;
            return paint;
        }
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="captchaText">验证码文字</param>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片高度</param>
        /// <param name="lineNum">干扰线数量</param>
        /// <param name="lineStrookeWidth">干扰线宽度</param>
        /// <returns></returns>
        public byte[] GetCaptcha(string captchaText, int width, int height, int lineNum = 1, int lineStrookeWidth = 1)
        {
            //创建bitmap位图
            using (SKBitmap image2d = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul))
            {
                //创建画笔
                using (SKCanvas canvas = new SKCanvas(image2d))
                {
                    //填充背景颜色为白色
                    canvas.DrawColor(SKColors.White);
                    //将文字写到画布上
                    using (SKPaint drawStyle = CreatePaint(SKColors.Black, height))
                    {
                        canvas.DrawText(captchaText, 1, height - 1, drawStyle);
                    }
                    //画随机干扰线
                    using (SKPaint drawStyle = new SKPaint())
                    {
                        Random random = new Random();
                        for (int i = 0; i < lineNum; i++)
                        {
                            drawStyle.Color = colors[random.Next(colors.Count)];
                            drawStyle.StrokeWidth = lineStrookeWidth;
                            canvas.DrawLine(random.Next(0, width), random.Next(0, height), random.Next(0, width), random.Next(0, height), drawStyle);
                        }
                    }
                    //返回图片byte
                    using (SKImage img = SKImage.FromBitmap(image2d))
                    {
                        using (SKData p = img.Encode(SKEncodedImageFormat.Png, 100))
                        {
                            return p.ToArray();
                        }
                    }
                }
            }
        }

    }

    public interface IImageCaptcha
    {
        byte[] GetCaptcha(string captchaText, int width, int height, int lineNum = 1, int lineStrookeWidth = 1);
    }
}
