using System;
using System.Xml.Linq;

namespace PerformanceUtilities.ResultTypes.Formatters
{
    public class ReliabilityXmlFormat : FormatResultsBase
    {
        public override int Precision { get; set; }

        public override string Format(ResultBase result)
        {
            var r = result as ReliabilityResult;
            var numFormat = "{0:" + String.Format("N{0}", Precision) + "}%";

            var root = new XElement("ReliabilityResult",
                new XElement("Valid", r.IsValid),
                new XElement("Count", (r.Passed + r.Failed)),
                new XElement("Passed",
                    new XElement("Count", r.Passed),
                    new XElement("Percent", String.Format(numFormat, r.PercentPassed))),
                new XElement("Failed",
                    new XElement("Count", r.Failed),
                    new XElement("Percent", String.Format(numFormat, r.PercentFailed)))
                );

            return root.ToString();
        }
    }
}