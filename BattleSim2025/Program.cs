using System.Diagnostics.Contracts;
using System.Runtime.ExceptionServices;

namespace BattleSim2025
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Character> army1 = GenerateArmy().ToList();
            List<Character> army2 = GenerateArmy().ToList();

            while (true)
            {
                for (int i = 0; i < Math.Min(army1.Count, army2.Count); i++)
                {
                    Character char1 = army1[i];
                    Character char2 = army2[i];

                    if (!char1.IsAlive() || !char2.IsAlive())
                        continue;
                    
                    Console.WriteLine($"{char1} VS {char2}");

                    char1.Attack(char2);
                    char2.Attack(char1);

                    if (!char1.IsAlive())
                        char2.Power += 1;
                    if (!char2.IsAlive())
                        char1.Power += 1;
                }

                // Remove dead characters
                army1 = GetAliveCharacters(army1);
                army2 = GetAliveCharacters(army2);

                if (army1.Count == 0 || army2.Count == 0)
                    break;

                Console.WriteLine();
            }
        }

        public static Character[] GenerateArmy()
        {
            Character[] army = new Character[10];
            Random random = new Random();
            int slotsOccupied = 0;

            // Spawn wizards
            for (int i = 0; i < random.Next(1, 4); i++)
            {
                army[slotsOccupied] = new Wizard($"Wizard{i + 1}");
                slotsOccupied += 1;
            }

            // Spawn warriors
            for (int i = 0; i < random.Next(1, 4); i++)
            {
                army[slotsOccupied] = new Warrior($"Warrior{i + 1}");
                slotsOccupied += 1;
            }

            // Spawn archers
            for (int i = 0; slotsOccupied < army.Length; i++)
            {
                army[slotsOccupied] = new Archer($"Archer{i + 1}");
                slotsOccupied += 1;
            }

            random.Shuffle(army);

            return army;
        }

        public static List<Character> GetAliveCharacters(List<Character> army)
        {
            List<Character> newArmy = new List<Character>();

            foreach (Character character in army)
            {
                if (character.IsAlive())
                    newArmy.Add(character);
            }

            return newArmy;
        }
    }

    public class Character
    {
        public string Name;
        public int Health;
        public int Power;

        public Character(string name)
        {
            Name = name;
        }

        public virtual void Attack(Character target)
        {
            target.TakeDamage(Power);
            if (target is Wizard)   // ew
                TakeDamage(Power / 2);
        }
        
        public virtual void TakeDamage(int amount)
        {
            Health -= amount;
        }

        public bool IsAlive()
        {
            return Health > 0;
        }

        public override string ToString()
        {
            return $"{this.GetType().Name} {Name} ({Health}/{Power})";
        }
    }

    public class Wizard : Character
    {
        public Wizard(string name) : base(name)
        {
            Health = 6;
            Power = 6;
        }

        public override void Attack(Character target)
        {
            Console.WriteLine("WIZARDAAAAAAAAAA");
            base.Attack(target);
        }
    }

    public class Warrior : Character
    {
        public Warrior(string name) : base(name)
        {
            Health = 12;
            Power = 4;
        }

        public override void Attack(Character target)
        {
            Console.WriteLine("WARRIORAAAAAAAAA");
            base.Attack(target);
        }
    }

    public class Archer : Character
    {
        public Archer(string name) : base(name)
        {
            Health = 9;
            Power = 5;
        }

        public override void Attack(Character target)
        {
            Console.WriteLine("ARCHERAAAAAAAAAA");
            base.Attack(target);
        }
    }
}
