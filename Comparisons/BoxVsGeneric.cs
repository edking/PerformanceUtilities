using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerformanceUtilities.Analysis.StatisticalTests;
using PerformanceUtilities.TestPatterns;

namespace Comparisons
{
    [TestClass]
    public class BoxVsGeneric
    {
        private const int cMinPerfIterations = 100000;
        private readonly List<object> _boxList = new List<object>();

        private readonly List<int> _noboxList = new List<int>();
        private readonly Random _rng = new Random();

        [TestMethod]
        public void BoxedListVsGenericList()
        {
            bool significant = PerformancePatterns.RunPerformanceComparison(cMinPerfIterations,
                "List<object>", (() =>
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
                }),
                "List<int>", (() =>
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
                }),
                0.0, TwoSampleHypothesis.ValuesAreDifferent, true);

            Assert.IsTrue(significant);
        }
    }
}