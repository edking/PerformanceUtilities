using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PerformanceUtilities.Analysis;
using PerformanceUtilities.Analysis.StatisticalTests;
using PerformanceUtilities.ResultTypes;

namespace PerformanceUtilities.TestPatterns
{
    public static class PerformancePatterns
    {
        public const int cHistogramBuckets = 20;

        public static bool RunPerformanceComparison(int numIterations, Action firstOperation, Action secondOperation,
            double hypothesizedDifference = 0, TwoSampleHypothesis hypothesis = TwoSampleHypothesis.ValuesAreDifferent,
            bool outputDetails = false)
        {
            return RunPerformanceComparison(numIterations, "first", firstOperation, "second", secondOperation,
                hypothesizedDifference, hypothesis, outputDetails);
        }

        public static bool RunConcurrentPerformanceComparison(int numIterations, int degreeParallelism,
            Action firstOperation, Action secondOperation, double hypothesizedDifference = 0,
            TwoSampleHypothesis hypothesis = TwoSampleHypothesis.ValuesAreDifferent, bool outputDetails = false)
        {
            return RunConcurrentPerformanceComparison(numIterations, degreeParallelism, "first", firstOperation,
                "second", secondOperation, hypothesizedDifference, hypothesis, outputDetails);
        }

        public static bool RunPerformanceComparison(int numIterations, string firstLabel, Action firstOperation,
            string secondLabel, Action secondOperation,
            double hypothesizedDifference = 0, TwoSampleHypothesis hypothesis = TwoSampleHypothesis.ValuesAreDifferent,
            bool outputDetails = false)
        {
            var result1 = RunPerformanceTest(numIterations, firstOperation);
            var result2 = RunPerformanceTest(numIterations, secondOperation);
            var comparison = new GeneralHypothesisTest(firstLabel, result1.DescriptiveResult, secondLabel,
                result2.DescriptiveResult,
                hypothesizedDifference, hypothesis);

            if (outputDetails)
            {
                Console.WriteLine(comparison.ToString());
                Console.WriteLine("-----------------------------{0}--------------------------------------", firstLabel);
                Console.WriteLine(result1.ToString());
                Console.WriteLine("-----------------------------{0}--------------------------------------", secondLabel);
                Console.WriteLine(result2.ToString());
            }

            return comparison.Significant;
        }

        public static bool RunConcurrentPerformanceComparison(int numIterations, int degreeParallelism,
            string firstLabel,
            Action firstOperation, string secondLabel, Action secondOperation, double hypothesizedDifference = 0,
            TwoSampleHypothesis hypothesis = TwoSampleHypothesis.ValuesAreDifferent, bool outputDetails = false)
        {
            var result1 = RunConcurrentPerformanceTest(numIterations, degreeParallelism, firstOperation);
            var result2 = RunConcurrentPerformanceTest(numIterations, degreeParallelism, secondOperation);
            var comparison = new GeneralHypothesisTest(firstLabel, result1.DescriptiveResult, secondLabel,
                result2.DescriptiveResult,
                hypothesizedDifference, hypothesis);

            if (outputDetails)
            {
                Console.WriteLine(comparison.ToString());
                Console.WriteLine("-----------------------------{0}--------------------------------------", firstLabel);
                Console.WriteLine(result1.ToString());
                Console.WriteLine("-----------------------------{0}--------------------------------------", secondLabel);
                Console.WriteLine(result2.ToString());
            }

            return comparison.Significant;
        }

        public static PerformanceResult RunConcurrentPerformanceTest(int numIterations, int degreeParallelism,
            Action operation)
        {
            int i;
            var taskList = new Task<PerformanceResult>[degreeParallelism];
            long startTime = HiResTimer.Ticks;
            int subIterations = numIterations/degreeParallelism;

            for (i = 0; i < degreeParallelism; i++)
            {
                var t = new Task<PerformanceResult>(() => RunPerformanceTest(subIterations, operation, true));
                taskList[i] = t;
            }

            for (i = 0; i < degreeParallelism; i++) taskList[i].Start();

            Task.WaitAll(taskList);
            long stopTime = HiResTimer.Ticks;

            var rawData = new List<double>();

            for (i = 0; i < degreeParallelism; i++)
            {
                rawData.AddRange(taskList[i].Result.DescriptiveResult.RawData);
            }

            var desc = new DescriptiveAnalysis(rawData);
            desc.Analyze(false);
            desc.AnalyzeHistogram(cHistogramBuckets);

            var res = new PerformanceResult
            {
                IsValid = true,
                Iterations = taskList.Sum(p => p.Result.Iterations),
                DegreeOfParallelism = degreeParallelism,
                TotalMilliseconds = ConvertToMs(startTime, stopTime),
                TotalSeconds = ConvertToSeconds(startTime, stopTime),
                TotalTicks = stopTime - startTime,
                DescriptiveResult = desc.Result
            };

            for (i = 0; i < degreeParallelism; i++) taskList[i].Dispose();

            return res;
        }

