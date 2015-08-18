using System.Collections.Generic;

namespace PrimesComparison
{
    public abstract class PrimesBase
    {
        protected int DegreeParallelism;
        protected int MaxPrime;
        protected int MinPrime;
        public List<int> Primes { get; protected set; }

        public void Init(int minimum, int maximum, int degree)
        {
            MinPrime = minimum;
            MaxPrime = maximum;
            DegreeParallelism = degree;
        }

        public abstract void Execute();
    }
}