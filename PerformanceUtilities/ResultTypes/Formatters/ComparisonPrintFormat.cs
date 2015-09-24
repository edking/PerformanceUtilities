using System;
using System.Collections.Generic;
using PerformanceUtilities.Analysis.StatisticalTests;

namespace PerformanceUtilities.ResultTypes.Formatters
{
    public class ComparisonPrintFormat : FormatResultsBase
    {
        private readonly Dictionary<TwoSampleHypothesis, string> _hypothesisLabels = new Dictionary
            <TwoSampleHypothesis, string>
                                                                                     {
                                                                                         {
                                                                                             TwoSampleHypothesis
                                                                                             .ValuesAreDifferent,
                                                                                             "The values are different by at least "
                                                                                         },
                                                                                         {
                                                                                             TwoSampleHypothesis
                                                                                             .FirstValueIsGreaterThanSecond,
                                                                                             "{0} is greater than {1} by at least "
                                                                                         },
                                                                                         {
                                                                                             TwoSampleHypothesis
                                                                                             .FirstValueIsSmallerThanSecond,
                                                                                             "{0} is less than {1} by at least "
                                                                                         },
                                                                                     };

        public override string Format(ResultBase result)
        {
            var r = result as ComparisonResult;

            var f = "{2:" + NumFormat + "}";
            var o1 = "{3:" + NumFormat + "}";
            var o2 = "{4:" + NumFormat + "}";
            var fmt = _hypothesisLabels[r.Hypothesis] + f + "ms. (Observed: " + o1 + " vs " + o2 + ")";
            string rv;

            if (!r.Significant)
            {
                if (r.HypothesizedDifference == 0)
                {
                    fmt = "Difference between {0} and {1} is not significant.";
                }
                else
                {
                    fmt = "Difference between {0} and {1} is not significant or is less than " + f + "ms. (Observed: " +
                          o1 + " vs " + o2 + ")";
                }
            }
            rv = String.Format(fmt, r.FirstSample.Name, r.SecondSample.Name, Math.Abs(r.HypothesizedDifference),
                r.FirstSample.Mean, r.SecondSample.Mean);
            return rv;
        }
    }
}