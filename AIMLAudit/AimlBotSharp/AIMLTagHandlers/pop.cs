using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using AIMLbot.Utils;
/******************************************************************************************
AltAIMLBot -- Copyright (c) 2011-2012,Kino Coursey, Daxtron Labs

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
associated documentation files (the "Software"), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute,
sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT
OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
**************************************************************************************************/

namespace AIMLbot.AIMLTagHandlers
{
    public class pop : AIMLTagHandler
    {

        public pop(Bot bot,
                User user,
                SubQuery query,
                Request request,
                Result result,
                XmlNode templateNode)
            : base(bot, user, query, request, result, templateNode)
        {
        }



        protected override string ProcessChange()
        {
            if (templateNode.Name.ToLower() == "pop")
            {
                // If there is a conversation memo then pop it
                // otherwise take the tag content as a srai (to trip say a random reply)

                try
                {
                    if (bot.conversationStack.Count > 0)
                    {
                        string converseMemo = bot.conversationStack.Pop();
                        return converseMemo;
                    }
                    else
                    {
                        string starContent = "UNKNOWNPOP";

                        if (this.templateNode.InnerText.Length > 0)
                        {
                            starContent = this.templateNode.InnerText;

                            //string starContent = Recurse();
                        }
                        //return callSRAI(starContent);
                        //return popSrai(starContent);
                        return sraiCall(starContent);
                    }

                }
                catch
                {

                }

            }
            return String.Empty;

        }


    }
}
