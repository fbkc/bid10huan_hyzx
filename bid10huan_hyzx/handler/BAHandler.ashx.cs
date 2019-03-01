using HRMSys.DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bid10huan_hyzx
{
    /// <summary>
    /// BAHandler 的摘要说明
    /// </summary>
    public class BAHandler : IHttpHandler
    {
        private BLL bll = new BLL();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string columnId = context.Request["cId"];
            string Id = context.Request["Id"];
            if (!string.IsNullOrEmpty(columnId) && !string.IsNullOrEmpty(Id))
            {
                List<htmlPara> pList = bll.GetHtmlBAPage(columnId, Id);//上一篇，下一篇
                object BPage = null, APage = null;
                if (pList.Count == 2)
                {
                    BPage = new { Href = pList[0].titleURL, Title = pList[0].title };
                    APage = new { Href = pList[1].titleURL, Title = pList[1].title };
                }
                else
                {
                    BPage = new { Href = pList[0].titleURL, Title = pList[0].title };
                    APage = new { Href = "javascript:alert('没有了');", Title = "没有了" };
                }
                var data = new
                {
                    BPage,
                    APage
                };
                string html = SqlHelperCatalog.WriteTemplate(data, "BAPage.html");
                context.Response.Write(html);
            }
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