using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace CnblogsImageBed
{
	//博客园登陆类
    public class Login
    {
        //全局变量保存cookie信息
        public CookieContainer cc = new CookieContainer();

        public string LoginCnblogs(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException();
            }

            string loginUrl = "https://passport.cnblogs.com/login.aspx?ReturnUrl=http%3A%2F%2Fwww.cnblogs.com%2F";
            string postData = string.Format("__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE=%2FwEPDwULLTE1MzYzODg2NzZkGAEFHl9fQ29udHJvbHNSZXF1aXJlUG9zdEJhY2tLZXlfXxYBBQtjaGtSZW1lbWJlcm1QYDyKKI9af4b67Mzq2xFaL9Bt&__EVENTVALIDATION=%2FwEdAAUyDI6H%2Fs9f%2BZALqNAA4PyUhI6Xi65hwcQ8%2FQoQCF8JIahXufbhIqPmwKf992GTkd0wq1PKp6%2B%2F1yNGng6H71Uxop4oRunf14dz2Zt2%2BQKDEIYpifFQj3yQiLk3eeHVQqcjiaAP&tbUserName={0}&tbPassword={1}&btnLogin=%E7%99%BB++%E5%BD%95&txtReturnUrl=http%3A%2F%2Fwww.cnblogs.com%2F", userName, password);

            HttpHeader header = new HttpHeader();
            header.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            header.ContentType = "application/x-www-form-urlencoded";
            header.Method = "POST";
            header.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36";
            header.MaxTry = 300;

            //获取cookie
            cc = HttpHelper.GetCookie(loginUrl, postData, header);

            //进入个人主页
            string homeUrl = "http://home.cnblogs.com";
            string html = HttpHelper.GetHtmlByCookie(homeUrl, cc, header);


            return html;
        }
    }
}
