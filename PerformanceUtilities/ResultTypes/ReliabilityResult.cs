using System;
using System.Text;
using System.Xml.Linq;

namespace PerformanceUtilities.ResultTypes
{
    public class ReliabilityResult
    {
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

        public override string ToString()
        {
            return ToString(ResultFormat.cPrintFormat);
        }

        public string ToString(string format, int precision = 2)
        {
            string rv = String.Empty;
            var numFormat = "{0:" + String.Format("N{0}", precision) + "}%";

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

        private string XmlFormat(string numFormat)
        {
            var root = new XElement("ReliabilityResult",
                new XElement("Valid", IsValid),
                new XElement("Count", (Passed + Failed)),
                new XElement("Passed",
                    new XElement("Count", Passed),
                    new XElement("Percent", String.Format(numFormat, PercentPassed))),
                new XElement("Failed",
                    new XElement("Count", Failed),
                    new XElement("Percent", String.Format(numFormat, PercentFailed)))
                );

            return root.ToString();
        }

        private string CsvFormat(string numFormat)
        {
            var sb = new StringBuilder();

            sb.AppendLine(String.Format("Valid,{0}", IsValid));
            sb.AppendLine(String.Format("Count,{0}", (Passed + Failed)));
            sb.AppendLine(String.Format("Passed," + numFormat + ",{1}", PercentPassed, Passed));
            sb.AppendLine(String.Format("Failed," + numFormat + ",{1}", PercentFailed, Failed));

            return sb.ToString();
        }

        private string PrintFormat(string numFormat)
        {
            var sb = new StringBuilder();

            sb.AppendLine(String.Format("Valid: {0}", IsValid));
            sb.AppendLine(String.Format("Total Iterations: {0}", (Passed + Failed)));
            sb.AppendLine(String.Format("Passed: " + numFormat + " ({1})", PercentPassed, Passed));
            sb.AppendLine(String.Format("Failed: " + numFormat + " ({1})", PercentFailed, Failed));

            return sb.ToString();
        }
    }
}