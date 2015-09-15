using System;
using System.Text;

namespace PerformanceUtilities.ResultTypes.Formatters
{
    public class ReliabilityCsvFormat : FormatResultsBase
    {
        public override int Precision { get; set; }

        public override string Format(ResultBase result)
        {
            var r = result as ReliabilityResult;
            var numFormat = "{0:" + String.Format("N{0}", Precision) + "}%";
            var sb = new StringBuilder();

            sb.AppendLine(String.Format("Valid,{0}", r.IsValid));
            sb.AppendLine(String.Format("Count,{0}", (r.Passed + r.Failed)));
            sb.AppendLine(String.Format("Passed," + numFormat + ",{1}", r.PercentPassed, r.Passed));
            sb.AppendLine(String.Format("Failed," + numFormat + ",{1}", r.PercentFailed, r.Failed));

            return sb.ToString();
        }
    }
}