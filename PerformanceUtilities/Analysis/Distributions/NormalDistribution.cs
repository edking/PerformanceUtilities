using System;

namespace PerformanceUtilities.Analysis.Distributions
{
    public class NormalDistribution : Distribution
    {
        // Distribution parameters
        private const double p95 = 1.95996398454005423552;
        private static readonly NormalDistribution standard = new NormalDistribution {immutable = true};
        private double? entropy;
        private bool immutable;
        private double lnconstant; // log(1/sqrt(2*pi*variance))
        private double mean; // mean μ
        private double stdDev = 1; // standard deviation σ

        // Distribution measures

        // Derived measures
        private double variance = 1; // σ²

        /// <summary>
        ///     Constructs a Normal (Gaussian) distribution
        ///     with zero mean and unit standard deviation.
        /// </summary>
        public NormalDistribution()
        {
            initialize(mean, stdDev, stdDev*stdDev);
        }

        /// <summary>
        ///     Constructs a Normal (Gaussian) distribution
        ///     with given mean and unit standard deviation.
        /// </summary>
        /// <param name="mean">The distribution's mean value μ (mu).</param>
        public NormalDistribution(double mean)
        {
            initialize(mean, stdDev, stdDev*stdDev);
        }

        /// <summary>
        ///     Constructs a Normal (Gaussian) distribution
        ///     with given mean and standard deviation.
        /// </summary>
        /// <param name="mean">The distribution's mean value μ (mu).</param>
        /// <param name="stdDev">The distribution's standard deviation σ (sigma).</param>
        public NormalDistribution(double mean, double stdDev)
        {
            if (stdDev <= 0)
            {
                throw new ArgumentOutOfRangeException("stdDev",
                    "Standard deviation must be positive.");
            }

            initialize(mean, stdDev, stdDev*stdDev);
        }


        /// <summary>
        ///     Gets the Mean value μ (mu) for this Normal distribution.
        /// </summary>
        public override double Mean
        {
            get { return mean; }
        }

        /// <summary>
        ///     Gets the median for this distribution.
        /// </summary>
        /// <remarks>
        ///     The normal distribution's median value
        ///     equals its <see cref="Mean" /> value μ.
        /// </remarks>
        /// <value>
        ///     The distribution's median value.
        /// </value>
        public override double Median
        {
            get { return mean; }
        }

        /// <summary>
        ///     Gets the Variance σ² (sigma-squared), which is the square
        ///     of the standard deviation σ for this Normal distribution.
        /// </summary>
        public override double Variance
        {
            get { return variance; }
        }

        /// <summary>
        ///     Gets the Standard Deviation σ (sigma), which is the
        ///     square root of the variance for this Normal distribution.
        /// </summary>
        public override double StandardDeviation
        {
            get { return stdDev; }
        }

        /// <summary>
        ///     Gets the skewness for this distribution. In
        ///     the Normal distribution, this is always 0.
        /// </summary>
        public double Skewness
        {
            get { return 0; }
        }

        /// <summary>
        ///     Gets the excess kurtosis for this distribution.
        ///     In the Normal distribution, this is always 0.
        /// </summary>
        public double Kurtosis
        {
            get { return 0; }
        }

        /// <summary>
        ///     Gets the Entropy for this Normal distribution.
        /// </summary>
        public override double Entropy
        {
            get
            {
                if (!entropy.HasValue)
                    entropy = 0.5*(Math.Log(2.0*Math.PI*variance) + 1);

                return entropy.Value;
            }
        }

        /// <summary>
        ///     Gets the Standard Gaussian Distribution, with zero mean and unit variance.
        /// </summary>
        public static NormalDistribution Standard
        {
            get { return standard; }
        }

