using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Parser
{
    public class RotekUser
    {
        public bool Deleted { get; set; }
        public string Name { get; set; }
        public RotekUser() { }
        public RotekUser(string ServiceName) {
            Name = ServiceName;
        }
    }

    /*public class RotekComputer
    {
        public string IP { get; set; }
        List<RotekComputerIdentity> Enrties;
    }*/

    public class RotekComputerIdentity
    {
        public RotekUser Who { get; set; }
        public DateTime When { get; set; }
        public static RotekUser FindUser(string user)
        {
            foreach (RotekUser x in DeAnonymizer.getAllUsers())
            {
                if (x.Name == user) return x;
            }
            return new RotekUser("unidentified");
        }
        public RotekComputerIdentity(string User, DateTime Date, TimeSpan Time)
        {
            When = Date + Time;
            Who = FindUser(User);
        }
    }

    public static class DeAnonymizer {

        public static Dictionary<string, List<RotekUser>> Groups = new Dictionary<string, List<RotekUser>>();
        public static Dictionary<string /* Computed IP */, List<RotekComputerIdentity>> Computers = new Dictionary<string, List<RotekComputerIdentity>>();
        public static bool Populated = false;


        static DeAnonymizer()
        {
            var groupFilesToParse = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\data\", "*.txt").ToList<string>();
            foreach (string groupFile in groupFilesToParse)
            {
                string group = Path.GetFileNameWithoutExtension(groupFile).Substring(1);
                Groups[group] = new List<RotekUser>();
                foreach (string entry in File.ReadAllLines(groupFile).ToList<string>())
                {
                    var x = entry.Replace("\"", "").Split(',').ToList<string>();
                    if (x.Count < 2) continue;
                    RotekUser tmpUser = new RotekUser();
                    foreach (var kv in x)
                    {
                        string key = kv.Split('=')[0];
                        string value = kv.Split('=')[1];
                        if (key == "CN") tmpUser.Name = value;
                        if ((key == "OU") && (value.ToLower() == "deleted users")) tmpUser.Deleted = true;
                    }
                    Groups[group].Add(tmpUser);
                }
            }
            Groups["_service"] = new List<RotekUser>();
            Groups["_service"].Add(new RotekUser("anonymous"));
            Groups["_service"].Add(new RotekUser("unidentified"));
        } //constructor

        public static List<RotekUser> getAllUsers() {
            var retMe = new List<RotekUser>();
            foreach(string key in Groups.Keys) {
                foreach(var usr in Groups[key]) {
                    retMe.Add(usr);
                }
            }
            return retMe;
        }

        public static List<RotekComputerIdentity> getLogsForIP(string IP)
        {
            if (Computers.Keys.Contains(IP))
            {
                if (Computers[IP].Count > 0)
                {
                    return Computers[IP];
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public static bool isEmpty(string IP)
        {
            if (Computers.Keys.Contains(IP))
            {
                if (Computers[IP].Count > 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static void Login(string Computer_IP, string User, DateTime DateWhen, TimeSpan TimeWhen) {
            if (Computer_IP == "-") return;
            string UserWithoutDomain = User.Replace("ROTEK\\","");
            if (!Computers.Keys.Contains(Computer_IP))
            {
                lock (Parser.Program.thisLock)
                {
                    Computers[Computer_IP] = new List<RotekComputerIdentity>();
                }
            }
            lock (Parser.Program.thisLock)
            {
                Computers[Computer_IP].Add(new RotekComputerIdentity(UserWithoutDomain, DateWhen, TimeWhen));
            }
        }
    }    


}
