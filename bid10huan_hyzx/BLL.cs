using HRMSys.DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace bid10huan_hyzx
{
    public class BLL
    {
        /// <summary>
        /// XML
        /// </summary>
        /// <returns></returns>
        public List<htmlPara> GetXML()
        {
            List<htmlPara> hParaList = new List<htmlPara>();
            DataTable dt = SqlHelperCatalog.ExecuteDataTable("select top 1000 Id, title,titleURL,addTime from htmlPara order by Id desc");
            if (dt.Rows.Count < 1)
                return null;
            foreach (DataRow row in dt.Rows)
            {
                htmlPara hPara = new htmlPara();
                hPara.Id = (long)SqlHelper.FromDBNull(row["Id"]);
                hPara.title = (string)SqlHelper.FromDBNull(row["title"]);
                hPara.titleURL = (string)SqlHelper.FromDBNull(row["titleURL"]);
                hPara.addTime = ((DateTime)SqlHelper.FromDBNull(row["addTime"])).ToString("yyyy-MM-dd");
                hParaList.Add(hPara);
            }
            return hParaList;
        }

        /// <summary>
        /// 获取单条信息
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="columnId"></param>
        /// <returns></returns>
        public htmlPara GetHtmlPara(string columnId, string Id)
        {
            htmlPara hPara = new htmlPara();
            DataTable dt = SqlHelperCatalog.ExecuteDataTable(@"select * from htmlPara h left join columnInfo c on h.columnId=c.Id where columnId =@columnId and h.Id=@Id",
               new SqlParameter("@columnId", columnId),
               new SqlParameter("@Id", Id));
            if (dt.Rows.Count != 1)
                return null;
            DataRow row = dt.Rows[0];
            hPara.Id = (long)SqlHelper.FromDBNull(row["Id"]);
            hPara.userId = SqlHelper.FromDBNull(row["userId"]).ToString();
            hPara.title = (string)SqlHelper.FromDBNull(row["title"]);
            hPara.titleImg = (string)SqlHelper.FromDBNull(row["titleImg"]);
            hPara.titleURL = (string)SqlHelper.FromDBNull(row["titleURL"]);
            hPara.columnId = (string)SqlHelper.FromDBNull(row["columnId"]);//栏目Id
            string content = (string)SqlHelper.FromDBNull(row["articlecontent"]);
            //hPara.articlecontent = ReplaceHtmlTag(content, 60);//产品简介
            hPara.articlecontent = content.Replace("\r\n", "").Replace("<br/>", "").Replace("&nbsp;", "").Replace("\r", "").Replace("\n", "");
            hPara.city = (string)SqlHelper.FromDBNull(row["city"]);//生产城市
            hPara.smallCount = (string)SqlHelper.FromDBNull(row["smallCount"]);//起订
            hPara.xinghao = (string)SqlHelper.FromDBNull(row["xinghao"]);//型号
            hPara.unit = (string)SqlHelper.FromDBNull(row["unit"]);//单位
            hPara.sumCount = (string)SqlHelper.FromDBNull(row["sumCount"]);//供货总量
            hPara.price = (string)SqlHelper.FromDBNull(row["price"]);//单价
            hPara.pinpai = (string)SqlHelper.FromDBNull(row["pinpai"]);//品牌
            hPara.companyName = (string)SqlHelper.FromDBNull(row["companyName"]);//公司名字
            hPara.ten_qq = (string)SqlHelper.FromDBNull(row["ten_qq"]);
            hPara.com_web = (string)SqlHelper.FromDBNull(row["com_web"]);//网址
            hPara.addTime = ((DateTime)SqlHelper.FromDBNull(row["addTime"])).ToString("yyyy-MM-dd HH:mm:ss");
            hPara.columnName = (string)SqlHelper.FromDBNull(row["columnName"]);//栏目名
            hPara.username = (string)SqlHelper.FromDBNull(row["username"]);//用户名
            return hPara;
        }
        /// <summary>
        /// 查询上一条下一条
        /// </summary>
        /// <param name="columnId"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public List<htmlPara> GetHtmlBAPage(string columnId, string Id)
        {
            List<htmlPara> hList = new List<htmlPara>();
            DataTable dt = SqlHelperCatalog.ExecuteDataTable(@"select top 1 -1 as [Sort],Id,title,columnId,titleURL from (select top 1 Id,title,columnId,titleURL from htmlPara where columnId=@columnId and Id<@Id order by Id desc) as tab_up 
                 union select top 1 1 as [Sort],Id,title,columnId,titleURL from (select top 1 Id,title,columnId,titleURL from htmlPara where columnId=@columnId and Id>@Id order by Id) as tab_up",
               new SqlParameter("@columnId", columnId),
               new SqlParameter("@Id", Id));
            if (dt.Rows.Count < 1)
                return null;
            foreach (DataRow row in dt.Rows)
            {
                htmlPara hPara = new htmlPara();
                hPara.Id = (long)SqlHelper.FromDBNull(row["Id"]);
                hPara.title = (string)SqlHelper.FromDBNull(row["title"]);
                hPara.titleURL = (string)SqlHelper.FromDBNull(row["titleURL"]);
                hPara.columnId = (string)SqlHelper.FromDBNull(row["columnId"]);
                hList.Add(hPara);
            }
            return hList;
        }
        /// <summary>
        /// 右侧浮动栏，前十条公司产品
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<htmlPara> GetProFloat(string userId, string cId)
        {
            List<htmlPara> hList = new List<htmlPara>();
            DataTable dt = SqlHelperCatalog.ExecuteDataTable(@"select top 10 Id,title,titleURL from htmlPara where userId=@userId and columnId!=@columnId order by addTime desc",
               new SqlParameter("@userId", userId),
               new SqlParameter("@columnId", cId));
            if (dt.Rows.Count < 1)
                return null;
            foreach (DataRow row in dt.Rows)
            {
                htmlPara hPara = new htmlPara();
                hPara.Id = (long)SqlHelper.FromDBNull(row["Id"]);
                hPara.title = (string)SqlHelper.FromDBNull(row["title"]);
                hPara.titleURL = (string)SqlHelper.FromDBNull(row["titleURL"]);
                hList.Add(hPara);
            }
            return hList;
        }
        /// <summary>
        /// 右侧浮动栏，前十条新闻
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<htmlPara> GetNewsFloat(string userId, string cId)
        {
            List<htmlPara> hList = new List<htmlPara>();
            DataTable dt = SqlHelperCatalog.ExecuteDataTable(@"select top 10 Id,title,titleURL from htmlPara where userId=@userId and columnId=@columnId order by addTime desc",
               new SqlParameter("@userId", userId),
               new SqlParameter("@columnId", cId));
            if (dt.Rows.Count < 1)
                return null;
            foreach (DataRow row in dt.Rows)
            {
                htmlPara hPara = new htmlPara();
                hPara.Id = (long)SqlHelper.FromDBNull(row["Id"]);
                hPara.title = (string)SqlHelper.FromDBNull(row["title"]);
                hPara.titleURL = (string)SqlHelper.FromDBNull(row["titleURL"]);
                hList.Add(hPara);
            }
            return hList;
        }
        /// <summary>
        /// 获取栏目页网页
        /// </summary>
        /// <param name="columnId">行业类别</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        public List<htmlPara> GetHtmlList(string columnId, int pageIndex, int pageSize)
        {
            //分页查询
            List<htmlPara> hList = new List<htmlPara>();
            DataTable dt = SqlHelperCatalog.ExecuteDataTable(@"select Id,title,titleURL,titleImg,userId,columnId,articlecontent,city,smallCount,companyName,ten_qq,com_web,addTime from 
                (select Id,title,titleURL,titleImg,userId,columnId,articlecontent,city,smallCount,companyName,ten_qq,com_web,addTime, ROW_NUMBER() OVER(order by addTime desc) AS RowId from htmlPara where columnId=@columnId) as b where b.RowId between @startNum and @endNum",
               new SqlParameter("@columnId", columnId),
               new SqlParameter("@startNum", (pageIndex - 1) * pageSize + 1),
               new SqlParameter("@endNum", pageIndex * pageSize));
            if (dt.Rows.Count < 1)
                return null;
            foreach (DataRow row in dt.Rows)
            {
                htmlPara hPara = new htmlPara();
                hPara.Id = (long)SqlHelper.FromDBNull(row["Id"]);
                hPara.userId = SqlHelper.FromDBNull(row["userId"]).ToString();
                hPara.title = (string)SqlHelper.FromDBNull(row["title"]);
                hPara.titleImg = (string)SqlHelper.FromDBNull(row["titleImg"]);
                hPara.titleURL = (string)SqlHelper.FromDBNull(row["titleURL"]);
                hPara.columnId = (string)SqlHelper.FromDBNull(row["columnId"]);//栏目Id
                string content = (string)SqlHelper.FromDBNull(row["articlecontent"]);
                hPara.articlecontent = ReplaceHtmlTag(content, 60);//产品简介
                hPara.city = (string)SqlHelper.FromDBNull(row["city"]);//生产城市
                hPara.smallCount = (string)SqlHelper.FromDBNull(row["smallCount"]);//起订
                hPara.companyName = (string)SqlHelper.FromDBNull(row["companyName"]);//公司名字
                hPara.ten_qq = (string)SqlHelper.FromDBNull(row["ten_qq"]);
                hPara.com_web = (string)SqlHelper.FromDBNull(row["com_web"]);//网址
                hPara.addTime = ((DateTime)SqlHelper.FromDBNull(row["addTime"])).ToString("yyyy-MM-dd");
                hList.Add(hPara);
            }
            return hList;
        }
        /// <summary>
        /// 获取前多少条数据
        /// </summary>
        /// <param name="count">条数</param>
        /// <param name="columnId">栏目Id</param>
        /// <returns></returns>
        public List<htmlPara> GetHtmlList(string count, string columnId)
        {
            List<htmlPara> hList = new List<htmlPara>();
            DataTable dt = SqlHelperCatalog.ExecuteDataTable(@"select top (CONVERT(int,@count)) Id,title,titleURL,addTime from htmlPara where columnId !='@columnId' order by addTime desc",
               new SqlParameter("@columnId", columnId),
               new SqlParameter("@count", count));
            if (dt.Rows.Count < 1)
                return null;
            foreach (DataRow row in dt.Rows)
            {
                htmlPara hPara = new htmlPara();
                hPara.Id = (long)SqlHelper.FromDBNull(row["Id"]);
                hPara.title = (string)SqlHelper.FromDBNull(row["title"]);
                hPara.titleURL = (string)SqlHelper.FromDBNull(row["titleURL"]);
                hPara.addTime = ((DateTime)SqlHelper.FromDBNull(row["addTime"])).ToString("yyyy-MM-dd");
                hList.Add(hPara);
            }
            return hList;
        }
        public static string ReplaceHtmlTag(string html, int length)
        {
            string strText = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");

            if (length > 0 && strText.Length > length)
                return strText.Substring(0, length);

            return strText;
        }
        /// <summary>
        /// 获取目录主页内容
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <returns></returns>
        public List<htmlInfo> GetMainHtmlList()
        {
            List<htmlInfo> hList = new List<htmlInfo>();
            //            DataTable dt = SqlHelper.ExecuteDataSet(@"select title,titleImg,titleURL,addTime,columnId,columnName from 
            //( select 
            //RANK()OVER(PARTITION BY columnInfo.Id ORDER BY htmlInfo.addTime DESC) AS
            //RANK2, title,titleImg,titleURL,addTime,htmlInfo.columnId,columnName from 
            //htmlInfo left join columnInfo On htmlInfo.columnId = columnInfo.Id) T
            //where RANK2<=10 ").Tables[0];
            DataTable dt = SqlHelperCatalog.ExecuteDataSet(@"select  title,titleURL,titleImg,addTime,columnId from 
( select 
RANK()OVER(PARTITION BY columnId ORDER BY addTime DESC) AS
RANK2, title,titleImg,titleURL,addTime,columnId from 
htmlPara ) T
where RANK2<=10").Tables[0];
            if (dt.Rows.Count < 1)
                return null;
            foreach (DataRow row in dt.Rows)
            {
                htmlInfo hInfo = new htmlInfo();
                hInfo.title = (string)SqlHelper.FromDBNull(row["title"]);
                hInfo.titleImg = (string)SqlHelper.FromDBNull(row["titleImg"]);
                hInfo.titleURL = (string)SqlHelper.FromDBNull(row["titleURL"]);
                hInfo.columnId = (string)SqlHelper.FromDBNull(row["columnId"]);//栏目Id
                hInfo.addTime = ((DateTime)SqlHelper.FromDBNull(row["addTime"])).ToString("yyyy-MM-dd");
                //hInfo.realmNameId = (string)row["realmNameId"];//目录名
                hList.Add(hInfo);
            }
            return hList;
        }

        /// <summary>
        /// 将html内容参数存入数据库
        /// </summary>
        /// <param name="info"></param>
        public void AddHtml(htmlPara info)
        {
            int a = SqlHelperCatalog.ExecuteNonQuery(@"INSERT INTO htmlPara
         ([title]
           ,[titleURL]
           ,[articlecontent]
           ,[columnId]
           ,[pinpai]
           ,[xinghao]
           ,[price]
           ,[smallCount]
           ,[sumCount]
           ,[unit]
           ,[city]
           ,[titleImg]
           ,[addTime]
           ,[ten_qq]
           ,[companyName]
           ,[com_web]
           ,[userId]
           ,[userName])
     VALUES
           (@title
           ,@titleURL+CONVERT(varchar(10), IDENT_CURRENT('htmlPara'))+'.html'
           ,@articlecontent
           ,@columnId
           ,@pinpai
           ,@xinghao
           ,@price
           ,@smallCount
           ,@sumCount
           ,@unit
           ,@city
           ,@titleImg
           ,@addTime
           ,@ten_qq
           ,@companyName
           ,@com_web
           ,@userId
           ,@userName)",
               new SqlParameter("@title", SqlHelper.ToDBNull(info.title)),
               new SqlParameter("@titleURL", SqlHelper.ToDBNull(info.titleURL)),
               new SqlParameter("@articlecontent", SqlHelper.ToDBNull(info.articlecontent)),
               new SqlParameter("@columnId", SqlHelper.ToDBNull(info.columnId)),
               new SqlParameter("@pinpai", SqlHelper.ToDBNull(info.pinpai)),
               new SqlParameter("@xinghao", SqlHelper.ToDBNull(info.xinghao)),
               new SqlParameter("@price", SqlHelper.ToDBNull(info.price)),
               new SqlParameter("@smallCount", SqlHelper.ToDBNull(info.smallCount)),
               new SqlParameter("@sumCount", SqlHelper.ToDBNull(info.sumCount)),
               new SqlParameter("@unit", SqlHelper.ToDBNull(info.unit)),
               new SqlParameter("@city", SqlHelper.ToDBNull(info.city)),
               new SqlParameter("@titleImg", SqlHelper.ToDBNull(info.titleImg)),
               new SqlParameter("@addTime", SqlHelper.ToDBNull(info.addTime)),
               new SqlParameter("@ten_qq", SqlHelper.ToDBNull(info.ten_qq)),
               new SqlParameter("@companyName", SqlHelper.ToDBNull(info.companyName)),
               new SqlParameter("@com_web", SqlHelper.ToDBNull(info.com_web)),
               new SqlParameter("@userId", SqlHelper.ToDBNull(info.userId)),
               new SqlParameter("@userName", SqlHelper.ToDBNull(info.username)));
        }
        /// <summary>
        /// 标题存储成功后返回titleUrl
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetTitleUrl(string userId)
        {
            object tUrl = SqlHelperCatalog.ExecuteScalar("select top 1 titleURL from htmlPara where userId=@userId order by Id desc",
                new SqlParameter("@userId", SqlHelper.ToDBNull(userId)));
            return tUrl.ToString();
        }

        /// <summary>
        /// 获取当前表最大Id
        /// </summary>
        /// <returns></returns>
        public int GetMaxId()
        {
            object ob = "";
            try
            {
                ob = SqlHelperCatalog.ExecuteScalar("select Id  from htmlPara order by Id desc");
                if (ob == null)
                    ob = 0;
            }
            catch (Exception ex)
            { return 1; }
            return int.Parse(ob.ToString());
        }

        /// <summary>
        /// 通过用户名找Id
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <returns></returns>
        public string GetUserId(string username)
        {
            object ob = SqlHelper.ExecuteScalar("select Id from userInfo where username='" + username + "'");
            return ob.ToString();
        }
        public int GetPageTotal(string columnId)
        {
            return (int)SqlHelperCatalog.ExecuteScalar("select count(*)  from htmlPara where columnId=@columnId",
                new SqlParameter("@columnId", columnId));
        }
        /// <summary>
        /// 获取栏目信息
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <returns></returns>
        public List<columnInfo> GetColumns(string sqlstr)
        {
            List<columnInfo> cList = new List<columnInfo>();
            DataTable dt = SqlHelperCatalog.ExecuteDataTable("select Id,columnName from columnInfo " + sqlstr);
            if (dt.Rows.Count < 1)
                return null;
            foreach (DataRow row in dt.Rows)
            {
                columnInfo cInfo = new columnInfo();
                cInfo.Id = (int)SqlHelper.FromDBNull(row["Id"]);
                cInfo.columnName = (string)SqlHelper.FromDBNull(row["columnName"]);
                cList.Add(cInfo);
            }
            return cList;
        }
    }
}