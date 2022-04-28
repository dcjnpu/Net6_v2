//using System.Drawing;
using System.DrawingCore;

namespace Cx.QRCode;

public interface I_QR_DATA
{
    /// <summary>
    /// 二维码下面说明文字
    /// </summary>
    string FootTitle { get; set; }

    /// <summary>
    /// 二维码内容
    /// </summary>
    string Content { get; set; }
}