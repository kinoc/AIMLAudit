using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Web;
using System.Threading;

namespace httpsqscs
{
   
    public class namedQueue
    {
        public Dictionary <string,Queue<string>> _nQueue = new Dictionary<string,Queue<string>> ();
        public Dictionary <string, int> _maxQueue = new Dictionary<string,int>();
        public Dictionary<string, List<string>> sinks = new Dictionary<string, List<string>>();
        
        public void ensureQueue(string name)
        {
            if (!_nQueue.ContainsKey(name)) 
            {
                _nQueue.Add(name, new Queue<string>());
                _maxQueue[name] = 100000;
            }
        }

        public bool addQueue(string name, string data)
        {
            ensureQueue(name);
            if (_nQueue[name].Count < _maxQueue[name])
            {
                // we have a real active queue
                _nQueue[name].Enqueue(data);
                // and subscribers
                if (sinks.ContainsKey(name))
                {
                    foreach (string sink in sinks[name])
                    {
                        addQueue(sink, data);
                    }
                }
                // sucess either way
                return true;
            }
            else
            {
                // our queue is full, but do we have downstream subscribbers?
                // one trick may be to null out the capacity of a publish queue
                // but push to the subscribers
                if (sinks.ContainsKey(name))
                {
                    foreach (string sink in sinks[name])
                    {
                        addQueue(sink, data);
                    }
                    return true;
                }

            }
            return false;
        }

        public bool addSink(string name, string source)
        {
            ensureQueue(name);
            ensureQueue(source);
            if (!sinks.ContainsKey(source))
            {
                sinks.Add(source, new List<string>());
            }
            List<string> subsList = sinks[source];
            if (subsList.Contains(name))
            {
                return true;
            }
            sinks[source].Add(name);
            return true;
        }
        public bool delSink(string name, string source)
        {
            if (!sinks.ContainsKey(source))
            {
                sinks.Add(source, new List<string>());
            }
            List<string> subsList = sinks[source];
            if (!subsList.Contains(name))
            {
                return true;
            }
            sinks[source].Remove(name);
            return true;
        }
    }

    public class httpsqsServer
    {
        // see https://code.google.com/p/httpsqs/
        // should have something for each processing type, even it its just a stub
        // instead of tokyocabinet it uses internal queues, so its like raw memcached
        //
        public static HttpListener listener = null;

        public static string startUpPath = null;
        public static Thread listenerThread = null;
        public static namedQueue ourQueue = null;
        public static string serverAuth ="mypass123";
              [ThreadStatic]
        public static HttpListenerContext tl_context;

        /// <summary>
        /// The idea of tl_serverRoot is it my be set by a http client who knows this machine by a 
        ///  public name such as http://12.1.1.12
        /// 
        /// </summary>
        private static string _t1_serverRoot;
        public static string tl_serverRoot
        {
            get
            {
                return _t1_serverRoot;
            }
            set
            {
                _t1_serverRoot = value;
            }
        }
        private static string _serverWithPort = "127.0.0.1:1218";
        public static string GlobalServerHostWithPort
        {
            get
            {
                return _serverWithPort;
            }
            set
            {
                _serverWithPort = value;
            }
        }
        public static string serverRoot
        {
            get
            {
                if (tl_serverRoot != null) return WithHttp(tl_serverRoot);
                return WithHttp(GlobalServerHostWithPort);
            }
            set { GlobalServerHostWithPort = value; }
        }


        /// <summary>
        /// WithHttp add a http:// prefix if missing
        ///       and removes a trailing slash if present to allow easier concatenation;
        /// </summary>
        /// <param name="root0"></param>
        /// <returns></returns>
        public static string WithHttp(string root0)
        {
            if (root0 == null) return null;
            var root = root0;
            if (!root.StartsWith("http://")) root = "http://" + root;
            if (root.EndsWith("/")) root = root.Substring(0, root.Length - 1);
            return root;
        }

