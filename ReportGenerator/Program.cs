using Services;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace ReportGenerator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static async Task Main(string[] args)
        {
            var reportGeneratorService = new ReportGeneratorService();
            if ((!Environment.UserInteractive))
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    reportGeneratorService
                };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                await reportGeneratorService.RunAsConsoleAsync(args);
                while (true)
                {
                    Thread.Sleep(30000);
                }
            }
        }

        private static async Task RunAsConsole()
        {
            IConfig config = new Config();
            IPowerService powerService = new PowerService();

            ReportGenerator reportGenerator = new ReportGenerator(config, powerService);
            //await reportGenerator.RunExtractAsync(); //if need to run just once (without scheduling)

            string timeIntervalInMinutes = ConfigurationManager.AppSettings["TimeInterval"];
            int intervalInMinutes = int.Parse(timeIntervalInMinutes);

            TaskScheduler scheduler = new TaskScheduler(reportGenerator.RunExtractAsync, intervalInMinutes);
            await scheduler.RunScheduleTaskAsync();

        }
    }
}
