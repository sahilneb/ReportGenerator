using Services;
using System.Configuration;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace ReportGenerator
{
    public partial class ReportGeneratorService : ServiceBase
    {
        public ReportGeneratorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Task.Run(() => StartReportGenerator());
        }

        private static async Task StartReportGenerator()
        {
            IConfig config = new Config();
            IPowerService powerService = new PowerService();

            var reportGenerator = new ReportGenerator(config, powerService);

            var intervalInMinutes = int.Parse(ConfigurationManager.AppSettings["TimeInterval"]);

            TaskScheduler scheduler = new TaskScheduler(reportGenerator.RunExtractAsync, intervalInMinutes);
            await scheduler.RunScheduleTaskAsync();
        }

        public async Task RunAsConsoleAsync(string[] args)
        {
            await StartReportGenerator();            
        }

        protected override void OnStop()
        {
        }
    }
}
