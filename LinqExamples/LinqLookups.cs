using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerformanceUtilities.Analysis.StatisticalTests;
using PerformanceUtilities.MSExtensions;
using PerformanceUtilities.TestPatterns;

namespace LinqExamples
{
    [TestClass]
    public class LinqLookups
    {
        // Rule: need lots of data -- size like prod will see it
        private const int cMaxList = (1 << 9);

        // Rule: do it a lot of times to smooth out the rough edges
        private const int cMinPerfIterations = 10000;

        private const string catLabels = "ABCDEFGHIJ0123456789";
        private const int catLength = 2;
        private const int maxCategories = 10;
        private readonly ThreadSafeRandom _rng = new ThreadSafeRandom();
        private int _basePayId = 1000000;
        private List<PayRecord> _bigList;
        private List<string> _categories;

        private ILookup<int, PayRecord> _myLookup;
        private int _uniquePayIds = 300;

        [TestInitialize]
        [TestCategory("Performance")]
        public void TestSetup()
        {
            _categories = new List<string>();
            while (_categories.Count < maxCategories)
            {
                string cat = String.Empty;
                for (int i = 0; i < catLength; i++)
                {
                    int c = _rng.Next(1, catLabels.Length);
                    cat = cat + catLabels[c];
                }
                if (!_categories.Contains(cat)) _categories.Add(cat);
            }

            _bigList = new List<PayRecord>(cMaxList);
            for (int i = 0; i < cMaxList; i++)
            {
                string cv = _categories[_rng.Next(0, _categories.Count - 1)];

                var pr = new PayRecord
                         {
                             PayId = _rng.Next(_basePayId, _basePayId + _uniquePayIds),
                             PaymentAmount = Convert.ToDecimal(_rng.Next(1, 10000))/100M,
                             Category = cv
                         };

                _bigList.Add(pr);
            }
            _myLookup = _bigList.ToLookup(p => p.PayId, p => p);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void CompareWithAndWithoutLookup()
        {
            bool significant = PerformancePatterns.RunPerformanceComparison(cMinPerfIterations,
                "serial with lookup", (() =>
                {
                    _myLookup = _bigList.ToLookup(p => p.PayId, p => p);

                    var payids = _myLookup.Select(g => g.Key);

                    var myStuff = (from payid in payids
                                   select new
                                          {
                                              PayId = payid,
                                              coverages =
                                                  string.Join(",", _myLookup[payid].Select(x => x.Category).Distinct()),
                                              total = _myLookup[payid].Select(x => x.PaymentAmount).Sum()
                                          }).ToList();
                }),
                "serial without lookup", (() =>
                {
                    var payids = (from payment in _bigList
                                  select payment.PayId).Distinct();

                    var myStuff = (from payid in payids
                                   select new
                                          {
                                              PayId = payid,
                                              coverages =
                                                  string.Join(", ",
                                                      _bigList.Where(x => x.PayId == payid)
                                                              .Select(x => x.Category)
                                                              .Distinct()),
                                              total =
                                                  _bigList.Where(x => x.PayId == payid)
                                                          .Select(x => x.PaymentAmount)
                                                          .Sum()
                                          }).ToList();
                }),
                0.0, TwoSampleHypothesis.FirstValueIsSmallerThanSecond, true);

            Assert.IsTrue(significant);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void CompareConcurrentWithAndWithoutLookups()
        {
            bool significant = PerformancePatterns.RunPerformanceComparison(cMinPerfIterations, "parallel with Lookup",
                (() =>
                {
                    _myLookup = _bigList.ToLookup(p => p.PayId, p => p);

                    var payids = _myLookup.Select(g => g.Key);

                    var myStuff = (from payid in payids.AsParallel()
                                   select new
                                          {
                                              PayId = payid,
                                              coverages =
                                                  string.Join(",", _myLookup[payid].Select(x => x.Category).Distinct()),
                                              total = _myLookup[payid].Select(x => x.PaymentAmount).Sum()
                                          }).ToList();
                }),
                "parallel without Lookup", (() =>
                {
                    var payids = (from payment in _bigList
                                  select payment.PayId).Distinct();

                    var myStuff = (from payid in payids.AsParallel()
                                   select new
                                          {
                                              PayId = payid,
                                              coverages =
                                                  string.Join(", ",
                                                      _bigList.Where(x => x.PayId == payid)
                                                              .Select(x => x.Category)
                                                              .Distinct()),
                                              total =
                                                  _bigList.Where(x => x.PayId == payid)
                                                          .Select(x => x.PaymentAmount)
                                                          .Sum()
                                          }).ToList();
                }),
                0.0, TwoSampleHypothesis.FirstValueIsSmallerThanSecond, true);

            Assert.IsTrue(significant);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void CompareConcurrentVsLookups()
        {
            bool significant = PerformancePatterns.RunPerformanceComparison(cMinPerfIterations,
                "serial with lookup", (() =>
                {
                    _myLookup = _bigList.ToLookup(p => p.PayId, p => p);

                    var payids = _myLookup.Select(g => g.Key);

                    var myStuff = (from payid in payids
                                   select new
                                          {
                                              PayId = payid,
                                              coverages =
                                                  string.Join(",", _myLookup[payid].Select(x => x.Category).Distinct()),
                                              total = _myLookup[payid].Select(x => x.PaymentAmount).Sum()
                                          }).ToList();
                }),
                "parallel without lookup", (() =>
                {
                    var payids = (from payment in _bigList
                                  select payment.PayId).Distinct();

                    var myStuff = (from payid in payids.AsParallel()
                                   select new
                                          {
                                              PayId = payid,
                                              coverages =
                                                  string.Join(", ",
                                                      _bigList.Where(x => x.PayId == payid)
                                                              .Select(x => x.Category)
                                                              .Distinct()),
                                              total =
                                                  _bigList.Where(x => x.PayId == payid)
                                                          .Select(x => x.PaymentAmount)
                                                          .Sum()
                                          }).ToList();
                }),
                0.0, TwoSampleHypothesis.FirstValueIsSmallerThanSecond, true);

            Assert.IsTrue(significant);
        }
    }
}