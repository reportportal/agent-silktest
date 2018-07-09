using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace ReportPortal.Addins.RPC.COM
{
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

        public bool ProxyAvailable => ReportPortalConfiguration.ProxyConfiguration != null;
        public string ProxyDomain => ReportPortalConfiguration.ProxyConfiguration?.Domain;
        public string ProxyServer => ReportPortalConfiguration.ProxyConfiguration?.Server;
        public string ProxyUser => ReportPortalConfiguration.ProxyConfiguration?.Username;
        public string ProxyPassword => ReportPortalConfiguration.ProxyConfiguration?.Password;
    }
}