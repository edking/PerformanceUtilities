using System;
using System.Linq;

namespace PrimesComparison
{
    public class PlinqPrimes : PrimesBase
    {
        public override void Execute()
        {
            Primes = Enumerable.Range(Math.Max(MinPrime, 8), MaxPrime).AsParallel()
                               .WithDegreeOfParallelism(DegreeParallelism)
                               .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                               .WithMergeOptions(ParallelMergeOptions.NotBuffered)
                               .Where(x => Enumerable.Range(2, (int) Math.Floor(Math.Sqrt(x)))
                                                     .All(y => x%y != 0)).Concat(new[] {2, 3, 5, 7}.AsParallel())
                               .Where(n => n >= MinPrime && n <= MaxPrime).OrderBy(x => x).ToList();
        }
    }
}