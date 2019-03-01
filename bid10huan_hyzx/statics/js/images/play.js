// JavaScript Document
$(function () {
    $(".case1").eq(0).show();
    $(".caseTitle span").eq(0).addClass("cur");
    $(".caseTitle span").eq(0).find("i").addClass("cur").siblings().removeClass("cur")
    $(".caseTitle span").mouseover(function () {
        $(this).addClass("cur").siblings().removeClass("cur");
        $(this).find("i").addClass("cur").siblings().removeClass("cur");
        i = $(this).index();
        $(".case1").eq(i).show().siblings(".case1").hide();
    }).mouseout(function () {
        $(this).find("i").removeClass("cur");

    })
    //index end
    //$(".service1").eq(0).show().siblings(".service1").hide();
    //$(".serTitle span").eq(0).addClass("cur").siblings("span").removeClass("cur"); 
    /*$(".serTitle span").click(function(){
        $(this).addClass("cur").siblings().removeClass("cur");
        n=$(this).index();
        $(".service1").eq(n).show().siblings(".service1").hide();
    })
    //service and solution end
    $("#news .service1").eq(0).show().siblings(".service1").hide();
    $("#news .serTitle span").eq(0).addClass("cur").siblings("span").removeClass("cur");
    $("#news .serTitle span").click(function(){
        $(this).addClass("cur").siblings().removeClass("cur");
        n=$(this).index();
        $("#news .service1").eq(n).show().siblings(".service1").hide();
    })*/

    //news end		   
    $(".caseleft").mouseover(function () {
        $(this).find(".picbox").addClass("cur").siblings(".picbox").removeClass("cur");
        $(this).find(".picFont").show();

    }).mouseout(function () {
        $(this).find(".picbox").removeClass("cur");
        $(this).find(".picFont").hide();
    })
    //case end	 


})