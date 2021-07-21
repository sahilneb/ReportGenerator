using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CsvHelper;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Timers;
using log4net;

namespace ReportGenerator
{

    public class ReportGenerator : IReportGenerator
    {
        ILog log = LogManager.GetLogger(nameof(ReportGenerator));

        private readonly IConfig _config;
        private readonly IPowerService powerService;

        public ReportGenerator(IConfig config, IPowerService powerService)
        {
            _config = config;
            this.powerService = powerService;
        }

        public async Task RunExtractAsync(DateTime runDateTime)
        {
            DateTime runDate = runDateTime.Date;

            log.Info($"Running Extract for {runDateTime}");

            try
            {
                //get positions
                var trades = await GetTradesAsync(runDate);

                //aggregate by hour
                var aggregatedPositions = AggregatePositions(trades);

                //get filename
                string fileFullPath = GetFullPath(runDateTime);

                //export file
                WriteToCsv(aggregatedPositions.Select(x => new { Time = ConvertToLocalTime(x.Key), Volume = x.Value }), fileFullPath);

                log.Info($"Report generated: {fileFullPath}");
            }
            catch (Exception ex)
            {
                log.Error($"Exception when running extract for {runDateTime}.", ex);
            }
        }

        public string GetFileName(DateTime timeOfExtract)
        {
            var filename = $"PowerPosition_{timeOfExtract:yyyyMMdd_HHmm}.csv";
            return filename;
        }

        public string GetFullPath(DateTime timeOfExtract)
        {
            string csvFileName = GetFileName(timeOfExtract);
            var fullPath = Path.Combine(_config.ReportLocation, csvFileName);
            return fullPath;
        }

        public Dictionary<int, double> AggregatePositions(IEnumerable<PowerTrade> trades)
        {
            var positions = new Dictionary<int, double>();
            foreach (var trade in trades)
            {
                foreach (var period in trade.Periods)
                {
                    if (positions.ContainsKey(period.Period))
                        positions[period.Period] += period.Volume;
                    else
                        positions[period.Period] = period.Volume;
                }
            }
            return positions;
        }

        public async Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date)
        {
            log.Info($"Requesting trades data for {date}");
            var powerTrades = await powerService.GetTradesAsync(date);
            return powerTrades;
        }

        public static string ConvertToLocalTime(int period)
        {
            return DateTime.Today.AddHours(period - 2).ToString("HH:mm");
        }

        public void WriteToCsv(IEnumerable<object> positionData, string fileFullPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileFullPath));
            using (var writer = new StreamWriter(fileFullPath))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecord(new { Time = "Local Time", Volume = "Volume" });
                csvWriter.NextRecord();
                csvWriter.WriteRecords(positionData);
            }
        }

    }
}
