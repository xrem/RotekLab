using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using PetaPoco;
using HtmlAgilityPack;
using Fizzler;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using LemmaSharp;

namespace Parser
{
    class Program
    {
        public static bool debug = false;
        public static bool isMultithreadingDisabled = false;
        public static bool ExcludeWarnings = false;

        public static UserAgentHelper _uaHelper = new UserAgentHelper();
        //public static List<WEBLogEntry> Warnings = new List<WEBLogEntry>();
        public static List<WEBLogEntry> Fetchd = new List<WEBLogEntry>();
        public static ulong GrandCounter = 0;
        public static List<string> logFiles = new List<string>();
        public static Multithread m;
        public static Object thisLock = new Object();
        public static ILemmatizer lemattorRu = new LemmatizerPrebuiltFull(LanguagePrebuilt.Russian);
        public static ILemmatizer lemattorEn = new LemmatizerPrebuiltFull(LanguagePrebuilt.English);

        public static class KekLoader
        {
            public static byte kek = 0;
            public static bool loaded = false;
            public static string KEK()
            {
                kek++;
                string l = "";
                if (kek == 1) l = "[      ]";
                if (kek == 2) l = "[#     ]";
                if (kek == 3) l = "[##    ]";
                if (kek == 4) l = "[ ##   ]";
                if (kek == 5) l = "[  ##  ]";
                if (kek == 6) l = "[   ## ]";
                if (kek == 7) l = "[    ##]";
                if (kek == 8) l = "[     #]";
                if (kek == 9) { l = "[      ]"; kek = 1; }
                return l;
            }
            public static void Loop(Object stateInfo)
            {
                loaded = false;
                while (!loaded)
                {
                    Console.Title = GrandCounter.ToString() + "~";
                    Console.Write("\rInit... {0}", KEK());
                    Thread.Sleep(100);
                }
            }
            public static void LoopParse(Object stateInfo)
            {
                loaded = false;
                while (!loaded)
                {
                    decimal progress = Decimal.Divide(GrandCounter, (ulong)stateInfo);
                    Console.Write("\r[{0}] processing...                   ", progress.ToString("P"));
                    Console.Title = progress.ToString("P");
                    Thread.Sleep(250);
                }
            }
            public static void ProcessFile(Object stateInfo)
            {
                string fc = (string)stateInfo;
                ulong got = (ulong)File.ReadLines(fc).Count();
                lock (thisLock)
                {
                    GrandCounter += got;
                }
            }
        }

        static void KEKWorker(object vvv)
        {
            m._ThreadPool.WaitOne();
            Thread.BeginThreadAffinity();
#pragma warning disable 618
            int osThreadId = AppDomain.GetCurrentThreadId();
#pragma warning restore  618
            ProcessThread thread = Process.GetCurrentProcess().Threads.Cast<ProcessThread>()
                               .Where(t => t.Id == osThreadId).Single();
            StringAndIntPtr x = (StringAndIntPtr)vvv;
            thread.ProcessorAffinity = x.i();
            //--
            KekLoader.ProcessFile(x.s());
            //--
            m._ThreadPool.Release();
            m.Return(x.i());
        }

        static void ParseFileWorker(object vvv)
        {
            m._ThreadPool.WaitOne();
            Thread.BeginThreadAffinity();
#pragma warning disable 618
            int osThreadId = AppDomain.GetCurrentThreadId();
#pragma warning restore  618
            ProcessThread thread = Process.GetCurrentProcess().Threads.Cast<ProcessThread>()
                               .Where(t => t.Id == osThreadId).Single();
            StringAndIntPtr x = (StringAndIntPtr)vvv;
            thread.ProcessorAffinity = x.i();
            //--
            Parse_File(x.s(), (ulong)x.i());
            //--
            m._ThreadPool.Release();
            m.Return(x.i());
        }

        static void PopulateFileWorker(object vvv)
        {
            m._ThreadPool.WaitOne();
            Thread.BeginThreadAffinity();
#pragma warning disable 618
            int osThreadId = AppDomain.GetCurrentThreadId();
#pragma warning restore  618
            ProcessThread thread = Process.GetCurrentProcess().Threads.Cast<ProcessThread>()
                               .Where(t => t.Id == osThreadId).Single();
            StringAndIntPtr x = (StringAndIntPtr)vvv;
            thread.ProcessorAffinity = x.i();
            //--
            Parse_File(x.s(), (ulong)x.i(), true);
            //--
            m._ThreadPool.Release();
            m.Return(x.i());
        }

