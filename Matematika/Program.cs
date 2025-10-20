using System.Text;

namespace Matematika
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }

    public struct Interval
    {
        public double LowerBound { get; }
        public double UpperBound { get; }

        public bool LowerBoundIncluded { get; }
        public bool UpperBoundIncluded { get; }

        public Interval(double lowerBound, double upperBound, bool lowerBoundIncluded, bool upperBoundIncluded)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
            LowerBoundIncluded = lowerBoundIncluded;
            UpperBoundIncluded = upperBoundIncluded;
        }

        public string GetIntervalRepresentation()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(LowerBoundIncluded ? '[' : '(');
            // sb.Append(Math.Abs(LowerBound) < double.MaxValue ? LowerBound : (LowerBound < 0 ? "-∞" : "∞"));
            sb.Append(LowerBound);
            sb.Append(", ");
            // sb.Append(Math.Abs(UpperBound) < double.MaxValue ? UpperBound : (UpperBound < 0 ? "-∞" : "∞"));
            sb.Append(UpperBound);
            sb.Append(UpperBoundIncluded ? ']' : ')');
            return sb.ToString();
        }
    }

    public abstract class MathFunction
    {
        public virtual string Name { get; }
        public virtual string Description { get; }
        public abstract Interval Domain { get; protected set; }
        public abstract Interval Range { get; protected set; }

        public MathFunction(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public abstract double GetValueAt(double x);

        public virtual void PrintInfo()
        {
            Console.WriteLine(Name);
            Console.WriteLine(Description);
            Console.WriteLine(Domain.GetIntervalRepresentation());
        }
    }

    public class LinearFunction : MathFunction
    {
        public double a { get; }
        public double b { get; }

        public LinearFunction(double a, double b) : base("Lineární funkce", $"f(x) = {a}x + {b}");
        {
            this.a = a;
            this.b = b;

            
        }
    }
}
