using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace Parser
{

    public class UserAgentHelper
    {
        public struct WW
        {
            public string b;
            public string type;
            public WW(string a, string z)
            {
                b = a;
                type = z;
            }
        }

        public List<string> IgnoreBase = new List<string>();
        public List<WW> WarningBase = new List<WW>();
        public List<string> ExcludeXORBase = new List<string>();

        public Regex QIPClient = new Regex(@"Mozilla\/.+\(.+qip\d+\;.*\)", RegexOptions.Compiled);

        public static Dictionary<string, int> userAgents = new Dictionary<string, int>();
        public UserAgentHelper()
        {
            IgnoreBase.Add("vkapp");
            IgnoreBase.Add("instagram");
            IgnoreBase.Add("android");
            IgnoreBase.Add("mobile");
            IgnoreBase.Add("phone");
            IgnoreBase.Add("Debian APT-HTTP/1.3 ("/*тут версия*/.ToLower());
            IgnoreBase.Add("Windows-Update-Agent".ToLower());
            IgnoreBase.Add("Microsoft - CryptoAPI /".ToLower());
            IgnoreBase.Add("Microsoft-CryptoAPI/".ToLower());
            IgnoreBase.Add("Microsoft BITS/".ToLower());
            IgnoreBase.Add("nokia");
            IgnoreBase.Add("vungle");
            IgnoreBase.Add("appstore");
            IgnoreBase.Add("google-play");
            IgnoreBase.Add("itunes");
            IgnoreBase.Add("viber");
            IgnoreBase.Add("kot49h");
            IgnoreBase.Add("darwin");
            IgnoreBase.Add("davlik");
            IgnoreBase.Add("speedcam");
            IgnoreBase.Add("avgsetup");
            IgnoreBase.Add("avginet");
            IgnoreBase.Add("sputnik");
            IgnoreBase.Add("guardmailru");
            ExcludeXORBase.Add("1C+Enterprise");
            ExcludeXORBase.Add("GoogleEarth");
            ExcludeXORBase.Add("MRAICQ");
            ExcludeXORBase.Add("2gis/");
            ExcludeXORBase.Add("MailRuUpdater");
            ExcludeXORBase.Add("RemoteConfigFetcher"); //тоже агент mail.ru
            ExcludeXORBase.Add("MD_VersionCheck"); //MDeamon email client 
            IgnoreBase.Add("heartbeat-client/"); //AVG cloud
            IgnoreBase.Add("onlineverification-client/"); //AVG license check
            IgnoreBase.Add("stats-client/"); //ucf.cloud.avg.com
            //----------------
            string Type = "Torrent";
            WarningBase.Add(new WW("btwebclient", Type));
            WarningBase.Add(new WW("mediaget", Type));
            WarningBase.Add(new WW("torrent", Type));
            WarningBase.Add(new WW("ut_core", Type));
            WarningBase.Add(new WW("skype", "Chating/Skype"));
            Type = "Games";
            WarningBase.Add(new WW("gamexpservice", Type));
            WarningBase.Add(new WW("games", Type));            
            WarningBase.Add(new WW("fullstuff", "Other/FULLSTUFF"));
        }

        public string getTypeFromWarningList(string userAgent)
        {
            foreach (WW warn in WarningBase)
            {
                if (userAgent.ToLower().Contains(warn.b))
                {
                    return warn.type;
                }
            }
            return null;
        }

        public bool isInExcludeList(string userAgent)
        {
            foreach (string x in ExcludeXORBase)
            {
                if (userAgent.ToLower().Contains(x.ToLower()))
                {
                    return true;
                }
                if (QIPClient.IsMatch(userAgent)) return true;
            }
            return false;
        }

        public bool isInIgnoreList(string userAgent)
        {
            foreach (string x in IgnoreBase)
            {
                if (userAgent.ToLower().Contains(x))
                {
                    return true;
                }
            }
            return false;
        }

        public void populate(string x)
        {
            if (userAgents.Keys.Contains<string>(x))
            {
                userAgents[x]++;
            }
            else
            {
                userAgents[x] = 1;
                //if (debug) Console.WriteLine("# New UA Discovered: " + x);
            }
        }

        public void saveDistinct()
        {
            var pupua = new List<string>();
            foreach (var key in userAgents.Keys)
            {
                pupua.Add(userAgents[key].ToString() + ";" + key);
            }
            pupua.Sort();
            File.WriteAllLines(Directory.GetCurrentDirectory() + @"\distinctAgents.csv", pupua);
        }
    }
}
