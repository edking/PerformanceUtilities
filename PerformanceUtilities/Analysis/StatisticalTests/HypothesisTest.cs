using PerformanceUtilities.Analysis.Distributions;

namespace PerformanceUtilities.Analysis.StatisticalTests
{
    public abstract class HypothesisTest<T> where T : Distribution
    {
        private double alpha = 0.05;

        /// <summary>
        ///     Gets the distribution associated
        ///     with the test statistic.
        /// </summary>
        public T StatisticDistribution { get; protected set; }

        /// <summary>
        ///     Gets the P-value associated with this test.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         In statistical hypothesis testing, the p-value is the probability of
        ///         obtaining a test statistic at least as extreme as the one that was
        ///         actually observed, assuming that the null hypothesis is true.
        ///     </para>
        ///     <para>
        ///         The lower the p-value, the less likely the result can be explained
        ///         by chance alone, assuming the null hypothesis is true.
        ///     </para>
        /// </remarks>
        public double PValue { get; protected set; }

        /// <summary>
        ///     Gets the test statistic.
        /// </summary>
        public double Statistic { get; protected set; }

        /// <summary>
        ///     Gets the significance level for the
        ///     test. Default value is 0.05 (5%).
        /// </summary>
        public double Size
        {
            get { return alpha; }
            set { alpha = value; }
        }

        /// <summary>
        ///     Gets whether the null hypothesis should be rejected.
        /// </summary>
        /// <remarks>
        ///     A test result is said to be statistically significant when the
        ///     result would be very unlikely to have occurred by chance alone.
        /// </remarks>
        public bool Significant
        {
            get { return PValue < alpha; }
        }

        /// <summary>
        ///     Gets the hypothesized difference between the two estimated values.
        /// </summary>
        public double HypothesizedDifference { get; protected set; }

        /// <summary>
        ///     Gets the actual difference between the two estimated values.
        /// </summary>
        public double ObservedDifference { get; protected set; }

        /// <summary>
        ///     Gets the standard error for the difference.
        /// </summary>
        public double StandardError { get; protected set; }

        /// <summary>
        ///     Gets the estimated value for the first sample.
        /// </summary>
        public double EstimatedValue1 { get; protected set; }

        /// <summary>
        ///     Gets the estimated value for the second sample.
        /// </summary>
        public double EstimatedValue2 { get; protected set; }

        /// <summary>
        ///     Gets the 95% confidence interval for the
        ///     <see cref="ObservedDifference" /> statistic.
        /// </summary>
        public double[] Confidence { get; protected set; }


        /// <summary>
        ///     Gets a confidence interval for the <see cref="ObservedDifference" />
        ///     statistic within the given confidence level percentage.
        /// </summary>
        /// <param name="percent">The confidence level. Default is 0.95.</param>
        /// <returns>A confidence interval for the estimated value.</returns>
        public double[] GetConfidenceInterval(double percent = 0.95)
        {
            double u = PValueToStatistic(1.0 - percent);

            var rv = new double[2];

            rv[0] = ObservedDifference - u*StandardError;
            rv[1] = ObservedDifference + u*StandardError;

            return rv;
        }

        /// <summary>
        ///     Converts a given test statistic to a p-value.
        /// </summary>
        /// <param name="x">The value of the test statistic.</param>
        /// <returns>The p-value for the given statistic.</returns>
        public abstract double StatisticToPValue(double x);

        /// <summary>
        ///     Converts a given p-value to a test statistic.
        /// </summary>
        /// <param name="p">The p-value.</param>
        /// <returns>The test statistic which would generate the given p-value.</returns>
        public abstract double PValueToStatistic(double p);
    }
}