using System;
using System.Xml.Linq;

namespace PerformanceUtilities.ResultTypes
{
    public class Bucket
    {
        public double RangeLow { get; set; }
        public double RangeHigh { get; set; }
        public int Count { get; set; }

        public override string ToString()
        {
            return ToString(ResultFormat.cPrintFormat);
        }

        public string ToString(string format, string numFormat = "N2")
        {
            string rv = String.Empty;

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
            var f = "{0:" + numFormat + "}";

            var ele = new XElement("Bucket",
                new XAttribute("lowerBound", String.Format(f, RangeLow)),
                new XAttribute("upperBound", String.Format(f, RangeHigh)),
                Count);

            return ele.ToString();
        }

        private string CsvFormat(string numFormat)
        {
            string hFormat = "Histogram,{0:" + numFormat + "},{1:" + numFormat + ",{2}";

            return String.Format(hFormat, RangeLow, RangeHigh, Count);
        }

        private string PrintFormat(string numFormat)
        {
            string hFormat = "{0:" + numFormat + "}-{1:" + numFormat + "}                                  ";
            string b = String.Format(hFormat, RangeLow, RangeHigh);
            string l = b.Substring(0, 22);

            return String.Format("{0}{1}", l, Count);
        }
    }
}