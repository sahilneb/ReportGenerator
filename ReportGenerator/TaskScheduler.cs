using log4net;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace ReportGenerator
{
    public class TaskScheduler
    {
        ILog log = LogManager.GetLogger(nameof(ReportGenerator));

        private Timer timer;
        public Func<DateTime, Task> TaskToRun { get; }
        public int IntervalInMinutes { get; private set; }

        public TaskScheduler(Func<DateTime, Task> taskToRun, int intervalInMinutes)
        {
            TaskToRun = taskToRun;
            IntervalInMinutes = intervalInMinutes;
        }

        public async Task RunScheduleTaskAsync()
        {
            log.Info($"Scheduling timer for {IntervalInMinutes} minutes");
            timer = new Timer
            {
                Interval = 1000 * IntervalInMinutes * 60
            };
            timer.Elapsed += new ElapsedEventHandler(TriggerElapsedAsync);
            
            timer.AutoReset = false;
            timer.Start();

            var runDateTime = DateTime.Now;
            await TaskToRun(runDateTime);
        }

        private async void TriggerElapsedAsync(object sender, ElapsedEventArgs e)
        {
            await RunScheduleTaskAsync();
        }
    }


}
