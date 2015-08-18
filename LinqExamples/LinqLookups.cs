using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerformanceUtilities.Analysis.StatisticalTests;
using PerformanceUtilities.ResultTypes;
using PerformanceUtilities.TestPatterns;

namespace LinqExamples
{
    [TestClass]
    public class LinqLookups
    {
        // Rule: need lots of data -- size like prod will see it
        private const int cMaxList = (1 << 9);

        private const int cDegreeConcurrency = 4;

        // Rule: do it a lot of times to smooth out the rough edges
        private const int cMinPerfIterations = 10000;

        private readonly Random _rng = new Random();
        private const string cResultFormat = "{0:N0} iterations took {1:n2} ms ({2:n3} seconds)";
        private List<PayRecord> _bigList;

        private ILookup<int, PayRecord> _myLookup;

        [TestInitialize]
        [TestCategory("Performance")]
        public void TestSetup()
        {
            _bigList = new List<PayRecord>(cMaxList);
            for (int i = 0; i < cMaxList; i++)
            {
                string cv = "++";

                switch (_rng.Next(1, 4))
                {
                    case 1:
                        cv = "AB";
                        break;
                    case 2:
                        cv = "CD";
                        break;
                    case 3:
                        cv = "XY";
                        break;
                    case 4:
                        cv = "**";
                        break;
                }

                var pr = new PayRecord
                {
                    PayId = _rng.Next(1000000, 1000300),
                    PaymentAmount = Convert.ToDecimal(_rng.Next(1, 10000))/100M,
                    CategoryCode = cv
                };

                _bigList.Add(pr);
            }
            _myLookup = _bigList.ToLookup(p => p.PayId, p => p);
        }

