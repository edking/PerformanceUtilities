using System;
using System.Text;

namespace PerformanceUtilities.ResultTypes.Formatters
{
    public class ReliabilityPrintFormat : FormatResultsBase
    {
        public override string Format(ResultBase result)
        {
            var sb = new StringBuilder();
            var r = result as ReliabilityResult;
            var numFormat = "{0:" + NumFormat + "}%";

            sb.AppendLine(String.Format("Valid: {0}", r.IsValid));
            sb.AppendLine(String.Format("Total Iterations: {0}", (r.Passed + r.Failed)));
            sb.AppendLine(String.Format("Passed: " + numFormat + " ({1})", r.PercentPassed, r.Passed));
            sb.AppendLine(String.Format("Failed: " + numFormat + " ({1})", r.PercentFailed, r.Failed));

            return sb.ToString();
        }
    }
}