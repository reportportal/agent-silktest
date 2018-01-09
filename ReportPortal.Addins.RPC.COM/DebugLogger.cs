using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportPortal.Addins.RPC.COM
{
    class DebugLogger
    {
        //Todo: replace with log4net
        private static string logName = @"C:\RPLog.txt";
        private static bool isEnabled = true;
        public static void SetupLogger(string fileName, bool enabled)
        {
            logName = fileName;
            isEnabled = enabled;
        }

        public static void Message( string message)
        {
            if (isEnabled)
            {
                File.AppendAllText(logName, DateTime.Now.ToString() + ": " +  message + "\r\n");
            }
        }

        public static void Message(IEnumerable<string> message)
        {
            File.AppendAllLines(logName, message);
        }
    }
}
