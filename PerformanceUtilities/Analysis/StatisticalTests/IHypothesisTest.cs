namespace PerformanceUtilities.Analysis.StatisticalTests
{
    public interface IHypothesisTest
    {
        double[] Confidence { get; set; }
        double HypothesizedDifference { get; set; }
        double ObservedDifference { get; set; }
        bool Significant { get; set; }
        double Size { get; set; }
        double StandardError { get; set; }
    }
}