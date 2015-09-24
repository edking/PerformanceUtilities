using System;
using System.Text;

namespace PerformanceUtilities.ResultTypes.Formatters
{
    public class DescriptivePrintFormat : FormatResultsBase
    {
        public override string Format(ResultBase result)
        {
            var r = result as DescriptiveResult;
            var sb = new StringBuilder();

            var formats = new string[4];

            for (int i = 0; i < 4; i++) formats[i] = "{" + String.Format("{0}", i) + ":" + NumFormat + "}";

            sb.AppendLine(String.Format("Samples: {0}", r.Count));
            sb.AppendLine(String.Format("Minimum: " + formats[0], r.Min));
            sb.AppendLine(String.Format("Maximum: " + formats[0], r.Max));
            sb.AppendLine();
            sb.AppendLine(String.Format(
                ("Mean: " + formats[0] + ", Median: " + formats[1] + ", Std Dev: " + formats[2]), r.Mean, r.Median,
                r.StdDev));

            sb.AppendLine();
            sb.AppendLine(String.Format(
                ("Quartiles: " + formats[0] + ", " + formats[1] + ", " + formats[2]), r.FirstQuartile, r.Median,
                r.ThirdQuartile));


            sb.AppendLine();
            sb.AppendLine("Percentiles");
            for (int x = 0; x < 10; x++)
            {
                var rs = new StringBuilder();
                for (int c = 0; c < 10; c++)
                {
                    rs.Append(String.Format(" " + formats[0], r.Percentile(c + x*10)));
                }
                sb.AppendLine(rs.ToString());
            }

            if (r.Histogram != null)
            {
                sb.AppendLine();
                sb.AppendLine("Histogram Data");
                sb.AppendLine("--------------");
                sb.AppendLine("Bucket              Count");
                sb.AppendLine("------              -----");
                foreach (var t in r.Histogram)
                {
                    sb.AppendLine(t.ToString(ResultFormat.cPrintFormat, NumFormat));
                }
            }
            return sb.ToString();
        }
    }
}