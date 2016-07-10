﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;


namespace AIMLbot.Utils
{
    class NodeData
    {
        public string token;
        private int linksOutCount;
        private double pageRank;
        private List<int> linksIn;

        public NodeData()
        {
            linksIn = new List<int>();
        }

        public int LinksOutCount
        {
            get
            {
                return linksOutCount;
            }
            set
            {
                linksOutCount = value;
            }
        }

        public double PageRank
        {
            get
            {
                return pageRank;
            }
            set
            {
                pageRank = value;
            }
        }

        public List<int> LinksIn
        {
            get
            {
                return linksIn;
            }
        }
    }

    public class TokenRanker
    {
        Dictionary<int, NodeData> tokenData = new Dictionary<int, NodeData>();
        List<int> nodesToRank = new List<int>();
        List<string> definingSentences = new List<string>();
        Regex SentRegex = new Regex(@"(\S.+?[.!?])(?=\s+|$)");
        Regex SepRegex = new Regex(@"([a-z\)])\.([A-Za-z])");
        Regex AlphaRegex = new Regex(@"[^A-za-z0-9 \.]");
        string origText = "";

        public string stoplist = " a an the this that and of for in to at by with so it on but as uh or";
        public double changeLim = 0.000001;
        public int maxIterations = 50;
        public double lamda = 0.15;



        public double scoreText(string text)
        {
            double sum = 0;
            text = SepRegex.Replace(text, @"$1. $2");
            text = AlphaRegex.Replace(text, @" ");
            string[] tokens = text.Split(' ');
            foreach (string w in tokens)
            {
                int id = w.GetHashCode();
                double v = 0;
                if (tokenData.ContainsKey(id)) v = tokenData[id].PageRank;
                sum += v;
            }
            // Note: another idea is "centrality per token" to get an importance density
            // a short sentence with many key tems may have a lower score than 
            // a long sentence that mentions everything.
            // cpt = sum / tokens.Length;

            return sum;
        }

        // If you have external knowledge here is where you can apply it
        public void associateWords(string w1, string w2, int count)
        {
            string word1 = w1.ToLower();
            string word2 = w2.ToLower();
            if (!stoplist.Contains(word1))
            {
                if (!stoplist.Contains(word2))
                {
                    int id1 = word1.GetHashCode();
                    int id2 = word2.GetHashCode();
                    if (!tokenData.ContainsKey(id1))
                    {
                        NodeData D = new NodeData();
                        D.token = word1;
                        tokenData.Add(id1, D);
                    }
                    if (!tokenData.ContainsKey(id2))
                    {
                        NodeData D = new NodeData();
                        D.token = word2;
                        tokenData.Add(id2, D);
                    }
                    tokenData[id2].LinksIn.Add(id1);
                    tokenData[id1].LinksOutCount += count;
                    //Console.WriteLine("  assoc({0},{1})", w1, w2);
                }
            }

        }
        public string summaryByRank(int lengthLim)
        {
            Dictionary<string, double> summaryOrder = new Dictionary<string, double>();
            foreach (string sent in definingSentences)
            {
                double v = scoreText(sent);
                if (summaryOrder.ContainsKey(sent))
                {
                    summaryOrder[sent] = summaryOrder[sent] + v;
                }
                else
                {
                    summaryOrder.Add(sent, v);
                }
            }
            summaryOrder = summaryOrder.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            string summ = "";
            foreach (string s in summaryOrder.Keys)
            {
                summ += s + "\n";
                if (summ.Length >= lengthLim) break;
            }
            return summ;
        }
        public void reportRanking()
        {
            Console.WriteLine("--------------------------");
            foreach (string sent in definingSentences)
            {
                double v = scoreText(sent);
                Console.WriteLine("   Val:{0}  Sent:{1}", v.ToString("F3"), sent);
            }

            Dictionary<string, double> summaryOrder = new Dictionary<string, double>();

            double thr = 1 / (double)tokenData.Count;
            foreach (int id in tokenData.Keys)
            {
                NodeData nd = tokenData[id];
                string word = nd.token;
                double v = nd.PageRank;
                if (summaryOrder.ContainsKey(word))
                {
                    summaryOrder[word] = summaryOrder[word] + v;
                }
                else
                {
                    summaryOrder.Add(word, v);
                }
            }
            summaryOrder = summaryOrder.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            int n = 0;
            foreach (string word in summaryOrder.Keys )
            {
                n++;
                if (n > 20) break;
                Console.WriteLine("   Val:{0}  Word:{1}", summaryOrder[word].ToString("F5"), word);
            }
        }
        public string summaryByOriginalSequence(int lengthLim)
        {
            Dictionary<string, double> summaryOrder = new Dictionary<string, double>();
            foreach (string sent in definingSentences)
            {
                double v = scoreText(sent);
                if (summaryOrder.ContainsKey(sent))
                {
                    summaryOrder[sent] = summaryOrder[sent] + v;
                }
                else
                {
                    summaryOrder.Add(sent, v);
                }
            }
            summaryOrder = summaryOrder.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            // remember those that are in the "in-set"
            List<string> keepList = new List<string>();
            string summ = "";
            foreach (string s in summaryOrder.Keys)
            {
                keepList.Add(s);
                summ += s + "\n";
                if (summ.Length >= lengthLim) break;
            }
            // build the actual in-sequence summary
            summ = "";
            foreach (string s in definingSentences)
            {
                if (keepList.Contains(s))
                {
                    summ += s + "\n";
                    if (summ.Length >= lengthLim) break;
                }
            }

            return summ;
        }

