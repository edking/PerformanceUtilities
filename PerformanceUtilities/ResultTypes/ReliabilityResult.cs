using System;
using PerformanceUtilities.ResultTypes.Formatters;

namespace PerformanceUtilities.ResultTypes
{
    public class ReliabilityResult : ResultBase
    {
        public ReliabilityResult() : base(new ReliabilityPrintFormat(), 2)
        {
        }

        public ReliabilityResult(FormatResultsBase formatter, int precision) : base(formatter, precision)
        {
        }

        public bool IsValid { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }

        public double PercentFailed
        {
            get { return (Convert.ToDouble(Failed*100)/Convert.ToDouble(Passed + Failed)); }
        }

        public double PercentPassed
        {
            get { return Convert.ToDouble(Passed*100)/Convert.ToDouble(Passed + Failed); }
        }
    }
}