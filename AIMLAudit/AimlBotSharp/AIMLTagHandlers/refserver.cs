using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Net;

namespace AIMLbot.AIMLTagHandlers
{
    public class refserver : AIMLbot.Utils.AIMLTagHandler
    {
        public refserver(AIMLbot.Bot bot,
                        AIMLbot.User user,
                        AIMLbot.Utils.SubQuery query,
                        AIMLbot.Request request,
                        AIMLbot.Result result,
                        XmlNode templateNode)
            : base(bot, user, query, request, result, templateNode)
        {
        }

        protected override string ProcessChange()
        {
            if (this.templateNode.Name.ToLower() == "refserver")
            {


                try
                {
                    //String templateNodeInnerValue = this.Recurse();
                    //string myUrl = TemplateNodeAttributes["url"].Value;
                    String templateNodeInnerValue = this.templateNode.InnerText ;
                    string myUrl = TemplateNodeAttributes["url"].Value;

                    if (myUrl == null) { myUrl = bot.GlobalSettings.grabSetting("refserverurl"); }


                    string query = templateNodeInnerValue;
                    //WebClient client = new WebClient();
                    WebDownload client = new WebDownload(4500);
                    string webAsk = myUrl + query;
                    string response = "";
                    try
                    {
                        response = client.DownloadString(webAsk);
                    }
                    catch (Exception e)
                    {
                        // Timeout or other failure, move on.
                    }

                    string[] responseSet = response.Split('\n');
                    string webAns = responseSet[0];
                    if (webAns.Contains("rcmd:"))
                    {
                        webAns = webAns.Substring(webAns.IndexOf("rcmd:"));
                        webAns = webAns.Replace("rcmd:", "");
                        webAns = webAns.Replace("say-", "");
                        webAns = webAns.Replace('"', ' ');
                    }
                    else
                    {
                        webAns = "Sorry, I don't understand.";
                    }
                    //webAns = "refserv sez " + webAns;
                    webAns = webAns.Replace("LSA sez", "");
                    //bot.logText(String.Format("refserver :" + webAns));
                    return webAns;
                }
                catch
                {
                }
            }
            
            return string.Empty;
        }
    }
}
