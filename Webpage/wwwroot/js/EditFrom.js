var EditForm = function (args) {
    var that = new Object();
    that._container = args.container||"#form1";
    that._btnsave = args.btn1 || "#btn_sub";

    that._url = args.url;
    that._check = args.check;
    that._append = args.append;
    that._success = args.success;
    that._autoget = args.auto || 1;
    that._autocheck = args.autocheck || true;

    $(that._btnsave).on("click", function () {
        var container = that._container;
        if (!that._CheckForm()) return false;

        var allchecked = true;

        $(container).find("[autocheck]").each(function (i) {
            if ($(this).val() != null && $(this).val().length < 1) {
                var title = $(this).parent().parent().find(".layui-form-label").eq(0).html();
                var id = $(this).attr("id");
                XAlert_F(title + " 不能为空", null, id);
                allchecked = false;
                return false;
            }
        });

        if (!allchecked) return false;


        var json = {};
        if (that._autoget == 1) {
            $(container).find(":text").each(function (i) {
                if ($(this).hasClass("layui-unselect")) return;
                if ($(this).val().length > 0)
                    eval("json." + $(this).attr("id") + "=\"" + $(this).val() + "\"");
            });

            $(container).find("input:hidden").each(function (i) {
                if ($(this).val().length > 0)
                    eval("json." + $(this).attr("id") + "=\"" + $(this).val() + "\"");
            });

            $(container).find("textarea").each(function (i) {
                //eval("json." + $(this).attr("id") + "=\"" + $(this).val() + "\"");
                if ($(this).val().length > 0)
                    json[$(this).attr("id")] = $(this).val();
            });

            $(container).find("select").each(function (i) {
                if ($(this).hasClass("layui-unselect")) return;
                if ($(this).val().length > 0)
                    eval("json." + $(this).attr("id") + "=\"" + $(this).val() + "\"");
            });
        }
        json = that._AppendJson(json);

        $.post(that._url
            , json
            , function (rData) {
                //eval("rData = " + Data);
                if (rData.id > 0) {
                    XAlert_S(rData.msg, function () { if (typeof (that._success) == "function") that._success(rData.data); else GetLocation(rData.data); });
                }
                else {
                    XAlert_F(rData.msg);
                }
            });
    });

    that._CheckForm = function () {
        if (typeof (that._check) == "function") {
            if (!that._check()) return false;
        }
        return true;
    }

    that._AppendJson = function (json) {
        if (typeof (that._append) == "function") {
            json = that._append(json);
        }
        return json;
    }


    return that;
};