using PerformanceUtilities.Analysis.StatisticalTests;
using PerformanceUtilities.ResultTypes.Formatters;

namespace PerformanceUtilities.ResultTypes
{
    public class ComparisonResult : ResultBase
    {
        public ComparisonResult(FormatResultsBase f, int p) : base(f, p)
        {
        }

        public ComparisonResult() : base(new ComparisonPrintFormat(), 2)
        {
        }

        public TwoSampleHypothesis Hypothesis { get; set; }
        public double[] Confidence { get; set; }
        public double HypothesizedDifference { get; set; }
        public double ObservedDifference { get; set; }
        public bool Significant { get; set; }
        public double Size { get; set; }
        public double StandardError { get; set; }
        public SampleInfo FirstSample { get; set; }
        public SampleInfo SecondSample { get; set; }
    }
}