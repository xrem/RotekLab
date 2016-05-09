using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Parser
{
    class Misc
    {
        /// <summary>
        /// Emulates the pause command in the console by invoking a cmd-process. This method blocks the execution until the user has pressed a button.
        /// </summary>
        public static void Pause()
        {
            Console.WriteLine();
            System.Diagnostics.Process pauseProc =
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo() { FileName = "cmd", Arguments = "/C pause", UseShellExecute = false });
            pauseProc.WaitForExit();
        }

        public static void Usage()
        {
            Console.WriteLine("USAGE: parser.exe \"H:\\isa\" [-learn][-debug][-xor] [-single]");
            Environment.Exit(-1);
        }

        public static bool isHTTPMIME(string mimetype) {
            string gmime = mimetype;
            if (gmime.Contains(";")) {
                gmime = gmime.Split(';')[0];
            }
            if (gmime.Contains("/"))
            {
                string type = gmime.Split('/')[0].ToLower();
                string kind = gmime.Split('/')[1].ToLower();
                if (type == "text")
                {
                    if (kind == "html" || kind == "plain")
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public struct StringAndIntPtr
    {
        public string _str;
        public IntPtr _int;
        public string s() { return _str; }
        public IntPtr i() { return _int; }
        public StringAndIntPtr(string a, IntPtr b)
        {
            _str = a;
            _int = b;
        }
        public StringAndIntPtr(IntPtr a, string b)
        {
            _str = b;
            _int = a;
        }
    }

    public static class RandomProvider
    {
        private static int seed = Environment.TickCount;

        private static ThreadLocal<Random> randomWrapper = new ThreadLocal<Random>(() =>
            new Random(Interlocked.Increment(ref seed))
        );

        public static Random GetThreadRandom()
        {
            return randomWrapper.Value;
        }
    }


}