        static void Parse_File(string logFile, ulong ttl, bool CollectOnly = false)
        {
            Random rng = RandomProvider.GetThreadRandom();
            ulong rngNum = 1;//(ulong)(1000 - rng.Next(7));
            int core = Convert.ToInt32(Math.Log(Convert.ToDouble(ttl),2.0));
            ISALog Current = null;
            dynamic _Current;
            if (isMultithreadingDisabled == true)
            {
                Console.Write("\r[!] Processing file ");
                Console.WriteLine(logFile);
            }
            else
            {
                Console.WriteLine("\r[CPU{0}] Processing file {1}",core,logFile);
            }
            var handle = new StreamReader(logFile);
            string line = null;
            bool isFWS = logFile.Contains("_FWS_");
            Decimal progress = new decimal(0.0);

            while ((line = handle.ReadLine()) != null)
            {
                GrandCounter++;
                if (isMultithreadingDisabled)
                {
                    progress = Decimal.Divide(GrandCounter, ttl);
                    //Console.Write("\r[{0}] processing... ", progress.ToString("P"));
                    Console.Title = progress.ToString("P");
                }
                if (line.StartsWith("#")) continue;
                if (isFWS)
                {
                    Current = new FWSLogEntry(line);
                    if (ExcludeWarnings)
                    {
                        break;
                    }
                }
                else
                {
                    Current = new WEBLogEntry(line);
                }
                _Current = Current;
                //Log line parsed, now go on.
                if (!ExcludeWarnings && !DeAnonymizer.Populated)
                {
                    if ((_Current.cs_username != "-") && (_Current.cs_username != "anonymous")) DeAnonymizer.Login(_Current.c_ip, _Current.cs_username, _Current.date, _Current.time);
                }
                if (CollectOnly) continue;
                if (_uaHelper.isInIgnoreList(_Current.c_agent)) continue;
                _uaHelper.populate(_Current.c_agent);
                if (Current.kind == ISALog.Kind.WEB && _Current.action == "Allowed")
                {
                    bool shouldBeAddedInWarningList = false;
                    string warnDetails = null;
                    warnDetails = _uaHelper.getTypeFromWarningList(_Current.c_agent);
                    if (warnDetails != null)
                    {
                        shouldBeAddedInWarningList = true;
                    }
                    if (!shouldBeAddedInWarningList)
                    {
                        warnDetails = ScamHelper.LookupForScam((WEBLogEntry)Current);
                        if (warnDetails != null)
                        {
                            shouldBeAddedInWarningList = true;
                        }
                    }
                    if (!ExcludeWarnings && !shouldBeAddedInWarningList && (_Current.s_operation == "GET" || _Current.s_operation == "POST") && (_Current.sc_status == "200") && Misc.isHTTPMIME(_Current.cs_mime_type))
                    {
                        try 
                        {
                            string wcontent = WebHelper.GetURIContent(_Current.cs_uri, _Current.s_operation);
                            
                            if (wcontent.Substring(0, 200).ToLower().Contains("<html>"))
                            {
                                //kind of valid html
                                try
                                {
                                    HtmlDocument _htdoc = new HtmlDocument();
                                    _htdoc.LoadHtml(wcontent.ToLower());
                                    HtmlNode charsetprobably = _htdoc.DocumentNode.SelectSingleNode("//meta[@http-equiv='content-type']");
                                    if (charsetprobably != null)
                                    {
                                        var _enc = charsetprobably.Attributes["content"].Value.Trim().Split(new string[] { "charset=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                                        string enc = _enc.Split(' ')[0];
                                        Encoding.GetEncoding(enc); //debug
                                        wcontent = WebHelper.GetURIContent(_Current.cs_uri, _Current.s_operation, enc);
                                    }
                                }
                                catch (Exception e)
                                {
                                    if (debug)
                                    {
                                        Console.WriteLine("\n ICONV CONVERT ERR \n" + e.Message + "\n");
                                    }
                                }
                                List<string> wrds = WebHelper.GetWordsFromContent(wcontent);
                                wrds = WebHelper.Lemmatize(wrds);
                            }
                        }
                        catch (Exception e)
                        {
                            if (debug)
                            {
                                Console.WriteLine("\n" + e.Message + "\n");
                            }
                        }

                    }
                    if (!ExcludeWarnings && shouldBeAddedInWarningList)
                    {
                        //new Task(() => { Reporting.Add((WEBLogEntry)Current, warnDetails); }).Start(); //fire-and-forget
                        Reporting.Add((WEBLogEntry)Current, warnDetails);
                    }
                    if (ExcludeWarnings && !shouldBeAddedInWarningList && !_uaHelper.isInExcludeList(_Current.c_agent) && !ScamHelper.IgnoreByURI(_Current.cs_uri))
                    {
                        //if ((GrandCounter % rngNum)==0) //Out of memory :)
                        Fetchd.Add((WEBLogEntry)Current);
                    }
                }
            }
            //Console.WriteLine();
            handle.Close();
            if (!isMultithreadingDisabled && debug)
            {
                Console.WriteLine("\r[Thread {0}] Released                               ", ttl, logFile);
            }
        }

        static void Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 3) Misc.Usage();
            if (args.Length == 2 && args[1] == "-debug") debug = true;
            if (args.Length == 2 && args[1] == "-learn") LearningMode.Start();
            if (args.Length == 2 && args[1] == "-xor") ExcludeWarnings = true;
            if (args.Length == 2 && args[1] == "-single") isMultithreadingDisabled = true;
            if (args.Length == 3 && (args[1] == "-single" || args[2] == "-single")) isMultithreadingDisabled = true;
            if (args.Length == 3 && (args[1] == "-debug" || args[2] == "-debug")) debug = true;
            if (args.Length == 3 && (args[1] == "-xor" || args[2] == "-xor")) ExcludeWarnings = true;

            if (Directory.Exists(args[0]))
            {
                logFiles = Directory.GetFiles(args[0], "*.w3c").ToList<string>();
            }
            else
            {
                Misc.Usage();
            }

            //---------------------
            Console.WriteLine("X-rem's W3C log parser started.");
            Categorizer.Init();
            Multithread.InitMultithreading(ref m, isMultithreadingDisabled);
            //-----------
            //var db = new PetaPoco.Database("Consorto");
            //Console.Write("Connecting to localDb... ");
            //Console.WriteLine(db.ExecuteScalar<string>("SELECT poco FROM peta"));
            ulong ttl = 0;
            if (isMultithreadingDisabled)
            {
                byte kek = 0;
                int fz = 0;
                foreach (string logFile in logFiles)
                {
                    kek++;
                    string l = "";
                    if (kek == 1) l = "\\";
                    if (kek == 2) l = "|";
                    if (kek == 3) l = "/";
                    if (kek == 4) { l = "-"; kek = 1; }
                    Console.Write("\rInit... {0}", l);
                    Console.Title = fz++.ToString() + @"/" + (logFiles.Count - 1).ToString();
                    ttl += (ulong)File.ReadLines(logFile).Count();
                }
            }
            else
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(KekLoader.Loop)); //красивая рисовашка
                foreach (string logFile in logFiles)
                {
                    while (!m.AnyThreadAvailable()) {
                        Thread.Sleep(500);
                    };
                    IntPtr aff = m.TakeOne();
                    Thread w = new Thread(new ParameterizedThreadStart(KEKWorker));
                    StringAndIntPtr n = new StringAndIntPtr(logFile, aff);
                    w.Start(n);
                }
                m.WaitAll();
                ttl = GrandCounter;
            }
            GrandCounter = 0;
            Console.WriteLine("\rInit... [OK]   ");
            ThreadPool.QueueUserWorkItem(new WaitCallback(KekLoader.LoopParse),ttl); //красивая рисовашка
            if (!ExcludeWarnings)
            {
                foreach (string logFile in logFiles)
                {
                    Parse_File(logFile, ttl, true);
                }
                m.WaitAll();
                DeAnonymizer.Populated = true;
                Console.WriteLine("\nReport for computers:");
                foreach (var ip in DeAnonymizer.Computers.Keys)
                {
                    Console.Write(ip + " : ");
                    List<string> usrs = new List<string>();
                    foreach (RotekComputerIdentity idents in DeAnonymizer.Computers[ip])
                    {
                        if (idents == null)
                        {
                            continue;
                        }
                        if (!usrs.Contains(idents.Who.Name) && (idents.Who.Name != "unidentified")) usrs.Add(idents.Who.Name);
                    }
                    foreach (var z in usrs)
                    {
                        Console.Write(z + "; ");
                    }
                    if (usrs.Count == 0)
                    {
                        Console.WriteLine("unidentified");
                    }
                    else
                    {
                        Console.WriteLine("");
                    }
                }
                //------
                Console.WriteLine("------------------");
                Misc.Pause();
            }
            else
            {
                DeAnonymizer.Populated = true;
            }
            GrandCounter = 0;
            ThreadPool.QueueUserWorkItem(new WaitCallback(KekLoader.LoopParse), ttl); //красивая рисовашка
            foreach (string logFile in logFiles)
            {
                if (isMultithreadingDisabled)
                {
                    Parse_File(logFile, ttl);
                }
                else
                {
                    while (!m.AnyThreadAvailable())
                    {
                        Thread.Sleep(500);
                    };
                    IntPtr aff = m.TakeOne();
                    Thread w = new Thread(new ParameterizedThreadStart(ParseFileWorker));
                    StringAndIntPtr n = new StringAndIntPtr(logFile, aff);
                    w.Start(n);
                }
            }
            m.WaitAll();
            if (!ExcludeWarnings)
            {
                Console.WriteLine("Saving userAgents...");
                _uaHelper.saveDistinct(); //---save userAgents to 'distinctUserAgents.csv' by popularity.
            }
            if (ExcludeWarnings)
            {
                File.WriteAllLines(Directory.GetCurrentDirectory() + @"\analyzeMe.txt", Fetchd.Where(x => x != null).Select(x => x.ConvertBack()));
                Console.WriteLine("DONE");
            }
            Console.Write("\rPreparing report...");
            GrandCounter = 0;
            File.WriteAllLines(Directory.GetCurrentDirectory() + @"\warninglist.txt", Reporting.Factory.Where(x => x != null).Select(x => x.tss()));
            //or save Report to file.
            Console.WriteLine("\rReport saved!                     ");
            Misc.Pause();
            Environment.Exit(0);
        }
    }
}
