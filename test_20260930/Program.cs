using hsbc_test_20260930.q1;
using static hsbc_test_20260930.interfaces.ProbabilisticRandomGen;

namespace hsbc_test_20260930
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            List<NumAndProbability> probs = [new(0, 50), new(1, 200), new(2, 100)];

            ProbabilisticRandomGenImpl prgi = new(probs);
            ProbabilisticRandomGenImpl2 prgi2 = new(probs);
            Dictionary<int, int> results = new();
            Dictionary<int, int> results2 = new();
            int total = 100000000;
            for ( int i = 0; i < total; i++ )
            {
                int r = prgi.nextFromSample();
                if(!results.ContainsKey(r))
                {
                    results[r] = 1;
                }
                else
                {
                    results[r]++;
                }

                int r2 = prgi2.nextFromSample();
                if ( !results2.ContainsKey(r2) )
                {
                    results2[r2] = 1;
                }
                else
                {
                    results2[r2]++;
                }
            }

            foreach( var r in results )
            {
                Console.WriteLine($"{r.Key}, count {r.Value}, {(float)r.Value / total}%");
            }

            foreach ( var r in results2 )
            {
                Console.WriteLine($"{r.Key}, count {r.Value}, {(float)r.Value / total}%");
            }

            Console.WriteLine("Hello, World!");
        }
    }
}
