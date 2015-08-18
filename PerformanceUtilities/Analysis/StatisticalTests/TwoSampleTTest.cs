using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerformanceUtilities.Analysis.Distributions;
using PerformanceUtilities.ResultTypes;

namespace PerformanceUtilities.Analysis.StatisticalTests
{
    /// <summary>
    ///   Two-sample Student's T test.
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///   The two-sample t-test assesses whether the means of two groups are statistically 
    ///   different from each other.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Student's_t-test">
    ///       Wikipedia, The Free Encyclopedia. Student's T-Test. </a></description></item>
    ///     <item><description><a href="http://www.le.ac.uk/bl/gat/virtualfc/Stats/ttest.html">
    ///       William M.K. Trochim. The T-Test. Research methods Knowledge Base, 2009. 
    ///       Available on: http://www.le.ac.uk/bl/gat/virtualfc/Stats/ttest.html </a></description></item>
    ///     <item><description><a href="http://en.wikipedia.org/wiki/One-way_ANOVA">
    ///       Graeme D. Ruxton. The unequal variance t-test is an underused alternative to Student's
    ///       t-test and the Mann–Whitney U test. Oxford Journals, Behavioral Ecology Volume 17, Issue 4, pp.
    ///       688-690. 2006. Available on: http://beheco.oxfordjournals.org/content/17/4/688.full </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// 
    public class TwoSampleTTest : HypothesisTest<TDistribution>
    {

        /// <summary>
        ///   Gets the alternative hypothesis under test. If the test is
        ///   <see cref="IHypothesisTest.Significant"/>, the null hypothesis can be rejected
        ///   in favor of this alternative hypothesis.
        /// </summary>
        /// 
        public TwoSampleHypothesis Hypothesis { get; private set; }

        /// <summary>
        ///   Gets whether the test assumes equal sample variance.
        /// </summary>
        /// 
        public bool AssumeEqualVariance { get; private set; }

        /// <summary>
        ///   Gets the combined sample variance.
        /// </summary>
        /// 
        public double Variance { get; protected set; }

        /// <summary>
        ///   Gets the degrees of freedom for the test statistic.
        /// </summary>
        /// 
        public double DegreesOfFreedom { get { return StatisticDistribution.DegreesOfFreedom; } }


        /// <summary>
        ///   Tests whether the means of two samples are different.
        /// </summary>
        /// 
        /// <param name="sample1">The first sample.</param>
        /// <param name="sample2">The second sample.</param>
        /// <param name="hypothesizedDifference">The hypothesized sample difference.</param>
        /// <param name="assumeEqualVariances">True to assume equal variances, false otherwise. Default is true.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TwoSampleTTest(List<double> sample1, List<double> sample2,
            bool assumeEqualVariances = true, double hypothesizedDifference = 0,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {
            // References: http://en.wikipedia.org/wiki/Student's_t-test#Worked_examples

            DescriptiveAnalysis s1 = new DescriptiveAnalysis(sample1);
            DescriptiveAnalysis s2 = new DescriptiveAnalysis(sample2);

            s1.Analyze(false);
            s2.Analyze(false);

            var r1 = s1.Result;
            var r2 = s2.Result;

            Compute(r1.Mean, r1.Variance, r1.Count, r2.Mean, r2.Variance, r2.Count,
                hypothesizedDifference, assumeEqualVariances, alternate);

        }

        /// <summary>
        ///   Tests whether the means of two samples are different.
        /// </summary>
        /// 
        /// <param name="sample1">The first sample.</param>
        /// <param name="sample2">The second sample.</param>
        /// <param name="hypothesizedDifference">The hypothesized sample difference.</param>
        /// <param name="assumeEqualVariances">True to assume equal variances, false otherwise. Default is true.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TwoSampleTTest(DescriptiveResult sample1, DescriptiveResult sample2,
     bool assumeEqualVariances = true, double hypothesizedDifference = 0,
     TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {
            // References: http://en.wikipedia.org/wiki/Student's_t-test#Worked_examples

            Compute(sample1.Mean, sample1.Variance, sample1.Count, sample2.Mean, sample2.Variance, sample2.Count,
                hypothesizedDifference, assumeEqualVariances, alternate);

        }
        /// <summary>
        ///   Tests whether the means of two samples are different.
        /// </summary>
        /// 
        /// <param name="mean1">The first sample's mean.</param>
        /// <param name="mean2">The second sample's mean.</param>
        /// <param name="var1">The first sample's variance.</param>
        /// <param name="var2">The second sample's variance.</param>
        /// <param name="samples1">The number of observations in the first sample.</param>
        /// <param name="samples2">The number of observations in the second sample.</param>
        /// <param name="assumeEqualVariances">True assume equal variances, false otherwise. Default is true.</param>
        /// <param name="hypothesizedDifference">The hypothesized sample difference.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TwoSampleTTest(double mean1, double var1, int samples1, double mean2, double var2, int samples2,
            bool assumeEqualVariances = true, double hypothesizedDifference = 0,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {

            Compute(mean1, var1, samples1, mean2, var2, samples2,
                hypothesizedDifference, assumeEqualVariances, alternate);

        }


