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

namespace Parser
{
    public static class LearningMode
    {
        public static void Start() {
            string learnUrl = null;
            Dictionary<string, List<String>> sts = new Dictionary<string, List<string>>();
            Console.WriteLine("type stop to stop and save report");
            while (true) {
                Console.WriteLine();
                Console.Write("URL: ");
                learnUrl = Console.ReadLine();
                if (learnUrl == "stop") break;
                try
                {
                    string wcontent = WebHelper.GetURIContent(learnUrl, "GET");
                    if (wcontent.Substring(0, 200).ToLower().Contains("<html"))
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
                                wcontent = WebHelper.GetURIContent(learnUrl, "GET", enc);
                            }
                        }
                        catch (Exception e) { Console.WriteLine("Problems with charset"); }
                        List<string> wrds = WebHelper.GetWordsFromContent(wcontent);
                        if (wrds != null)
                        {
                            Console.WriteLine("OK");
                            sts.Add(learnUrl.Replace("http://", ""), wrds);
                        }
                        else
                        {
                            Console.WriteLine("BAD");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Seems like invalid html");
                    }
                }
                catch (Exception e) { Console.WriteLine("Can't get content"); }
            }
            foreach (var s in sts.Keys)
            {
                File.WriteAllLines(Directory.GetCurrentDirectory() + @"\learningMode\" + s + ".txt", sts[s]);
            }
            Environment.Exit(0);
        }

        
    }
}