        public static PerformanceResult RunPerformanceTest(int numIterations, Action operation, bool isParallel = false)
        {
            // grab the start time
            long startTime = HiResTimer.Ticks;
            var measures = new List<double>(numIterations);
            // Same operation as the functional test, we just
            // do a lot of them.
            for (int i = 0; (i < numIterations); i++)
            {
                long aStart = HiResTimer.Ticks;
                operation();
                long aStop = HiResTimer.Ticks;
                measures.Add(ConvertToMs(aStart, aStop));
            }

            // grab the stop time
            long stopTime = HiResTimer.Ticks;

            // If they all worked, we can report a valid result.
            // If they didn't then we call the perf test inconclusive.

            var descriptive = new DescriptiveAnalysis(measures);
            descriptive.Analyze(isParallel);
            if (!isParallel) descriptive.AnalyzeHistogram(cHistogramBuckets);

            return new PerformanceResult
            {
                IsValid = true,
                Iterations = numIterations,
                DegreeOfParallelism = 1,
                TotalSeconds = ConvertToSeconds(startTime, stopTime),
                TotalMilliseconds = ConvertToMs(startTime, stopTime),
                TotalTicks = (stopTime - startTime),
                DescriptiveResult = descriptive.Result
            };
        }

        public static PerformanceResult RunConcurrentPerformanceTest(int numIterations, int degreeParallelism,
            Func<bool> operation)
        {
            int i;
            var taskList = new Task<PerformanceResult>[degreeParallelism];

            int subIterations = numIterations/degreeParallelism;

            for (i = 0; i < degreeParallelism; i++)
            {
                var t = new Task<PerformanceResult>(() => RunPerformanceTest(subIterations, operation, true));
                taskList[i] = t;
            }

            for (i = 0; i < degreeParallelism; i++) taskList[i].Start();

            Task.WaitAll(taskList);

            var rawData = new List<double>();
            bool valid = true;

            for (i = 0; i < degreeParallelism; i++)
            {
                valid &= taskList[i].Result.IsValid;
                rawData.AddRange(taskList[i].Result.DescriptiveResult.RawData);
            }

            var desc = new DescriptiveAnalysis(rawData);
            desc.Analyze(false);
            desc.AnalyzeHistogram(cHistogramBuckets);

            var res = new PerformanceResult
            {
                IsValid = valid,
                TotalMilliseconds = taskList.Max(p => p.Result.TotalMilliseconds),
                TotalSeconds = taskList.Max(p => p.Result.TotalSeconds),
                TotalTicks = taskList.Max(p => p.Result.TotalTicks),
                DescriptiveResult = desc.Result
            };

            for (i = 0; i < degreeParallelism; i++) taskList[i].Dispose();

            return res;
        }

        public static PerformanceResult RunPerformanceTest(int numIterations, Func<bool> operation,
            bool isParallel = false)
        {
            bool isResultValid = true;
            var measures = new List<double>(numIterations);

            // grab the start time
            long startTime = HiResTimer.Ticks;

            // Same operation as the functional test, we just
            // do a lot of them.
            for (int i = 0; ((i < numIterations) && (isResultValid)); i++)
            {
                long aStart = HiResTimer.Ticks;
                isResultValid &= (operation());
                long aStop = HiResTimer.Ticks;
                if (isResultValid) measures.Add(ConvertToMs(aStart, aStop));
            }

            // grab the stop time
            long stopTime = HiResTimer.Ticks;

            var descriptive = new DescriptiveAnalysis(measures);
            descriptive.Analyze(isParallel);
            if (!isParallel) descriptive.AnalyzeHistogram(cHistogramBuckets);

            // If they all worked, we can report a valid result.
            // If they didn't then we call the perf test inconclusive.
            return new PerformanceResult
            {
                IsValid = isResultValid,
                TotalSeconds = ConvertToSeconds(startTime, stopTime),
                TotalMilliseconds = ConvertToMs(startTime, stopTime),
                TotalTicks = (stopTime - startTime),
                DescriptiveResult = descriptive.Result
            };
        }

        public static double ConvertToMs(long start, long stop)
        {
            return Convert.ToDouble(1000*(stop - start))/Convert.ToDouble(HiResTimer.TicksPerSecond);
        }

        public static double ConvertToSeconds(long start, long stop)
        {
            return Convert.ToDouble(stop - start)/Convert.ToDouble(HiResTimer.TicksPerSecond);
        }
    }
}