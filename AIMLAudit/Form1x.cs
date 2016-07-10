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

namespace VoiceInterface
{
    public partial class Form1x : Form
    {
        HttpSQSClient myClient;
        //DictationGrammar Grammar = new DictationGrammar();
        //SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        ToyBot myBot = new ToyBot();
       // SpeechRecognitionEngine Engine = new SpeechRecognitionEngine();
       // SpeechRecognitionEngine rec = new SpeechRecognitionEngine();


        bool useGoogle = false;
        bool useBing = false;
        bool useLocal = true;
        bool useLocalBot = false;
        bool useSQS = false;

        string lastVis = "VSMAA";
        Queue<string> nullVisemes = new Queue<string>();

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
        public Form1x()
        {
            InitializeComponent();
            myClient = new HttpSQSClient("localhost", "1218", "utf-8", "mypass123");
            myClient.active = false;
            /*
            synthesizer.Volume = 10;  // 0...100
            synthesizer.Rate = -2;     // -10...10
            synthesizer.SetOutputToDefaultAudioDevice();
            synthesizer.VisemeReached += new EventHandler<VisemeReachedEventArgs>(synthesizer_VisemeReached);
            synthesizer.Speak("hello.");
             */
            flushQ("speechout");
            startRecEngine();
        }
/*
        void synthesizer_VisemeReached(object sender, VisemeReachedEventArgs e)
        {
            int viseme = e.Viseme;
            

            int mouthHeight = vis2height[viseme];
            int mouthWidth = vis2width[viseme];
            float blendHeight = ((float)mouthHeight/255f) * 100;
            
            //mouthUpturn = 153;
            //jawOpen = 127;
            //teethLowerVisible = 127;
            //teethUpperVisible = 127;
            //tonguePosn = 127;
            //lipTension = 0;
             
            //throw new NotImplementedException();
            //myClient.Put("mouthheight", mouthHeight.ToString());
            //myClient.Put("mouthwidth", mouthWidth.ToString());
            //myClient.Put("blendcommand", "BottomLipDown:" + blendHeight);
            //myClient.Put("blendcommand", "UpperLipUp:" + blendHeight);
            //myClient.Put("dazcommand", "PHMMouthOpen:" + blendHeight);
 
            string newVis = vis2morph[viseme];
            if (newVis=="") 
            {
               // myClient.Put("dazcommand", lastVis+":0");
            }
            else
            {
                //myClient.Put("dazcommand", lastVis+":0");
                myClient.Put("dazcommand", newVis+":25");
                nullVisemes.Enqueue(newVis + ":0");
            }
            lastVis = newVis;
        }
*/
        private void Form1_Load(object sender, EventArgs e)
        {

        }

 
        private void button1_Click_1(object sender, EventArgs e)
        {

            startRecEngine();
        }

        void startRecEngine()
        {
            /*
            rec.LoadGrammar(new Grammar(new GrammarBuilder("exit"))); // load grammar
            // rec.GrammarBuilder.Append(string);
            rec.LoadGrammar(new DictationGrammar());

            rec.SpeechRecognized += speech1;
            rec.BabbleTimeout = TimeSpan.FromSeconds(1.0);
            rec.EndSilenceTimeout = TimeSpan.FromSeconds(1.0);
            rec.EndSilenceTimeoutAmbiguous = TimeSpan.FromSeconds(1.0);
            rec.InitialSilenceTimeout = TimeSpan.FromSeconds(1.0);
            rec.MaxAlternates = 5;

            rec.SetInputToDefaultAudioDevice(); // set input to default audio device
            rec.RecognizeAsync(RecognizeMode.Multiple); // recognize speech
             */
            textBox1.BackColor = Color.Green;
        }