        /// <summary>
        ///     Gets the cumulative distribution function (cdf) for
        ///     the this Normal distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">
        ///     A single point in the distribution range.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         The Cumulative Distribution Function (CDF) describes the cumulative
        ///         probability that a given value or any value smaller than it will occur.
        ///     </para>
        ///     <para>
        ///         The calculation is computed through the relationship to the error function
        ///         as <see cref="Accord.Math.Special.Erfc">erfc</see>(-z/sqrt(2)) / 2.
        ///     </para>
        ///     <para>
        ///         References:
        ///         <list type="bullet">
        ///             <item>
        ///                 <description>
        ///                     Weisstein, Eric W. "Normal Distribution." From MathWorld--A Wolfram Web Resource.
        ///                     Available on: http://mathworld.wolfram.com/NormalDistribution.html
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     <a href="http://en.wikipedia.org/wiki/Normal_distribution#Cumulative_distribution_function">
        ///                         Wikipedia, The Free Encyclopedia. Normal distribution. Available on:
        ///                         http://en.wikipedia.org/wiki/Normal_distribution#Cumulative_distribution_function
        ///                     </a>
        ///                 </description>
        ///             </item>
        ///         </list>
        ///     </para>
        /// </remarks>
        /// <example>
        ///     See <see cref="NormalDistribution" />.
        /// </example>
        public override double DistributionFunction(double x)
        {
            return Normal.Function((x - mean)/stdDev);
        }

        /// <summary>
        ///     Gets the complementary cumulative distribution function
        ///     (ccdf) for this distribution evaluated at point <c>x</c>.
        ///     This function is also known as the Survival function.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public override double ComplementaryDistributionFunction(double x)
        {
            return Normal.Complemented((x - mean)/stdDev);
        }


        /// <summary>
        ///     Gets the inverse of the cumulative distribution function (icdf) for
        ///     this distribution evaluated at probability <c>p</c>. This function
        ///     is also known as the Quantile function.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The Inverse Cumulative Distribution Function (ICDF) specifies, for
        ///         a given probability, the value which the random variable will be at,
        ///         or below, with that probability.
        ///     </para>
        ///     <para>
        ///         The Normal distribution's ICDF is defined in terms of the
        ///         <see cref="Normal.Inverse">
        ///             standard normal inverse cumulative
        ///             distribution function I
        ///         </see>
        ///         as <c>ICDF(p) = μ + σ * I(p)</c>.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     See <see cref="NormalDistribution" />.
        /// </example>
        public override double InverseDistributionFunction(double p)
        {
            double inv = Normal.Inverse(p);

            double icdf = mean + stdDev*inv;

            return icdf;
        }

        /// <summary>
        ///     Gets the probability density function (pdf) for
        ///     the Normal distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">
        ///     A single point in the distribution range. For a
        ///     univariate distribution, this should be a single
        ///     double value. For a multivariate distribution,
        ///     this should be a double array.
        /// </param>
        /// <returns>
        ///     The probability of <c>x</c> occurring
        ///     in the current distribution.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The Probability Density Function (PDF) describes the
        ///         probability that a given value <c>x</c> will occur.
        ///     </para>
        ///     <para>
        ///         The Normal distribution's PDF is defined as
        ///         <c>PDF(x) = c * exp((x - μ / σ)²/2)</c>.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     See <see cref="NormalDistribution" />.
        /// </example>
        public override double ProbabilityDensityFunction(double x)
        {
            double z = (x - mean)/stdDev;
            double lnp = lnconstant - z*z*0.5;

            return Math.Exp(lnp);
        }

        /// <summary>
        ///     Gets the probability density function (pdf) for
        ///     the Normal distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">
        ///     A single point in the distribution range. For a
        ///     univariate distribution, this should be a single
        ///     double value. For a multivariate distribution,
        ///     this should be a double array.
        /// </param>
        /// <returns>
        ///     The probability of <c>x</c> occurring
        ///     in the current distribution.
        /// </returns>
        /// <remarks>
        ///     The Probability Density Function (PDF) describes the
        ///     probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// <example>
        ///     See <see cref="NormalDistribution" />.
        /// </example>
        public override double LogProbabilityDensityFunction(double x)
        {
            double z = (x - mean)/stdDev;
            double lnp = lnconstant - z*z*0.5;

            return lnp;
        }

        /// <summary>
        ///     Gets the Z-Score for a given value.
        /// </summary>
        public double ZScore(double x)
        {
            return (x - mean)/stdDev;
        }


        private void initialize(double mu, double dev, double var)
        {
            mean = mu;
            stdDev = dev;
            variance = var;

            // Compute derived values
            lnconstant = -Math.Log(Constants.Sqrt2PI*dev);
        }
    }
}