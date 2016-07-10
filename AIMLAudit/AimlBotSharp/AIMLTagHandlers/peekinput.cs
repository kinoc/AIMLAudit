using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Net;

namespace AIMLbot.AIMLTagHandlers
{
    public class peekinput : AIMLbot.Utils.AIMLTagHandler
    {
        public peekinput(AIMLbot.Bot bot,
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
            if (this.TemplateNodeName == "peekinput")
            {
                return request.rawInput;
                //BehaviorContext bh = this.bot.BotBehaving;
                //return bh.lastBehaviorChatInputPeek();
            }
            return String.Empty;

        }

    }
}