        /*
        void speech1(object sender, SpeechRecognizedEventArgs e)
        {
            textBox1.BackColor = Color.Yellow;
            useGoogle = checkBoxGoogle.Checked;
            useBing = checkBoxGoogle.Checked;
            useLocal = checkBoxLocal.Checked;
            useLocalBot = localBot.Checked;
            string localInput = "";

            if (e.Result.Text == "exit")
            {
                Application.Exit();
            }
            else
            {
                dictationTimer.Enabled = false;
                dictationTimer.Interval = 500;
                textBox1.BackColor = Color.Green;
                string altlist = "\r\n";
                foreach(RecognizedPhrase p in e.Result.Alternates)
                {
                    altlist += "   "+p.Confidence +"-->"+p.Text+"\r\n";
                }
                myClient.Put("speechin.Alternate",altlist  );
                if (useGoogle)
                {
                    // Google based
                    myClient.Put("recengine", "G");
                    string googleResult = TryGoogleWithRecordedAudio(e.Result.Audio);
                    myClient.Put("speechin", googleResult);
                    richTextBox1.AppendText("User: " + googleResult + "\r\n");
                    textBox1.AppendText(googleResult + " ");
                    localInput = googleResult;
                }
                if (useBing)
                {
                    // Bing based
                    myClient.Put("recengine", "B");
                    string msResult = BingVoiceRec.TryMSWithRecordedAudio(e.Result.Audio);
                    myClient.Put("speechin", msResult);
                    richTextBox1.AppendText("User: " + msResult + "\r\n");
                    textBox1.AppendText(msResult + " ");
                    localInput = msResult;
                }
                
                if ((!useGoogle) && (!useBing))
                {
                    // Local dictation engine based
                    myClient.Put("recengine", "L");
                    myClient.Put("speechin.confidence", e.Result.Confidence.ToString());
                    if ((e.Result.Confidence < 0.5) && (e.Result.Text.Length<4))
                    {
                        myClient.Put("speechin", "unknown input");
                    }
                    else
                    {
                        myClient.Put("speechin", e.Result.Text);
                    }
                    richTextBox1.AppendText("User: " + e.Result.Text + "\r\n");
                    textBox1.AppendText(e.Result.Text + " ");
                    localInput = e.Result.Text;
                }
                dictationTimer.Enabled = true;

            }
           // rec.RecognizeAsync(RecognizeMode.Multiple); // recognize speech

        }
        */

        /*
        public  string TryGoogleWithRecordedAudio(RecognizedAudio audio)
        {
            if (audio == null)
                return "";
            MemoryStream audioStream = new MemoryStream();
            audio.WriteToWaveStream(audioStream);
            string str = "";
            try
            {
                //str = SoundRecognition.WavStreamToGoogleX(audioStream,audio.Format);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                 //myClient.Put("gooleResult", str);
                 //myClient.Put("googleConfidence", SoundRecognition.lastConfidence.ToString());
                 //myClient.Put("googleGuesses", SoundRecognition.lastGoogleSummary);
            }
            return str; 
        }
        */


        private void dictationTimer_Tick(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.Cyan;
            dictationTimer.Enabled = false;
            //var output = myBot.getOutput(textBox1.Text.Trim());
            flushQ("speechout");
            var output="";
            if (useLocalBot)
            {
                string reply = "";
                reply = myBot.getOutput(textBox1.Text.Trim());
                if (reply != "") output=reply;
            }
            else
                output = getCiproAnswer(textBox1.Text.Trim());

            textBox1.Text = "";
            
            richTextBox1.AppendText("Bot: " + output + "\r\n");
            flushQ("speechout");
            myClient.Put("speechout", output);
            serviceSpeechOut();
            textBox1.BackColor = Color.Green;
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            serviceSpeechOut();
            serviceNullVisemeQueue();
        }

        void serviceNullVisemeQueue()
        {
            while (nullVisemes.Count > 0)
            {
                string dcomd = nullVisemes.Dequeue();
                myClient.Put("dazcommand", dcomd);
            }
        }
        void serviceSpeechOut()
        {
            string text = myClient.Get("speechout");
            if (text.Contains("HTTPSQS_GET_END"))
                return;
            if (text.Contains("HTTPSQS_GET_ERR"))
                return;

            if (text.StartsWith("data="))
            {
                text = text.Replace("data=", "");
            }
            text = text.Replace("...", " ");
            if (text.Contains("<a"))
            {
                int p = text.IndexOf("<a");
                text = text.Substring(0, p - 1);
            }
            if (text.Contains("<oob>"))
            {
                int p1 = text.IndexOf("<oob>");
                int p2 = text.IndexOf("</oob>");
                int len = p2 - p1;
                string frag = text.Substring(p1, len);
                text = text.Replace(frag," ");
 
            }
            //synthesizer.SpeakAsync(text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            dictationTimer.Enabled = false;
            dictationTimer.Interval = 100;
            richTextBox1.AppendText("User: " + hearText.Text + "\r\n");
            myClient.Put("speechin", hearText.Text);
            textBox1.AppendText(hearText.Text + " ");
            dictationTimer.Enabled = true;
            textBox1.BackColor = Color.Green;
        }

