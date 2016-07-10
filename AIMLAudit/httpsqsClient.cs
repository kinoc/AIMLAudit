using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;


namespace httpsqscs
{
    public class HttpSQSClient
    {
        // see http://www.cnblogs.com/jenqz/archive/2012/12/14/2818481.html

        private String server, port, charset,auth;
        public bool active = true;

        public HttpSQSClient(String server, String port, String charset,String auth)
        {
            this.server = server;
            this.port = port;
            this.charset = charset;
            this.auth = auth;
        }

        private string DoProcess(String urlstr)
        {
            string result = String.Empty;

            Uri url = null;
            try
            {
                url = new Uri(urlstr);
            }
            catch (Exception e)
            {
                return "The httpsqs server must be error";
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "get";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                System.Text.Encoding encode;
                try
                {
                    encode = System.Text.Encoding.GetEncoding(response.CharacterSet);
                }
                catch
                {
                    encode = System.Text.Encoding.Default;
                }

                System.IO.Stream stream = response.GetResponseStream();
                System.IO.StreamReader read = new System.IO.StreamReader(stream, encode);
                result = read.ReadToEnd();

                response.Close();
                result = HttpUtility.UrlDecode(result);
                return result;
            }
            catch (IOException ex)
            {
                //return "Get data error";
                return "HTTPSQS_GET_ERR";
            }
            catch (Exception ex)
            {
                return "HTTPSQS_GET_ERR";
            }
        }

        public String MaxQueue(String queue_name, String num)
        {
            String urlstr = "http://" + this.server + ":" + this.port + "/?name="
                    + queue_name + "&opt=maxqueue&num=" + num + "&auth="+this.auth;

            String result = null;

            result = this.DoProcess(urlstr);
            return result;
        }

        public String Link(String queue_name, String source_queue)
        {
            String urlstr = "http://" + this.server + ":" + this.port + "/?name="
                    + queue_name + "&opt=link&source=" + source_queue + "&auth=" + this.auth;

            String result = null;

            result = this.DoProcess(urlstr);
            return result;
        }

        public String Unlink(String queue_name, String source_queue)
        {
            String urlstr = "http://" + this.server + ":" + this.port + "/?name="
                    + queue_name + "&opt=unlink&source=" + source_queue + "&auth=" + this.auth;

            String result = null;

            result = this.DoProcess(urlstr);
            return result;
        }

        public String NullCapacity(String queue_name)
        {
            String result = MaxQueue(queue_name, "-1");
            return result;
        }

        public String Reset(String queue_name)
        {
            String urlstr = "http://" + this.server + ":" + this.port + "/?name="
                    + queue_name + "&opt=reset" + "&auth=" + this.auth;
            String result = null;

            result = this.DoProcess(urlstr);
            return result;
        }

        public String View(String queue_name, String pos)
        {
            String urlstr = "http://" + this.server + ":" + this.port
                    + "/?charset=" + this.charset + "&name=" + queue_name
                    + "&opt=view&pos=" + pos + "&auth=" + this.auth;
            String result = null;

            result = this.DoProcess(urlstr);
            return result;
        }

        public String Status(String queue_name)
        {
            String urlstr = "http://" + this.server + ":" + this.port + "/?name="
                    + queue_name + "&opt=status" + "&auth=" + this.auth;
            String result = null;

            result = this.DoProcess(urlstr);
            return result;
        }

        public String Get(String queue_name)
        {
            if (!active) return "HTTPSQS_GET_END";

            String urlstr = "http://" + this.server + ":" + this.port
                    + "/?charset=" + this.charset + "&name=" + queue_name
                    + "&opt=get" + "&auth=" + this.auth;
            String result = null;

            result = this.DoProcess(urlstr);
            return result;
        }

        public String Put(String queue_name, String data)
        {
            if (!active) return String.Empty;
            String ret = String.Empty;
            String urlstr = "http://" + this.server + ":" + this.port + "/?name="
                    + queue_name + "&opt=put" + "&auth=" + this.auth;
            Uri url = null;
            try
            {
                url = new Uri(urlstr);
            }
            catch (Exception)
            {
                return "The httpsqs server must be error";
            }

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "post";
            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = requestBytes.Length;
            request.Timeout = 6000;
            try
            {
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);
                ret = reader.ReadToEnd();
            }
            catch (Exception)
            {
                return "Put data error";
            }

            return ret;
        }
    }
}
