using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace ReportPortal.Addins.RPC.COM
{
    [ComVisible(false)]
    public interface IConfiguration
    {
        string ServerUrl { get; }
        string ServerProjectName { get; }
        string ServerPassword { get; }

        string LaunchName { get; }
        Client.Models.LaunchMode LaunchMode { get; }

        bool ProxyAvailable { get; }
        string ProxyDomain { get; }
        string ProxyServer { get; }
        string ProxyUser { get; }
        string ProxyPassword { get; }

    }

    [ComVisible(false)]
    public class Configuration : IConfiguration

    {
        private const string ConfigFileName = "ReportPortalConfiguration.xml";

        public Configuration()
        {
            ReportPortalConfiguration = new ReportPortalConfiguration();
            var formatter = XmlSerializer.FromTypes(new[] {typeof(ReportPortalConfiguration)})[0];
            var exeConfigPath = Assembly.GetExecutingAssembly().Location;
            using (var fs = new FileStream(Path.GetDirectoryName(exeConfigPath) + "\\" + ConfigFileName,
                FileMode.OpenOrCreate))
            {
                ReportPortalConfiguration = (ReportPortalConfiguration) formatter.Deserialize(fs);
            }
        }


        private ReportPortalConfiguration ReportPortalConfiguration { get; }

        public string ServerUrl => ReportPortalConfiguration.ServerConfiguration.Url;
        public string ServerProjectName => ReportPortalConfiguration.ServerConfiguration.Project;
        public string ServerPassword => ReportPortalConfiguration.ServerConfiguration.Password;


        public string LaunchName => ReportPortalConfiguration.LaunchConfiguration.LaunchName;
        public Client.Models.LaunchMode LaunchMode => ReportPortalConfiguration.LaunchConfiguration.DebugMode ? Client.Models.LaunchMode.Debug : Client.Models.LaunchMode.Default;

        public bool ProxyAvailable => ReportPortalConfiguration.GeneralConfiguration.ProxyConfiguration != null;
        public string ProxyDomain => ReportPortalConfiguration.GeneralConfiguration.ProxyConfiguration?.Domain;
        public string ProxyServer => ReportPortalConfiguration.GeneralConfiguration.ProxyConfiguration?.Server;
        public string ProxyUser => ReportPortalConfiguration.GeneralConfiguration.ProxyConfiguration?.Username;
        public string ProxyPassword => ReportPortalConfiguration.GeneralConfiguration.ProxyConfiguration?.Password;
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