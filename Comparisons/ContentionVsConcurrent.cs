using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerformanceUtilities.Analysis.StatisticalTests;
using PerformanceUtilities.MSExtensions;
using PerformanceUtilities.TestPatterns;

namespace Comparisons
{
    [TestClass]
    public class ContentionVsConcurrent
    {
        private const int cDegreeConcurrency = 16;
        private const int cMinPerfIterations = 100000;
        private static readonly List<int> _bigList = new List<int>();

        private static readonly BlockingCollection<int> _concurrentQ = new BlockingCollection<int>();
        private static readonly object _collectionLock = new object();
        private static readonly ThreadSafeRandom _rng = new ThreadSafeRandom();

        [TestMethod]
        [TestCategory("Performance")]
        public void LockVsConcurrentQueue()
        {
            bool significant = PerformancePatterns.RunConcurrentPerformanceComparison(cMinPerfIterations,
                cDegreeConcurrency,
                "Lock{}", (() =>
                {
                    lock (_collectionLock)
                    {
                        int v = _rng.Next(1, cMinPerfIterations);
                        if (_bigList.Count > (cMinPerfIterations >> 2))
                        {
                            var s = _bigList[v%_bigList.Count];
                            _bigList.RemoveAt(v%_bigList.Count);
                        }
                        else
                        {
                            _bigList.Add(v);
                        }
                    }
                }),
                "BlockingCollection<int>", (() =>
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
                }),
                0.0, TwoSampleHypothesis.FirstValueIsGreaterThanSecond, true);

            Assert.IsTrue(significant);
        }
    }
}