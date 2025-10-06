using System.Globalization;
using System.Runtime.InteropServices;

namespace PrijmeniJmeno_Navigace
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Vstup:");
            Map? map = Map.LoadFromInput();
            if (map == null)
            {
                Console.WriteLine("Neplatný vstup.");
                return;
            }

            string? startAndGoal = Console.ReadLine();
            if (startAndGoal == null)
            {
                Console.WriteLine("Neplatný vstup.");
                return;
            }

#pragma warning disable CS8602 // Přístup přes ukazatel k možnému odkazu s hodnotou null
            string[] startAndGoalSplit = startAndGoal.Split(" ");
#pragma warning restore CS8602 // Přístup přes ukazatel k možnému odkazu s hodnotou null

            bool startLoadSuccess = int.TryParse(startAndGoalSplit[0], out int startIndex);
            bool goalLoadSuccess = int.TryParse(startAndGoalSplit[1], out int goalIndex);

            if (!startLoadSuccess || !goalLoadSuccess)
            {
                Console.WriteLine("Neplatný vstup.");
                return;
            }


        }
    }


    public class Map
    {
        public int[,] CityDistances { get; set; }
        public bool[,] HasToll { get; set; }

        public Map(int[,] cityDistances, bool[,] hasToll)
        {
            CityDistances = cityDistances;
            HasToll = hasToll;
        }

        public static Map? LoadFromInput()
        {
            string? graphDimensions = Console.ReadLine();

            if (graphDimensions == null)
                return null;

            string[] graphDimensionsSplit = graphDimensions.Split(" ");
            bool cityLoadSuccess = int.TryParse(graphDimensionsSplit[0], out int numCities);
            bool roadLoadSuccess = int.TryParse(graphDimensionsSplit[1], out int numRoads);

            if (!cityLoadSuccess || !roadLoadSuccess)
                return null;

            if (numCities <= 0 || numRoads < 0)
                return null;

            int[,] cityDistances = new int[numCities, numCities];
            bool[,] hasToll = new bool[numCities, numCities];

            for (int i = 0; i < numRoads; i++)
            {
                string? roadInfo = Console.ReadLine();
                if (roadInfo == null)
                    return null;

                string[] roadInfoSplit = roadInfo.Split(" ");
                if (roadInfoSplit.Length != 4)  // city1, city2, distance, hasToll
                    return null;

                bool city1LoadSuccess = int.TryParse(roadInfoSplit[0], out int city1);
                bool city2LoadSuccess = int.TryParse(roadInfoSplit[1], out int city2);
                bool distanceLoadSuccess = int.TryParse(roadInfoSplit[2], out int distance);
                bool tollLoadSuccess = int.TryParse(roadInfoSplit[3], out int toll);

                // Will fail if the value is not an integer
                if (!city1LoadSuccess || !city2LoadSuccess || !distanceLoadSuccess || !tollLoadSuccess)
                    return null;

                if ((city1 < 0 || city1 >= numCities) || (city2 < 0 || city2 >= numCities))
                    return null;

                if (distance <= 0)
                    return null;

                if (toll != 0 && toll != 1)
                    return null;

                cityDistances[city1, city2] = distance;
                cityDistances[city2, city1] = distance;

                hasToll[city1, city2] = toll == 1;
                hasToll[city2, city1] = toll == 1;
            }

            Map newMap = new Map(cityDistances, hasToll);

            return newMap;
        }
    }
}
