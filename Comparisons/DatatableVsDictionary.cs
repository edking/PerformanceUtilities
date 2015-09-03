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
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void MediumStringKey()
        {
            InitStringData(cMediumTableSize);

            RunStringTests();
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void LargeStringKey()
        {
            InitStringData(cLargeTableSize);

            RunStringTests();
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void ExtraLargeStringKey()
        {
            InitStringData(cXLargeTableSize);

            RunStringTests();
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void SmallIntKey()
        {
            InitIntData(cSmallTableSize);

            RunIntTests();
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void MediumIntKey()
        {
            InitIntData(cMediumTableSize);

            RunIntTests();
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void LargeIntKey()
        {
            InitIntData(cLargeTableSize);

            RunIntTests();
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void ExtraLargeIntKey()
        {
            InitIntData(cXLargeTableSize);

            RunIntTests();
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void ExtraLargeIntKeyvStringKey()
        {
            InitSIData(cXLargeTableSize);

            RunSITests();
        }

        private void RunSITests()
        {
            bool significant = PerformancePatterns.RunPerformanceComparison(cMinPerfIterations,
                "datatable with int keys", (() =>
                {
                    var ndx = _intKeys[_rng.Next(0, _intKeys.Count)];
                    string desc;

                    DataRow[] rows = _intDataTable.Select(String.Format("codeColumn = {0}", ndx));
                    if (rows.Length > 0) desc = rows[0]["valueColumn"].ToString();
                }),
                "datatable with string keys", (() =>
                {
                    var ndx = _stringKeys[_rng.Next(0, _stringKeys.Count)];

                    string desc;

                    DataRow[] rows = _stringDataTable.Select("codeColumn = '" + ndx + "'");
                    if (rows.Length > 0) desc = rows[0]["valueColumn"].ToString();
                }),
                100.0, TwoSampleHypothesis.ValuesAreDifferent, true);
            Assert.IsTrue(significant);
        }

        private void RunStringTests()
        {
            bool significant = PerformancePatterns.RunPerformanceComparison(cMinPerfIterations,
                "dictionary with string keys", (() =>
                {
                    var ndx = _stringKeys[_rng.Next(0, _stringKeys.Count)];
                    string desc;
                    _dictionary.TryGetValue(ndx, out desc);
                }),
                "datatable with string keys", (() =>
                {
                    var ndx = _stringKeys[_rng.Next(0, _stringKeys.Count)];

                    string desc;

                    DataRow[] rows = _stringDataTable.Select("codeColumn = '" + ndx + "'");
                    if (rows.Length > 0) desc = rows[0]["valueColumn"].ToString();
                }),
                00.0, TwoSampleHypothesis.FirstValueIsSmallerThanSecond, true);
            Assert.IsTrue(significant);
        }

        private void RunIntTests()
        {
            bool significant = PerformancePatterns.RunPerformanceComparison(cMinPerfIterations,
                "dictionary with int keys", (() =>
                {
                    var ndx = _intKeys[_rng.Next(0, _intKeys.Count)];
                    string desc;
                    _intDictionary.TryGetValue(ndx, out desc);
                }),
                "datatable with int keys", (() =>
                {
                    var ndx = _intKeys[_rng.Next(0, _intKeys.Count)];
                    string desc;

                    DataRow[] rows = _intDataTable.Select(String.Format("codeColumn = {0}", ndx));
                    if (rows.Length > 0) desc = rows[0]["valueColumn"].ToString();
                }),
                00.0, TwoSampleHypothesis.FirstValueIsSmallerThanSecond, true);
            Assert.IsTrue(significant);
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