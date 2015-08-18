using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerformanceUtilities.Analysis.StatisticalTests;
using PerformanceUtilities.ResultTypes;
using PerformanceUtilities.TestPatterns;

namespace PrimesComparison
{
    [TestClass]
    public class PrimesComparison
    {
        private const int cDegreeConcurrency = 16;
        private const int cMinPerfIterations = 200;
        private const int cMinPrime = 2;
        private const int cMaxPrime = 100000;
        private const string cResultFormat = "{0:N0} iterations took {1:n2} ms ({2:n3} seconds)";

        [TestMethod]
        public void TestMethod1()
        {
            var concurrent = new ConcurrentPrimes();
            concurrent.Init(cMinPrime, cMaxPrime, cDegreeConcurrency);

            PerformanceResult concResult = PerformancePatterns.RunPerformanceTest(cMinPerfIterations,
                (() => concurrent.Execute()));

            Console.WriteLine(cResultFormat + " with concurrent collection.", cMinPerfIterations,
                concResult.TotalMilliseconds,
                concResult.TotalSeconds);

            var plinqed = new PlinqPrimes();
            plinqed.Init(cMinPrime, cMaxPrime, cDegreeConcurrency);

            PerformanceResult pResult = PerformancePatterns.RunPerformanceTest(cMinPerfIterations,
                (() => plinqed.Execute()));

            Console.WriteLine(cResultFormat + " with plinq.", cMinPerfIterations, pResult.TotalMilliseconds,
                pResult.TotalSeconds);

            var comparison = new TwoSampleZTest(pResult.DescriptiveResult, concResult.DescriptiveResult, 0.0,
                TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

            Console.WriteLine(concResult.ToString());
            Console.WriteLine(pResult.ToString());

            Assert.IsTrue(comparison.Significant);
            Assert.AreEqual(concurrent.Primes.Count,plinqed.Primes.Count);
        }
    }
}