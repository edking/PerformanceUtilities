using System;

namespace PerformanceUtilities.Analysis.Distributions
{
    /// <summary>
    ///     Abstract class for univariate continuous probability Distributions.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A probability distribution identifies either the probability of each value of an
    ///         unidentified random variable (when the variable is discrete), or the probability
    ///         of the value falling within a particular interval (when the variable is continuous).
    ///     </para>
    ///     <para>
    ///         The probability distribution describes the range of possible values that a random
    ///         variable can attain and the probability that the value of the random variable is
    ///         within any (measurable) subset of that range.
    ///     </para>
    ///     <para>
    ///         The function describing the probability that a given value will occur is called
    ///         the probability function (or probability density function, abbreviated PDF), and
    ///         the function describing the cumulative probability that a given value or any value
    ///         smaller than it will occur is called the distribution function (or cumulative
    ///         distribution function, abbreviated CDF).
    ///     </para>
    ///     <para>
    ///         References:
    ///         <list type="bullet">
    ///             <item>
    ///                 <description>
    ///                     <a href="http://en.wikipedia.org/wiki/Probability_distribution">
    ///                         Wikipedia, The Free Encyclopedia. Probability distribution. Available on:
    ///                         http://en.wikipedia.org/wiki/Probability_distribution
    ///                     </a>
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <description>
    ///                     <a href="http://mathworld.wolfram.com/StatisticalDistribution.html">
    ///                         Weisstein, Eric W. "Statistical Distribution." From MathWorld--A Wolfram Web Resource.
    ///                         http://mathworld.wolfram.com/StatisticalDistribution.html
    ///                     </a>
    ///                 </description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    public abstract class Distribution
    {
        [NonSerialized] private double? median;

        [NonSerialized] private double? mode;
        [NonSerialized] private double? stdDev;


        /// <summary>
        ///     Gets the mean for this distribution.
        /// </summary>
        /// <value>The distribution's mean value.</value>
        public abstract double Mean { get; }

        /// <summary>
        ///     Gets the variance for this distribution.
        /// </summary>
        /// <value>The distribution's variance.</value>
        public abstract double Variance { get; }

        /// <summary>
        ///     Gets the entropy for this distribution.
        /// </summary>
        /// <value>The distribution's entropy.</value>
        public abstract double Entropy { get; }

        /// <summary>
        ///     Gets the median for this distribution.
        /// </summary>
        /// <value>The distribution's median value.</value>
        public virtual double Median
        {
            get
            {
                if (median == null)
                    median = InverseDistributionFunction(0.5);

                return median.Value;
            }
        }

        /// <summary>
        ///     Gets the Standard Deviation (the square root of
        ///     the variance) for the current distribution.
        /// </summary>
        /// <value>The distribution's standard deviation.</value>
        public virtual double StandardDeviation
        {
            get
            {
                if (!stdDev.HasValue)
                    stdDev = Math.Sqrt(Variance);
                return stdDev.Value;
            }
        }


        /// <summary>
        ///     Gets the cumulative distribution function (cdf) for
        ///     this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">
        ///     A single point in the distribution range.
        /// </param>
        /// <remarks>
        ///     The Cumulative Distribution Function (CDF) describes the cumulative
        ///     probability that a given value or any value smaller than it will occur.
        /// </remarks>
        public abstract double DistributionFunction(double x);

        /// <summary>
        ///     Gets the cumulative distribution function (cdf) for this
        ///     distribution in the semi-closed interval (a; b] given as
        ///     <c>P(a &lt; X ≤ b)</c>.
        /// </summary>
        /// <param name="a">The start of the semi-closed interval (a; b].</param>
        /// <param name="b">The end of the semi-closed interval (a; b].</param>
        /// <remarks>
        ///     The Cumulative Distribution Function (CDF) describes the cumulative
        ///     probability that a given value or any value smaller than it will occur.
        /// </remarks>
        public virtual double DistributionFunction(double a, double b)
        {
            if (a > b)
            {
                throw new ArgumentOutOfRangeException("b",
                    "The start of the interval a must be smaller than b.");
            }
            if (a == b)
            {
                return 0;
            }

            return DistributionFunction(b) - DistributionFunction(a);
        }

        /// <summary>
        ///     Gets the complementary cumulative distribution function
        ///     (ccdf) for this distribution evaluated at point <c>x</c>.
        ///     This function is also known as the Survival function.
        /// </summary>
        /// <param name="x">
        ///     A single point in the distribution range.
        /// </param>
        /// <remarks>
        ///     The Complementary Cumulative Distribution Function (CCDF) is
        ///     the complement of the Cumulative Distribution Function, or 1
        ///     minus the CDF.
        /// </remarks>
        public virtual double ComplementaryDistributionFunction(double x)
        {
            return 1.0 - DistributionFunction(x);
        }

        /// <summary>
        ///     Gets the inverse of the cumulative distribution function (icdf) for
        ///     this distribution evaluated at probability <c>p</c>. This function
        ///     is also known as the Quantile function.
        /// </summary>
        /// <remarks>
        ///     The Inverse Cumulative Distribution Function (ICDF) specifies, for
        ///     a given probability, the value which the random variable will be at,
        ///     or below, with that probability.
        /// </remarks>
        /// <param name="p">A probability value between 0 and 1.</param>
        /// <returns>
        ///     A sample which could original the given probability
        ///     value when applied in the <see cref="DistributionFunction(double)" />.
        /// </returns>
        public virtual double InverseDistributionFunction(double p)
        {
            double lower = 0;
            double upper = 0;

            double f = DistributionFunction(0);

            if (f > p)
            {
                while (f > p && !Double.IsInfinity(lower))
                {
                    upper = lower;
                    lower = 2*lower - 1;
                    f = DistributionFunction(lower);
                }
            }
            else
            {
                while (f < p && !Double.IsInfinity(upper))
                {
                    lower = upper;
                    upper = 2*upper + 1;
                    f = DistributionFunction(upper);
                }
            }


            if (Double.IsNegativeInfinity(lower))
                lower = Double.MinValue;

            if (Double.IsPositiveInfinity(upper))
                upper = Double.MaxValue;

            double value = BrentSearch.Find(DistributionFunction, p, lower, upper);

            return value;
        }

        /// <summary>
        ///     Gets the first derivative of the
        ///     <see cref="InverseDistributionFunction">
        ///         inverse distribution function
        ///     </see>
        ///     (icdf) for this distribution evaluated
        ///     at probability <c>p</c>.
        /// </summary>
        /// <param name="p">A probability value between 0 and 1.</param>
        public virtual double QuantileDensityFunction(double p)
        {
            return 1.0/ProbabilityDensityFunction(InverseDistributionFunction(p));
        }

        /// <summary>
        ///     Gets the probability density function (pdf) for
        ///     this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">
        ///     A single point in the distribution range.
        /// </param>
        /// <remarks>
        ///     The Probability Density Function (PDF) describes the
        ///     probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// <returns>
        ///     The probability of <c>x</c> occurring
        ///     in the current distribution.
        /// </returns>
        public abstract double ProbabilityDensityFunction(double x);


        /// <summary>
        ///     Gets the log-probability density function (pdf) for
        ///     this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">
        ///     A single point in the distribution range.
        /// </param>
        /// <remarks>
        ///     The Probability Density Function (PDF) describes the
        ///     probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// <returns>
        ///     The logarithm of the probability of <c>x</c>
        ///     occurring in the current distribution.
        /// </returns>
        public virtual double LogProbabilityDensityFunction(double x)
        {
            return Math.Log(ProbabilityDensityFunction(x));
        }

        /// <summary>
        ///     Gets the hazard function, also known as the failure rate or
        ///     the conditional failure density function for this distribution
        ///     evaluated at point <c>x</c>.
        /// </summary>
        /// <remarks>
        ///     The hazard function is the ratio of the probability
        ///     density function f(x) to the survival function, S(x).
        /// </remarks>
        /// <param name="x">
        ///     A single point in the distribution range.
        /// </param>
        /// <returns>
        ///     The conditional failure density function <c>h(x)</c>
        ///     evaluated at <c>x</c> in the current distribution.
        /// </returns>
        public virtual double HazardFunction(double x)
        {
            return ProbabilityDensityFunction(x)/ComplementaryDistributionFunction(x);
        }

        /// <summary>
        ///     Gets the cumulative hazard function for this
        ///     distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">
        ///     A single point in the distribution range.
        /// </param>
        /// <returns>
        ///     The cumulative hazard function <c>H(x)</c>
        ///     evaluated at <c>x</c> in the current distribution.
        /// </returns>
        public virtual double CumulativeHazardFunction(double x)
        {
            return -Math.Log(ComplementaryDistributionFunction(x));
        }
    }
}