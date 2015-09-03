using System;
using System.Collections.Generic;
using PerformanceUtilities.ResultTypes;

namespace PerformanceUtilities.Analysis.StatisticalTests
{
    public class GeneralHypothesisTest : IHypothesisTest
    {
        private readonly TwoSampleHypothesis _hypothesis;

        private readonly Dictionary<TwoSampleHypothesis, string> _hypothesisLabels = new Dictionary
            <TwoSampleHypothesis, string>
        {
            {TwoSampleHypothesis.ValuesAreDifferent, "The values are different by at least "},
            {TwoSampleHypothesis.FirstValueIsGreaterThanSecond, "{0} is greater than {1} by at least "},
            {TwoSampleHypothesis.FirstValueIsSmallerThanSecond, "{0} is less than {1} by at least "},
        };

        private readonly double firstMean;
        private readonly double secondMean;

        private TwoSampleTTest _tTest;
        private TwoSampleZTest _zTest;


        public GeneralHypothesisTest(DescriptiveResult sample1, DescriptiveResult sample2,
            double hypothesizedDifference = 0,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
            : this("first", sample1, "second", sample2, hypothesizedDifference, alternate)
        {
        }

        public GeneralHypothesisTest(string firstLabel, DescriptiveResult sample1, string secondLabel,
            DescriptiveResult sample2, double hypothesizedDifference = 0,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {
            int samples1 = sample1.Count;
            int samples2 = sample2.Count;
            HypothesizedDifference = Math.Abs(hypothesizedDifference);
            _hypothesis = alternate;
            FirstLabel = firstLabel;
            SecondLabel = secondLabel;
            firstMean = sample1.Mean;
            secondMean = sample2.Mean;

            if (samples1 < 30 || samples2 < 30)
            {
                _tTest = new TwoSampleTTest(sample1, sample2, false, HypothesizedDifference, alternate);
                Confidence = _tTest.Confidence;
                ObservedDifference = _tTest.ObservedDifference;
                Significant = _tTest.Significant;
                Size = _tTest.Size;
                StandardError = _tTest.StandardError;
            }
            else
            {
                _zTest = new TwoSampleZTest(sample1, sample2, HypothesizedDifference, alternate);
                Confidence = _zTest.Confidence;
                ObservedDifference = _zTest.ObservedDifference;
                Significant = _zTest.Significant;
                Size = _zTest.Size;
                StandardError = _zTest.StandardError;
            }
        }

        public string FirstLabel { get; set; }
        public string SecondLabel { get; set; }

        #region IHypothesisTest Members

        public double[] Confidence { get; set; }
        public double HypothesizedDifference { get; set; }
        public double ObservedDifference { get; set; }
        public bool Significant { get; set; }
        public double Size { get; set; }
        public double StandardError { get; set; }

        #endregion

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
            throw new NotImplementedException();
        }

        private string CsvFormat(string numFormat)
        {
            throw new NotImplementedException();
        }

        private string PrintFormat(string numFormat)
        {
            var f = "{2:" + numFormat + "}";
            var o1 = "{3:" + numFormat + "}";
            var o2 = "{4:" + numFormat + "}";
            var fmt = _hypothesisLabels[_hypothesis] + f + "ms. (Observed: " + o1 + " vs " + o2 + ")";
            string rv;

            if (!Significant)
            {
                if (HypothesizedDifference == 0)
                {
                    fmt = "Difference between {0} and {1} is not significant.";
                }
                else
                {
                    fmt = "Difference between {0} and {1} is not significant or is less than " + f + "ms. (Observed: " +
                          o1 + " vs " + o2 + ")";
                }
            }
            rv = String.Format(fmt, FirstLabel, SecondLabel, Math.Abs(HypothesizedDifference), firstMean, secondMean);
            return rv;
        }
    }
}