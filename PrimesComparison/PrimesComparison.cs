using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerformanceUtilities.Analysis.StatisticalTests;
using PerformanceUtilities.TestPatterns;

namespace PrimesComparison
{
    [TestClass]
    public class PrimesComparison
    {
        private const int cDegreeConcurrency = 16;
        private const int cMinPerfIterations = 100;
        private const int cMinPrime = 2;
        private const int cMaxPrime = 100000;

        [TestMethod]
        public void CompareManualConcurrencyVsLinq()
        {
            var concurrent = new ConcurrentPrimes();
            concurrent.Init(cMinPrime, cMaxPrime, cDegreeConcurrency);
            var plinqed = new PlinqPrimes();
            plinqed.Init(cMinPrime, cMaxPrime, cDegreeConcurrency);

            bool significant = PerformancePatterns.RunPerformanceComparison(cMinPerfIterations,
                "concurrent collection", (concurrent.Execute),
                "plinq", (plinqed.Execute),
                250.0, TwoSampleHypothesis.FirstValueIsGreaterThanSecond, true);

            Assert.IsTrue(significant);
            Assert.AreEqual(concurrent.Primes.Count, plinqed.Primes.Count);
        }
    }
}