using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace PrimesComparison
{
    public class ConcurrentPrimes : PrimesBase
    {
        private readonly BlockingCollection<int> _primes = new BlockingCollection<int> {2, 3, 5, 7};

        public override void Execute()
        {
            var subTasks = new Task[DegreeParallelism];

            int current = 8;

            for (int i = 0; i < DegreeParallelism; i++, current++)
            {
                int c1 = current;
                subTasks[i] = new Task(() => { if (IsPrime(c1)) AddToPrimesList(c1); });
                subTasks[i].Start();
            }

            while (current < MaxPrime)
            {
                int t = Task.WaitAny(subTasks);
                int c = current;

                subTasks[t].Dispose();
                subTasks[t] = new Task(() => { if (IsPrime(c)) AddToPrimesList(c); });
                subTasks[t].Start();

                current++;
            }
            Task.WaitAll(subTasks);

            Primes = _primes.Where(p => p >= MinPrime && p <= MaxPrime).ToList();
        }

        private bool IsPrime(int val)
        {
            return _primes.All(p => (val%p != 0));
        }

        private void AddToPrimesList(int val)
        {
            _primes.Add(val);
        }
    }
}