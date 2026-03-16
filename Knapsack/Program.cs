using System.Runtime.CompilerServices;
using System.Text;

namespace Knapsack
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<KnapsackTest> tests = LoadKnapsacks();

            for (int i = 0; i < tests.Count; i++)
            {
                KnapsackTest test = tests[i];
                Console.WriteLine($"Test {i+1}");
                test.OutputSolution();
                Console.WriteLine();
            }
        }

        public static List<KnapsackTest> LoadKnapsacks()
        {
            string testFileName = "Knapsack_testy.txt";
            List<KnapsackTest> tests = new List<KnapsackTest>();

            using (StreamReader sr = new StreamReader(testFileName))
            {
                while (!sr.EndOfStream)
                {
                    string? profits = sr.ReadLine();
                    string? weights = sr.ReadLine();
                    string? maxWeight = sr.ReadLine();
                    string? correctProfit = sr.ReadLine();
                    string? correctItems = sr.ReadLine();
                    sr.ReadLine();  // Every block ends with an empty line

                    if (correctItems == null)
                        continue;   // Malformed test data, should end here

                    Item.ResetID();

                    try
                    {
                        Item[] items = profits!.Split().Zip(
                            weights!.Split(),
                            (a, b) => new Item(int.Parse(a), int.Parse(b))
                        ).ToArray();

                        Knapsack knapsack = new Knapsack(items, int.Parse(maxWeight!));

                        // Trim the leading '-> '
                        correctProfit = correctProfit!.Substring(3);
                        correctItems = correctItems.Substring(3);

                        KnapsackTest test = new KnapsackTest(knapsack, correctProfit, correctItems);
                        tests.Add(test);
                    }
                    catch (Exception ex) when (
                        ex is FormatException ||
                        ex is OverflowException)
                    {
                        continue;   // Malformed test data, continue to the next one
                    }
                }   
            }

            return tests;
        }
    }
    public readonly struct Item
    {
        private static int _currItemID = 1;

        public int ItemID { get; }
        public int Profit { get; }
        public int Weight { get; }
        public int ProfitPerWeight { get; }

        public Item(int profit, int weight)
        {
            ItemID = _currItemID;
            _currItemID++;

            Profit = profit;
            Weight = weight;
            ProfitPerWeight = weight == 0 ? int.MaxValue : profit / Weight;
        }

        public static void ResetID()
        {
            _currItemID = 1;
        }
    }

    public class Node
    {
        public Node? Parent { get; }
        public bool ItemAdded { get; }
        public int Level { get; }           // Index of the item in the sorted array
        public int TotalWeight { get; }     // Includes this node and all the nodes with level <= this.Level
        public int TotalProfit { get; }     // Includes this node and all the nodes with level <= this.Level

        public Node(Node? parent, bool itemAdded, int level, int totalWeight, int totalProfit)
        {
            Parent = parent;
            ItemAdded = itemAdded;
            Level = level;
            TotalWeight = totalWeight;
            TotalProfit = totalProfit;
        }
    }

    public class Knapsack
    {
        // Distinguish between the original items and the sorted ones to keep the original public reference
        private Item[] _sortedItems;
        public Item[] Items { get; }
        public int MaxWeight { get; }

        public Item[]? PickedItems { get; protected set; }
        public int? MaxProfit { get; protected set; }

        public Knapsack(Item[] items, int maxWeight)
        {
            Items = items;
            MaxWeight = maxWeight;

            Item[] sortedItems = (Item[])items.Clone();
            Array.Sort(sortedItems, (item1, item2) =>
                item2.ProfitPerWeight.CompareTo(item1.ProfitPerWeight));

            _sortedItems = sortedItems;
        }

        public float Bound(Node node)
        {
            if (node.TotalWeight > MaxWeight)
                return 0;   // Over the weight limit, discourage this

            float profitBound = node.TotalProfit;
            float weight = node.TotalWeight;
            int n = Items.Length;

            // Use the greedy approach to add items based on their profit per weight
            for (int j = node.Level + 1; j < n; j++)
            {
                Item item = _sortedItems[j];
                if (weight + item.Weight > MaxWeight)
                {
                    // If the item can't fit fully, add it partially and break the loop
                    profitBound += item.ProfitPerWeight * (MaxWeight - weight);
                    break;
                }

                weight += item.Weight;
                profitBound += item.Profit;
            }

            return profitBound;
        }

        public void EvaluateKnapsack()
        {
            PriorityQueue<Node, float> priorityQueue = new PriorityQueue<Node, float>();

            Node root = new Node(null, false, -1, 0, 0);     // pre-root
            priorityQueue.Enqueue(root, 0);

            int maxProfit = 0;
            int n = Items.Length;
            Node lastNode = root;

            while (priorityQueue.Count > 0)
            {
                Node node = priorityQueue.Dequeue();

                // Check if we're already on a leaf; continue if yes
                if (node.Level == n - 1)
                    continue;

                // A node without adding the next item
                Node nextNode0 = new Node(
                    node,
                    false,
                    node.Level + 1,
                    node.TotalWeight,
                    node.TotalProfit
                );

                // A node with adding the next item
                Node nextNode1 = new Node(
                    node,
                    true,
                    node.Level + 1,
                    node.TotalWeight + _sortedItems[node.Level + 1].Weight,
                    node.TotalProfit + _sortedItems[node.Level + 1].Profit
                );

                // First check if adding the item is benefitial; update if yes
                if (nextNode1.TotalWeight <= MaxWeight && nextNode1.TotalProfit > maxProfit)
                {
                    // Only check when the next item is added because not including the next item can't increase profit
                    maxProfit = nextNode1.TotalProfit;
                    lastNode = nextNode1;
                }

                // Then check if the item has potential to be even better later on
                float nextBound1 = Bound(nextNode1);
                if (nextBound1 > maxProfit)
                    priorityQueue.Enqueue(nextNode1, nextBound1);

                // Finally check the potential of not adding the item
                float nextBound0 = Bound(nextNode0);
                if (nextBound0 > maxProfit)
                    priorityQueue.Enqueue(nextNode0, nextBound0);
            }

            // At last, choose the added items and sort the list
            List<Item> pickedItems = new List<Item>();
            while (lastNode.Parent != null)
            {
                if (lastNode.ItemAdded)
                {
                    Item item = _sortedItems[lastNode.Level];
                    pickedItems.Add(item);
                }

                lastNode = lastNode.Parent;
            }

            pickedItems.Sort((item1, item2) => item1.ItemID.CompareTo(item2.ItemID));

            PickedItems = pickedItems.ToArray();
            MaxProfit = maxProfit;
        }

        public void OutputSolution()
        {
            if (MaxProfit == null || PickedItems == null)
                throw new Exception("Tried to output solution of a knapsach that is yet to be evaluated!");

            int[] pickedIDs = new int[PickedItems.Length];
            for (int i = 0; i < pickedIDs.Length; i++)
            {
                Item item = PickedItems[i];
                pickedIDs[i] = item.ItemID;
            }

            string items = string.Join(' ', pickedIDs);

            Console.WriteLine($"-> {MaxProfit}");
            Console.WriteLine($"-> {items}");
        }
    }

    public class KnapsackTest
    {
        public Knapsack TestedKnapsack { get; }
        public string CorrectMaxProfit { get; }
        public string CorrectPickedItems { get; }

        public KnapsackTest(Knapsack testedKnapsack, string correctMaxProfit, string correctPickedItems)
        {
            TestedKnapsack = testedKnapsack;
            CorrectMaxProfit = correctMaxProfit;
            CorrectPickedItems = correctPickedItems;
        }

        public void OutputSolution()
        {
            if (TestedKnapsack.PickedItems == null || TestedKnapsack.MaxProfit == null)
                TestedKnapsack.EvaluateKnapsack();

            // Convert picked items to a string first
            int[] pickedIDs = new int[TestedKnapsack.PickedItems!.Length];
            for (int i = 0; i < pickedIDs.Length; i++)
            {
                Item item = TestedKnapsack.PickedItems[i];
                pickedIDs[i] = item.ItemID;
            }
            string items = string.Join(' ', pickedIDs);

            // Output the solution
            Console.WriteLine($"--> '{TestedKnapsack.MaxProfit}' (correct: '{CorrectMaxProfit}')");
            Console.WriteLine($"--> '{items}' (correct: '{CorrectPickedItems}')");
        }
    }
}
