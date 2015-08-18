using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerformanceUtilities.Analysis.StatisticalTests;
using PerformanceUtilities.ResultTypes;
using PerformanceUtilities.TestPatterns;

namespace Comparisons
{
    [TestClass]
    public class DatatableVsDictionary
    {
        private const int cSmallTableSize = 10;
        private const int cMediumTableSize = 100;
        private const int cLargeTableSize = 1000;
        private const int cXLargeTableSize = 100000;

        private const int cMinKey = 0x100000;
        private const int cMaxKey = 0xffffff;
        private const string cResultFormat = "{0:N0} iterations took {1:n2} ms ({2:n3} seconds)";

        // Rule: do it a lot of times to smooth out the rough edges
        private const int cMinPerfIterations = 10000;
        private readonly Random _rng = new Random();

        private DataSet _dataSet;
        private PerformanceResult _dictResult;
        private Dictionary<string, string> _dictionary;
        private DataTable _intDataTable;
        private Dictionary<int, string> _intDictionary;
        private List<int> _intKeys;
        private DataTable _stringDataTable;
        private List<string> _stringKeys;

        private PerformanceResult _tableResult;

        [TestMethod]
        [TestCategory("Performance")]
        public void SmallStringKey()
        {
            InitStringData(cSmallTableSize);

            RunStringTests();

            var comparison = new TwoSampleZTest(_dictResult.DescriptiveResult, _tableResult.DescriptiveResult, 0.0,
                TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

            Console.WriteLine(_dictResult.ToString());
            Console.WriteLine(_tableResult.ToString());

            Assert.IsTrue(comparison.Significant);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void MediumStringKey()
        {
            InitStringData(cMediumTableSize);

            RunStringTests();

            var comparison = new TwoSampleZTest(_dictResult.DescriptiveResult, _tableResult.DescriptiveResult, 0.0,
                TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

            Console.WriteLine(_dictResult.ToString());
            Console.WriteLine(_tableResult.ToString());

            Assert.IsTrue(comparison.Significant);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void LargeStringKey()
        {
            InitStringData(cLargeTableSize);

            RunStringTests();

            var comparison = new TwoSampleZTest(_dictResult.DescriptiveResult, _tableResult.DescriptiveResult, 0.0,
                TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

            Console.WriteLine(_dictResult.ToString());
            Console.WriteLine(_tableResult.ToString());

            Assert.IsTrue(comparison.Significant);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void ExtraLargeStringKey()
        {
            InitStringData(cXLargeTableSize);

            RunStringTests();

            var comparison = new TwoSampleZTest(_dictResult.DescriptiveResult, _tableResult.DescriptiveResult, 0.0,
                TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

            Console.WriteLine(_dictResult.ToString());
            Console.WriteLine(_tableResult.ToString());

            Assert.IsTrue(comparison.Significant);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void SmallIntKey()
        {
            InitIntData(cSmallTableSize);

            RunIntTests();

            var comparison = new TwoSampleZTest(_dictResult.DescriptiveResult, _tableResult.DescriptiveResult, 0.0,
                TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

            Console.WriteLine(_dictResult.ToString());
            Console.WriteLine(_tableResult.ToString());

            Assert.IsTrue(comparison.Significant);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void MediumIntKey()
        {
            InitIntData(cMediumTableSize);

            RunIntTests();

            var comparison = new TwoSampleZTest(_dictResult.DescriptiveResult, _tableResult.DescriptiveResult, 0.0,
                TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

            Console.WriteLine(_dictResult.ToString());
            Console.WriteLine(_tableResult.ToString());

            Assert.IsTrue(comparison.Significant);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void LargeIntKey()
        {
            InitIntData(cLargeTableSize);

            RunIntTests();

            var comparison = new TwoSampleZTest(_dictResult.DescriptiveResult, _tableResult.DescriptiveResult, 0.0,
                TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

            Console.WriteLine(_dictResult.ToString());
            Console.WriteLine(_tableResult.ToString());

            Assert.IsTrue(comparison.Significant);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void ExtraLargeIntKey()
        {
            InitIntData(cXLargeTableSize);

            RunIntTests();

            var comparison = new TwoSampleZTest(_dictResult.DescriptiveResult, _tableResult.DescriptiveResult, 0.0,
                TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

            Console.WriteLine(_dictResult.ToString());
            Console.WriteLine(_tableResult.ToString());

            Assert.IsTrue(comparison.Significant);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void ExtraLargeIntKeyvStringKey()
        {
            InitSIData(cXLargeTableSize);

            RunSITests();

            var comparison = new TwoSampleZTest(_dictResult.DescriptiveResult, _tableResult.DescriptiveResult, 0.0,
                TwoSampleHypothesis.ValuesAreDifferent);

            Console.WriteLine(_dictResult.ToString());
            Console.WriteLine(_tableResult.ToString());

            // Interesting view of "significant" here.  A test with this many runs is almost always significant
            // statistically.  But another read of "significant" is that sometimes this shows as a significant diff
            // and sometimes not.  Depends on the run.
            Assert.IsFalse(comparison.Significant);
        }

        private void RunSITests()
        {
            _dictResult = PerformancePatterns.RunPerformanceTest(cMinPerfIterations,
                (() =>
                {
                    var ndx = _intKeys[_rng.Next(0, _intKeys.Count)];
                    string desc;

                    DataRow[] rows = _intDataTable.Select(String.Format("codeColumn = {0}", ndx));
                    if (rows.Length > 0) desc = rows[0]["valueColumn"].ToString();
                }));
            Console.WriteLine(cResultFormat + " with int-keyed DataTable.", cMinPerfIterations,
                _dictResult.TotalMilliseconds,
                _dictResult.TotalSeconds);

            _tableResult = PerformancePatterns.RunPerformanceTest(cMinPerfIterations,
                (() =>
                {
                    var ndx = _stringKeys[_rng.Next(0, _stringKeys.Count)];

                    string desc;

                    DataRow[] rows = _stringDataTable.Select("codeColumn = '" + ndx + "'");
                    if (rows.Length > 0) desc = rows[0]["valueColumn"].ToString();
                }));
            Console.WriteLine(cResultFormat + " with string-keyed DataTable.", cMinPerfIterations,
                _tableResult.TotalMilliseconds,
                _tableResult.TotalSeconds);
        }

        private void RunStringTests()
        {
            _dictResult = PerformancePatterns.RunPerformanceTest(cMinPerfIterations,
                (() =>
                {
                    var ndx = _stringKeys[_rng.Next(0, _stringKeys.Count)];
                    string desc;
                    _dictionary.TryGetValue(ndx, out desc);
                }));
            Console.WriteLine(cResultFormat + " with dictionary.", cMinPerfIterations, _dictResult.TotalMilliseconds,
                _dictResult.TotalSeconds);
            _tableResult = PerformancePatterns.RunPerformanceTest(cMinPerfIterations,
                (() =>
                {
                    var ndx = _stringKeys[_rng.Next(0, _stringKeys.Count)];

                    string desc;

                    DataRow[] rows = _stringDataTable.Select("codeColumn = '" + ndx + "'");
                    if (rows.Length > 0) desc = rows[0]["valueColumn"].ToString();
                }));
            Console.WriteLine(cResultFormat + " with DataTable.", cMinPerfIterations, _tableResult.TotalMilliseconds,
                _tableResult.TotalSeconds);
        }

        private void RunIntTests()
        {
            _dictResult = PerformancePatterns.RunPerformanceTest(cMinPerfIterations,
                (() =>
                {
                    var ndx = _intKeys[_rng.Next(0, _intKeys.Count)];
                    string desc;
                    _intDictionary.TryGetValue(ndx, out desc);
                }));
            Console.WriteLine(cResultFormat + " with dictionary.", cMinPerfIterations, _dictResult.TotalMilliseconds,
                _dictResult.TotalSeconds);

            _tableResult = PerformancePatterns.RunPerformanceTest(cMinPerfIterations,
                (() =>
                {
                    var ndx = _intKeys[_rng.Next(0, _intKeys.Count)];
                    string desc;

                    DataRow[] rows = _intDataTable.Select(String.Format("codeColumn = {0}", ndx));
                    if (rows.Length > 0) desc = rows[0]["valueColumn"].ToString();
                }));
            Console.WriteLine(cResultFormat + " with DataTable.", cMinPerfIterations, _tableResult.TotalMilliseconds,
                _tableResult.TotalSeconds);
        }

        private void InitSIData(int numEntries)
        {
            _dataSet = new DataSet("myDataSet");
            _stringKeys = new List<string>();
            _intKeys = new List<int>();

            _stringDataTable = new DataTable("myStringTable");
            _stringDataTable.Columns.Add(new DataColumn("codeColumn"));
            _stringDataTable.Columns.Add(new DataColumn("valueColumn"));
            _intDataTable = new DataTable("myIntTable");
            _intDataTable.Columns.Add(new DataColumn("codeColumn", cMinKey.GetType()));
            _intDataTable.Columns.Add(new DataColumn("valueColumn"));

            _dataSet.Tables.Add(_stringDataTable);
            _dataSet.Tables.Add(_intDataTable);

            for (int i = 0; i < numEntries; i++)
            {
                var iCode = _rng.Next(cMinKey, cMaxKey);

                while (_intKeys.Contains(iCode)) iCode = _rng.Next(cMinKey, cMaxKey);
                var sCode = String.Format("{0:X6}", iCode);

                var desc = String.Format("Some lengthy description text for {0}", sCode);

                var row = _stringDataTable.NewRow();
                row["codeColumn"] = sCode;
                row["valueColumn"] = desc;
                _stringDataTable.Rows.Add(row);

                _stringKeys.Add(sCode);

                row = _intDataTable.NewRow();
                row["codeColumn"] = iCode;
                row["valueColumn"] = desc;
                _intDataTable.Rows.Add(row);

                _intKeys.Add(iCode);
            }
        }

        private void InitStringData(int numEntries)
        {
            _dictionary = new Dictionary<string, string>();
            _dataSet = new DataSet("myDataSet");
            _stringKeys = new List<string>();
            _stringDataTable = new DataTable("myTable");
            _stringDataTable.Columns.Add(new DataColumn("codeColumn"));
            _stringDataTable.Columns.Add(new DataColumn("valueColumn"));

            _dataSet.Tables.Add(_stringDataTable);

            for (int i = 0; i < numEntries; i++)
            {
                var code = String.Format("{0:X6}", _rng.Next(cMinKey, cMaxKey));

                while (_dictionary.ContainsKey(code)) code = String.Format("{0:00000}", _rng.Next(cMinKey, cMaxKey));

                var desc = String.Format("Some lengthy description text for {0}", code);

                var row = _stringDataTable.NewRow();
                row["codeColumn"] = code;
                row["valueColumn"] = desc;
                _stringDataTable.Rows.Add(row);

                _dictionary.Add(code, desc);
                _stringKeys.Add(code);
            }
        }

        private void InitIntData(int numEntries)
        {
            _intDictionary = new Dictionary<int, string>();
            _dataSet = new DataSet("myDataSet");
            _intKeys = new List<int>();
            _intDataTable = new DataTable("myTable");
            _intDataTable.Columns.Add(new DataColumn("codeColumn", cMinKey.GetType()));
            _intDataTable.Columns.Add(new DataColumn("valueColumn"));

            _dataSet.Tables.Add(_intDataTable);

            for (int i = 0; i < numEntries; i++)
            {
                var code = _rng.Next(cMinKey, cMaxKey);

                while (_intDictionary.ContainsKey(code)) code = _rng.Next(cMinKey, cMaxKey);

                var desc = String.Format("Some lengthy description text for {0}", code);

                var row = _intDataTable.NewRow();
                row["codeColumn"] = code;
                row["valueColumn"] = desc;
                _intDataTable.Rows.Add(row);

                _intDictionary.Add(code, desc);
                _intKeys.Add(code);
            }
        }
    }
}