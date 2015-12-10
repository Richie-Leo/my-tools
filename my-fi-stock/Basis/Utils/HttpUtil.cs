using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;

namespace Pandora.Basis.Utils
{
    public class HttpUtil{
        /// <summary>
        /// 使用HTTP GET方式获取网页内容
        /// </summary>
        /// <returns>网页HTML内容</returns>
        /// <param name="url">目标URL地址</param>
        /// <param name="encoding">Request字符集编码</param>
        /// <param name="tryTimes">如遇HTTP错误，最大重试次数</param>
        /// <param name="sleepInterval">重试前的间隔时间（单位毫秒）</param>
        /// <param name="throwException">如果遇到HTTP错误是否抛出异常</param>
        public static string HttpGet(string url, string encoding, int tryTimes, int sleepInterval, bool throwException){
            int retries = 0;
            string lastMessage = null;
            Exception lastException = null;
            while (retries < tryTimes){
                retries++;
                string html = "";
                try{
                    ServicePointManager.DefaultConnectionLimit = 10;
                    //抓取html
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    req.ContentType = "text/html";
                    req.Method = "GET";
                    HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                    if(resp.StatusCode != HttpStatusCode.OK){
                        lastMessage = resp.StatusCode.ToString() + ": " + resp.StatusDescription;
                        lastException = null;
                        continue;
                    }

                    using(Stream stream = resp.GetResponseStream()){
                        using(StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(encoding))){
                            html = reader.ReadToEnd();
                        }
                    }
                    return html;
                } catch (Exception ex){
                    lastMessage = null;
                    lastException = ex;
                    Thread.Sleep(sleepInterval);
                }
            }
            if(throwException && (!string.IsNullOrEmpty(lastMessage) || lastException!=null)){
                if (string.IsNullOrEmpty(lastMessage))
                    throw lastException;
                throw new Exception(lastMessage, lastException);
            }
            return string.Empty;
        }
    }
}