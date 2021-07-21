using System.Configuration;

namespace ReportGenerator
{
    public class Config : IConfig
    {
        public string ReportLocation => ConfigurationManager.AppSettings["ReportLocation"];
    }
}
