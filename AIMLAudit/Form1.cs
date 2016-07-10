using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using System.Speech.Recognition;
//using System.Speech.Synthesis;
//using System.Speech.AudioFormat;
using System.Threading;
using System.Threading.Tasks;

using httpsqscs;
using AIMLbot;
//using GoogleSpeechRecognizer;

namespace AIMLAudit
{
    public partial class Form1 : Form
    {
        HttpSQSClient myClient;
        ToyBot myBot ; //= new ToyBot();
        bool useGoogle = false;
        bool useBing = false;
        bool useLocal = true;
        bool useLocalBot = false;
        string lastVis = "VSMAA";
        Queue<string> nullVisemes = new Queue<string>();
        string[] userlog;
        string[] userpairs;
        string curOut = "";
        Dictionary<string, string> mergedTraces = new Dictionary<string, string>();
        Dictionary<string, int> mergedCounts = new Dictionary<string, int>();

        private static string[] vis2morph = {
                                                 "","VSMAA","VSMAA","VSMAA","VSMEH",
                                                 "VSMER","VSMIY","VSMUW","VSMOW","VSMOW",
                                                 "VSMIY","VSMIY","VSMIY","VSMIH","VSMER","VSML",
                                                 "VSMS","VSMSH","VSMTH","VSMF","VSMT",
                                                 "VSMK","VSMW"
                                             };
        private static  int[] vis2height = {
                    0,150,250,250,150,
                    150,100, 50, 150,200,
                    150,200,100, 50, 50,
                    50, 50, 20, 50, 50,
                    50, 50
                    };

        private static  int[] vis2width = {
                        117,150,80, 100,180,
                        100,200,20,150,50,
                        50, 50, 200,200,200,
                        200,200,200,200,200,
                        250,200
                        };
        int totals = 0;

        public Form1()
        {
            InitializeComponent();
            myClient = new HttpSQSClient("localhost", "1218", "utf-8", "mypass123");
            myClient.active = false;
            flushQ("speechout");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Show();
            this.Enabled = false;
            listBox1.Items.Clear();
            listBox1.Items.Add("Loading Configuration ...");
            userLogFilePath.Text = "usercount.txt";
            outputDir.Text = Path.GetDirectoryName(Application.ExecutablePath);
            this.Refresh();
            myBot = new ToyBot();
            listBox1.Items.Add("Load Complete.");
            listBox1.Items.Add("Current Directory:" + outputDir.Text);
            listBox1.Items.Add("PathToConfigFiles:" + myBot.AimlBot.PathToConfigFiles);
            listBox1.Items.Add("PathToAIML:" + myBot.AimlBot.PathToAIML);
            listBox1.Items.Add("PathToLogs:" + myBot.AimlBot.PathToLogs);
            this.Enabled = true;
        }

        private string getCiproAnswer(string input)
        {
            String result = null;
            string writequeue = "ciproinput";
            String readqueue = "ciprooutput";
            flushQ(writequeue);
            flushQ(readqueue);
            myClient.Put(writequeue, input);
            result = myClient.Get(readqueue);
            while (result.Contains("HTTPSQS_GET_END"))
            {
                try
                {
                    Thread.Sleep(100);                 //1000 milliseconds is one second.
                }
                catch
                {
                }

                result = myClient.Get(readqueue);
            }
            return result;

        }

        private void button1_Click_0(object sender, EventArgs e)
        {

        }

        public void flushQ(string queue_name)
        {
            if (!myClient.active) return; 
            string text = myClient.Get(queue_name);
            while (!text.Contains("HTTPSQS_GET_END"))
            {
                text = myClient.Get(queue_name);
            }
            return;
        }

 
        private void button4_Click(object sender, EventArgs e)
        {
            string[] userlog = File.ReadAllLines("usercount.txt");
            string[] userpairs = new string[userlog.Length];
            File.WriteAllText("curIncremental2.txt", "");
            for(int ji=0; ji<userlog.Length ; ji++)
            {
                try
                {
                    string u = userlog[ji];
                    string[] args = u.Split('\t');
                    string userinput = args[0];
                    int n = int.Parse(args[1]);
                    double log_n = Math.Log10(n);
                    int reps =1+ (int)log_n;
                    for (int x = 0; x < reps; x++)
                    {
                        string reply = "";
                        myBot.myUser.SetThat("");
                        myBot.myUser.Topic = "";

                        reply = myBot.getOutput(userinput.Trim());
                        userpairs[ji] = String.Format("{0}\t{1}\t{2}\n", userinput.Trim(), reply, args[1]);
                        File.AppendAllText("curIncremental2.txt", userpairs[ji]);
                    }
                }
                catch
                {
                }
            }
            File.WriteAllLines("curResponses2.txt", userpairs);
        }

