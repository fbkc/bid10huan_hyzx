using AutoSend;
using HRMSys.DAL;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace bid10huan_hyzx
{
    /// <summary>
    /// ModelHandler 的摘要说明
    /// </summary>
    public class ModelHandler : IHttpHandler
    {
        private BLL bll = new BLL();
        private string hostUrl = "http://bid.10huan.com/hyzx";
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            StringBuilder _strContent = new StringBuilder();
            if (_strContent.Length == 0)
            {
                string _strAction = context.Request.Params["action"];
                if (string.IsNullOrEmpty(_strAction))
                {
                    _strContent.Append(_strContent.Append("404.html"));
                }
                else
                {
                    switch (_strAction.Trim())
                    {
                        case "moduleHtml": _strContent.Append(ModuleHtml(context)); break;
                        default: break;
                    }
                }
            }
            context.Response.Write(_strContent.ToString());
        }

        #region 发布系统调用的post接口
        /// <summary>
        /// 发布系统调用的post接口
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string ModuleHtml(HttpContext context)
        {
            string username = context.Request["username"];
            if (string.IsNullOrEmpty(username))
                return json.WriteJson(0, "用户名不能为空", new { });
            //key值判断
            string keyValue = NetHelper.GetMD5(username + "100dh888");
            string key = context.Request["key"];
            if (key != keyValue)
                return json.WriteJson(0, "key值错误", new { });
            //根据username调用tool接口获取userInfo
            string strjson = NetHelper.HttpGet("http://tool.100dh.cn/UserHandler.ashx?action=GetUserByUsername&username=" + username, "", Encoding.UTF8);//公共接口，调用user信息
            JObject jo = (JObject)JsonConvert.DeserializeObject(strjson);
            cmUserInfo userInfo = JsonConvert.DeserializeObject<cmUserInfo>(jo["detail"]["cmUser"].ToString());
            //时间间隔必须大于60秒
            DateTime dt = DateTime.Now;
            DateTime sdt = Convert.ToDateTime(userInfo.beforePubTime);
            TimeSpan d3 = dt.Subtract(sdt);
            if (d3.TotalSeconds < 10)
                return json.WriteJson(0, "信息发布过快，请隔60秒再提交！", new { });
            //判断今日条数是否达到1000条
            if (userInfo.endTodayPubCount > 999)
                return json.WriteJson(0, "今日投稿已超过限制数！", new { });
            //判断所有条数是否发完
            if (!(userInfo.canPubCount > userInfo.endPubCount))
                return json.WriteJson(0, "会员套餐信息条数已发完！", new { });
            string url = "";
            try
            {
                htmlPara hInfo = new htmlPara();
                hInfo.userId = userInfo.Id.ToString();//用户名
                hInfo.title = context.Request["title"];
                string cid = context.Request["catid"];
                if (string.IsNullOrEmpty(cid))
                    return json.WriteJson(0, "行业或栏目不能为空", new { });
                hInfo.columnId = cid;//行业id，行业新闻id=20
                string content = context.Request["content"];
                if (string.IsNullOrEmpty(content) || content.Length < 500)
                    return json.WriteJson(0, "文章不能少于500字，请丰富文章内容", new { });
                hInfo.articlecontent = content;
                //hInfo.articlecontent = HttpUtility.UrlDecode(jo["content"].ToString(), Encoding.UTF8);//内容,UrlDecode解码
                //命名规则：ip/目录/用户名/show_行业id+(五位数id)
                hInfo.titleURL = hostUrl + "/" + username + "/ashow-" + cid + "-";
                hInfo.pinpai = context.Request["pinpai"];
                hInfo.xinghao = context.Request["xinghao"];
                hInfo.price = context.Request["price"];
                hInfo.smallCount = context.Request["qiding"];
                hInfo.sumCount = context.Request["gonghuo"];
                hInfo.unit = context.Request["unit"];
                hInfo.city = context.Request["city"];
                hInfo.titleImg = context.Request["thumb"];
                hInfo.addTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                hInfo.username = username;
                hInfo.ten_qq = userInfo.ten_qq;
                hInfo.companyName = userInfo.companyName;
                hInfo.com_web = userInfo.com_web;
                //hInfo.realmNameId = "1";//发到哪个站
                bll.AddHtml(hInfo);//存入数据库
                url = bll.GetTitleUrl(userInfo.Id.ToString());
                //调用tool接口，更新userInfo已发条数等信息
                NetHelper.HttpGet("http://tool.100dh.cn/UserHandler.ashx?action=UpUserPubInformation&userId=" + userInfo.Id, "", Encoding.UTF8);//公共接口，调用user信息
            }
            catch (Exception ex)
            {
                return json.WriteJson(0, ex.ToString(), new { });
            }
            return json.WriteJson(1, "发布成功", new { url, username });
        }
        #endregion

        #region 写模板
        /// <summary>
        /// 写模板
        /// </summary>
        /// <param name="hInfo"></param>
        /// <param name="uInfo"></param>
        /// <param name="username"></param>
        /// <param name="hName"></param>
        /// <returns></returns>
        public static bool WriteFile(string moduleHtml, string htmlfilename, string username)
        {
            //文件输出目录
            string path = HttpContext.Current.Server.MapPath("~/" + username + "/");
            //无此路径，则创建路径
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            // 写文件
            using (StreamWriter sw = new StreamWriter(path + htmlfilename, true))
            {
                sw.Write(moduleHtml);
                sw.Flush();
                sw.Close();
            }
            return true;
        }
        #endregion

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}