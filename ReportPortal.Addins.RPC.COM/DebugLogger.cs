using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM
{
    [ComVisible(false)]
    static class DebugLogger
    {
        //Todo: replace with log4net
        private static string _logName = @"C:\RPLog.txt";
        private static bool _isEnabled = true;
        public static void SetupLogger(string fileName, bool enabled)
        {
            _logName = fileName;
            _isEnabled = enabled;
        }

        public static void Message( string message)
        {
            if (_isEnabled)
            {
                File.AppendAllText(_logName, DateTime.Now.ToString(CultureInfo.InvariantCulture) + ": " +  message + "\r\n");
            }
        }

        public static void Message(IEnumerable<string> message)
        {
            File.AppendAllLines(_logName, message);
        }
    }
}