        private void threadedSurvey_Click(object sender, EventArgs e)
        {
            string outdir = outputDir.Text;
            if (!outdir.EndsWith(@"\")) outdir += @"\";
            userlog = File.ReadAllLines(userLogFilePath.Text);
            userpairs = new string[userlog.Length];

            reportProgress("---- processing ----");
            reportProgress("outdir =" + outdir);
            reportProgress("userlog.Length =" + userlog.Length.ToString());

            Directory.CreateDirectory(outdir);
            File.WriteAllText(outdir + "curIncrementalThr.txt", "");
            File.WriteAllText(outdir + "patternMatchCount.txt", "");
            File.WriteAllText(outdir + "patternMatchCountByVal.txt", "");
            File.WriteAllText(outdir + "patternMatchCount_p.txt", "");
            File.WriteAllText(outdir + "patternMatchCountByVal_p.txt", "");
            File.WriteAllText(outdir + "patternMatchRespondsCountByVal.txt", "");
            File.WriteAllText(outdir + "patternMatchRespondsCountByVal_p.txt", "");
            File.WriteAllText(outdir + "patternMatchCount.txt", "");
            File.WriteAllText(outdir + "normalizedMatchCount.txt", "");

            List<Task> taskList = new List<Task>();
            int lastOut = 0;
            int maxlim = userlog.Length;
            //for (int indx = 0; indx < userlog.Length; indx++)
            totals = 0;
            for (int indx = 0; indx < maxlim; indx++)
            {
                try
                {

                    curOut = "";
                    string u = userlog[indx];
                    string[] args = u.Split('\t');
                    string userinput = args[0];
                    int n = int.Parse(args[1]);
                    totals += n;
                }
                catch
                {
                }
            }
            for (int indx = 0; indx < maxlim; indx++)
               {
                   if ((indx % 2000) == 0)
                   {
                       reportProgress(indx + " of " + maxlim + " completed.");
                   }
                try
                {
                    
                    curOut = "";
                    string u = userlog[indx];
                    string[] args = u.Split('\t');
                    string userinput = args[0];
                    int n = int.Parse(args[1]);
                    //totals += n;
                    double log_n = Math.Log10(n);
                    int reps = 2 + (int)log_n;
                    userpairs[indx] = "";
                    for (int x = 0; x < reps; x++)
                    {
                        //string taskin = userinput;
                        string taskin = quickNormalize(userinput);
                        int taskIndx = indx;
                        int taskN = n/reps;
                        if (taskN < 1) taskN = 1;
                        Task t = Task.Factory.StartNew(() => processGenResponse(taskIndx, taskin, taskN));
                        taskList.Add(t);
                    }
                    if (taskList.Count > 256)
                    {
                        Application.DoEvents();
                        Task.WaitAll(taskList.ToArray());
                        for (int outp = lastOut; outp < indx; outp++)
                        {
                            File.AppendAllText(outdir + "curIncrementalThr.txt", userpairs[outp]);
                        }
                        lastOut = indx;
                        taskList = new List<Task>();
                        curOut = "";
                    }
                }
                catch
                {
                }
            }
            // FinishUp
            Task.WaitAll(taskList.ToArray());

            for (int outp = lastOut; outp < maxlim; outp++)
            {
                File.AppendAllText(outdir + "curIncrementalThr.txt", userpairs[outp]);
            }
            taskList = new List<Task>();

            reportProgress("AIML trace complete. Generating Logs.");

            File.WriteAllLines(outdir + "curResponsesThr.txt", userpairs);

            reportProgress("Generating PatternMatchCount.");
            List<string> tracekeys = myBot.traceCount.Keys.ToList();
            tracekeys.Sort();
            foreach (string fk in tracekeys)
            {
                string s = string.Format("{0}\t{1}\n",fk,myBot.traceCount[fk]);
                File.AppendAllText(outdir + "patternMatchCount.txt", s);
                string s1 = string.Format("{0}\t{1}\n", fk, Math.Log((double)myBot.traceCount[fk]/(double)totals));
                File.AppendAllText(outdir + "patternMatchCount_p.txt", s1);
            }

            var items = from pair in myBot.traceCount  orderby pair.Value descending select pair;
            foreach (KeyValuePair <string,int> pair in items)
            {
                string s = string.Format("{0}\t{1}\n",pair.Key,pair.Value);
                File.AppendAllText(outdir + "patternMatchCountByVal.txt", s);
                string s2 = string.Format("{0}\t{1}\n", pair.Key, Math.Log((double)pair.Value/(double)totals));
                File.AppendAllText(outdir + "patternMatchCountByVal_p.txt", s2);

            }

            reportProgress("Generating patternMatchRespondsCount.");
            var itemsF = from pair in myBot.traceFinalCount orderby pair.Value descending select pair;
            foreach (KeyValuePair<string, int> pair in itemsF)
            {
                string s = string.Format("{0}\t{1}\n", pair.Key, pair.Value);
                File.AppendAllText(outdir + "patternMatchRespondsCountByVal.txt", s);
                string s2 = string.Format("{0}\t{1}\n", pair.Key, Math.Log((double)pair.Value / (double)totals));
                File.AppendAllText(outdir + "patternMatchRespondsCountByVal_p.txt", s2);

            }

            reportProgress("Generating normalizedMatchCount.");
            var itemsM = from pair in mergedCounts orderby pair.Value descending select pair;
            foreach (KeyValuePair<string, int> pair in itemsM)
            {
                string s = string.Format("{0}\n", mergedTraces[pair.Key]);
                File.AppendAllText(outdir + "normalizedMatchCount.txt", s);
            }
            File.WriteAllText(outdir + "totalCountThr.txt", totals.ToString());
            reportProgress("Done.");
            reportProgress("---- Audit Complete ----");
        }

        void reportProgress(string msg)
        {
            listBox1.Items.Add(msg);
            listBox1.Refresh();
        }

        public void processGenResponse(int index,string userinput,int n)
        {
            string reply = "";
            string lastCat = "";
            reply = myBot.getOutputThreaded(userinput.Trim(),n,out lastCat);
            // Fix any subsitution problems
            reply = reply.Replace("Xyou", "you");
            reply = reply.Replace("XPhil", "Sopheeyah");
            string respRec= String.Format("{0}\t{1}\t{2}\t{3}\n", userinput.Trim(), lastCat,reply,n);
            lock (userpairs[index])
            {
                userpairs[index] += respRec;
            }
            lock(mergedTraces)
            {
               if (mergedTraces.ContainsKey(userinput))
                    mergedTraces[userinput] += respRec;
                else
                    mergedTraces[userinput] = respRec;

                if (mergedCounts.ContainsKey(userinput))
                    mergedCounts[userinput] += n;
                else
                    mergedCounts[userinput] = n;
            }
            //lock (myBot)
            //{
            //    File.AppendAllText("curIncrementalThr.txt", respRec);
            //}

        }
        public string quickNormalize(string input)
        {
            string output = input.ToLower();
            output = output.Replace("?", " ");
            output = output.Replace(".", " ");
            output = output.Replace(",", " ");
            output = output.Replace("!", " ");
            output = output.Replace(":", " ");
            while (output.Contains("  ")) output = output.Replace("  ", " ");
            return output.Trim();
        }

        private void OpenUserLog_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "TXT files|*.txt";
            theDialog.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            theDialog.FileName = Path.GetFullPath(userLogFilePath.Text);
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    userLogFilePath.Text = theDialog.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void OpenOutputDir_Click(object sender, EventArgs e)
        {
            if (!outputDir.Text.EndsWith(@"\")) outputDir.Text += @"\";
            FolderBrowserDialog theDialog = new FolderBrowserDialog();
            theDialog.Description = "Output Directory";
            theDialog.SelectedPath = Path.GetDirectoryName(outputDir.Text);
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    outputDir.Text = theDialog.SelectedPath;
                    if (!outputDir.Text.EndsWith(@"\")) outputDir.Text += @"\";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }





    }



    public class ToyBot
    {
        //see: https://xinyustudio.wordpress.com/2014/03/08/get-started-with-aiml-c-programming-i-just-say-hello-to-the-robot/
        const string UserId = "Dude";
        public Bot AimlBot;
        public User myUser;
        public Dictionary<string, int> traceCount = new Dictionary<string, int>();
        public Dictionary<string, int> traceFinalCount = new Dictionary<string, int>();

        public ToyBot()
        {
            AimlBot = new Bot();
            myUser = new User(UserId, AimlBot);
            Initialize();
        }

        // Loads all the AIML files in the \AIML folder         
        public void Initialize()
        {
            AimlBot.loadSettings();
            AimlBot.isAcceptingUserInput = false;
            AimlBot.loadAIMLFromFiles();
            AimlBot.isAcceptingUserInput = true;
        }

        // Given an input string, finds a response using AIMLbot lib
        public String getOutput(String input)
        {
            Request r = new Request(input, myUser, AimlBot);
            Result res = AimlBot.Chat(r);
            return (res.Output);
        }
        public String getOutputThreaded(String input,int N,out string lastCat)
        {
            User threadedUser = new User(UserId, AimlBot);
            threadedUser.SetThat("");
            threadedUser.Topic = "";

            Request r = new Request(input, threadedUser, AimlBot);
            Result res = AimlBot.Chat(r);
            string[] trace = r.responseTrace.Split('\n');
            string lastt = "";
            lock (traceCount)
            {
                foreach (string t in trace)
                {
                    if (t.Length == 0) continue;
                    if (!traceCount.ContainsKey(t)) traceCount[t] = 0;
                    traceCount[t] += N;
                    lastt = t;
                }
                if (!traceFinalCount.ContainsKey(lastt)) traceFinalCount[lastt] = 0;
                traceFinalCount[lastt] += N;
            }
            lastCat = lastt;
            return (res.Output);
        }
        
    }
}
