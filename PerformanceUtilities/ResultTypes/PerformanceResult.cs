using PerformanceUtilities.ResultTypes.Formatters;

namespace PerformanceUtilities.ResultTypes
{
    public class PerformanceResult : ResultBase
    {
        public PerformanceResult() : base(new PerformancePrintFormat(), 2)
        {
        }

        public PerformanceResult(FormatResultsBase formatter, int precision) : base(formatter, precision)
        {
        }

        public bool IsValid { get; set; }
        public int Iterations { get; set; }
        public int DegreeOfParallelism { get; set; }
        public long TotalTicks { get; set; }
        public double TotalSeconds { get; set; }
        public double TotalMilliseconds { get; set; }
        public DescriptiveResult DescriptiveResult { get; set; }
    }
}