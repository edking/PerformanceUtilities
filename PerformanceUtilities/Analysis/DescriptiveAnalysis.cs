using System;
using System.Collections.Generic;
using System.Linq;
using PerformanceUtilities.ResultTypes;

namespace PerformanceUtilities.Analysis
{
    /// <summary>
    ///     DescriptiveAnalysis class calculates descriptive statistics
    /// </summary>
    public class DescriptiveAnalysis
    {
        private readonly List<double> _data;
        private readonly DescriptiveResult _descriptiveResult = new DescriptiveResult();
        private List<double> _sortedData;

        #region Constructors

        /// <summary>
        ///     DescriptiveAnalysis analysis default constructor
        /// </summary>
        public DescriptiveAnalysis()
        {
        }

        // default empty constructor

        /// <summary>
        ///     DescriptiveAnalysis analysis constructor
        /// </summary>
        /// <param name="dataVariable"> Data List&lt;double&gt; </param>
        public DescriptiveAnalysis(List<double> dataVariable)
        {
            _data = dataVariable;
        }

        #endregion //  Constructors

        public List<double> RawData
        {
            get { return _data; }
        }

        /// <summary>
        ///     DescriptiveAnalysis results
        /// </summary>
        public DescriptiveResult Result
        {
            get { return _descriptiveResult; }
        }

        /// <summary>
        ///     Run the analysis to obtain descriptive information of the _data
        /// </summary>
        public void Analyze(bool includeRawData)
        {
            // initializations
            Result.Count = _data.Count;

            Result.Min = _data.Min();
            Result.Max = _data.Max();
            Result.Range = Result.Max - Result.Min;
            Result.Mean = _data.Average();
            Result.Sum = _data.Sum();

            Result.SumOfError = _data.Sum(p => Math.Abs(p - Result.Mean));
            Result.SumOfErrorSquare = _data.Sum(p => (p - Result.Mean)*(p - Result.Mean));
            Result.Variance = Result.SumOfErrorSquare/((double) Result.Count - 1);
            Result.StdDev = Math.Sqrt(Result.Variance);

            double skewCum = _data.Sum(t => Math.Pow((t - Result.Mean), 3))/Result.Count;
            Result.Skew = ((Result.Count*Result.Count)/((Result.Count - 1)*(Result.Count - 2)))*
                          (skewCum/Math.Pow(Result.StdDev, 3));

            // calculate quartiles
            _sortedData = new List<double>();
            _sortedData.AddRange(_data);
            _sortedData.Sort();

            // copy the sorted _data to result object so that
            // user can calculate percentile easily
            for (var i = 0; i < 100; i++)
            {
                Result._percentiles[i] = CalcPercentile(_sortedData, i);
            }

            if (includeRawData)
            {
                Result.RawData = RawData;
            }
        }

        public void AnalyzeHistogram(int buckets)
        {
            double step = Result.Max/buckets;

            AnalyzeHistogram(0, step);
        }

        public void AnalyzeHistogram(double min, double step)
        {
            Result.Histogram = new List<Bucket>();
            double val = min;

            while (val <= Result.Max)
            {
                var thisBucket = new Bucket
                                 {
                                     RangeLow = val,
                                     RangeHigh = val + step,
                                     Count = _data.Count(p => p > val && p <= (val + step))
                                 };
                Result.Histogram.Add(thisBucket);
                val += step;
            }
        }


        /// <summary>
        ///     Calculate percentile of a sorted _data set
        /// </summary>
        /// <param name="sortedData"> </param>
        /// <param name="p"> </param>
        /// <returns> </returns>
        private static double CalcPercentile(List<double> sortedData, int p)
        {
            // algo derived from Aczel pg 15 bottom
            if (p >= 100.0d) return sortedData[sortedData.Count - 1];

            double position = (sortedData.Count + 1)*p/100.0;
            double leftNumber;
            double rightNumber;

            double n = p/100.0d*(sortedData.Count - 1) + 1.0d;

            if (position >= 1)
            {
                leftNumber = sortedData[(int) Math.Floor(n) - 1];
                rightNumber = sortedData[(int) Math.Floor(n)];
            }
            else
            {
                leftNumber = sortedData[0]; // first _data
                rightNumber = sortedData[1]; // first _data
            }

            if (Math.Abs(leftNumber - rightNumber) < 0.0000001)
                return leftNumber;

            double part = n - Math.Floor(n);
            return leftNumber + part*(rightNumber - leftNumber);
        }
    }
}