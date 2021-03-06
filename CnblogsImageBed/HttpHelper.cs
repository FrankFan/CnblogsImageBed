﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace CnblogsImageBed
{
    public class HttpHelper
    {
        /// <summary>
        /// 获取cookie
        /// </summary>
        /// <param name="loginUrl"></param>
        /// <param name="postedData"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static CookieContainer GetCookie(string loginUrl, string postedData, HttpHeader header, out string cookieStr)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream requestStream = null;
            CookieContainer cc = new CookieContainer();
            cookieStr = string.Empty;

            try
            {
                //准备发起请求
                request = (HttpWebRequest)WebRequest.Create(loginUrl);
                request.Method = header.Method;
                request.ContentType = header.ContentType;
                byte[] postDataByte = Encoding.UTF8.GetBytes(postedData);
                request.ContentLength = postDataByte.Length;
                request.CookieContainer = cc;
                request.KeepAlive = true;
                request.AllowAutoRedirect = false;

                //提交请求
                requestStream = request.GetRequestStream();
                requestStream.Write(postDataByte, 0, postDataByte.Length);

                //接收响应
                response = (HttpWebResponse)request.GetResponse();
                //response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);
                response.Cookies = cc.GetCookies(request.RequestUri);

                CookieCollection cookieCollection = response.Cookies;

                cc.Add(cookieCollection);

                cookieStr = request.CookieContainer.GetCookieHeader(request.RequestUri);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                request = null;
                response.Close();
                requestStream.Close();   
            }

            return cc;
        }

        /// <summary>
        /// Get方式获携带cookie获取页面html
        /// </summary>
        /// <param name="url">请求的url</param>
        /// <param name="cc">cookie</param>
        /// <param name="header">请求Header对象</param>
        /// <returns>页面源代码html</returns>
        public static string GetHtmlByCookie(string url, CookieContainer cc, HttpHeader header,string cookieStr)
        {
            string html = string.Empty;
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader streamReader = null;
            Stream responseStream = null;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.CookieContainer = cc;
                request.ContentType = header.ContentType;
                request.Referer = url;
                request.Accept = header.Accept;
                request.UserAgent = header.UserAgent;
                request.Method = "GET";
                request.Headers.Add("Cookie:" + cookieStr); //每次请求时把cookie传给服务器

                //发起请求，得到Response
                response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                streamReader = new StreamReader(responseStream, Encoding.UTF8);
                html = streamReader.ReadToEnd();

                cookieStr = request.CookieContainer.GetCookieHeader(request.RequestUri);
            }
            catch (Exception ex)
            {
                if (request != null)
                    request.Abort();
                if (response != null)
                    response.Close();

                return string.Empty;
                throw ex;
            }
            finally
            {
                //关闭各种资源
                streamReader.Close();
                responseStream.Close();
                request.Abort();
                response.Close();
            }
            return html;
        }

        /// <summary>
        /// 此方法未用到
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <param name="cc"></param>
        /// <returns></returns>
        public static string HttpPostWithCookie(string url, string filePath, CookieContainer cc)
        {
            string html = string.Empty;
            Stream myResponseStream = null;
            StreamReader sreamReader = null;
            StreamWriter myStreamWriter = null;
            HttpWebResponse response = null;

            //set file to post
            FileInfo f = new FileInfo(filePath);
            int fileLength = (int)f.Length;
            byte[] bufPost = new byte[fileLength];

            FileStream fStream = new FileStream(filePath, FileMode.Open);
            fStream.Read(bufPost, 0, fileLength);
            
            //create a http web request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //set request type
            request.Method = "POST";
            request.ContentType = "application/octet-stream";
            request.ContentLength = bufPost.Length;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36";
            request.Accept = "*/*";
            request.Referer = "http://upload.cnblogs.com/imageuploader/upload?host=www.cnblogs.com&editor=4";
            request.KeepAlive = true;

            //set cookie
            request.CookieContainer = cc;


            try
            {
                //get request stream
                Stream responseStream = request.GetRequestStream();
                //用StreamWriter向流request stream中写入字符，格式是UTF8
                myStreamWriter = new StreamWriter(responseStream, Encoding.UTF8);
                myStreamWriter.Write(bufPost);

                request.ServicePoint.ConnectionLimit = 50;

                try
                {
                    //get http response
                    response = request.GetResponse() as HttpWebResponse;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                //获取服务器的cookie
                //response.Cookies = cookieContainer.GetCookies(response.ResponseUri);
                //得到response stream
                myResponseStream = response.GetResponseStream();
                //用StreamReader解析response stream
                sreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                //将流转化成字符串
                html = sreamReader.ReadToEnd();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return html;
        }

        /// <summary>
        /// 模拟ajax上传
        /// </summary>
        /// <param name="url">带Query String的url</param>
        /// <param name="postedFile">post图片路径</param>
        /// <param name="cc">Cookie容器</param>
        /// <returns></returns>
        public static string HttpPost(string url, string postedFile, CookieContainer cc)
        {
            string retString = string.Empty;
            StreamReader streamReader = null;
            Stream responseStream = null;

            System.Net.ServicePointManager.DefaultConnectionLimit = 50;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = cc;
            request.Referer = "http://upload.cnblogs.com/imageuploader/upload?host=www.cnblogs.com&editor=4";
            request.Accept = "*/*";
            request.Headers["Accept-Language"] = "zh-CN,zh;q=0.8,en;q=0.6,zh-TW;q=0.4";
            request.Headers["Accept-Charset"] = "gzip,deflate,sdch";
            request.UserAgent = "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36";
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.ContentType = "application/octet-stream";
            request.Method = "POST";

            //根据网站的编码自定义
            Encoding encoding = Encoding.UTF8;
            //postDataStr即为发送的数据，格式还是和上次说的一样
            byte[] postData = FileContent(postedFile);

            request.ContentLength = postData.Length;
            try
            {
                System.GC.Collect();

                //request
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(postData, 0, postData.Length);

                

                //response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();

                streamReader = new StreamReader(responseStream, encoding);

                retString = streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //关闭资源
                streamReader.Close();
                responseStream.Close();
            }

            return retString;
        }

        /// <summary>
        /// 将指定路径的文件转化为字节流
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static byte[] FileContent(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fs.Length];

            try
            {
                fs.Read(buffer, 0, (int)fs.Length);

                return buffer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs!=null)
                {
                    //关闭资源
                    fs.Close();
                }
            }
        }
    }
}
