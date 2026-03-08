using System.Linq.Expressions;

namespace Coins
{
    internal class Program
    {
        static void Main(string[] args)
        {
            (int[] coins, int sum)? result = GetInput();
            if (result == null)
            {
                throw new Exception("Invalid input!");
            }

            // Empty line for aesthetics
            Console.WriteLine();

            bool isPossibleToPartition = false;
            foreach (List<int>? partitioned in CoinsUtils.PartitionSum(result.Value.sum, result.Value.coins))
            {
                if (partitioned == null)
                    continue;

                isPossibleToPartition = true;
                partitioned.Reverse();
                string partitionString = string.Join(" ", partitioned);
                Console.WriteLine(partitionString);
            }

            if (!isPossibleToPartition)
                Console.WriteLine("Nelze.");
        }

        public static (int[], int)? GetInput()
        {
            string? coinsString = Console.ReadLine();
            if (coinsString == null)
                return null;

            int[] coins;
            try
            {
                coins = Array.ConvertAll(coinsString.Split(), Convert.ToInt32);
            }
            catch
            {
                return null;
            }

            string? sumString = Console.ReadLine();
            if (sumString == null)
                return null;

            bool success = int.TryParse(sumString, out int sum);
            if (!success)
                return null;
            
            return (coins, sum);
        }
    }

    public static class CoinsUtils
    {
        /// <summary>
        /// An iterator that returns the next unique partition of a sum in an ascending order of coins for each iteration
        /// </summary>
        /// <param name="sum">The total value of the partition</param>
        /// <param name="coins">The allowed coins' values, in descending order</param>
        /// <returns>Partitions of the sum or null if it is not possible to fully partition the sum</returns>
        public static IEnumerable<List<int>?> PartitionSum(int sum, int[] coins)
        {

            if (sum == 0)
            {
                yield return new List<int>();
                yield break;
            }

            bool hasYielded = false;
            foreach (int coin in coins)
            {
                if (coin > sum)
                    continue;

                foreach (List<int>? continuation in PartitionSum(sum - coin, coins.Where(x => coin >= x).ToArray()))
                {
                    if (continuation == null)
                        continue;

                    hasYielded = true;
                    continuation.Add(coin);
                    yield return continuation;
                }
            }

            if (!hasYielded)
                yield return null;
        }
    }
}
