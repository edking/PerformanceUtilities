using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerformanceUtilities.Analysis.StatisticalTests;
using PerformanceUtilities.ResultTypes;
using PerformanceUtilities.TestPatterns;

namespace Comparisons
{
    [TestClass]
    public class BoxVsGeneric
    {
        private const int cMinPerfIterations = 100000;
        private const string cResultFormat = "{0:N0} iterations took {1:n2} ms ({2:n3} seconds)";
        private readonly List<object> _boxList = new List<object>();

        private readonly List<int> _noboxList = new List<int>();
        private readonly Random _rng = new Random();

        [TestMethod]
        public void TestMethod1()
        {
            PerformanceResult boxResult = PerformancePatterns.RunPerformanceTest(cMinPerfIterations,
                (() =>
                {
                    if (_boxList.Count < (cMinPerfIterations >> 2))
                    {
                        _boxList.Add(_rng.Next());
                    }
                    else
                    {
                        int i = _rng.Next(0, _boxList.Count - 1);
                        var r = (int) _boxList[i];
                        _boxList.RemoveAt(i);
                    }
                }));

            Console.WriteLine(cResultFormat + " with boxing.", cMinPerfIterations, boxResult.TotalMilliseconds,
                boxResult.TotalSeconds);

            PerformanceResult noboxResult = PerformancePatterns.RunPerformanceTest(cMinPerfIterations,
                (() =>
                {
                    if (_noboxList.Count < (cMinPerfIterations >> 2))
                    {
                        _noboxList.Add(_rng.Next());
                    }
                    else
                    {
                        int i = _rng.Next(0, _noboxList.Count - 1);
                        int r = _noboxList[i];
                        _noboxList.RemoveAt(i);
                    }
                }));

            Console.WriteLine(cResultFormat + " with generic collection.", cMinPerfIterations,
                noboxResult.TotalMilliseconds, noboxResult.TotalSeconds);

            var comparison = new TwoSampleZTest(noboxResult.DescriptiveResult, boxResult.DescriptiveResult, 0.0,
                TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

            Console.WriteLine(noboxResult.ToString());
            Console.WriteLine(boxResult.ToString());

            Assert.IsTrue(comparison.Significant);
        }
    }
}