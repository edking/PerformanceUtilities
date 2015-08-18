using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerformanceUtilities.Analysis.StatisticalTests;
using PerformanceUtilities.ResultTypes;
using PerformanceUtilities.TestPatterns;

namespace Comparisons
{
    [TestClass]
    public class ContentionVsConcurrent
    {
        private const int cDegreeConcurrency = 16;
        private const int cMinPerfIterations = 100000;
        private const string cResultFormat = "{0:N0} iterations took {1:n2} ms ({2:n3} seconds)";
        private static readonly List<int> _bigList = new List<int>();

        private static readonly BlockingCollection<int> _concurrentQ = new BlockingCollection<int>();
        private static readonly object _collectionLock = new object();
        private static Random _rng = new Random();

        [TestMethod]
        [TestCategory("Performance")]
        public void Test1()
        {
            PerformanceResult contentionResult = PerformancePatterns.RunConcurrentPerformanceTest(cMinPerfIterations,
                cDegreeConcurrency, (() =>
                {
                    lock (_collectionLock)
                    {
                        int v = _rng.Next(1, cMinPerfIterations);
                        if (_bigList.Count > (cMinPerfIterations >> 2))
                        {
                            var s = _bigList[v%_bigList.Count];
                            _bigList.RemoveAt(v % _bigList.Count);
                        }
                        else
                        {
                            _bigList.Add(v);
                        }
                    }
                }));

            Console.WriteLine(cResultFormat + " with contention.", cMinPerfIterations, contentionResult.TotalMilliseconds, contentionResult.TotalSeconds);

            PerformanceResult concurrentResult = PerformancePatterns.RunConcurrentPerformanceTest(cMinPerfIterations,
                cDegreeConcurrency, (() =>
                {
                    int v = _rng.Next(1, cMinPerfIterations);
                    if (_concurrentQ.Count > (cMinPerfIterations >> 2))
                    {
                        var s = _concurrentQ.Take();
                    }
                    else
                    {
                        _concurrentQ.Add(v);
                    }
                }));
            Console.WriteLine(cResultFormat + " with concurrent collection.", cMinPerfIterations, concurrentResult.TotalMilliseconds, concurrentResult.TotalSeconds);

            var comparison = new TwoSampleZTest(concurrentResult.DescriptiveResult, contentionResult.DescriptiveResult, 0.0, TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

            Console.WriteLine(concurrentResult.ToString());
            Console.WriteLine(contentionResult.ToString());

            Assert.IsTrue(comparison.Significant);

        }
    }
}