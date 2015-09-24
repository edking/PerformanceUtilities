using System;
using System.Xml.Linq;

namespace PerformanceUtilities.ResultTypes.Formatters
{
    public class PerformanceXmlFormat : FormatResultsBase
    {
        private readonly FormatResultsBase _descriptiveFormatter;
        private int _precision = 2;

        public PerformanceXmlFormat()
        {
            _descriptiveFormatter = new DescriptiveXmlFormat {Precision = _precision};
        }

        public override int Precision
        {
            get { return _precision; }
            set
            {
                _precision = value;
                if (_descriptiveFormatter != null) _descriptiveFormatter.Precision = value;
            }
        }


        public override string Format(ResultBase result)
        {
            var r = result as PerformanceResult;
            var f = "{0:" + NumFormat + "}";

            var root = new XElement("PerformanceResult",
                new XElement("Header",
                    new XElement("Iterations", r.Iterations),
                    new XElement("DegreeOfParallelism", r.DegreeOfParallelism),
                    new XElement("TotalSeconds", String.Format(f, r.TotalSeconds)),
                    new XElement("TotalMilliseconds", String.Format(f, r.TotalMilliseconds)),
                    new XElement("TotalTicks", String.Format(f, r.TotalTicks))
                    ));

            var details = XElement.Parse(_descriptiveFormatter.Format(r.DescriptiveResult));
            root.Add(details);

            return root.ToString();
        }
    }
}