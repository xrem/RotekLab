using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    [PetaPoco.TableName("Warns")]
    [PetaPoco.PrimaryKey("Id")]
    public class Warn
    {
        public string ComputerIP { get; set; }
        public DateTime DTWhen { get; set; }
        public string Who { get; set; }
        public string Warning { get; set; }
        public Warn(WEBLogEntry x,String warning)
        {
            ComputerIP = x.c_ip;
            DTWhen = x.date + x.time;
            Who = x.cs_username;
            Warning = warning;
        }
        public string tss()
        {
            return ComputerIP + "\t" + Who + "\t" + DTWhen + "\t" + Warning;
        }
    }
    
    public static class Reporting
    {

        public static List<Warn> Factory = new List<Warn>();

        public static void Add(WEBLogEntry EntryReported, string Details)
        {
            WEBLogEntry logToWorkWith = EntryReported;
            if ((EntryReported.cs_username == "-") || (EntryReported.cs_username == "anonymous"))
            {
                DateTime _when = EntryReported.date + EntryReported.time;
                if (DeAnonymizer.getLogsForIP(EntryReported.c_ip) != null) //If got history for this PC
                {
                    long min = _when.Ticks;
                    RotekUser chngme = new RotekUser("unidentified");
                    if (!DeAnonymizer.isEmpty(EntryReported.c_ip))
                    {
                        foreach (var log in DeAnonymizer.getLogsForIP(EntryReported.c_ip))
                        {
                            if (Math.Abs(log.When.Ticks - _when.Ticks) < min)
                            {
                                min = Math.Abs(log.When.Ticks - _when.Ticks); //find most recent
                                chngme = log.Who;
                            }
                        }
                        if (min != _when.Ticks)
                        {
                            if (chngme.Name == "unidentified")
                            {
                                logToWorkWith.cs_username = chngme.Name;
                            }
                            else
                            {
                                logToWorkWith.cs_username = "ROTEK\\" + chngme.Name;
                            }
                        }
                    }
                    else
                    {
                        logToWorkWith.cs_username = chngme.Name; //no logs for pc = unidentified.
                    }
                }
            }
            if (logToWorkWith.cs_username != "unidentified")
            {
                Warn whatIwantToAdd = new Warn(logToWorkWith, Details);
               // if (Details!="Torrent")
                Factory.Add(whatIwantToAdd);
            }
        }
    }
}
