using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Fizzler;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace Parser
{
    public static class WebHelper
    {
        public static string GetURIContent(string URI, string Method, string encoding_s = null)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(URI);
                request.Timeout = 10000;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.94 Safari/537.36";
                request.Method = Method;
                string wcontent;
                Encoding encoding = null;
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    var contentType = response.ContentType;

                    if (contentType != null)
                    {
                        var match = Regex.Match(contentType, @"(?<=charset\=).*");
                        if (match.Success)
                        {
                            var encx = match.ToString();
                            if (encx.ToLower() == "utf8") encx = "UTF-8";
                            if (encx.ToLower() == "cp1251") encx = "Windows-1251";
                            encoding = Encoding.GetEncoding(encx);
                        }
                    }

                    if (encoding_s != null)
                    {
                        encoding = Encoding.GetEncoding(encoding_s);
                    }
                    else
                    {
                        encoding = encoding ?? Encoding.UTF8;
                    }

                    using (var reader = new StreamReader(stream, encoding))
                        wcontent = reader.ReadToEnd();
                }

                return wcontent;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<string> GetWordsFromContent(string wcontent, bool verbose = false)
        {
            try
            {
                HtmlDocument htdoc = new HtmlDocument();
                htdoc.LoadHtml(wcontent);
                HtmlNode WholeParsedPage = htdoc.DocumentNode;
                HtmlNode descriptionNode = WholeParsedPage.SelectSingleNode("//meta[@name='description']");
                HtmlNode keywordsNode = WholeParsedPage.SelectSingleNode("//meta[@name='keywords']");
                HtmlNode titleNode = WholeParsedPage.SelectSingleNode("//head/title");
                //TODO: parse body
                StringBuilder tmpStr = new StringBuilder();
                if (keywordsNode != null)
                {
                    tmpStr.Append(" ");
                    tmpStr.Append(keywordsNode.Attributes["content"].Value.Trim().Replace(",", " "));
                }
                if (titleNode != null)
                {
                    tmpStr.Append(" ");
                    tmpStr.Append(titleNode.InnerText.Trim());
                }
                if (descriptionNode != null)
                {
                    tmpStr.Append(" ");
                    tmpStr.Append(descriptionNode.Attributes["content"].Value.Trim());
                }
                StringBuilder sb = new StringBuilder();
                IEnumerable<HtmlNode> nodes = WholeParsedPage.Descendants().Where(n =>
                    n.NodeType == HtmlNodeType.Text &&
                    n.ParentNode.Name != "script" &&
                    n.ParentNode.Name != "style" &&
                    n.ParentNode.Name != "form" &&
                    n.ParentNode.Name != "#comment" &&
                    n.ParentNode.Name != "head" &&
                    n.ParentNode.Name != "title" &&
                    n.ParentNode.Name != "select");
                foreach (HtmlNode node in nodes)
                {
                    tmpStr.Append(" ");
                    tmpStr.Append(node.InnerText.Trim());
                    if (verbose)
                    {
                        string nn = node.ParentNode.Name;
                        if (nn != "div" && nn != "a" && nn != "span" && nn != "p" && nn != "td" && nn != "tr" && nn != "table" &&
                            nn != "#text")
                            Console.WriteLine(nn);
                    }
                }
                string _tmpStr = WebUtility.HtmlDecode(tmpStr.ToString());
                List<string> wordsFromPage = _tmpStr.Trim().Split(new char[] { ' ', '\n', '\t', '\r', ';', '.', ',', '(', ')', ':', '-' }).Where(x => x.Length > 2 && x.All(char.IsLetter)).ToList<string>();
                if (wordsFromPage.Count > 3)
                {
                    return wordsFromPage;
                }
                return null;
            }
            catch (Exception e)
            {
                if (verbose) Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
