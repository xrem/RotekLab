using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Parser
{
    public static class ScamHelper
    {
        private static Regex URIvkImages = new Regex(@"(http)(s)?(\:\/\/)(\d{1,3}\.){3}\d{1,3}\/v\d+\/.+(jpg|bmp|gif|png|jpeg|tif|tiff|svg).?" , RegexOptions.Compiled);
        private static Regex HOSTvkImages = new Regex(@"cs\d+\.vk\.(me|com)", RegexOptions.Compiled);
        private static Regex HOSTanythingFromVK_com = new Regex(@".+\.vk\.com", RegexOptions.Compiled);
        private static Regex okRu = new Regex(@"http.+\/dk\?(st\.)?cmd\=", RegexOptions.Compiled);
        private static Regex AgentMailRu = new Regex(@"(((.+\.agent)|(mra[0-9])).mail.ru)(\:\d+)?", RegexOptions.Compiled);
        private static Regex AVGIEPackageAVGTBAVG = new Regex(@"\/partners\/wtu\/.+\/.+coid=avgtbavg", RegexOptions.Compiled);
        private static Regex aliExpress = new Regex(@"\/kf\/.+\.jpg_\d+x\d+\.jpg", RegexOptions.Compiled);

        public static bool isTuneUpUtilities(string uri_orginal) {
            Regex TuneUpUtilities = new Regex(@"\/\?.+&z=[0-9a-f]{40}", RegexOptions.Compiled);
            string uri = TuneUpUtilities.Match(uri_orginal,5).Value;
            string x = "";
            if (uri.Length < 40) return false;
            foreach (char z in uri.Substring(2))
            {
                if (z == '&') break;
                x += z;
            }
            if (x.Length < 3) return false;
            try
            {
                Encoding.UTF8.GetString(Convert.FromBase64String(x));
            }
            catch (FormatException)
            {
                Console.WriteLine("<!> Regex for TuneUpUtilities should be modified");
            }
            return true;
        }

        public static string LookupForScam(WEBLogEntry x)
        {
            string ScamType;

            string uri = x.cs_uri.ToLower();

            ScamType = "Social/";
            if (URIvkImages.IsMatch(uri)) return ScamType+"VK";
            if (HOSTvkImages.IsMatch(x.r_host)) return ScamType + "VK";
            if (HOSTanythingFromVK_com.IsMatch(x.r_host)) return ScamType + "VK";
            if (x.r_host.Contains("vkontakte.ru")) return ScamType + "VK";
            if (x.r_host.Contains("vkr-server.com")) return ScamType + "VK";
            if (okRu.IsMatch(uri)) return ScamType + "OK";
            if (uri.EndsWith("/al_im.php")) return ScamType + "VK";
            if (uri.EndsWith("/feed2.php")) return ScamType + "VK";
            if (uri.Substring(uri.Length - 6).StartsWith(@"/im")) return ScamType + "VK";
            if (uri.Contains("my.mail.ru:443")) return ScamType + "MY";

            ScamType = "Erotic";
            if (x.r_host.Contains("maximonline.")) return ScamType;

            ScamType = "Dating";
            if (uri.Contains(@"/znakomstva_light/")) return ScamType;
            if (x.r_host.Contains("love.mail.ru")) return ScamType;

            ScamType = "Entertainment/";
            if (x.r_host.Contains("joyreactor.")) return ScamType + "joyreactor";
            if (x.r_host.Contains("pikabu.ru")) return ScamType + "pikabu";
            if (x.r_host.Contains("yaplakal.")) return ScamType + "yaplakal";
            if (x.r_host.Contains("yapfiles.ru")) return ScamType + "yaplakal";
            if (uri.Contains("/videoplayback?")) return ScamType + "youtube";
            if (uri == "http://144.76.130.66/ajax/messanger.php") return ScamType + "rirl.ru"; //форум историй из жизни. лол.

            ScamType = "Shopping/";
            if (x.r_host.Contains("ulmart.ru")) return ScamType + "ulmart";
            if (aliExpress.IsMatch(uri)) return ScamType + "aliexpress";
            if (uri.Contains(@"sl-love.ru")) return ScamType + "sl-love.ru";
            if (uri.Contains(@"loverepublic.ru")) return ScamType + "loverepublic.ru";
            if (uri.Contains(@"alibaba.com")) return ScamType + "aliexpress";

            ScamType = "Jobs";
            if (uri.Contains(@".hh.ru/")) return ScamType;
            if (uri.Contains(@"carrer.ru/")) return ScamType;
            if (uri.Contains(@"job.ru/")) return ScamType;
            
            ScamType = "Other/";
            if (x.r_host.Contains("eminem50cent.ru")) return ScamType + "eninem50cent.ru";
            if (x.r_host.Contains("zara.net")) return ScamType + "zara.net";
            if (x.r_host.Contains("yves-rocher")) return ScamType + "yves-rocher";
            if (x.r_host.Contains("ryanair.")) return ScamType + "ryanair";
            if (x.r_host.Contains("hitline-mebel.ru")) return ScamType + "hitline-mebel.ru";
            if (x.r_host.Contains("pandora.")) return ScamType + "pandora";
            if (x.r_host.Contains("yuliaprokhorova.com")) return ScamType + "yuliaprokhorova.com";
            if (x.r_host.Contains("bobsoccer.ru")) return ScamType + "bobsoccer.ru";

            return null;
        }

        public static bool IgnoreByURI(string uri_original)
        {
            string uri = uri_original.ToLower();
            if (uri.Contains("array.dll?get.routing.script")) return true;
            if (uri.Contains("mra.mail.ru:80")) return true; //CheckALLOW?
            if (uri.Contains("/rtl/cef.php")) return true; //W32.Meteit!inf
            if (uri.Contains("avg.com")) return true;
            if (AVGIEPackageAVGTBAVG.IsMatch(uri)) return true;
            if (AgentMailRu.IsMatch(uri)) return true; //CheckALLOW?
            if (isTuneUpUtilities(uri)) return true;
            if (uri.Contains("ecosystem/repo/readynas")) return true;
            if (uri.Contains("/8se/413?mi=")) return true; //??????
            if (uri.Contains("kh.google.com/flatfile?")) return true; //CheckALLOW?
            return false;
        }
    }
}