        private void localBot_CheckedChanged(object sender, EventArgs e)
        {

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
        string[] userlog;
        string[] userpairs;
        string curOut = "";
        Dictionary<string,string> mergedTraces = new Dictionary<string,string> ();
        Dictionary<string, int> mergedCounts = new Dictionary<string, int>();
        private void button5_Click(object sender, EventArgs e)
        {
            userlog = File.ReadAllLines("usercount.txt");
            userpairs = new string[userlog.Length];
            File.WriteAllText("curIncrementalThr4.txt", "");
            File.WriteAllText("patternMatchCount4.txt", "");
            File.WriteAllText("patternMatchCount4ByVal.txt", "");
            File.WriteAllText("patternMatchCount4_p.txt", "");
            File.WriteAllText("patternMatchCount4ByVal_p.txt", "");
            File.WriteAllText("patternMatchRespondsCount4ByVal.txt", "");
            File.WriteAllText("patternMatchRespondsCount4ByVal_p.txt", "");
            File.WriteAllText("patternMatchCount4.txt", "");
            File.WriteAllText("normalizedMatchCount4.txt", "");
            List<Task> taskList = new List<Task>();
            int lastOut = 0;
            int maxlim =  userlog.Length;
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
                        Task.WaitAll(taskList.ToArray());
                        for (int outp = lastOut; outp < indx; outp++)
                        {
                            File.AppendAllText("curIncrementalThr4.txt", userpairs[outp]);
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
                File.AppendAllText("curIncrementalThr4.txt", userpairs[outp]);
            }
            taskList = new List<Task>();

            File.WriteAllLines("curResponsesThr4.txt", userpairs);

            List<string> tracekeys = myBot.traceCount.Keys.ToList();
            tracekeys.Sort();
            foreach (string fk in tracekeys)
            {
                string s = string.Format("{0}\t{1}\n",fk,myBot.traceCount[fk]);
                File.AppendAllText("patternMatchCount4.txt", s);
                string s1 = string.Format("{0}\t{1}\n", fk, Math.Log((double)myBot.traceCount[fk]/(double)totals));
                File.AppendAllText("patternMatchCount4_p.txt", s1);
            }
            var items = from pair in myBot.traceCount  orderby pair.Value descending select pair;
            foreach (KeyValuePair <string,int> pair in items)
            {
                string s = string.Format("{0}\t{1}\n",pair.Key,pair.Value);
                File.AppendAllText("patternMatchCount4ByVal.txt", s);
                string s2 = string.Format("{0}\t{1}\n", pair.Key, Math.Log((double)pair.Value/(double)totals));
                File.AppendAllText("patternMatchCount4ByVal_p.txt", s2);

            }

            var itemsF = from pair in myBot.traceFinalCount orderby pair.Value descending select pair;
            foreach (KeyValuePair<string, int> pair in itemsF)
            {
                string s = string.Format("{0}\t{1}\n", pair.Key, pair.Value);
                File.AppendAllText("patternMatchRespondsCount4ByVal.txt", s);
                string s2 = string.Format("{0}\t{1}\n", pair.Key, Math.Log((double)pair.Value / (double)totals));
                File.AppendAllText("patternMatchRespondsCount4ByVal_p.txt", s2);

            }

            var itemsM = from pair in mergedCounts orderby pair.Value descending select pair;
            foreach (KeyValuePair<string, int> pair in itemsM)
            {
                string s = string.Format("{0}\n", mergedTraces[pair.Key]);
                File.AppendAllText("normalizedMatchCount4.txt", s);
            }
            File.WriteAllText("totalCountThr4.txt", totals.ToString ());
        }

        public void processGenResponse(int index,string userinput,int n)
        {
            string reply = "";
            string lastCat = "";
            reply = myBot.getOutputThreaded(userinput.Trim(),n,out lastCat);
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
    }



    public class ToyBotx
    {
        //see: https://xinyustudio.wordpress.com/2014/03/08/get-started-with-aiml-c-programming-i-just-say-hello-to-the-robot/
        const string UserId = "Dude";
        public Bot AimlBot;
        public User myUser;
        public Dictionary<string, int> traceCount = new Dictionary<string, int>();
        public Dictionary<string, int> traceFinalCount = new Dictionary<string, int>();

        public ToyBotx()
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
