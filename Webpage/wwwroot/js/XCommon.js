
function ImgShow(ob) {
    window.open($(ob).attr("src"));
}
/*页面控制部分-开始*/
$(function () {
    //if (typeof (layer) == 'undefined') {
    //    layui.use('layer', function () {
    //        layer = layui.layer;
    //    });
    //}


    var searchStatus = false;
    //查询
    if ($('#searchEntry') && $("#btn_search")) {
        $('#searchEntry').on('click', function () {
            if (searchStatus == false) {
                $('#searchForm').show();
                searchStatus = true;
            } else {
                $('#searchForm').hide();
                searchStatus = false;
            }

        });
        $("#btn_search").on('click', function () {
            _load(1);
        });
    }

    //添加滚动条
    if ($(".left-nav")) $(".left-nav").addClass("scroll");
    //滚动
    if ($(".scroll") && typeof ($(".scroll").mCustomScrollbar) == "function") $(".scroll").mCustomScrollbar();
    //多tab
    if ($(".detail-tabs  li")) {
        $(".detail-tabs  li").on("click", function () {

            var curIndex = 0;
            var tbs = $(".detail-tabs  li");
            for (var i = 0; i < tbs.length; i++) {
                if (tbs[i] == this) { curIndex = i; break; }
            }

            $(this).siblings().removeClass("cur");
            $(this).addClass("cur");

            $(".detail-item").eq(curIndex).siblings().css("display", "none");
            $(".detail-item").eq(curIndex).css("display", "block");
        });
    };

    //排序,绑定点击事件
    if ($(".layui-table-sort")) {
        $(".layui-table-sort > .layui-edge").on("click", function () {
            setsort(this);
        })

        //排序,设置排序
        if ($("#hid_orderby") && $("#hid_orderby").length > 0) {
            setorderby();
        }
    }




})

function setorderby() {
    var ab = $("#hid_orderby").val();
    var vars = ab.split(",");
    if (vars.length != 2) return;
    var a = vars[0]; var b = vars[1];
    $(".layui-table-sort").each(function (i) {
        if ($(this).attr("lay-sortby") == a) {
            $(this).attr("lay-sort", (b == 0 ? "asc" : "desc"));
            return;
        }
    });

}


function refreshpage() {
    layer.load(1, { shade: false });
    window.location.reload();
}

function _reset() {
    layer.load(1, { shade: false });
    //if ($("#searchForm")) {
    //    $("#searchForm").find("input:text").each(function (i) {
    //        if ($(this).hasClass("layui-unselect")) return;
    //        if ($(this).val().length > 0) $(this).val("");

    //    });

    //    $("#searchForm").find("input:hidden").each(function (i) {
    //        if ($(this).val().length > 0) $(this).val("");
    //    });
    //    $("#searchForm").find("select").each(function (i) {
    //        if ($(this).hasClass("layui-unselect")) return;
    //        if ($(this).val().length > 0)  $(this).val($(this).find("option").first().val());
    //    });
    //}
    //_load(1);
    var h = location.origin + location.pathname;
    location.href = h;
}

function setsort(ob) {
    var orderdes = $(ob).parent().attr("lay-sort");
    $(".layui-table-sort").attr("lay-sort", "");
    var cur_orderdes = "";
    if ($(ob).hasClass("layui-table-sort-asc")) {
        $(ob).parent().attr("lay-sort", "asc");
        if (orderdes == "asc") return;

    }
    if ($(ob).hasClass("layui-table-sort-desc")) {
        $(ob).parent().attr("lay-sort", "desc");
        if (orderdes == "desc") return;
    }
    _load(1);
}


function _load(page) {
    var json = getparam();
    json["page"] = page;

    var q = setQuery(json);
    var h = location.origin + location.pathname + "?" + q;
    location.href = h;
}

function _export(type) {
    var json = getparam();
    json["export"] = type;

    var q = setQuery(json);
    var h = location.origin + location.pathname + "?" + q;
    location.href = h;
}

function getparam() {
    var json = {};
    if ($("#searchForm")) {
        $("#searchForm").find("input:text").each(function (i) {
            if ($(this).hasClass("layui-unselect")) return;
            if ($(this).val().length > 0) json[$(this).attr("id")] = $(this).val();

        });

        $("#searchForm").find("input:hidden").each(function (i) {
            if ($(this).val().length > 0) json[$(this).attr("id")] = $(this).val();
        });
        $("#searchForm").find("select").each(function (i) {
            if ($(this).hasClass("layui-unselect")) return;
            if ($(this).val().length > 0) json[$(this).attr("id")] = $(this).val();
        });
    }

    if ($(".layui-table-sort")) {
        $(".layui-table-sort").each(function (i) {
            if ($(this).attr("lay-sort").length > 0) {
                json["orderby"] = $(this).attr("lay-sortby") + "," + ($(this).attr("lay-sort") == "asc" ? 0 : 1);
                return;
            }
        })
    }

    return json;
}

