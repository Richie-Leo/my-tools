using System;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;

namespace Pandora.Basis.Utils
{
    public class HttpUtil{
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(HttpUtil));

        private static bool INITIALIZED = false;
        private static long EXEC_COUNT = 0;

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
            if (!INITIALIZED){
                ServicePointManager.DefaultConnectionLimit = 1500;
                ServicePointManager.UseNagleAlgorithm = false;
                ServicePointManager.Expect100Continue = false;
                INITIALIZED = true;
            }

            EXEC_COUNT++;
            if (EXEC_COUNT % 100 == 0){
                System.GC.Collect();
                log.Info("GC...");
            }
            
            int retries = 0;
            string lastMessage = null;
            Exception lastException = null;
            while (retries < tryTimes){
                retries++;
                string html = "";
                HttpWebRequest req = null;
                HttpWebResponse resp = null;
                try{
                    //抓取html
                    req = (HttpWebRequest)WebRequest.Create(url);
                    req.Method = "GET";
                    req.Timeout = 1500;
                    req.ReadWriteTimeout = 1500;
                    req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
                    req.Proxy = null;
                    req.AllowWriteStreamBuffering = false;
                    //req.KeepAlive = true;
                    //req.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                    //req.Accept = "text/html,application/xhtml+xml,application/xml";
                    //req.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.85 Safari/537.36";
                    using(resp = req.GetResponse() as HttpWebResponse){
                        if(resp.StatusCode != HttpStatusCode.OK){
                            lastMessage = resp.StatusCode.ToString() + ": " + resp.StatusDescription;
                            lastException = null;
                            req.Abort();
                            continue;
                        }

                        Stream stream = null;
                        if(resp.ContentEncoding.ToLower().Contains("gzip"))
                            stream = new GZipStream(resp.GetResponseStream(), CompressionMode.Decompress);
                        else
                            stream = resp.GetResponseStream();
                        using(stream){
                            using(StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(encoding))){
                                html = reader.ReadToEnd();
                            }
                        }
                        req.Abort();
                        return html;
                    }
                } catch (Exception ex){
                    lastMessage = null;
                    lastException = ex;
                    if (req != null){
                        try{ req.Abort(); } catch { }
                    }
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