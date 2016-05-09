using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Parser
{
    public class Multithread
    {
        public Semaphore _ThreadPool;
        private Dictionary<IntPtr, bool> _ProcAff;
        public Multithread(int AvailableThreads, Dictionary<IntPtr, bool> ProcAff)
        {
            _ThreadPool = new Semaphore(AvailableThreads, AvailableThreads);
            _ProcAff = new Dictionary<IntPtr, bool>(ProcAff);
        }
        public int Count
        {
            get { return _ProcAff.Keys.Count; }
        }
        public bool AnyThreadAvailable()
        {
            bool available = false;
            foreach (var x in _ProcAff.Keys)
            {
                if (_ProcAff[x] == true) return true;
            }
            return available;
        }
        public IntPtr TakeOne()
        {
            IntPtr procAff = new IntPtr();
            bool gotOne = false;
            foreach (var x in _ProcAff.Keys)
            {
                if (_ProcAff[x] == true)
                {
                    _ProcAff[x] = false;
                    procAff = x;
                    gotOne = true;
                    break;
                }
            }
            if (!gotOne)
            {
                throw new Exception("Multithreading logic error");
            }
            return procAff;
        }
        public void Return(IntPtr z)
        {
            if (_ProcAff.Keys.Contains(z)) _ProcAff[z] = true;
        }
        public void WaitAll()
        {
            int released = 0;
            while (released < Count)
            {
                released = 0;
                foreach (var x in _ProcAff.Keys)
                {
                    if (_ProcAff[x] == true) released++;
                }
                Thread.Sleep(100);
            }
            Program.KekLoader.loaded = true;
            Thread.Sleep(250);
            for (int i = 0; i < Count; i++)
            {
                _ThreadPool.WaitOne();
            }
            _ThreadPool.Release(Count);
        }

        public static void InitMultithreading(ref Multithread m, bool MultithreadingDisabled = false)
        {
            int affMask = Process.GetCurrentProcess().ProcessorAffinity.ToInt32();
            int CoProcessorsAvailable = Convert.ToInt32(Math.Ceiling(Math.Log(Convert.ToDouble(affMask), 2.0)));
            if (MultithreadingDisabled)
            {
                CoProcessorsAvailable = 1;
                Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1);
            }
            else
            {
                if (CoProcessorsAvailable <= 1)
                {
                    CoProcessorsAvailable = 1;
                }
                else
                {
                    CoProcessorsAvailable--;
                    string coprocmask;
                    if (CoProcessorsAvailable > 3)
                    {
                        coprocmask = "1"; //should be 0
                    }
                    else
                    {
                        coprocmask = "1";
                    }
                    for (int i = 1; i < CoProcessorsAvailable; i++) coprocmask += "1";
                    coprocmask += "1"; //should be 0
                    Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(Convert.ToInt32(coprocmask, 2));
                    CoProcessorsAvailable = coprocmask.Count(x => x == '1');
                    Dictionary<IntPtr, bool> avac = new Dictionary<IntPtr, bool>();
                    string tmpstr = coprocmask.Replace('1', '0');
                    for (int i = 0; i < coprocmask.Length; i++)
                    {
                        if (coprocmask[i] == '1')
                        {
                            string x = "";
                            for (int z = 0; z < tmpstr.Length; z++)
                            {
                                if (z == i)
                                {
                                    x += '1';
                                }
                                else
                                {
                                    x += '0';
                                }
                            }
                            IntPtr aaaa = new IntPtr(Convert.ToInt32(x, 2));
                            avac[aaaa] = true;
                        }
                    }
                    m = new Multithread(CoProcessorsAvailable, avac);
                }
            }
            if (m == null)
            {
                MultithreadingDisabled = true;
            }
            if (!MultithreadingDisabled)
            {
                Console.WriteLine("Thread Workers set to {0}", m.Count);
            }
            else
            {
                Console.WriteLine("Thread Workers set to {0}", CoProcessorsAvailable);
            }
        }

    }

    
}
