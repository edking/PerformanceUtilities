using System;
using System.Text;
using System.Xml.Linq;

namespace PerformanceUtilities.ResultTypes
{
    public class PerformanceResult
    {
        public bool IsValid { get; set; }
        public int Iterations { get; set; }
        public int DegreeOfParallelism { get; set; }
        public long TotalTicks { get; set; }
        public double TotalSeconds { get; set; }
        public double TotalMilliseconds { get; set; }
        public DescriptiveResult DescriptiveResult { get; set; }

        public override string ToString()
        {
            return ToString(ResultFormat.cPrintFormat);
        }

        public string ToString(string format, int precision = 2)
        {
            string rv = String.Empty;
            var numFormat = String.Format("N{0}", precision);

            if (String.IsNullOrEmpty(format)) format = ResultFormat.cPrintFormat;

            switch (format)
            {
                case ResultFormat.cPrintFormat:
                    rv = PrintFormat(numFormat);
                    break;

                case ResultFormat.cCsvFormat:
                    rv = CsvFormat(numFormat);
                    break;

                case ResultFormat.cXmlFormat:
                    rv = XmlFormat(numFormat);
                    break;
            }

            return rv;
        }

        private string PrintFormat(string numFormat)
        {
            var sb = new StringBuilder();

            sb.AppendLine(String.Format("Total Iterations: {0}", Iterations));
            sb.AppendLine(String.Format("Degree Of Parallelism: {0}", DegreeOfParallelism));

            var f = "{0:" + numFormat + "}";

            var t = String.Format(f, TotalTicks);
            var s = String.Format(f, TotalSeconds);
            var m = String.Format(f, TotalMilliseconds);

            sb.AppendLine(String.Format("Total Time: {0} seconds, {1} milliseconds, {2} ticks", s, m, t));
            sb.AppendLine();
            sb.AppendLine("Statistics (ms)");
            sb.AppendLine("---------------");

            sb.Append(DescriptiveResult.ToString(ResultFormat.cPrintFormat, numFormat));

            return sb.ToString();
        }

        private string XmlFormat(string numFormat)
        {
            var f = "{0:" + numFormat + "}";

            var root = new XElement("PerformanceResult",
                new XElement("Header",
                    new XElement("Iterations", Iterations),
                    new XElement("DegreeOfParallelism", DegreeOfParallelism),
                    new XElement("TotalSeconds", String.Format(f, TotalSeconds)),
                    new XElement("TotalMilliseconds", String.Format(f, TotalMilliseconds)),
                    new XElement("TotalTicks", String.Format(f, TotalTicks))
                    ));

            var details = XElement.Parse(DescriptiveResult.ToString(ResultFormat.cXmlFormat, numFormat));
            root.Add(details);

            return root.ToString();
        }

        private string CsvFormat(string numFormat)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Iterations,DegreeOfParallelism");
            sb.AppendLine(String.Format("{0},{1}", Iterations, DegreeOfParallelism));

            var f = "{0:" + numFormat + "}";

            var t = String.Format(f, TotalTicks);
            var s = String.Format(f, TotalSeconds);
            var m = String.Format(f, TotalMilliseconds);

            sb.AppendLine("TotalSeconds,TotalMilliseconds,TotalTicks");
            sb.AppendLine(String.Format("{0},{1},{2}", s, m, t));
            sb.AppendLine();

            sb.Append(DescriptiveResult.ToString(ResultFormat.cCsvFormat, numFormat));

            return sb.ToString();
        }
    }
}