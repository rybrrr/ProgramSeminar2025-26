using System.ComponentModel.Design;

namespace StabilniManzelstvi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Load input
            Console.WriteLine("vstup (nejprve n, preference žen, pak mužů):");

            bool succ = int.TryParse(Console.ReadLine(), out int n);
            if (!succ)
                throw new Exception("Invalid input!");

            // Add both women and man to not repeat myself (1am coding moment)
            Woman[] women = new Woman[n];
            Man[] men = new Man[n];
            for (int i = 0;  i < 2*n; i++)
            {
                string? preference = Console.ReadLine();
                if (preference == null)
                    throw new Exception("Invalid input!");

                int[] preferenceValues = preference.Split(' ').Select(n => int.Parse(n)).ToArray();
                if (preferenceValues.Length != n)
                    throw new Exception("Invalid input!");

                if (i < n)
                    women[i] = new Woman(i + 1, preferenceValues);
                else
                    men[i - n] = new Man(i - n + 1, preferenceValues);
            }

            // Magic
            LilSocialExperiment sgrfojg = new LilSocialExperiment(men, women);
            while (sgrfojg.IsAnyWomanSingle())
            {
                sgrfojg.StartTheNextRoundOfLegallyBindingEmotionalSupportContracts();
            }

            // As long as it works, let's not touch it
            Console.WriteLine("výstup (muži přiřazení k ženám):");
            foreach (Woman woman in sgrfojg.Women)
            {
                Console.WriteLine(woman.MarriedToId);
            }
        }
    }

    class LilSocialExperiment
    {
        public Man[] Men { get; set; }
        public Woman[] Women { get; set; }

        public LilSocialExperiment(Man[] men, Woman[] women)
        {
            Men = men;
            Women = women;
        }

        public void StartTheNextRoundOfLegallyBindingEmotionalSupportContracts()    // What sleep deprevation and access to chatgpt does to a man
        {
            // U+2615
            foreach (Woman woman in Women)
            {
                if (woman.MarriedToId == null)
                {
                    int nextPreference = woman.Preference.Dequeue();
                    Men[nextPreference - 1].ReceivedProposals.Add(woman.Id); // Why the hell do Ids start at 1, this isn't Lua, the index doesn't match the Id....
                }
            }

            // U+1F4AA
            foreach (Man man in Men)
            {
                int? bestMatch = man.MarriedToId;
                int bestIndex = bestMatch == null ? int.MaxValue : man.Preference[(int) bestMatch];
                foreach (int proposalId in man.ReceivedProposals)
                {
                    if (bestIndex > man.Preference[proposalId])
                    {
                        bestMatch = proposalId;
                        bestIndex = man.Preference[proposalId];
                    }
                }

                if (bestMatch == null)  // No way to get divorced and die alone, no need to handle that
                    continue;

                if (bestMatch != man.MarriedToId)
                {
                    // 💔💔💔
                    if (man.MarriedToId != null)
                        Women[(int) man.MarriedToId - 1].MarriedToId = null;    // 2 am, can't even decide whether using the Id or the Woman object is the better choice

                    man.MarriedToId = bestMatch;
                    Women[(int) bestMatch - 1].MarriedToId = man.Id;
                }
            }
        }

        public bool IsAnyWomanSingle()
        {
            foreach (Woman woman in Women)
                if (woman.MarriedToId == null)
                    return true;

            return false;
        }
    }

    abstract class Person
    {
        // Oh God, please forgive me for bringing such trash code into this world
        public int Id { get; set; }
        public int? MarriedToId { get; set; }

        // public nonexistent IEnumerable<int> Preference { get; set; } // I have no clue what I'm doing

        public Person(int id)//, IEnumerable<int> preference)
        {
            Id = id;
            // Preference = preference; // huh
        }
    }

    class Woman : Person
    {
        // Sht I'm bout to pass out
        public Queue<int> Preference { get; set; }

        public Woman(int id, IEnumerable<int> preference) : base(id) //, new Queue<int>(preference))
        {
            // Ughh???
            Preference = new Queue<int>(preference);
        }
    }

    class Man : Person
    {
        public Dictionary<int, int> Preference { get; set; } // womanId to preference ranking (0 is best)
        public List<int> ReceivedProposals { get; set; }

        public Man(int id, int[] preference) : base(id) // , preference)
        {
            Dictionary<int, int> preferenceDict = new Dictionary<int, int>();
            for (int i = 0; i < preference.Length; i++)
            {
                preferenceDict.Add(preference[i], i);
            }

            Preference = preferenceDict;
            ReceivedProposals = new List<int>();
        }
    }
}

// finish time: 2:27am