        public void PrintResult(PerformanceResult res)
        {
            Console.WriteLine(res.ToString());
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void WithLookup()
        {
            PerformanceResult result = PerformancePatterns.RunPerformanceTest(cMinPerfIterations,
                (() => FuncWithLookup()));

            Console.WriteLine(cResultFormat + " with lookup.", cMinPerfIterations, result.TotalMilliseconds,
                result.TotalSeconds);

            PrintResult(result);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void CompareMethods()
        {
            PerformanceResult resultWith = PerformancePatterns.RunPerformanceTest(cMinPerfIterations,
                (() => FuncWithLookup()));

            Console.WriteLine(cResultFormat + " with lookup.", cMinPerfIterations, resultWith.TotalMilliseconds,
                resultWith.TotalSeconds);

            PerformanceResult resultWithout = PerformancePatterns.RunPerformanceTest(cMinPerfIterations,
                (() =>
                {
                    var payids = (from payment in _bigList
                                  select payment.PayId).Distinct();

                    var myStuff = (from payid in payids.AsParallel()
                                   select new
                                   {
                                       PayId = payid,
                                       coverages =
                                           string.Join(", ", _bigList.Where(x => x.PayId == payid).Select(x => x.CategoryCode)),
                                       total = _bigList.Where(x => x.PayId == payid).Select(x => x.PaymentAmount).Sum()
                                   }).ToList();

                    var myList = myStuff.ToList();

                }));

            Console.WriteLine(cResultFormat + " without lookup.", cMinPerfIterations, resultWithout.TotalMilliseconds,
                resultWithout.TotalSeconds);

            var comparison = new TwoSampleZTest(resultWith.DescriptiveResult, resultWithout.DescriptiveResult, 0.0, TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

            Assert.IsTrue(comparison.Significant);

        }

        
        
        [TestMethod]
        [TestCategory("Performance")]
        public void WithConcurrentLookup()
        {
            PerformanceResult result = PerformancePatterns.RunConcurrentPerformanceTest(cMinPerfIterations,
                cDegreeConcurrency,
                (Action) (FuncWithLookup));

            Console.WriteLine(cResultFormat + " CONCURRENT={3} with lookup.", cMinPerfIterations, result.TotalMilliseconds,
                result.TotalSeconds, cDegreeConcurrency);

            PrintResult(result);
        }


        [TestMethod]
        [TestCategory("Performance")]
        public void WithoutLookupParallel()
        {
            PerformanceResult result = PerformancePatterns.RunPerformanceTest(cMinPerfIterations,
                (() =>
                {
                    var payids = (from payment in _bigList
                                  select payment.PayId).Distinct();

                    var myStuff = (from payid in payids.AsParallel()
                                  select new
                                  {
                                      PayId = payid,
                                      coverages =
                                          string.Join(", ", _bigList.Where(x => x.PayId == payid).Select(x => x.CategoryCode)),
                                      total = _bigList.Where(x => x.PayId == payid).Select(x => x.PaymentAmount).Sum()
                                  }).ToList();

                    var myList = myStuff.ToList();

                }));

            Console.WriteLine(cResultFormat + " with lookup.", cMinPerfIterations, result.TotalMilliseconds,
                result.TotalSeconds);
            PrintResult(result);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void WithPreLookup()
        {
            PerformanceResult result = PerformancePatterns.RunPerformanceTest(cMinPerfIterations,
                (() =>
                {
                    var payids = (from payment in _bigList
                                  select payment.PayId).Distinct();

                    var myStuff = from payid in payids
                                  select new
                                  {
                                      PayId = payid,
                                      coverages = string.Join(",", _myLookup[payid].Select(x => x.CategoryCode)),
                                      total = _myLookup[payid].Select(x => x.PaymentAmount).Sum()
                                  };

                    var myList = myStuff.ToList();

                }));

            Console.WriteLine(cResultFormat + " with pre-lookup.", cMinPerfIterations, result.TotalMilliseconds,
                result.TotalSeconds);
            PrintResult(result);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void WithoutLookup()
        {
            PerformanceResult result = PerformancePatterns.RunPerformanceTest(cMinPerfIterations,
                (() =>
                {
                    var payids = (from payment in _bigList
                        select payment.PayId).Distinct();

                    var myStuff = from payid in payids
                        select new
                        {
                            PayId = payid,
                            coverages =
                                string.Join(", ", _bigList.Where(x => x.PayId == payid).Select(x => x.CategoryCode)),
                            total = _bigList.Where(x => x.PayId == payid).Select(x => x.PaymentAmount).Sum()
                        };

                    var myList = myStuff.ToList();

                }));

            Console.WriteLine(cResultFormat + " without lookup.", cMinPerfIterations, result.TotalMilliseconds,
                result.TotalSeconds);
            PrintResult(result);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void WithoutLookupWithoutEnumeration()
        {
            PerformanceResult result = PerformancePatterns.RunPerformanceTest(cMinPerfIterations,
                (() =>
                {
                    var payids = (from payment in _bigList
                                  select payment.PayId).Distinct();

                    var myStuff = from payid in payids
                                  select new
                                  {
                                      PayId = payid,
                                      coverages =
                                          string.Join(", ", _bigList.Where(x => x.PayId == payid).Select(x => x.CategoryCode)),
                                      total = _bigList.Where(x => x.PayId == payid).Select(x => x.PaymentAmount).Sum()
                                  };
                }));

            Console.WriteLine(cResultFormat + " without lookup.", cMinPerfIterations, result.TotalMilliseconds,
                result.TotalSeconds);
            PrintResult(result);
        }

        public void FuncWithLookup()
        {
            _myLookup = _bigList.ToLookup(p => p.PayId, p => p);

            var payids = _myLookup.Select(g => g.Key);

           var myStuff = from payid in payids
                select new 
                {
                    PayId = payid,
                    coverages = string.Join(",", _myLookup[payid].Select(x => x.CategoryCode)),
                    total = _myLookup[payid].Select(x => x.PaymentAmount).Sum()
                };

            var myList = myStuff.ToList();

        }
    }
}