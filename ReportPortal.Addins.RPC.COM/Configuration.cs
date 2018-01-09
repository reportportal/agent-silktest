using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace ReportPortal.Addins.RPC.COM
{
    public static class Configuration

    {
        static ReportPortalConfiguration xmlConf;
        const string configFileName = "ReportPortalConfiguration.xml";
        const string backUpLogFileName = "RPLog.txt";
        static Configuration()
        {
            try
            {
                if (xmlConf != null)
                {
                    return;
                }
                xmlConf = new ReportPortalConfiguration();
                XmlSerializer formatter = XmlSerializer.FromTypes(new[] { typeof(ReportPortalConfiguration) })[0];
                var exeConfigPath = Assembly.GetExecutingAssembly().Location;
                using (FileStream fs = new FileStream(Path.GetDirectoryName(exeConfigPath) + "\\" + configFileName, FileMode.OpenOrCreate))
                {
                    xmlConf = (ReportPortalConfiguration)formatter.Deserialize(fs);

                }
            }
            catch (Exception e)
            {
                File.AppendAllText(Path.GetTempPath() + "\\" + backUpLogFileName, e.Message + "\r\n");
                File.AppendAllText(Path.GetTempPath() + "\\" + backUpLogFileName, e.StackTrace + "\r\n");
            }
        }


        public static ReportPortalConfiguration ReportPortalConfiguration
        {
            get { return xmlConf; }
        }

    }

    [Serializable]
    public class ReportPortalConfiguration
    {
        public GeneralConfiguration GeneralConfiguration { get; set; }
        public LaunchConfiguration LaunchConfiguration { get; set; }
        public ServerConfiguration ServerConfiguration { get; set; }
        public ReportPortalConfiguration() { }
    }

    [Serializable]
    public class GeneralConfiguration
    {
        public string LogFile { get; set; }
        public string LibraryPath { get; set; }
        public bool DebugMode { get; set; }
        public ProxyConfiguration ProxyConfiguration { get; set; }
        public GeneralConfiguration() { }
    }

        [Serializable]
    public class LaunchConfiguration
    {
        public string LaunchName { get; set; }
        public bool DebugMode { get; set; }
        public string Tags { get; set; }

        public LaunchConfiguration() { }
    }

    [Serializable]
    public class ServerConfiguration
    {
        public string Url { get; set; }
        public string Project { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ServerConfiguration() { }

    }

    [Serializable]
    public class ProxyConfiguration
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string Server { get; set; }
        public ProxyConfiguration() { }
    }

}
