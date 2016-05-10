using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Parser
{
    public static class Categorizer
    {
        public static Dictionary<string, string> KnownSites = new Dictionary<string, string>();
        public static Dictionary<string, Dictionary<string, double>> Categories = new Dictionary<string, Dictionary<string, double>>();
        private static string CaterogiesPath = Directory.GetCurrentDirectory() + @"\learningMode";
        public static List<string> stopWords = new List<string>();

        public static Dictionary<string, double> GetWeightScales(List<string> x)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            x.Sort();
            int c = x.Count;
            int i = 0;
            string lastWord = "";
            foreach (var l in x)
            {
                if (lastWord == "")
                {
                    lastWord = l;
                    i++;
                    continue;
                }
                if (lastWord != l) {
                    result.Add(lastWord, Convert.ToDouble(Decimal.Divide(i, c)));
                    i = 1;
                    lastWord = l;
                    continue;
                }
                i++;
                lastWord = l;
            }
            result.Add(lastWord, Convert.ToDouble(Decimal.Divide(i,c)));
            result = result.OrderByDescending(z => z.Value).ToDictionary(k => k.Key, k => k.Value);
            return result;
        }

        public static void Init()
        {
            stopWords = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\superStopList.txt").ToList();
            stopWords.ForEach(d => d.ToLower());
            foreach (var categoryDir in Directory.GetDirectories(CaterogiesPath))
            {
                DirectoryInfo di = new DirectoryInfo(categoryDir);
                string currentCat = di.Name;
                Categories.Add(currentCat, new Dictionary<string,double>());
                //Categories[currentCat]
                List<string> words = new List<string>();
                foreach (var file in Directory.GetFiles(CaterogiesPath + @"\" + currentCat, "*.txt").ToList<string>())
                {
                    words.InsertRange(words.Count, File.ReadLines(file));
                }
                words = WebHelper.Lemmatize(words);
                Categories[currentCat] = GetWeightScales(words);
            }
        }
    }
}
