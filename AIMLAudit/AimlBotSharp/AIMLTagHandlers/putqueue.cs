using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using AIMLbot.Utils;
namespace AIMLbot.AIMLTagHandlers
{
     public class putqueue : Utils.AIMLTagHandler
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
         public putqueue(AIMLbot.Bot bot,
                         AIMLbot.User user,
                         AIMLbot.Utils.SubQuery query,
                         AIMLbot.Request request,
                         AIMLbot.Result result,
                         XmlNode templateNode)
            : base(bot, user, query, request, result, templateNode)
        {
            this.isRecursive = true;
            //this.isBoring = true;
        }
         protected override string ProcessChange()
         {
             string msg = string.Empty;
             if (this.TemplateNodeName == "putqueue")
             {
                 if (this.TemplateNodeHasText(this, bot, templateNode))
                 {
                     // non atomic version of the node
                    // msg = this.Recurse(this, bot, templateNode);
                     msg = templateNode.InnerText;
                     string queue_name = "botqueue";
                     if (TemplateNodeAttributes["queue"] != null)
                     {
                         queue_name = TemplateNodeAttributes["queue"].Value;

                      }

                     bot.httpsqsClient.Put(queue_name, msg);
                 }
             }
             return string.Empty;
         }
    }
}
