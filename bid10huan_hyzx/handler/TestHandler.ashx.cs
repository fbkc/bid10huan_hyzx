using AutoSend;
using HRMSys.DAL;
using Model;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace bid10huan_hyzx
{
    /// <summary>
    /// TestHandler 的摘要说明
    /// </summary>
    public class TestHandler : IHttpHandler
    {
        private string hostName = "赢商云";
        private string hostUrl = "http://bid.10huan.com/hyzx";
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            StringBuilder _strContent = new StringBuilder();
            if (_strContent.Length == 0)
            {
                string _strAction = context.Request.Params["action"];
                if (string.IsNullOrEmpty(_strAction))
                {
                    _strContent.Append(_strContent.Append(SqlHelperCatalog.WriteTemplate("", "404.html")));
                }
                else
                {
                    switch (_strAction.Trim())
                    {
                        case "MainPage": _strContent.Append(MainPage(context)); break;//主页
                        case "GetProduct": _strContent.Append(GetProduct(context)); break;//产品列表页
                        //case "GetNews": _strContent.Append(GetNews(context)); break;//新闻列表
                        //case "MoreProduct": _strContent.Append(MoreProduct(context)); break;//更多产品
                        //case "MoreNews": _strContent.Append(MoreNews(context)); break;//更多新闻
                        case "DetailPage": _strContent.Append(DetailPage(context)); break;//渲染动态详情页，伪静态可用
                        case "sitemap": _strContent.Append(SiteMapXML(context)); break;//站点地图XML
                        case "sitemap.html": _strContent.Append(SiteMapHtml(context)); break;//站点地图Html
                        default: break;
                    }
                }
            }
            context.Response.Write(_strContent.ToString());
        }

        /// <summary>
        /// siteMap XML
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string SiteMapXML(HttpContext context)
        {
            context.Response.ContentType = "application/xml;charset=utf-8";
            List<htmlPara> hInfo = bll.GetXML();
            var data = new
            {
                hInfo
            };
            string html = SqlHelperCatalog.WriteTemplate(data, "sitemap.xml");
            return html;
        }
        /// <summary>
        /// siteMap Html
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string SiteMapHtml(HttpContext context)
        {
            List<htmlPara> hInfo = bll.GetXML();
            var data = new
            {
                hInfo,
                htmlTitle = hostName,
                hostName,
                hostUrl,
                columnsList = bll.GetColumns(""),//导航
            };
            return SqlHelperCatalog.WriteTemplate(data, "sitemap.html");
        }
        /// <summary>
        /// 渲染详情页，伪静态
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string DetailPage(HttpContext context)
        {
            string columnId = context.Request["cId"];
            string Id = context.Request["Id"];
            if (string.IsNullOrEmpty(columnId) || string.IsNullOrEmpty(Id))
                return SqlHelperCatalog.WriteTemplate("", "404.html");
            try
            {
                htmlPara hInfo = bll.GetHtmlPara(columnId, Id);
                string keyword = "";//关键词
                if (hInfo.title.Length > 6)
                    keyword = hInfo.title + "," + hInfo.title.Substring(0, 2) + "," + hInfo.title.Substring(2, 2) + "," + hInfo.title.Substring(4, 2);
                else
                    keyword = hInfo.title;
                var data = new
                {
                    htmlTitle = hInfo.title + "_" + hInfo.companyName,
                    hInfo,
                    keyword,
                    description = BLL.ReplaceHtmlTag(hInfo.articlecontent, 80),//产品简介
                    host = hostUrl,
                    ProductFloat = bll.GetProFloat(hInfo.userId,"22"),
                    NewsFloat = bll.GetNewsFloat(hInfo.userId,"22")
                };
                string html = SqlHelperCatalog.WriteTemplate(data, "DetailModel.html");
                return html;
            }
            catch (Exception ex)
            {
                return json.WriteJson(0, ex.ToString(), new { });
            }
        }

        /// <summary>
        /// 主页
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string MainPage(HttpContext context)
        {
            var data = new
            {
                htmlTitle = hostName,
                hostName,
                hostUrl,
                columnsList = bll.GetColumns(""),//导航
                productTitle = GetNoNewsByCId("50", "20"),//最新产品，无分类
                newsTitle = GetParaByCId("20", 1, 50)//最新新闻 

                //lunboTitle = GetParaByCId("20", 1, 6),//轮播标题，推荐新闻
                //tuijianTitle = GetNoNewsByCId("12", "20"),//推荐产品
            };
            return SqlHelperCatalog.WriteTemplate(data, "MainPage.html");
        }
        /// <summary>
        /// 产品列表页
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cId"></param>
        /// <returns></returns>
        public string GetProduct(HttpContext context)
        {
            int pageIndex = 1;
            if (context.Request["pageIndex"] != null)
            {
                try
                {
                    pageIndex = int.Parse(context.Request["pageIndex"]);
                }
                catch
                {

                }
            }
            string cId = context.Request["cId"];
            int paraTotal = bll.GetPageTotal(cId);//此行业总条数
            int pageCount = (int)Math.Ceiling(paraTotal / 20.0);//总页数（每页20条）
            List<object> pageData = new List<object>();
            if (pageCount > 8)
            {
                if (pageIndex > 4)
                {
                    int maxIndex = pageIndex + 4;
                    if (maxIndex > pageCount)
                        maxIndex = pageCount;
                    for (int i = pageIndex - 5; i < maxIndex; i++)
                    {
                        pageData.Add(new { Href = "list_" + cId + "_" + (i + 1), Title = i + 1 });
                    }
                }
                else
                {
                    for (int i = 0; i < 9; i++)
                    {
                        pageData.Add(new { Href = "list_" + cId + "_" + (i + 1), Title = i + 1 });
                    }
                }
            }
            else
            {
                for (int i = 0; i < pageCount; i++)
                {
                    pageData.Add(new { Href = "list_" + cId + "_" + (i + 1), Title = i + 1 });
                }
            }
            string columnName = "";
            List<columnInfo> columnsList = bll.GetColumns("");//导航
            foreach (columnInfo c in columnsList)
            {
                if (c.Id.ToString() == cId)
                    columnName = c.columnName;
            }
            var data = new
            {
                htmlTitle = columnName + "_" + hostName,
                hostName,
                hostUrl,
                cId,
                columnsList,//导航
                columnName,//栏目名称
                paraTotal,//总条数
                pageIndex,//当前页
                pageData,//页码渲染
                pageCount,//总页数
                productList = GetParaByCId(cId, pageIndex, 20),
                newsList = GetParaByCId("20", 1, 20),//右侧浮动栏新闻20条
                productListNocId = GetNoNewsByCId("20", "20")//右侧浮动栏产品20条
            };
            return SqlHelperCatalog.WriteTemplate(data, "Product.html");
        }
        /// <summary>
        /// 新闻列表页
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cId"></param>
        /// <returns></returns>
        public string GetNews(HttpContext context)
        {
            int pageIndex = 1;
            if (context.Request["pageIndex"] != null)
            {
                try
                {
                    pageIndex = int.Parse(context.Request["pageIndex"]);
                }
                catch
                {

                }
            }
            string cId = context.Request["cId"];
            int paraTotal = bll.GetPageTotal(cId);//此行业总条数
            int pageCount = (int)Math.Ceiling(paraTotal / 20.0);//总页数（每页20条）
            object[] pageData = new object[pageCount];
            for (int i = 0; i < pageCount; i++)
            {
                pageData[i] = new { Href = "http://hyzx.100dh.cn/xinxi/list_" + cId + "/" + (i + 1) + ".html", Title = i + 1 };
            }
            var data = new
            {
                htmlTitle = "新闻栏目",
                hostName,
                hostUrl,
                cId,
                columnsList = bll.GetColumns(""),//导航
                columnName = bll.GetColumns("where Id=" + cId)[0].columnName,
                paraTotal,//总条数
                pageIndex,//当前页
                pageData,//页码渲染
                pageCount,//总页数
                newsList = GetParaByCId("20", pageIndex, 20),
                productList = GetNoNewsByCId("20", "20")
            };
            return SqlHelperCatalog.WriteTemplate(data, "News.html");
        }
        /// <summary>
        /// 更多产品页
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string MoreProduct(HttpContext context)
        {
            string cId = "21";
            var data = new
            {
                htmlTitle = "产品栏目",
                hostName,
                hostUrl,
                cId,
                c1Title = GetParaByCId("1", 1, 10),
                c2Title = GetParaByCId("2", 1, 10),
                c3Title = GetParaByCId("3", 1, 10),
                c4Title = GetParaByCId("4", 1, 10),//工具量具，不显示
                c5Title = GetParaByCId("5", 1, 10),
                c6Title = GetParaByCId("6", 1, 10),
                c7Title = GetParaByCId("7", 1, 10),
                c8Title = GetParaByCId("8", 1, 10),
                c9Title = GetParaByCId("9", 1, 10),
                c10Title = GetParaByCId("10", 1, 10),
                c11Title = GetParaByCId("11", 1, 10),
                c12Title = GetParaByCId("12", 1, 10),
                c13Title = GetParaByCId("13", 1, 10),
                c14Title = GetParaByCId("14", 1, 10),
                c15Title = GetParaByCId("15", 1, 10),
                c16Title = GetParaByCId("16", 1, 10),
                c17Title = GetParaByCId("17", 1, 10),
                c18Title = GetParaByCId("18", 1, 10),
                c19Title = GetParaByCId("19", 1, 10)
            };
            return SqlHelperCatalog.WriteTemplate(data, "MoreProduct.html");
        }
        /// <summary>
        /// 更多新闻页
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string MoreNews(HttpContext context)
        {
            string cId = "22";
            var data = new
            {
                htmlTitle = "新闻栏目",
                hostName,
                hostUrl,
                cId,
                newsTitle = GetParaByCId("20", 1, 30)
            };
            return SqlHelperCatalog.WriteTemplate(data, "MoreNews.html");
        }

        private BLL bll = new BLL();
        /// <summary>
        /// 根据行业id获取最新产品
        /// </summary>
        /// <param name="columnId">行业Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        public List<htmlPara> GetParaByCId(string columnId, int pageIndex, int pageSize)
        {
            List<htmlPara> hList = bll.GetHtmlList(columnId, pageIndex, pageSize);
            return hList;
        }
        /// <summary>
        /// 获取最新产品，无分类
        /// </summary>
        /// <param name="count">产品条数</param>
        /// <param name="columnId">行业id</param>
        /// <returns></returns>
        public List<htmlPara> GetNoNewsByCId(string count, string columnId)
        {
            List<htmlPara> hList = bll.GetHtmlList(count, columnId);
            return hList;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}