        /// <summary>
        ///  Adds a trailing "/" if needed
        /// </summary>
        /// <param name="root0"></param>
        /// <returns></returns>
        public static string WithSlash(string root0)
        {
            if (root0.EndsWith("/")) return root0;
            return root0 + "/";
        }

        public static string GetServerRoot(string hostSuggest)
        {
            string sr = GlobalServerHostWithPort;
            sr = sr.Replace("127.0.0.1", "localhost");
            sr = sr.Replace("+:", "localhost:");
            sr = sr.Replace("*:", "localhost:");
            var ctx = tl_context;
            if (ctx != null)
            {
                var r = ctx.Request;
                if (r != null)
                {
                    var s = ctx.Request.UserHostAddress;
                    if (s != null)
                    {
                        sr = sr.Replace("localhost:" + serverPort, hostSuggest);
                        sr = sr.Replace(s, hostSuggest);
                    }
                }
            }
            return WithHttp(sr);

        }
        private static int _serverPort = 1218;

        public static int serverPort
        {
            get
            {
                return _serverPort;
            }
            set
            {
                _serverPort = value;
            }
        }

        public static bool IsMicrosoftCLR()
        {
            return (Type.GetType("Mono.Runtime") == null);
        }

        public static void beginService(namedQueue theQueue)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("***** HttpListener is not supported on this platform. *****");
                return;
            }
            ourQueue = theQueue;
            //listener.Prefixes.Add("http://192.168.2.141:8123/");
            //listener.Prefixes.Add("http://192.168.0.145:8123/");
            if (listener == null) listener = new HttpListener();
            lock (listener)
            {
                string pfadd = "";
                try
                {

                    Console.WriteLine("Listener Adding:" + serverRoot);
                    listener.Prefixes.Add(WithSlash(serverRoot));
                }
                catch (Exception e)
                {
                    Console.WriteLine("FAIL Listener Adding:" + serverRoot);
                    Console.WriteLine("" + e);
                }
                try
                {
                    if (IsMicrosoftCLR())
                    {
                        pfadd = "http://+:" + serverPort.ToString() + "/";
                    }
                    else
                    {
                        pfadd = "http://*:" + serverPort.ToString() + "/";
                    }
                    Console.WriteLine("Listener Adding:" + pfadd);
                    listener.Prefixes.Add(pfadd);
                    Console.WriteLine("Listener Added:" + pfadd);
                }
                catch (Exception e)
                {
                    Console.WriteLine("FAIL Listener Adding:" + pfadd);
                    Console.WriteLine("" + e);
                }
                try
                {
                    listener.Start();
                    listenerThread = new Thread(new ThreadStart(clientListener));
                    listenerThread.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine("FAIL listener.Start()");
                    Console.WriteLine("" + e);
                }

            }

        }

        public static void clientListener()
        {
            while (true)
            {
                try
                {
                    if (listener == null)
                    {
                        Thread.Sleep(1000);
                        Console.Error.WriteLine("clientListener : No listener Yet");
                    }
                    else
                    {
                        HttpListenerContext request = listener.GetContext();
                        ThreadPool.QueueUserWorkItem(processRequest, request);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("EXCEPTION: clientListener :" + e.Message);
                }
            }
        }

        // maybe http://localhost:8888/aiml/zenoaeronaut_resident/bstore/READ_ALICEINWONDERLAND.BTX
        public static void processRequest(object listenerContext)
        {
            try
            {
                processRequest0(listenerContext);
            }
            catch
            {

            }
        }

        public static void processRequest0(object listenerContext)
        {
            var context = (HttpListenerContext)listenerContext;
            try
            {
                tl_context = context;
                WebLinksWriter.tl_title = context.Request.Url.AbsoluteUri;
                tl_serverRoot = GetServerRoot(context.Request.UserHostName);

                switch (context.Request.HttpMethod)
                {
                    case "POST":
                        CREATE(context);
                        break;
                    case "GET":
                        READ(context);
                        break;
                    case "PUT":
                        UPDATE(context);
                        break;
                    case "DELETE":
                        DELETE(context);
                        break;
                    default:
                        ERR(context);
                        break;
                }

                //if ((context != null) && (context.Response != null))
                //    context.Response.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION WebServitor: {0}", e);
                if (context != null)
                {
                    var cr = context.Response;
                    cr.StatusCode = (int)HttpStatusCode.InternalServerError;
                    //context.Response.Close();
                    //context.Response.Abort();
                    var os = cr.OutputStream;
                    if (os != null && os.CanWrite)
                    {
                        new StreamWriter(os).WriteLine("<pre>" + e + "</pre>");
                    }
                    return;
                }

            }
            // finally
            // {
            if ((context != null) && (context.Response != null))
            {
                //Console.WriteLine("Webservitor processRequest close");
                context.Response.Close();
            }
            else
            {
                if ((context != null) && (context.Response == null)) Console.WriteLine("Webservitor processRequest (context.Response == null)");
                if (context == null) Console.WriteLine("Webservitor processRequest (context == null)");
            }
            //  }
        }
        public static void CREATE(HttpListenerContext context)
        {
            ProcessReq(context, true);

        }
        private static TextWriter HtmlStreamWriter(HttpListenerContext context)
        {
            return WebLinksWriter.HtmlStreamWriter(context);
        }

        public static void READ(HttpListenerContext context)
        {
            ProcessReq(context,false);
        }
        public static void ProcessReq(HttpListenerContext context,bool isPost)
        {
            WebLinksWriter.tl_BodyDepth = 0;
            //GET (READ from DB)
            string[] sections = context.Request.RawUrl.Split('?');
            string justURL = sections[0];
            string furi = justURL;
            string filename = Path.GetFileName(justURL);
            string path = "." + justURL;
            WebLinksWriter.tl_MultiPage = null;
            WebLinksWriter.tl_AsHTML = false;
            string name = context.Request.QueryString["name"];
            string auth = context.Request.QueryString["auth"];
            string opt = context.Request.QueryString["opt"];
            string pos = context.Request.QueryString["pos"];
            string data ;
           if (isPost)
            {
                           // var context = listener.GetContext();
                var request = context.Request;
                string text;
                using (var reader = new StreamReader(request.InputStream,
                                                     request.ContentEncoding))
                {
                    data = reader.ReadToEnd();
                }
            }
               else
            {
                    data = context.Request.QueryString["data"];
            }
           Console.WriteLine("{0} {1} :{1}", opt, name, data);
 
            string webReply = "";

            if (auth != serverAuth)
            {
                webReply="HTTPSQS_AUTH_FAILED";
            }
            else

            switch(opt.ToLower())

            {
                case "put":
                    try
                    {
                        if (ourQueue.addQueue(name,data))
                        {
                            webReply = "HTTPSQS_PUT_OK";
                        }
                        else
                        {
                            webReply = "HTTPSQS_PUT_END";
                        }
                    }
                    catch (Exception e)
                    {
                        webReply ="HTTPSQS_PUT_ERROR";
                    }
                   break;
                case "get":
                    try
                    {
                        ourQueue.ensureQueue(name);
                        if (ourQueue._nQueue[name].Count>0)
                        {
                            webReply=ourQueue._nQueue[name].Dequeue();
                        }
                        else
                        {
                            webReply ="HTTPSQS_GET_END";
                        }
                    
                    }
                    catch (Exception e)
                    {
                        webReply ="HTTPSQS_GET_END";
                    }
                    break;
                case "status":
                   try
                    {
                        ourQueue.ensureQueue(name);

                        webReply+="HTTP Simple Queue Service v1.3\n";
                        webReply+="------------------------------\n";
                        webReply += String.Format("Queue Name: {0}\n", name);
                        webReply += String.Format("Maximum number of queues: {0}\n", ourQueue._maxQueue[name]);
                        webReply += String.Format("Number of unread queue:{0}\n", ourQueue._nQueue[name].Count);

                   
                    }
                    catch (Exception e)
                    {
                        webReply ="HTTPSQS_GET_END";
                    }
                   break;
            case "status_json":
                   try
                    {
                        ourQueue.ensureQueue(name);
                        webReply+=String.Format("{\"name\": \"{0}\",",name);
                        webReply += String.Format("\"maxqueue\":{0}. ", ourQueue._maxQueue[name]);
                        webReply += String.Format("\"unread\":{0} }\n", ourQueue._nQueue[name].Count);

                   
                    }
                    catch (Exception e)
                    {
                        webReply ="HTTPSQS_GET_END";
                    }
                   break;
                case "view":
                    try
                    {
                        ourQueue.ensureQueue(name);
                        int posIndex= int.Parse(pos);
                        if (ourQueue._nQueue[name].Count<posIndex)
                        {
                            string [] queueArray = ourQueue._nQueue[name].ToArray();
                            webReply=queueArray[posIndex];
                        }
                        else
                        {
                            webReply ="HTTPSQS_GET_END";
                        }
                    
                    }
                    catch (Exception e)
                    {
                        webReply ="HTTPSQS_GET_END";
                    }
                    break;
                case "reset":
                    try
                    {
                        ourQueue.ensureQueue(name);
                        ourQueue._nQueue[name].Clear();      
                        webReply ="HTTPSQS_RESET_OK";
                     }
                    catch (Exception e)
                    {
                        webReply ="HTTPSQS_RESET_ERROR";
                    }
                    break;
                case "maxqueue":
                    try
                    {
                    ourQueue.ensureQueue(name);
                    string num = context.Request.QueryString["num"];
                    int maxq = int.Parse (num);
                    ourQueue._maxQueue[name] = maxq;
                    webReply ="HTTPSQS_MAXQUEUE_OK";
                    }
                    catch (Exception e)
                    {
                        webReply = "HTTPSQS_MAXQUEUE_CANCEL";
                    }
                    break;
                case "synctime":
                    webReply ="HTTPSQS_SYNCTIME_OK";
                    break;

                case "link":
                    try
                    {
                        string source = context.Request.QueryString["source"];
                        if (ourQueue.addSink(name, source))
                        {
                            webReply = "HTTPSQS_LINK_OK";
                        }
                        else
                        {
                            webReply = "HTTPSQS_LINK_ERR";
                        }
                    }
                    catch (Exception e)
                    {
                        webReply = "HTTPSQS_LINK_CANCEL";
                    }
                    break;
                case "unlink":
                    try
                    {
                        string source = context.Request.QueryString["source"];
                        if (ourQueue.delSink(name, source))
                        {
                            webReply = "HTTPSQS_UNLINK_OK";
                        }
                        else
                        {
                            webReply = "HTTPSQS_UNLINK_ERR";
                        }
                    }
                    catch (Exception e)
                    {
                        webReply = "HTTPSQS_UNLINK_CANCEL";
                    }
                    break;
                default:
                    webReply ="HTTPSQS_ERROR";
                    break;

            }
            WebLinksWriter.tl_title = path;

           using (var writer = HtmlStreamWriter(context))
            {
                try
                {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    writer.WriteLine(webReply);
                    writer.Close();

                }
                catch (Exception e)
                {
                    writer.WriteLine("<{0}> <hasErrorMessage> \"{1}\"", furi, e.Message);
                    writer.WriteLine("<{0}> <hasErrorStackTrace> \"{1}\"", furi,
                                     e.StackTrace.Replace('\n', ' ').Replace('\r', ' '));
                    writer.Close();
                }
            }
            return;
        }
        
        public static void UPDATE(HttpListenerContext context)
        {
        }
        public static void DELETE(HttpListenerContext context)
        {
        }
        public static void ERR(HttpListenerContext context)
        {
            byte[] msg;
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            msg = File.ReadAllBytes(startUpPath + "\\webroot\\error.html");
            context.Response.ContentLength64 = msg.Length;
            using (Stream s = context.Response.OutputStream)
                s.Write(msg, 0, msg.Length);
        }


    }

    }
    public class WebLinksWriter : TextWriter
    {
        [ThreadStatic]
        public static TextWriter tl_WarnWriter = null;
        [ThreadStatic]
        public static int tl_BodyDepth;
        [ThreadStatic]
        public static bool tl_AsHTML;
        [ThreadStatic]
        public static TextWriter tl_MultiPage;
        [ThreadStatic]
        public static string tl_title;

        private static void WriteHtmlPreBody(TextWriter writer, string title)
        {
            if (!tl_AsHTML) return;
            tl_title = title ?? tl_title;
            if (tl_BodyDepth == 0)
            {
                writer.WriteLine("<html><head><title>{0}</title></head><body>", tl_title);
            }
            tl_BodyDepth++;
        }

        private static void WriteHtmlPostBody(TextWriter writer)
        {
            if (!tl_AsHTML) return;
            tl_BodyDepth--;
            if (tl_BodyDepth == 0)
            {
                writer.WriteLine("</body></html>");
            }
        }

        public static TextWriter HtmlStreamWriter(HttpListenerContext context)
        {
            if (tl_MultiPage != null) return tl_MultiPage;
            Stream s = context.Response.OutputStream;
            TextWriter writer = new StreamWriter(s);
            if (tl_AsHTML)
            {
                var writer1 = writer;
                WriteHtmlPreBody(writer, tl_title);
                WebLinksWriter writer2 = WebLinksWriter.EnsureWriteLinksWriter(writer, null);
                writer = writer2;
                //writer2.OnClose = () =>
                //{
                //    tl_AsHTML = true;
                //    tl_BodyDepth = 1;
                //    WriteHtmlPostBody(writer1);
                //};
            }
            return AddWarnWriter(writer);
        }

        private readonly TextWriter w;
       // public Action OnClose;
        private bool selfWriting = false;
        public LinkifyArgPred LinkifyArg = LinkifyArgDefault;

        public override void Close()
        {
            if (KeepOpen) return;
            ReallyClose();
        }

        public override void Flush()
        {
            w.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            if (KeepOpen) return;
            ReallyClose();
            if (disposing)
            {
                w.Dispose();
            }
        }

        private bool IsClosed = false;
        private void ReallyClose()
        {
            RemoveWarnWriter(this);
            lock (this)
            {
                if (IsClosed) return;
                IsClosed = true;
            }
            w.Flush();
           // if (OnClose != null)
           // {
           //     try
           //     {
          //          OnClose();
          //      }
          //      catch (Exception)
          //      {
          //      }
          //  }
            w.Close();
        }

        private bool KeepOpen
        {
            get
            {
                if (tl_MultiPage != null)
                {
                    //Flush();
                    return true;
                }
                return false;
            }
        }

        static public WebLinksWriter EnsureWriteLinksWriter(TextWriter tw, LinkifyArgPred pred)
        {
            if (tw is WebLinksWriter) return (WebLinksWriter)tw;
            return new WebLinksWriter(tw) { LinkifyArg = pred ?? LinkifyArgDefault };
        }

        private WebLinksWriter(TextWriter writer)
        {
            this.w = writer;
        }


        public override void Write(char[] buffer, int index, int count)
        {
            w.Write(buffer, index, count);
        }

        public override void WriteLine(string format)
        {
            WriteLineParms("{0}", EntityFormat(format));
        }
        public override void WriteLine(object arg0)
        {
            WriteLineParms("{0}", arg0);
        }
        public override void WriteLine(string format, object arg0)
        {
            WriteLineParms(format, arg0);
        }
        public override void WriteLine(string format, object arg0, object arg1)
        {
            WriteLineParms(format, arg0, arg1);
        }
        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            WriteLineParms(format, arg0, arg1, arg2);
        }
        public override void WriteLine(string format, params object[] arg)
        {
            var args = LinkifyArgs(arg);
            WriteLineParms(format, args);
        }

        public void WriteLineParms(string format, params object[] arg)
        {
            WriteParms(format, arg);
            w.WriteLine("<br/>");
        }

        public override void Write(string format)
        {
            WriteParms(format);
        }
        public override void Write(object arg0)
        {
            WriteParms("{0}", arg0);
        }
        public override void Write(string format, object arg0)
        {
            WriteParms(format, arg0);
        }
        public override void Write(string format, object arg0, object arg1)
        {
            WriteParms(format, arg0, arg1);
        }
        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            WriteParms(format, arg0, arg1, arg2);
        }
        public override void Write(string format, params object[] arg)
        {
            var args = LinkifyArgs(arg);
            WriteParms(format, args);
        }
        public void WriteParms(string format, params object[] arg)
        {
            var args = LinkifyArgs(arg);
            selfWriting = true;
            try
            {
                if (arg.Length > 0)
                {
                    format = EntityFormat(format);
                }
                else
                {
                    string newForm;
                    if (LinkifyArg(format, out newForm))
                    {
                        format = newForm;
                    }
                }
                w.Write(format, args);
            }
            finally
            {
                selfWriting = false;
            }
        }

        static public string EntityFormat(string format)
        {
            return System.Web.HttpUtility.HtmlEncode(format);
        }


        public object[] LinkifyArgs(object[] arg)
        {
            for (int i = 0; i < arg.Length; i++)
            {
                object o = arg[i];
                string newVal;
                if (LinkifyArg(o, out newVal))
                {
                    arg[i] = newVal;
                }
            }
            return arg;
        }

        public delegate bool LinkifyArgPred(object o, out string val);

        private static bool LinkifyArgDefault(object o, out string s)
        {
            s = "" + o;
            if (o is Uri)
            {
                s = ((Uri)o).AbsoluteUri;
            }
            if (s.StartsWith("http"))
            {
                string link = s;
                if (link.ToLower().EndsWith(".btx"))
                {
                    link = link.Replace("/behavior/", "/scheduler/");
                    link = link + "?a=info";
                }
                s = string.Format("<a href='{0}'>{1}</a>", link, s);
                return true;
            }
            return false;
        }

        public override Encoding Encoding
        {
            get { return w.Encoding; }
        }

        public override int GetHashCode()
        {
            return w.GetHashCode();
        }
        public override IFormatProvider FormatProvider
        {
            get
            {
                return w.FormatProvider;
            }
        }
        public override string NewLine
        {
            get
            {
                return w.NewLine;
            }
            set
            {
                w.NewLine = value;
            }
        }

        public static TextWriter WarnWriter
        {
            get { return tl_WarnWriter; }
        }

        public static void RemoveWarnWriter(TextWriter writer)
        {
            if (tl_WarnWriter == writer)
            {
                tl_WarnWriter = null;
            }
        }

        public static TextWriter AddWarnWriter(TextWriter writer)
        {
            tl_WarnWriter = writer;
            return writer;
        }
    }
    public class NonClosingTextWriter : TextWriter
    {
        private TextWriter noClose;
        public NonClosingTextWriter(TextWriter writer)
        {
            noClose = writer;
        }

        public override Encoding Encoding
        {
            get { return noClose.Encoding; }
        }
        public override void Flush()
        {
            noClose.Flush();
        }
        public override void Close()
        {
            noClose.Flush();
        }

        public override void Write(char[] buffer, int index, int count)
        {
            noClose.Write(buffer, index, count);
        }
        public override void Write(object value)
        {
            noClose.Write(value);
        }
    }