/*页面控制部分-结束*/

$(function () {
    $(".curtab").on("click", function () {
        $(".curtab").removeClass("cur");
        $(this).addClass("cur");
    });
})


function _content(msg) {
    return "<div class=\"side-item\" style=\"text-align: center;line-height: initial;\">" + msg + "</div>";
}

function _content2(Msg) {
    var content = '';
    content += '<div style="text-align:center;width:100%;min-width: 300px;padding:20px 0px 10px 0px;font-size:16px;color:#323232;">' + Msg + '</div>';
    return content;
}
function _content3(Msg) {
    var content = '';
    content += '<div style="width:320px;height:30px;display:block;">';
    content += '<span style="width:320px; line-height: 26px; text-align: left;float:left;font-family: microsoft yahei;font-size: 14px; ">';
    content += '' + Msg + '</span></div>';
    return content;
}

///1:正确，2：错误，3：提示
function XAlert_S(Msg, Fn, Focus) {
    var layer1 = layer.msg(Msg, { icon: 1 }, function () {
        mc_execfn(Fn, null);
        mc_setfocus(Focus);
    });
}
///1:正确，2：错误，3：提示
function XAlert_F(Msg, Fn, Focus) {
    var layer1 = layer.msg(Msg, { icon: 2 }, function () {
        mc_execfn(Fn, null);
        mc_setfocus(Focus);
    });
}
///1:正确，2：错误，3：提示
function XAlert_T(Msg, Fn, Focus) {
    var layer1 = layer.msg(Msg, { icon: 3 }, function () {
        mc_execfn(Fn, null);
        mc_setfocus(Focus);
    });
}

function XConfirm(json) {
    var title = GetValue(json.title, "确认操作");
    var content = GetValue(json.content, "");
    var btn_yes = GetValue(json.btnyes, "确认");


    layer.open({
        title: title,
        type: 1,
        content: _content2(content),
        btn: [btn_yes, "取消"],
        yes: function (index, layero) {
            if (mc_isfunction(json.yes)) json.yes();
            else top.layer.close(index);
        },
        btn2: function (index, layero) {
            if (mc_isfunction(json.no)) json.no();
            else top.layer.close(index);
        }
    });
}

/*设置焦点*/
function mc_setfocus(oid) {
    if (!oid) return;
    if ($.type(oid) == 'string') {
        if (!oid.startWith('#')) oid = '#' + oid;
        oid = $(oid);
    }
    if (oid.length <= 0) return;
    oid.focus();
}


/*执行外部函数*/
function mc_execfn(fn, args) {
    if (!mc_isfunction(fn)) return null;
    return fn(args);
}

function AjaxPost(url, data, fn) {
    var Data = $.ajax({ url: url, async: false, type: 'post', data: data }).responseText;
    if (fn) { if (typeof (fn) == "function") fn(Data); }
}
function AjaxPost2(url, data, fn) {
    var Data = $.ajax({ url: url, async: false, type: 'post', data: data }).responseText;
    try {
        eval("rdata=" + Data);
    } catch (e) {
        XAlert_T("您的浏览器不支持该功能");
    }
    if (fn) { if (typeof (fn) == "function") fn(rdata); }
}
function AjaxPost_Async(url, data, fn) {
    $.ajax({
        url: url, async: true, type: 'post', data: data, success: function (Data) {
            try {
                eval("rdata=" + Data);
            } catch (e) {
                XAlert_T("您的浏览器不支持该功能");
            }
            if (fn) { if (typeof (fn) == "function") fn(rdata); }
        }
    })

}

function AjaxGet(url) {
    var Data = $.ajax({ url: url, async: false, type: 'get' }).responseText;
    return Data;
}


/*是否为 函数*/
function mc_isfunction(fn) {
    if (!fn) return false;
    if (!$.isFunction(fn)) return false;
    return true;
}

