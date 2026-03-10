namespace Knapsack
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Item[] items = new Item[] {
                new Item(100, 10),
                new Item(120, 8),
                new Item(50, 5),
                new Item(50, 3),
            };

            Knapsack knapsack = new Knapsack(items, 15);
        }
    }
    public struct Item
    {
        public int Value { get; }
        public int Weight { get; }
        public int ValuePerWeight { get; }

        public Item(int value, int weight)
        {
            Value = value;
            Weight = weight;
            ValuePerWeight = weight == 0 ? int.MaxValue : value / Weight;
        }
    }

    public struct Node
    {
        public int Level { get; }           // Index of the item in the sorted array
        public int TotalWeight { get; }     // Includes this node and all the nodes with level <= this.Level
        public int TotalProfit { get; }     // Includes this node and all the nodes with level <= this.Level

        public Node(int level, int totalWeight, int totalProfit)
        {
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
                item2.ValuePerWeight.CompareTo(item1.ValuePerWeight));

            _sortedItems = sortedItems;
        }

        private void EvaluateKnapsack()
        {
            PriorityQueue<Item, float> priorityQueue = new PriorityQueue<Item, float>();

        }
    }
}