        public double scoreToken(string token)
        {
            double v = 0;
            int id = token.GetHashCode();
            if (tokenData.ContainsKey(id)) v = tokenData[id].PageRank;
            return v;
        }

        public Dictionary<string, double> tokenRanks()
        {
            Dictionary<string, double> tkrs = new Dictionary<string, double>();
            foreach (int k in tokenData.Keys)
            {
                tkrs.Add(tokenData[k].token, tokenData[k].PageRank);
            }
            return tkrs;
        }

        public string createSummaryByRank(string text, int lengthLim)
        {
            defineRank(text);
            return summaryByRank(lengthLim);
        }

        public string createByOriginalSequence(string text, int lengthLim)
        {
            defineRank(text);
            return summaryByOriginalSequence(lengthLim);
        }

        public void defineRank(string text)
        {
            //Expects sentences
            // Will create a ranking after skipping stopwords
            try
            {
                text = SepRegex.Replace(text, @"$1. $2");
                List<string> sl = stoplist.ToLower().Split(' ').ToList<string>();
                origText = text.ToLower();

                // Create all the word tokens first
                foreach (Match match in SentRegex.Matches(text))
                {
                    int i = match.Index;
                    string s = match.Value;
                    definingSentences.Add(s);

                    s = AlphaRegex.Replace(s, @" ");
                    string[] tokens = s.Split(' ');
                    foreach (string t in tokens)
                    {
                        string word = t.ToLower();
                        if (sl.Contains(word)) continue;
                        int id = word.GetHashCode();
                        if (!tokenData.ContainsKey(id))
                        {
                            NodeData D = new NodeData();
                            D.token = word;
                            tokenData.Add(id, D);
                        }

                    }
                }

                // Remove stoplist words
                foreach (string s in sl)
                {
                    int id = s.GetHashCode();
                    if (tokenData.ContainsKey(id))
                    {
                        tokenData.Remove(id);
                    }
                }

                double startPageRank = 1.0 / tokenData.Count;

                /* Book -> aproaches 1 as lamda aproaches 1 */
                double PR = lamda / tokenData.Count;
                double Q = 1 - lamda;

                /* Wikipedia -> aproaches 1 as lamda aproaches 0 */
                //double PR = (1.0 - lamda) / siteNodes.Count;
                //double Q = lamda;

                /* Wikipedia -> aproaches N as lamda aproaches 0 */
                //double PR = (1.0 - lamda);
                //double Q = lamda

                //Add all the links between words in a sentence


                foreach (string s in definingSentences)
                {
                    string s2 = AlphaRegex.Replace(s, @" ");
                    string[] tokens = s2.Split(' ');

                    for (int j = 0; j < tokens.Length - 1; j++)
                    {
                        string word1 = tokens[j].ToLower();
                        if (!sl.Contains(word1))
                        {
                            // Find next non-stop-list word
                            bool ifound = false;
                            for (int k = j + 1; k < tokens.Length && !ifound; k++)
                            {
                                string word2 = tokens[k].ToLower();
                                if (!sl.Contains(word2))
                                {
                                    int id1 = word1.GetHashCode();
                                    int id2 = word2.GetHashCode();
                                    tokenData[id2].LinksIn.Add(id1);
                                    tokenData[id1].LinksOutCount++;
                                    Console.WriteLine("  assoc({0},{1})", word1, word2);
                                    ifound = true;
                                }
                            }
                        }
                    }
                }

                // Complete initialization
                foreach (int k in tokenData.Keys)
                {
                    nodesToRank.Add(k);
                    tokenData[k].PageRank = startPageRank;
                }

                // Page rank algorithm
                double MaxChange = 1;
                int iterations = 0;
                while ((MaxChange > changeLim) && (iterations < maxIterations))
                {
                    //Console.WriteLine(MaxChange);
                    MaxChange = 0;

                    foreach (int id in nodesToRank)
                    {
                        double newPageRank = 0.0;

                        // Add all the ranks of pages linking to the site
                        foreach (int pageid in tokenData[id].LinksIn)
                        {
                            newPageRank += (tokenData[pageid].PageRank / tokenData[pageid].LinksOutCount);
                        }

                        newPageRank = newPageRank * Q + PR; // Calculate new page rank

                        double difference = Math.Abs(newPageRank - tokenData[id].PageRank);
                        if (difference > MaxChange)
                        {
                            MaxChange = difference;
                        }

                        tokenData[id].PageRank = newPageRank; //set the new rank to this document
                    }
                    iterations++;
                }
                reportRanking();
            }
            catch (Exception e)
            {
                Console.WriteLine("TEXTRANK ERROR:{0} {1}", e.Message, e.StackTrace);
            }
        }

    }
}
