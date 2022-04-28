using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cx.Data
{
    public enum EYesNo
    {
        [Text("是的")]
        [Description("是")]
        Yes = 1,
        [Text("不是的")]
        [Description("否")]
        No = 2,
    }
}