        /// <summary>
        ///   Creates a new two-sample T-Test.
        /// </summary>
        protected TwoSampleTTest()
        {
        }


        /// <summary>
        ///   Computes the T Test.
        /// </summary>
        /// 
        protected void Compute(
            double x1, double s1, int n1,
            double x2, double s2, int n2,
            double hypothesizedDifference, bool equalVar,
            TwoSampleHypothesis alternate)
        {
            double df;

            this.AssumeEqualVariance = equalVar;
            this.EstimatedValue1 = x1;
            this.EstimatedValue2 = x2;

            if (AssumeEqualVariance)
            {
                // Assume the two samples share the same underlying population variance.
                Variance = ((n1 - 1) * s1 + (n2 - 1) * s2) / (n1 + n2 - 2);
                StandardError = Math.Sqrt(Variance) * Math.Sqrt(1.0 / n1 + 1.0 / n2);
                df = n1 + n2 - 2;
            }
            else
            {
                // Assume samples have unequal variances.
                StandardError = Math.Sqrt(s1 / n1 + s2 / n2);
                Variance = (s1 + s2) / 2.0;

                double r1 = s1 / n1;
                double r2 = s2 / n2;
                df = ((r1 + r2) * (r1 + r2)) / ((r1 * r1) / (n1 - 1) + (r2 * r2) / (n2 - 1));
            }



            this.ObservedDifference = (x1 - x2);
            this.HypothesizedDifference = hypothesizedDifference;

            this.Statistic = (ObservedDifference - HypothesizedDifference) / StandardError;
            this.StatisticDistribution = new TDistribution(df);
            this.Hypothesis = alternate;
            this.PValue = StatisticToPValue(Statistic);

        }


        /// <summary>
        ///   Converts a given p-value to a test statistic.
        /// </summary>
        /// 
        /// <param name="p">The p-value.</param>
        /// 
        /// <returns>The test statistic which would generate the given p-value.</returns>
        /// 
        public override double PValueToStatistic(double p)
        {
            return PValueToStatistic(p, StatisticDistribution, Hypothesis);
        }

        private double PValueToStatistic(double p, TDistribution distribution, TwoSampleHypothesis type)
        {
            double t;
            switch (type)
            {
                case TwoSampleHypothesis.FirstValueIsSmallerThanSecond:
                    t = distribution.InverseDistributionFunction(p);
                    break;
                case TwoSampleHypothesis.FirstValueIsGreaterThanSecond:
                    t = distribution.InverseDistributionFunction(1.0 - p);
                    break;
                case TwoSampleHypothesis.ValuesAreDifferent:
                    t = distribution.InverseDistributionFunction(1.0 - p / 2.0);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return t;
        }

        /// <summary>
        ///   Converts a given test statistic to a p-value.
        /// </summary>
        /// 
        /// <param name="x">The value of the test statistic.</param>
        /// 
        /// <returns>The p-value for the given statistic.</returns>
        /// 
        public override double StatisticToPValue(double x)
        {
            return StatisticToPValue(x, StatisticDistribution, Hypothesis);
        }

        private double StatisticToPValue(double t, TDistribution distribution, TwoSampleHypothesis type)
        {
            double p;
            switch (type)
            {
                case TwoSampleHypothesis.ValuesAreDifferent:
                    p = 2.0 * distribution.ComplementaryDistributionFunction(Math.Abs(t));
                    break;

                case TwoSampleHypothesis.FirstValueIsGreaterThanSecond:
                    p = distribution.ComplementaryDistributionFunction(t);
                    break;

                case TwoSampleHypothesis.FirstValueIsSmallerThanSecond:
                    p = distribution.DistributionFunction(t);
                    break;

                default:
                    throw new InvalidOperationException();
            }
            return p;
        }

    }
}
