using System;
using System.Collections.Generic;
using System.Diagnostics;
using PerformanceUtilities.Analysis.Distributions;
using PerformanceUtilities.ResultTypes;

namespace PerformanceUtilities.Analysis.StatisticalTests
{
    public class TwoSampleZTest : HypothesisTest<NormalDistribution>
    {
        /// <summary>
        ///     Constructs a Z test.
        /// </summary>
        /// <param name="sample1">The first data sample.</param>
        /// <param name="sample2">The second data sample.</param>
        /// <param name="hypothesizedDifference">The hypothesized sample difference.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        public TwoSampleZTest(List<double> sample1, List<double> sample2, double hypothesizedDifference = 0,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {
            int samples1 = sample1.Count;
            int samples2 = sample2.Count;

            if (samples1 < 30 || samples2 < 30)
            {
                Trace.TraceWarning(
                    "Warning: running a Z test for less than 30 samples. Consider running a Student's T Test instead.");
            }

            var s1 = new DescriptiveAnalysis(sample1);
            var s2 = new DescriptiveAnalysis(sample2);

            s1.Analyze(false);
            s2.Analyze(false);

            var r1 = s1.Result;
            var r2 = s2.Result;

            Compute(r1.Mean, r2.Mean, r1.Variance/r1.Count, r2.Variance/r2.Count,
                hypothesizedDifference, alternate);
        }

        /// <summary>
        ///     Tests whether the means of two samples are different.
        /// </summary>
        /// <param name="sample1">The first sample.</param>
        /// <param name="sample2">The second sample.</param>
        /// <param name="hypothesizedDifference">The hypothesized sample difference.</param>
        /// <param name="assumeEqualVariances">True to assume equal variances, false otherwise. Default is true.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        public TwoSampleZTest(DescriptiveResult sample1, DescriptiveResult sample2,
            double hypothesizedDifference = 0, TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {
            // References: http://en.wikipedia.org/wiki/Student's_t-test#Worked_examples

            Compute(sample1.Mean, sample2.Mean, sample1.Variance/sample1.Count, sample2.Variance/sample2.Count,
                hypothesizedDifference, alternate);
        }

        /// <summary>
        ///     Constructs a Z test.
        /// </summary>
        /// <param name="mean1">The first sample's mean.</param>
        /// <param name="mean2">The second sample's mean.</param>
        /// <param name="var1">The first sample's variance.</param>
        /// <param name="var2">The second sample's variance.</param>
        /// <param name="samples1">The number of observations in the first sample.</param>
        /// <param name="samples2">The number of observations in the second sample.</param>
        /// <param name="hypothesizedDifference">The hypothesized sample difference.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        public TwoSampleZTest(
            double mean1, double var1, int samples1,
            double mean2, double var2, int samples2,
            double hypothesizedDifference = 0,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {
            if (samples1 < 30 || samples2 < 30)
            {
                Trace.TraceWarning(
                    "Warning: running a Z test for less than 30 samples. Consider running a Student's T Test instead.");
            }

            double sqStdError1 = var1/samples1;
            double sqStdError2 = var2/samples2;

            Compute(mean1, mean2, sqStdError1, sqStdError2, hypothesizedDifference, alternate);
        }


        /// <summary>
        ///     Constructs a Z test.
        /// </summary>
        protected TwoSampleZTest()
        {
        }

        /// <summary>
        ///     Gets the alternative hypothesis under test. If the test is
        ///     <see cref="IHypothesisTest.Significant" />, the null hypothesis can be rejected
        ///     in favor of this alternative hypothesis.
        /// </summary>
        public TwoSampleHypothesis Hypothesis { get; private set; }

        /// <summary>
        ///     Computes the Z test.
        /// </summary>
        protected void Compute(double value1, double value2,
            double squareStdError1, double squareStdError2,
            double hypothesizedDifference, TwoSampleHypothesis alternate)
        {
            EstimatedValue1 = value1;
            EstimatedValue2 = value2;

            double diff = value1 - value2;
            double stdError = Math.Sqrt(squareStdError1 + squareStdError2);

            Compute(diff, hypothesizedDifference, stdError, alternate);
        }

        /// <summary>
        ///     Computes the Z test.
        /// </summary>
        protected void Compute(double observedDifference, double hypothesizedDifference,
            double standardError, TwoSampleHypothesis alternate)
        {
            ObservedDifference = observedDifference;
            HypothesizedDifference = hypothesizedDifference;
            StandardError = standardError;

            // Compute Z statistic
            double z = (ObservedDifference - HypothesizedDifference)/StandardError;

            Compute(z, alternate);
        }

        /// <summary>
        ///     Computes the Z test.
        /// </summary>
        protected void Compute(double statistic, TwoSampleHypothesis alternate)
        {
            Statistic = statistic;
            StatisticDistribution = NormalDistribution.Standard;

            Hypothesis = alternate;

            PValue = StatisticToPValue(Statistic);
        }


        /// <summary>
        ///     Converts a given p-value to a test statistic.
        /// </summary>
        /// <param name="p">The p-value.</param>
        /// <returns>The test statistic which would generate the given p-value.</returns>
        public override double PValueToStatistic(double p)
        {
            return PValueToStatistic(p, Hypothesis);
        }

        private double PValueToStatistic(double p, TwoSampleHypothesis type)
        {
            double z;
            switch (type)
            {
                case TwoSampleHypothesis.ValuesAreDifferent:
                    z = NormalDistribution.Standard.InverseDistributionFunction(p);
                    break;
                case TwoSampleHypothesis.FirstValueIsGreaterThanSecond:
                    z = NormalDistribution.Standard.InverseDistributionFunction(1.0 - p);
                    break;
                case TwoSampleHypothesis.FirstValueIsSmallerThanSecond:
                    z = NormalDistribution.Standard.InverseDistributionFunction(1.0 - p/2.0);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return z;
        }

        /// <summary>
        ///     Converts a given test statistic to a p-value.
        /// </summary>
        /// <param name="x">The value of the test statistic.</param>
        /// <returns>The p-value for the given statistic.</returns>
        public override double StatisticToPValue(double x)
        {
            return StatisticToPValue(x, Hypothesis);
        }

        private double StatisticToPValue(double z, TwoSampleHypothesis type)
        {
            double p;
            switch (type)
            {
                case TwoSampleHypothesis.ValuesAreDifferent:
                    p = 2.0*NormalDistribution.Standard.ComplementaryDistributionFunction(Math.Abs(z));
                    break;

                case TwoSampleHypothesis.FirstValueIsGreaterThanSecond:
                    p = NormalDistribution.Standard.ComplementaryDistributionFunction(z);
                    break;

                case TwoSampleHypothesis.FirstValueIsSmallerThanSecond:
                    p = NormalDistribution.Standard.DistributionFunction(z);
                    break;

                default:
                    throw new InvalidOperationException();
            }
            return p;
        }
    }
}