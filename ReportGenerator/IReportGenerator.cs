using Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportGenerator
{
    public interface IReportGenerator
    {
        Dictionary<int, double> AggregatePositions(IEnumerable<PowerTrade> trades);
        string GetFileName(DateTime timeOfExtract);
        string GetFullPath(DateTime timeOfExtract);
        Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date);
        Task RunExtractAsync(DateTime runDateTime);
        void WriteToCsv(IEnumerable<object> positionData, string fileFullPath);
    }
}