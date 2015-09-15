using System;
using System.Text;

namespace PerformanceUtilities.ResultTypes.Formatters
{
    public class PerformanceCsvFormat : FormatResultsBase
    {
        private readonly FormatResultsBase _descriptiveFormatter;
        private int _precision = 2;

        public PerformanceCsvFormat()
        {
            _descriptiveFormatter = new DescriptiveCsvFormat {Precision = _precision};
        }

        public override int Precision
        {
            get { return _precision; }
            set
            {
                _precision = value;
                if (_descriptiveFormatter != null) _descriptiveFormatter.Precision = value;
            }
        }


        public override string Format(ResultBase result)
        {
            var sb = new StringBuilder();
            var r = result as PerformanceResult;
            sb.AppendLine("Iterations,DegreeOfParallelism");
            sb.AppendLine(String.Format("{0},{1}", r.Iterations, r.DegreeOfParallelism));

            var f = "{0:" + NumFormat + "}";

            var t = String.Format(f, r.TotalTicks);
            var s = String.Format(f, r.TotalSeconds);
            var m = String.Format(f, r.TotalMilliseconds);

            sb.AppendLine("TotalSeconds,TotalMilliseconds,TotalTicks");
            sb.AppendLine(String.Format("{0},{1},{2}", s, m, t));
            sb.AppendLine();

            sb.Append(_descriptiveFormatter.Format(r.DescriptiveResult));

            return sb.ToString();
        }
    }
}