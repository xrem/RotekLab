using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public class ISALog
    {
        public enum Kind { WEB, FWS };
        public Kind kind;
    }

    [PetaPoco.TableName("Logs")]
    [PetaPoco.PrimaryKey("Id")]
    public class WEBLogEntry : ISALog
    {
        public string c_ip { get; set; }                //Client IP
        public string cs_username { get; set; }         //Client Username
        public string c_agent { get; set; }             //Client Agent
        public bool sc_authenticated { get; set; }    //Authenticated Client
        public DateTime date { get; set; }                //Log Date
        public TimeSpan time { get; set; }                //Log Time
        public string s_svcname { get; set; }           //Service
        public string s_computername { get; set; }      //Server Name
        public string cs_referred { get; set; }         //Referring Server
        public string r_host { get; set; }              //Destination Host Name
        public string r_ip { get; set; }                //Destination IP
        public int r_port { get; set; }              //Destination Port
        public int time_taken { get; set; }          //Processing Time
        public int cs_bytes { get; set; }            //Bytes Received
        public int sc_bytes { get; set; }            //Bytes Sent
        public string cs_protocol { get; set; }         //Protocol
        public string cs_transport { get; set; }        //Transport
        public string s_operation { get; set; }         //HTTP Method
        public string cs_uri { get; set; }              //URL
        public string cs_mime_type { get; set; }        //MIME Type
        public string s_object_source { get; set; }     //Object Source
        public string sc_status { get; set; }           //Result Code
        public string s_cache_info { get; set; }        //Cache Info
        public string rule { get; set; }                //Rule
        public string FilterInfo { get; set; }          //Filter Information
        public string cs_Network { get; set; }          //Source Network
        public string sc_Network { get; set; }          //Destination Network
        public string error_info { get; set; }          //Error info (ErrorInfo)
        public string action { get; set; }              //Type of action that can be performed by the Microsoft Firewall service for a connection or a session.
        public WEBLogEntry() { kind = Kind.WEB; }
        public WEBLogEntry(string Log)
        {
            var entries = Log.Split('\t').ToList<string>();
            if (entries.Count != 29)
            {
                throw new Exception("Invalid WEB-W3C log file passed");
            }
            kind = Kind.WEB;
            c_ip = entries[0].Trim();
            cs_username = entries[1].Trim();
            c_agent = entries[2].Trim();
            sc_authenticated = entries[3].Trim().ToUpper()=="Y" ? true : false;
            var date2parse = entries[4].Trim().Split('-').ToArray<string>();
            date = new DateTime(Int32.Parse(date2parse[0].ToString()), Int32.Parse(date2parse[1].ToString()), Int32.Parse(date2parse[2].ToString()));
            var time2parse = entries[5].Trim().Split(':').ToArray<string>();
            time = new TimeSpan(Int32.Parse(time2parse[0].ToString()), Int32.Parse(time2parse[1].ToString()), Int32.Parse(time2parse[2].ToString()));
            s_svcname = entries[6].Trim();
            s_computername = entries[7].Trim();
            cs_referred = entries[8].Trim();
            r_host = entries[9].Trim();
            r_ip = entries[10].Trim();
            r_port = entries[11].Trim() == "-" ? 0 : Int32.Parse(entries[11].Trim());
            time_taken = entries[12].Trim() == "-" ? 0 :  Int32.Parse(entries[12].Trim());
            cs_bytes = entries[13].Trim() == "-" ? 0 : Int32.Parse(entries[13].Trim());
            sc_bytes = entries[14].Trim()=="-" ? 0 : Int32.Parse(entries[14].Trim());
            cs_protocol = entries[15].Trim();
            cs_transport = entries[16].Trim();
            s_operation = entries[17].Trim();
            cs_uri = entries[18].Trim();
            cs_mime_type = entries[19].Trim();
            s_object_source = entries[20].Trim();
            sc_status = entries[21].Trim();
            s_cache_info = entries[22].Trim();
            rule = entries[23].Trim();
            FilterInfo = entries[24].Trim();
            cs_Network = entries[25].Trim();
            sc_Network = entries[26].Trim();
            error_info = entries[27].Trim();
            action = entries[28].Trim();
        }
        public string ConvertBack()
        {
            return c_ip + "\t" + cs_username + "\t" + c_agent + "\t" + sc_authenticated + "\t" + date + "\t" + time + "\t" + s_svcname + "\t" + s_computername + "\t" + cs_referred + "\t" + r_host + "\t" + r_ip + "\t" + r_port + "\t" + time_taken + "\t" + cs_bytes + "\t" + sc_bytes + "\t" + cs_protocol + "\t" + cs_transport + "\t" + s_operation + "\t" + cs_uri + "\t" + cs_mime_type + "\t" + s_object_source + "\t" + sc_status + "\t" + s_cache_info + "\t" + rule + "\t" + FilterInfo + "\t" + cs_Network + "\t" + sc_Network + "\t" + error_info + "\t" + action;
        }
    }

    public class FWSLogEntry : ISALog
    {
        public string servername { get; set; }  	    //computer
        public DateTime date { get; set; }	            //date
        public TimeSpan time { get; set; }	            //time
        public string protocol { get; set; }	        //IP protocol
        public string source { get; set; }      	    //source
        public string destination { get; set; }	        //destination
        public string c_ip { get; set; }	//original client IP
        public string SourceNetwork { get; set; }	    //source network
        public string DestinationNetwork { get; set; }	//destination network
        public string Action { get; set; }	            //action
        public string resultcode { get; set; }	        //status
        public string rule { get; set; }	            //rule
        public string ApplicationProtocol { get; set; }	//application protocol
        public bool Bidirectional { get; set; }	    //bidirectional
        public int bytessent { get; set; }	        //bytes sent
        public int bytessentDelta { get; set; }	    //bytes sent intermediate
        public int bytesrecvd { get; set; }	        //bytes received
        public int bytesrecvdDelta { get; set; }	    //bytes received intermediate
        public int connectiontime { get; set; }	    //connection time
        public int connectiontimeDelta { get; set; }	//connection time intermediate
        public string SourceProxy { get; set; }	        //source proxy
        public string DestinationProxy { get; set; }	//destination proxy
        public string SourceName { get; set; }	        //source name
        public string DestinationName { get; set; }	    //destination name
        public string cs_username { get; set; }  	//username
        public string c_agent { get; set; }     	//agent
        public string sessionid { get; set; }	        //session ID
        public string connectionid { get; set; }	    //connection ID
        public string Interface { get; set; }	        //interface
        public string IPHeader { get; set; }	        //IP header
        public string Payload { get; set; }	            //protocol payload
        public FWSLogEntry() { kind = Kind.FWS; }
        public FWSLogEntry(string Log)
        {
            var entries = Log.Split('\t').ToList<string>();
            if (entries.Count != 31)
            {
                throw new Exception("Invalid FWS-W3C log file passed");
            }
            kind = Kind.FWS;
            servername = entries[0].Trim();
            var date2parse = entries[1].Trim().Split('-').ToArray<string>();
            date = new DateTime(Int32.Parse(date2parse[0].ToString()), Int32.Parse(date2parse[1].ToString()), Int32.Parse(date2parse[2].ToString()));
            var time2parse = entries[2].Trim().Split(':').ToArray<string>();
            time = new TimeSpan(Int32.Parse(time2parse[0].ToString()), Int32.Parse(time2parse[1].ToString()), Int32.Parse(time2parse[2].ToString()));
            protocol = entries[3].Trim();
            source = entries[4].Trim();
            destination = entries[5].Trim();
            c_ip = entries[6].Trim();
            SourceNetwork = entries[7].Trim();
            DestinationNetwork = entries[8].Trim();
            Action = entries[9].Trim();
            resultcode = entries[10].Trim();
            rule = entries[11].Trim();
            ApplicationProtocol = entries[12].Trim();
            Bidirectional = entries[13].Trim().ToUpper() == "Y" ? true : false;
            try
            {
                bytessent = entries[14].Trim() == "-" ? 0 : Int32.Parse(entries[14].Trim());
            }
            catch (OverflowException)
            {
                bytessent = Int32.MaxValue - 1;
            }
            try
            {
                bytessentDelta = entries[15].Trim() == "-" ? 0 : Int32.Parse(entries[15].Trim());
            }
            catch (OverflowException)
            {
                bytessentDelta = Int32.MaxValue - 1;
            }
            try
            {
                bytesrecvd = entries[16].Trim() == "-" ? 0 : Int32.Parse(entries[16].Trim());
            }
            catch (OverflowException)
            {
                bytesrecvd = Int32.MaxValue - 1;
            }
            try
            {
                bytesrecvdDelta = entries[17].Trim() == "-" ? 0 : Int32.Parse(entries[17].Trim());
            }
            catch (OverflowException)
            {
                bytesrecvdDelta = Int32.MaxValue - 1;
            }
            try
            {
                connectiontime = entries[18].Trim() == "-" ? 0 : Int32.Parse(entries[18].Trim());
            }
            catch (OverflowException)
            {
                connectiontime = Int32.MaxValue - 1;
            }
            try
            {
                connectiontimeDelta = entries[19].Trim() == "-" ? 0 : Int32.Parse(entries[19].Trim());
            }
            catch (OverflowException)
            {
                connectiontimeDelta = Int32.MaxValue - 1;
            }
            SourceProxy = entries[20].Trim();
            DestinationProxy = entries[21].Trim();
            SourceName = entries[22].Trim();
            DestinationName = entries[23].Trim();
            cs_username = entries[24].Trim();
            c_agent = entries[25].Trim();
            sessionid = entries[26].Trim();
            connectionid = entries[27].Trim();
            Interface = entries[28].Trim();
            IPHeader = entries[29].Trim();
            Payload = entries[30].Trim();
        }
    }
}
