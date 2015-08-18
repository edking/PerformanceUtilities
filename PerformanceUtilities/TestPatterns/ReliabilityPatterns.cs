using System;
using System.Threading.Tasks;
using PerformanceUtilities.ResultTypes;

namespace PerformanceUtilities.TestPatterns
{
    public static class ReliabilityPatterns
    {
        public static ReliabilityResult RunConcurrentReliabilityTest(int numIterations, int degreeParallelism,
            Func<bool> operation)
        {
            int i;
            var taskList = new Task<ReliabilityResult>[degreeParallelism];

            int subIterations = numIterations/degreeParallelism;

            for (i = 0; i < degreeParallelism; i++)
            {
                var t = new Task<ReliabilityResult>(() => RunReliabilityTest(subIterations, operation));
                taskList[i] = t;
            }

            for (i = 0; i < degreeParallelism; i++) taskList[i].Start();

            Task.WaitAll(taskList);

            bool valid = true;
            int passed = 0;
            int failed = 0;

            for (i = 0; i < degreeParallelism; i++)
            {
                valid &= taskList[i].Result.IsValid;
                passed += taskList[i].Result.Passed;
                failed += taskList[i].Result.Failed;
            }

            var res = new ReliabilityResult
            {
                IsValid = valid,
                Passed = passed,
                Failed = failed
            };

            for (i = 0; i < degreeParallelism; i++) taskList[i].Dispose();

            return res;
        }


        public static ReliabilityResult RunReliabilityTest(int numIterations, Func<bool> operation)
        {
            var rv = new ReliabilityResult();

            // Same operation as the functional test, we just
            // do a lot of them.
            for (int i = 0; (i < numIterations); i++)
            {
                if (operation())
                {
                    rv.Passed++;
                }
                else
                {
                    rv.Failed++;
                }
            }

            rv.IsValid = ((rv.Passed + rv.Failed) > 0);
            return rv;
        }
    }
}