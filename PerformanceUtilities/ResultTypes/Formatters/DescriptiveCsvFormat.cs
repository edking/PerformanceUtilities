using System;
using System.Text;

namespace PerformanceUtilities.ResultTypes.Formatters
{
    internal class DescriptiveCsvFormat : FormatResultsBase
    {
        public override string Format(ResultBase result)
        {
            var r = result as DescriptiveResult;

            var sb = new StringBuilder();

            var formats = new string[3];

            for (int i = 0; i < 3; i++) formats[i] = "{" + String.Format("{0}", i) + ":" + NumFormat + "}";

            sb.AppendLine(String.Format("Summary,Samples,{0}", r.Count));
            sb.AppendLine(String.Format("Summary,Minimum," + formats[0], r.Min));
            sb.AppendLine(String.Format("Summary,Maximum," + formats[0], r.Max));
            sb.AppendLine(String.Format("Summary,Mean," + formats[0], r.Mean));
            sb.AppendLine(String.Format("Summary,Median," + formats[0], r.Median));
            sb.AppendLine(String.Format("Summary,StdDev," + formats[0], r.StdDev));

            sb.AppendLine(String.Format("Quartiles,First," + formats[0], r.FirstQuartile));
            sb.AppendLine(String.Format("Quartiles,Second," + formats[0], r.Median));
            sb.AppendLine(String.Format("Quartiles,Third," + formats[0], r.ThirdQuartile));

            for (int p = 0; p < 100; p++)
            {
                sb.AppendLine(String.Format("Percentiles,{0}," + formats[1], p, r.Percentile(p)));
            }


            if (r.Histogram != null)
            {
                foreach (var t in r.Histogram)
                {
                    sb.AppendLine(t.ToString(ResultFormat.cCsvFormat, NumFormat));
                }
            }
            return sb.ToString();
        }
    }
}