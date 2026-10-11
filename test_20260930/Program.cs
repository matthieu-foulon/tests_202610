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
            ProbabilisticRandomGenImpl3 prgi3 = new(probs, 1024);
            Dictionary<int, int> results = new();
            Dictionary<int, int> results2 = new();
            Dictionary<int, int> results3 = new();
            int total = 1_000_000_000;
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

                int r3 = prgi3.nextFromSample();
                if ( !results3.ContainsKey(r3) )
                {
                    results3[r3] = 1;
                }
                else
                {
                    results3[r3]++;
                }
            }

            foreach(var p in probs)
            {
                Console.WriteLine($"{p.Number} :");
                Console.WriteLine($"    count {results[p.Number]}, {(float)results[p.Number] / total}%");
                Console.WriteLine($"    count {results2[p.Number]}, {(float)results2[p.Number] / total}%");
                Console.WriteLine($"    count {results3[p.Number]}, {(float)results3[p.Number] / total}%");
            }

            Console.WriteLine("That's all folks !");
        }
    }
}
