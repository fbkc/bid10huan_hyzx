using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace xinxi
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            //RouteTable.Routes.Add(new Route("TestHandler.html", new ReWriteUrl("~/TestHandler.ashx")));//地址重写
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            ////Accept the URL inputed by users
            //string url = Request.RawUrl;

            //Regex regex = new Regex(@"hyzx/(\d+).html");

            //Match match = regex.Match(url);
            //if (match.Success)
            //{

            //    string id = match.Groups[1].Value;

            //    string s = "handler/TestHandler.ashx?action=" + id;

            //    HttpContext.Current.RewritePath(s);
            //}
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}