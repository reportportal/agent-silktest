using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace ReportPortal.Addins.RPC.COM
{
    [ComVisible(false)]
    public static class Configuration

    {
        private const string ConfigFileName = "ReportPortalConfiguration.xml";
        private const string BackUpLogFileName = "RPLog.txt";

        static Configuration()
        {
            try
            {
                if (ReportPortalConfiguration != null)
                    return;
                ReportPortalConfiguration = new ReportPortalConfiguration();
                var formatter = XmlSerializer.FromTypes(new[] {typeof(ReportPortalConfiguration)})[0];
                var exeConfigPath = Assembly.GetExecutingAssembly().Location;
                using (var fs = new FileStream(Path.GetDirectoryName(exeConfigPath) + "\\" + ConfigFileName,
                    FileMode.OpenOrCreate))
                {
                    ReportPortalConfiguration = (ReportPortalConfiguration) formatter.Deserialize(fs);
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(Path.GetTempPath() + "\\" + BackUpLogFileName, e.Message + "\r\n");
                File.AppendAllText(Path.GetTempPath() + "\\" + BackUpLogFileName, e.StackTrace + "\r\n");
            }
        }


        public static ReportPortalConfiguration ReportPortalConfiguration { get; }
    }

    [ComVisible(false)]
    [Serializable]
    public class ReportPortalConfiguration
    {
        public GeneralConfiguration GeneralConfiguration { get; set; }
        public LaunchConfiguration LaunchConfiguration { get; set; }
        public ServerConfiguration ServerConfiguration { get; set; }
    }

    [ComVisible(false)]
    [Serializable]
    public class GeneralConfiguration
    {
        public string LogFile { get; set; }
        public string LibraryPath { get; set; }
        public bool DebugMode { get; set; }
        public ProxyConfiguration ProxyConfiguration { get; set; }
    }

    [ComVisible(false)]
    [Serializable]
    public class LaunchConfiguration
    {
        public string LaunchName { get; set; }
        public bool DebugMode { get; set; }
        public string Tags { get; set; }
    }

    [ComVisible(false)]
    [Serializable]
    public class ServerConfiguration
    {
        public string Url { get; set; }
        public string Project { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [ComVisible(false)]
    [Serializable]
    public class ProxyConfiguration
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string Server { get; set; }
    }
}