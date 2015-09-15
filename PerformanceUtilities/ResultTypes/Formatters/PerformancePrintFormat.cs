using System;
using System.Text;

namespace PerformanceUtilities.ResultTypes.Formatters
{
    public class PerformancePrintFormat : FormatResultsBase
    {
        private readonly FormatResultsBase descriptiveFormatter;
        private int _precision = 2;

        public PerformancePrintFormat()
        {
            descriptiveFormatter = new DescriptivePrintFormat {Precision = _precision};
        }

        public override int Precision
        {
            get { return _precision; }
            set
            {
                _precision = value;
                if (descriptiveFormatter != null) descriptiveFormatter.Precision = value;
            }
        }

        public override string Format(ResultBase result)
        {
            var sb = new StringBuilder();
            var r = result as PerformanceResult;

            sb.AppendLine(String.Format("Total Iterations: {0}", r.Iterations));
            sb.AppendLine(String.Format("Degree Of Parallelism: {0}", r.DegreeOfParallelism));

            var f = "{0:" + NumFormat + "}";

            var t = String.Format(f, r.TotalTicks);
            var s = String.Format(f, r.TotalSeconds);
            var m = String.Format(f, r.TotalMilliseconds);

            sb.AppendLine(String.Format("Total Time: {0} seconds, {1} milliseconds, {2} ticks", s, m, t));
            sb.AppendLine();
            sb.AppendLine("Statistics (ms)");
            sb.AppendLine("---------------");

            sb.Append(descriptiveFormatter.Format(r.DescriptiveResult));

            return sb.ToString();
        }
    }
}