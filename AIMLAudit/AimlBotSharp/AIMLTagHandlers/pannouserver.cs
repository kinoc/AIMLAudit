using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using AIMLbot.Utils;
//using AltAIMLParser;
using AIMLbot;
//using Sipro.Utils;
using JSON = AIMLbot.Utils.JSON;

/*
 * Uses Pannous AIML bot server for question answering.
 * Pannous returns JSON
 * https://docs.google.com/document/d/1dVG_B5Sc2x-fi1pN6iJJjfF1bJY6KEFzUqjOb8NsntI/edit?pli=1# 
 * 
 * <category>
 * <pattern>SEARCH WEATHER *</pattern>
 * <template>
 * <pannouserver url="http://botecho.pannous.com/bot1" login="test-user" key="">weather <star/></pannouserver>
 * </template>
 * </category>
 * 
 */
namespace AIMLbot.AIMLTagHandlers
{
    public class pannouserver : Utils.AIMLTagHandler
    {

        public pannouserver(AIMLbot.Bot bot,
                         AIMLbot.User user,
                         AIMLbot.Utils.SubQuery query,
                         AIMLbot.Request request,
                         AIMLbot.Result result,
                         XmlNode templateNode)
            : base(bot, user, query, request, result, templateNode)
        {
            isRecursive = true;
            //IsStarAtomically = true;
            //isBoring = true;
        }

        protected override string ProcessChange()
        {
            if (this.TemplateNodeName == "pannouserver")
            {
                string webAns = "WEBANS:Sorry, I don't understand.";
                // Simply push the filled in tag contents onto the stack
                try
                {
                    String templateNodeInnerValue = this.Recurse(this, bot, templateNode);
                    //String templateNodeInnerValue = this.templateNode.InnerText;

                    if (templateNodeInnerValue.StartsWith("WEBANS:"))
                    {
                        return templateNodeInnerValue.Replace("WEBANS:","");
                    }

                    string myUrl = TemplateNodeAttributes["url"].Value;
                    string myLogin = TemplateNodeAttributes["login"].Value;
                    string myKey = TemplateNodeAttributes["key"].Value;

                    string onFail = null;
                    try
                    {
                        if (TemplateNodeAttributes["onfail"] != null)
                            onFail = TemplateNodeAttributes["onfail"].Value;
                    }
                    catch (Exception e) { }

                    string onSuccess = null;
                    try
                    {
                        if (TemplateNodeAttributes["onsuccess"] != null)
                            onSuccess = TemplateNodeAttributes["onsuccess"].Value;
                    }
                    catch (Exception e) { }

                    string query = templateNodeInnerValue;

                    if (myUrl == null) { myUrl = bot.GlobalSettings.grabSetting("pannouserverurl"); }
                    if (myLogin == null) { myLogin = bot.GlobalSettings.grabSetting("pannouserverlogin"); }
                    if (myKey == null) { myKey = bot.GlobalSettings.grabSetting("pannouserverkey"); }

                    query = query.Replace('\n', ' ');
                    query = query.Replace('\r', ' ');
                    query = query.Trim();

                    string webAsk = myUrl + "?clientFeatures=say&locale=en_US&login=" + myLogin + "&key=" + myKey + "&input=" + query;
                    bot.logText(String.Format("WEBQUERY:{0}", webAsk));
                    WebDownload client = new WebDownload(4500);
                    //client.Timeout = 4000;
                    string response ="";
                    try
                    {
                        response = client.DownloadString(webAsk);
                    }
                    catch (Exception e)
                    {
                        // Timeout or other failure, move on.
                    }
                    
                    bot.logText(String.Format("WEBResponse:{0}", response));
                    // We want <tk:text_result>(OUR ANSWER) </tk:result>
                    string finalSay = "";
                    if (String.IsNullOrEmpty(response))
                    {
                    }
                    else
                    {
                        Hashtable top = (Hashtable)JSON.JsonDecode(response);
                        if (top.ContainsKey("output"))
                        {
                            //Hashtable info = (Hashtable)top["info"];
                            ArrayList output = (ArrayList)top["output"];
                            if (output.Count > 0)
                            {
                                Hashtable output1 = (Hashtable)output[0];
                                if (output1.ContainsKey("actions"))
                                {
                                    Hashtable actions = (Hashtable)output1["actions"];
                                    if (actions.ContainsKey("say"))
                                    {
                                        //finalSay = (string)actions["say"];
                                        finalSay = "WEBANS:";
                                        Hashtable sayTable = (Hashtable)actions["say"];
                                        foreach (string key in sayTable.Keys)
                                        {
                                            if (finalSay.Length > 0) finalSay += " ";
                                            finalSay +=  sayTable[key];
                                        }
                                    }
                                }

                            }
                        }
                    }

                    if (finalSay.Length > 1)
                    {
                        webAns = finalSay;
                        if ((webAns != null) && (webAns.Length > 0))
                        {
                            // any answer post processing ?
                        }
                        else
                        {

                            webAns = "WEBANS:Sorry, I don't understand.";
                            if (onFail != null)
                            {
                                //bot.myBehaviors.queueEvent(onFail);
                                webAns = "WEBANS:" + sraiCall(onFail);
                            }

                        }

                    }
                    else
                    {

                        webAns = "WEBANS:Sorry, I don't understand.";
                        if (onFail != null)
                        {
                            //bot.myBehaviors.queueEvent(onFail);
                            webAns = "WEBANS:"+sraiCall(onFail);
                        }

                    }
                    bot.logText(String.Format("pannouserver :" + webAns));
                    return webAns;
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERR: {0} {1}", e.Message, e.StackTrace);
                    webAns = "Processing caused the following error. " + e.Message;
                    return webAns;
                }

            }
            return String.Empty;

        }

    }

    public class WebDownloadx : WebClient
    {
        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public int Timeout { get; set; }

        public WebDownloadx() : this(4500) { }

        public WebDownloadx(int timeout)
        {
            this.Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = this.Timeout;
            }
            return request;
        }
    }
    public class WebDownload
    {
        public static CookieContainer cookieJar = new CookieContainer();

        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public int Timeout { get; set; }

        public WebDownload() : this(4500) { }

        public WebDownload(int timeout)
        {
            this.Timeout = timeout;
        }

        public bool success = false;
        public string err = "";
        public string DownloadString(string url)
        {
            success = false;
            err = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

           // IPAddress[] addresslist = Dns.GetHostAddresses(request.RequestUri.DnsSafeHost);


            request.Timeout = Timeout;
            request.ReadWriteTimeout = Timeout;
            request.Proxy = GlobalProxySelection.GetEmptyWebProxy(); //null;
            request.CookieContainer = cookieJar;
            request.ServicePoint.Expect100Continue = false;
            try
            {
                using (WebResponse response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();
                    // Do something with result
                    success = true;
                    return result;
                }
            }
            catch (Exception e)
            {
                err = e.Message;
                success = false;
                return "";
            }
        }
     }

}
