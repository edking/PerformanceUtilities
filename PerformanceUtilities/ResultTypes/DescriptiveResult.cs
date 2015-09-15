using System.Collections.Generic;
using PerformanceUtilities.ResultTypes.Formatters;

namespace PerformanceUtilities.ResultTypes
{
    /// <summary>
    ///     The result class the holds the analysis results
    /// </summary>
    public class DescriptiveResult : ResultBase
    {
        internal readonly double[] _percentiles = new double[100];
        public List<Bucket> Histogram;

        public List<double> RawData;

        public DescriptiveResult() : base(new DescriptivePrintFormat(), 2)
        {
        }

        public DescriptiveResult(FormatResultsBase formatter, int precision)
            : base(formatter, precision)
        {
        }

        /// <summary>
        ///     Count
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///     Interquartile range
        /// </summary>
        public double IQR
        {
            get { return _percentiles[75] - _percentiles[25]; }
        }

        /// <summary>
        ///     Maximum value
        /// </summary>
        public double Max { get; set; }

        /// <summary>
        ///     Arithmatic mean
        /// </summary>
        public double Mean { get; set; }

        /// <summary>
        ///     Median, or second quartile, or at 50 percentile
        /// </summary>
        public double Median
        {
            get { return _percentiles[50]; }
        }

        /// <summary>
        ///     Minimum value
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        ///     The range of the values
        /// </summary>
        public double Range { get; set; }

        /// <summary>
        ///     Skew of the data distribution
        /// </summary>
        public double Skew { get; set; }

        /// <summary>
        ///     Sample standard deviation
        /// </summary>
        public double StdDev { get; set; }

        /// <summary>
        ///     Sum
        /// </summary>
        public double Sum { get; set; }

        /// <summary>
        ///     Sum of Error
        /// </summary>
        internal double SumOfError { get; set; }

        /// <summary>
        ///     The sum of the squares of errors
        /// </summary>
        internal double SumOfErrorSquare { get; set; }


        /// <summary>
        ///     First quartile, at 25 percentile
        /// </summary>
        public double FirstQuartile
        {
            get { return _percentiles[25]; }
        }


        /// <summary>
        ///     Third quartile, at 75 percentile
        /// </summary>
        public double ThirdQuartile
        {
            get { return _percentiles[75]; }
        }

        /// <summary>
        ///     Sample variance
        /// </summary>
        public double Variance { get; set; }

        /// <summary>
        ///     Percentile
        /// </summary>
        /// <param name="percent"> Pecentile, between 0 to 100 </param>
        /// <returns> Percentile </returns>
        public double Percentile(int percent)
        {
            return _percentiles[percent];
        }
    }
}