var c_doing = !1, _c_t, readMore = function () {
    function e() {
        r ? ($(".article_content").height("").css({
            overflow: "hidden"
        }), $(".readall_box").show().addClass("readall_box_nobg"), $(".readall_box").hide().addClass("readall_box_nobg"), r = !1) : ($(".article_content").height(2 * t).css({
            overflow: "hidden"
        }), $(".readall_box").show().removeClass("readall_box_nobg"), r = !0)
    }
    var t = $(window).height(),
        n = $(".article_content").height();
    !1;
    if (n > 2 * t) {
        $(".article_content").height(2 * t - 680).css({
            overflow: "hidden"
        });
        var r = !0;
        $(".read_more_btn").on("click", e)
    } else r = !0,
        $(".article_content").removeClass("article_Hide"),
        $(".readall_box").hide().addClass("readall_box_nobg")
};