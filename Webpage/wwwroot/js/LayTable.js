var LayTableHelper = function (args) {
    var that = new Object();
    that._elem = args.elem || "#table1";
    that._id = args.id || "_laytable1";
    that._url = args.url;
    that._cols = args.cols;
    that._page = args.page || true;
    that._layfilter = args.layfilter||"f_table";
    that._fun_tool = args.fun_tool;
    that._searchbtn = args.searchbtn || "#btn_search";
    that._searchform = args.searchform || "#searchForm";

    that.GetParam = function () {
        var json = {};
        if ($(that._searchform)) {

            $(that._searchform).find("input:text").each(function (i) {
                if ($(this).hasClass("layui-unselect")) return true;
                if ($(this).val().length > 0) json[$(this).attr("id")] = $(this).val();

            });

            $(that._searchform).find("input:hidden").each(function (i) {
                if ($(this).val().length > 0) json[$(this).attr("id")] = $(this).val();
            });
            $(that._searchform).find("select").each(function (i) {
                if ($(this).hasClass("layui-unselect")) return true;
                if ($(this).val().length > 0) json[$(this).attr("id")] = $(this).val();
            });
        }
        return json;
    }

    if ($(that._searchbtn)) {
        $(that._searchbtn).bind("click", function () {
            var json = that.GetParam();
            laytable.reload(that._id, {
                where: json
                , page: {
                    curr: 1 //重新从第 1 页开始
                }
            });
        });
    }

    layui.use('table', function () {
        laytable = layui.table;
        laytable.render({
            elem: that._elem,
            url: that._url,
            method: "post",
            autoSort: false,
            cols: that._cols,
            page: that._page,
            id: that._id
        });

        laytable.on("tool(" + that._layfilter + ")", function (obj) {
            if (typeof (that._fun_tool) == "function") that._fun_tool(obj);
        });

        laytable.on('sort(' + that._layfilter + ')', function (obj) { //注：sort 是工具条事件名，test 是 table 原始容器的属性 lay-filter="对应的值"
            //if (typeof (that._fun_tool) == "function") that._fun_tool(obj);
            console.log(obj.field); //当前排序的字段名
            console.log(obj.type); //当前排序类型：desc（降序）、asc（升序）、null（空对象，默认排序）
            console.log(this); //当前排序的 th 对象

            //尽管我们的 table 自带排序功能，但并没有请求服务端。
            //有些时候，你可能需要根据当前排序的字段，重新向服务端发送请求，从而实现服务端排序，如：
            var json = that.GetParam() || {};
            json.field = obj.field;
            json.order = obj.type;

            laytable.reload(that._id, {
                initSort: obj //记录初始排序，如果不设的话，将无法标记表头的排序状态。
                , where: json
            });

            /*layer.msg('服务端排序。order by ' + obj.field + ' ' + obj.type);*/
        });
    });
    return that;
}