namespace Knapsack
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
            Item[] items = new Item[] {
                new Item(100, 10),
                new Item(120, 8),
                new Item(50, 5),
                new Item(50, 3),
                new Item(5, 2),
                new Item(90, 6),
                new Item(40, 3),
                new Item(1, 1),
                new Item(4, 4),
            };
            int weight = 25;
            */

            /*
            Item[] items = new Item[] {
                new Item(2, 3),
                new Item(2, 1),
                new Item(4, 3),
                new Item(5, 4),
                new Item(3, 2),
            };
            int weight = 7;
            */

            
            Item[] items = new Item[]
            {
                new Item(2, 2),
                new Item(2, 1),
            };
            int weight = 3;
            

            Knapsack knapsack = new Knapsack(items, weight);
            knapsack.EvaluateKnapsack();

        }
    }
    public struct Item
    {
        public int Profit { get; }
        public int Weight { get; }
        public int ProfitPerWeight { get; }

        public Item(int profit, int weight)
        {
            Profit = profit;
            Weight = weight;
            ProfitPerWeight = weight == 0 ? int.MaxValue : profit / Weight;
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
        public Item[] PickedItems { get; }
        public int MaxWeight { get; }

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
                Node nextNode0;     // Without adding the next item
                Node nextNode1;     // With adding the next item

                if (node.Level == n - 1)
                    continue;   // The next node is going to be a leaf
                else if (node.Level == -1)
                    nextNode1 = new Node(
                        node,
                        true,
                        0,
                        _sortedItems[0].Weight,
                        _sortedItems[0].Profit
                    );   // Actual 0,0,0 root
                else
                    nextNode1 = new Node(
                        node,
                        true,
                        node.Level + 1,
                        node.TotalWeight + _sortedItems[node.Level + 1].Weight,
                        node.TotalProfit + _sortedItems[node.Level + 1].Profit
                    );

                if (nextNode1.TotalWeight <= MaxWeight && nextNode1.TotalProfit > maxProfit)
                    maxProfit = nextNode1.TotalProfit;   // We can already see that this node is going to be better

                float nextBound1 = Bound(nextNode1);
                if (nextBound1 > maxProfit)
                    priorityQueue.Enqueue(nextNode1, nextBound1); // The next node has the potential to be better

                nextNode0 = new Node(
                    node,
                    false,
                    node.Level + 1,
                    node.TotalWeight,
                    node.TotalProfit
                    );
                float nextBound0 = Bound(nextNode0);
                if (nextBound0 > maxProfit)
                    priorityQueue.Enqueue(nextNode0, nextBound0);
            }

            Console.WriteLine($"Max profit: {maxProfit}");
        }
    }
}
