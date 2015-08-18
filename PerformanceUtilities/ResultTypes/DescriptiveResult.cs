using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace PerformanceUtilities.ResultTypes
{

    /// <summary>
    ///     The result class the holds the analysis results
    /// </summary>
    public class DescriptiveResult
    {
        internal readonly double[] _percentiles = new double[100];
        public List<Bucket> Histogram;

        public List<double> RawData;

        /// <summary>
        ///     Count
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///     Interquartile range
        /// </summary>
        public double IQR
        {
            get { return _percentiles[75] - _percentiles[25]; }
        }

        /// <summary>
        ///     Maximum value
        /// </summary>
        public double Max { get; set; }

        /// <summary>
        ///     Arithmatic mean
        /// </summary>
        public double Mean { get; set; }

        /// <summary>
        ///     Median, or second quartile, or at 50 percentile
        /// </summary>
        public double Median
        {
            get { return _percentiles[50]; }
        }

        /// <summary>
        ///     Minimum value
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        ///     The range of the values
        /// </summary>
        public double Range { get; set; }

        /// <summary>
        ///     Skew of the data distribution
        /// </summary>
        public double Skew { get; set; }

        /// <summary>
        ///     Sample standard deviation
        /// </summary>
        public double StdDev { get; set; }

        /// <summary>
        ///     Sum
        /// </summary>
        public double Sum { get; set; }

        /// <summary>
        ///     Sum of Error
        /// </summary>
        internal double SumOfError { get; set; }

        /// <summary>
        ///     The sum of the squares of errors
        /// </summary>
        internal double SumOfErrorSquare { get; set; }


        /// <summary>
        ///     First quartile, at 25 percentile
        /// </summary>
        public double FirstQuartile
        {
            get { return _percentiles[25]; }
        }


        /// <summary>
        ///     Third quartile, at 75 percentile
        /// </summary>
        public double ThirdQuartile
        {
            get { return _percentiles[75]; }
        }

        /// <summary>
        ///     Sample variance
        /// </summary>
        public double Variance { get; set; }

        /// <summary>
        ///     Percentile
        /// </summary>
        /// <param name="percent"> Pecentile, between 0 to 100 </param>
        /// <returns> Percentile </returns>
        public double Percentile(int percent)
        {
            return _percentiles[percent];
        }

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

            var root = new XElement("PerformanceDetails");
            var summary = new XElement("Summary",
                new XElement("Samples", Count),
                new XElement("Minimum", String.Format(f, Min)),
                new XElement("Maximum", String.Format(f, Max)),
                new XElement("Mean", String.Format(f, Mean)),
                new XElement("Median", String.Format(f, Median)),
                new XElement("StdDev", String.Format(f, StdDev))
                );

            var quartiles = new XElement("Quartiles",
                new XElement("First", String.Format(f, FirstQuartile)),
                new XElement("Second", String.Format(f, Median)),
                new XElement("Third", String.Format(f, ThirdQuartile))
                );

            var pctiles = new XElement("Percentiles");

            for (int p = 0; p < 99; p++)
            {
                var pct = new XElement("Percentile",
                    new XAttribute("pct", p), String.Format(f, _percentiles[p])
                    );
                pctiles.Add(pct);
            }

            root.Add(summary);
            root.Add(quartiles);
            root.Add(pctiles);

            if (Histogram != null)
            {
                var histogram = new XElement("Histogram",
                    from h in Histogram
                    select XElement.Parse(h.ToString(ResultFormat.cXmlFormat,f))
                    );

                root.Add(histogram);
            }

            return root.ToString();
        }

        private string CsvFormat(string numFormat)
        {
            var sb = new StringBuilder();

            var formats = new string[3];

            for (int i = 0; i < 3; i++) formats[i] = "{" + String.Format("{0}", i) + ":" + numFormat + "}";

            sb.AppendLine(String.Format("Summary,Samples,{0}", Count));
            sb.AppendLine(String.Format("Summary,Minimum," + formats[0], Min));
            sb.AppendLine(String.Format("Summary,Maximum," + formats[0], Max));
            sb.AppendLine(String.Format("Summary,Mean," + formats[0], Mean));
            sb.AppendLine(String.Format("Summary,Median," + formats[0], Median));
            sb.AppendLine(String.Format("Summary,StdDev," + formats[0], StdDev));

            sb.AppendLine(String.Format("Quartiles,First," + formats[0], FirstQuartile));
            sb.AppendLine(String.Format("Quartiles,Second," + formats[0], Median));
            sb.AppendLine(String.Format("Quartiles,Third," + formats[0], ThirdQuartile));

            for (int p = 0; p < 100; p++)
            {
                sb.AppendLine(String.Format("Percentiles,{0}," + formats[1], p, _percentiles[p]));
            }


            if (Histogram != null)
            {
                foreach (var t in Histogram)
                {
                    sb.AppendLine(t.ToString(ResultFormat.cCsvFormat,numFormat));
                }
            }
            return sb.ToString();
        }

        private string PrintFormat(string numFormat)
        {
            var sb = new StringBuilder();

            var formats = new string[4];

            for (int i = 0; i < 4; i++) formats[i] = "{" + String.Format("{0}", i) + ":" + numFormat + "}";

            sb.AppendLine(String.Format("Samples: {0}", Count));
            sb.AppendLine(String.Format("Minimum: " + formats[0], Min));
            sb.AppendLine(String.Format("Maximum: " + formats[0], Max));
            sb.AppendLine();
            sb.AppendLine(String.Format(
                ("Mean: " + formats[0] + ", Median: " + formats[1] + ", Std Dev: " + formats[2]), Mean, Median, StdDev));

            sb.AppendLine();
            sb.AppendLine(String.Format(
                ("Quartiles: " + formats[0] + ", " + formats[1] + ", " + formats[2]), FirstQuartile, Median,
                ThirdQuartile));


            sb.AppendLine();
            sb.AppendLine("Percentiles");
            for (int r = 0; r < 10; r++)
            {
                var rs = new StringBuilder();
                for (int c = 0; c < 10; c++)
                {
                    rs.Append(String.Format(" " + formats[0], _percentiles[c + r*10]));
                }
                sb.AppendLine(rs.ToString());
            }

            if (Histogram != null)
            {
                sb.AppendLine();
                sb.AppendLine("Histogram Data");
                sb.AppendLine("--------------");
                sb.AppendLine("Bucket              Count");
                sb.AppendLine("------              -----");
                foreach (var t in Histogram)
                {
                    sb.AppendLine(t.ToString(ResultFormat.cPrintFormat,numFormat));
                }
            }
            return sb.ToString();
        }
    }
}