//只允许整数输入
function OnlyInt(o) {
    var partten = /^[1-9]$/;
    var Len = $(o).val().length;
    var ResultStr = "";
    var A_Result = "";

    if (Len == 1 && $(o).val() == "0") { return; }

    for (var i = 0; i < Len; i++) {
        A_Result = $(o).val().substring(i, i + 1);
        if (i > 0 && A_Result == "0") ResultStr += A_Result;
        else {
            if (partten.test(A_Result)) ResultStr += A_Result;
        }
    }
    $(o).val(ResultStr);
}
//只允许正实数输入
function OnlyNumber(o) {
    var partten = /^[0-9]$/;
    var Len = $(o).val().length;
    var ResultStr = "";
    var A_Result = "";
    var PointLen = 0;
    var ZeroLen = 0;
    for (var i = 0; i < Len; i++) {
        A_Result = $(o).val().substring(i, i + 1);
        if (i == 0) {
            if (A_Result == "0") {
                ZeroLen = 1;
                ResultStr += A_Result;
            }
            else if (A_Result == ".") {
                PointLen = 1;
                ResultStr += "0.";
            }

            else if (partten.test(A_Result)) ResultStr += A_Result;
        }
        else {
            if (PointLen == 0 && A_Result == ".") { ResultStr += A_Result; PointLen = 1; }
            else if (partten.test(A_Result)) ResultStr += A_Result;
        }

        //        if (i > 0 && A_Result == "0") ResultStr += A_Result;
        //        else if (PointLen == 0 && A_Result == ".") { ResultStr += A_Result; PointLen = 1; }
        //        else if (partten.test(A_Result)) ResultStr += A_Result;
    }
    $(o).val(ResultStr);
}

//获取字符长度，汉字算两个字符，字母算一个
String.prototype.LenBytes = function (trimAll) {
    var str = trimAll ? this.replace(/\s+/g, '') : this.replace(/^(\s|\u00A0)+/, '').replace(/(\s|\u00A0)+$/, '');
    return str.replace(/[^\x00-\xff]/g, 'xx').length;
};


$(function () {
    $(".tab_sty3").find("td:even").css({ "text-align": "right", "background": "#fafafa" });
    $(".tab_sty4").css("width", "700px");
    $(".tab_sty4").find("td:even").css({ "text-align": "right", "width": "180px", "background": "#fafafa" });
    $(".tab_sty4").find("td:odd").css({ "width": "220px" });
    $(".tab_sty6").find("td:even").css({ "text-align": "right", "width": "140px", "background": "#fafafa" });
});


function GetMoney(value) {
    if (value.length < 0) return 0;
    else {
        var Fv = parseFloat(value);
        if (isNaN(Fv)) return 0;
        return Math.round(Fv * 100) / 100
    }
}
//验证数字(整数、浮点数都可以通过)
function IsFloat(oNum) {
    if (!oNum) return false;
    var strP = /^\d+(\.\d+)?$/;
    if (!strP.test(oNum)) return false;
    try {
        if (parseFloat(oNum) != oNum) return false;

    } catch (ex) {
        return false;
    }
    return true;
}

//json格式检查
var Obj2Str = function (obj) {
    var r = [];
    if (typeof obj == "string") return "\"" + obj.replace(/([\'\"\\])/g, "\\$1").replace(/(\n)/g, "\\n").replace(/(\r)/g, "\\r").replace(/(\t)/g, "\\t") + "\"";
    if (typeof obj == "undefined") return "undefined";
    if (typeof obj == "object") {
        if (obj === null) return "null";
        else if (!obj.sort) {
            for (var pro in obj)
                r.push(pro + ":" + Obj2Str(obj[pro]));
            /*obj.hasOwnProperty(pro) && r.push(pro + ":" + obj[pro].Obj2Str());*/
            r = "{" + r.join() + "}";
        } else {
            for (var len = 0; len < obj.length; len++)
                r.push(Obj2Str(obj[len]));
            r = "[" + r.join() + "]";
        }
        return r;
    }
    return obj.toString();
}

GetLocation = function (fid) {
    var lo = this.location.href;
    var f = "?";
    if (lo.indexOf("?") > -1) { f = "&"; }

    if (lo.toLowerCase().indexOf("?id") == -1 && lo.toLowerCase().indexOf("&id") == -1) {
        location.href = lo + f + "id=" + fid;
    }
    else
        location.reload(true);
}

function GetValue(ob, dv) {
    if (typeof (ob) == "undefined") return dv;
    return ob;
}

function LyHtml(url, title, width, height) {
    var _title = title || "";
    width = width || '80%';
    height = height || '90%';
    top.layer.open({
        type: 2,
        title: _title,
        area: [width, height],
        shadeClose: true,
        content: url,
        end: function () {
            location.reload(true);
        }
    });
}
//获取url的参数
function getQueryJson() {
    var json = {};
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        json[pair[0]] = pair[1];
        //if (pair[0] == variable) { return pair[1]; }
    }
    return json;
    //return (false);
}
function getQueryVariable(variable) {
    var json = getQueryJson();
    return json[variable];
}
function setQuery(json) {
    var q = "";
    for (var pro in json) {
        if (q.length > 0) q += "&";
        q += pro + "=" + json[pro];
    }
    return q;
}

function newtab(url, title) {
    if (top.layui.index) {
        top.layui.index.openTabsPage(url, title);
    }
    else window.open(url);
}