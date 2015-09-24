using System;
using PerformanceUtilities.ResultTypes;

namespace PerformanceUtilities.Analysis.StatisticalTests
{
    public class GeneralHypothesisTest : IHypothesisTest
    {
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

            var s1 = new SampleInfo
                     {
                         Name = firstLabel,
                         Count = sample1.Count,
                         Mean = sample1.Mean,
                         StdDev = sample1.StdDev
                     };

            var s2 = new SampleInfo
                     {
                         Name = secondLabel,
                         Count = sample2.Count,
                         Mean = sample2.Mean,
                         StdDev = sample2.StdDev
                     };

            Result = new ComparisonResult
                     {
                         FirstSample = s1,
                         SecondSample = s2,
                         Hypothesis = alternate,
                         HypothesizedDifference = HypothesizedDifference
                     };


            if (samples1 < 30 || samples2 < 30)
            {
                _tTest = new TwoSampleTTest(sample1, sample2, false, HypothesizedDifference, alternate);
                Result.Confidence = _tTest.Confidence;
                Result.ObservedDifference = _tTest.ObservedDifference;
                Result.Significant = _tTest.Significant;
                Result.Size = _tTest.Size;
                Result.StandardError = _tTest.StandardError;
            }
            else
            {
                _zTest = new TwoSampleZTest(sample1, sample2, HypothesizedDifference, alternate);
                Result.Confidence = _zTest.Confidence;
                Result.ObservedDifference = _zTest.ObservedDifference;
                Result.Significant = _zTest.Significant;
                Result.Size = _zTest.Size;
                Result.StandardError = _zTest.StandardError;
            }
        }

        public ComparisonResult Result { get; set; }

        #region IHypothesisTest Members

        public double[] Confidence { get; set; }
        public double HypothesizedDifference { get; set; }
        public double ObservedDifference { get; set; }
        public bool Significant { get; set; }
        public double Size { get; set; }
        public double StandardError { get; set; }

        #endregion
    }
}