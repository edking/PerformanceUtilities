using System;
using System.Linq;
using System.Xml.Linq;

namespace PerformanceUtilities.ResultTypes.Formatters
{
    internal class DescriptiveXmlFormat : FormatResultsBase
    {
        public override string Format(ResultBase result)
        {
            var f = "{0:" + NumFormat + "}";
            var r = result as DescriptiveResult;
            var root = new XElement("PerformanceDetails");
            var summary = new XElement("Summary",
                new XElement("Samples", r.Count),
                new XElement("Minimum", String.Format(f, r.Min)),
                new XElement("Maximum", String.Format(f, r.Max)),
                new XElement("Mean", String.Format(f, r.Mean)),
                new XElement("Median", String.Format(f, r.Median)),
                new XElement("StdDev", String.Format(f, r.StdDev))
                );

            var quartiles = new XElement("Quartiles",
                new XElement("First", String.Format(f, r.FirstQuartile)),
                new XElement("Second", String.Format(f, r.Median)),
                new XElement("Third", String.Format(f, r.ThirdQuartile))
                );

            var pctiles = new XElement("Percentiles");

            for (int p = 0; p < 99; p++)
            {
                var pct = new XElement("Percentile",
                    new XAttribute("pct", p), String.Format(f, r.Percentile(p))
                    );
                pctiles.Add(pct);
            }

            root.Add(summary);
            root.Add(quartiles);
            root.Add(pctiles);

            if (r.Histogram != null)
            {
                var histogram = new XElement("Histogram",
                    from h in r.Histogram
                    select XElement.Parse(h.ToString(ResultFormat.cXmlFormat, f))
                    );

                root.Add(histogram);
            }

            return root.ToString();
        }
    }
}