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

            (int Start, int Goal)? pathEnds = GetPathEndsFromInput(map);
            if (pathEnds == null)
            {
                Console.WriteLine("Neplatný vstup.");
                return;
            }

            (int[] Nodes, int Distance)? path = map.FindPath(pathEnds.Value.Start, pathEnds.Value.Goal, 1);

            Console.WriteLine();
            Console.WriteLine("Výstup:");
            Map.PrettyPrintPath(path);
        }

        static (int Start, int Goal)? GetPathEndsFromInput(Map map)
        {
            string? startAndGoal = Console.ReadLine();
            if (startAndGoal == null)
                return null;

            string[] startAndGoalSplit = startAndGoal.Split(" ");

            bool startLoadSuccess = int.TryParse(startAndGoalSplit[0], out int startIndex);
            bool goalLoadSuccess = int.TryParse(startAndGoalSplit[1], out int goalIndex);

            if (!startLoadSuccess || !goalLoadSuccess)
                return null;

            if (!map.DoesCityExist(startIndex) || !map.DoesCityExist(goalIndex))
                return null;

            return (startIndex, goalIndex);
        }
    }

    public class Map
    {
        public byte?[,] Tolls { get; }    // 1 if there is toll, 0 if there is none
        public int?[,] CityDistances { get; }
        public int CityCount { get; }

        public Map(int?[,] cityDistances, byte?[,] tolls)
        {
            Tolls = tolls;
            CityDistances = cityDistances;
            CityCount = cityDistances.GetLength(0);
        }

        public bool DoesCityExist(int cityID)
        {
            return cityID >= 0 && cityID < CityCount;
        }

        public (int[] Nodes, int Distance)? FindPath(int startID, int goalID, byte maxTolls)
        {
            int?[,] tollsXDistances = new int?[CityCount, maxTolls+1];
            int?[,] tollsXParents = new int?[CityCount, maxTolls+1];

            // Initialize start distances
            for (int i = 0; i <= maxTolls; i++)
                tollsXDistances[startID, i] = 0;

            // Enqueue touples consisting of (cityID, tollCount)
            // Assume the maximum number of tolls is <= 255
            PriorityQueue<(int, byte), int> openNodes = new PriorityQueue<(int, byte), int>();
            openNodes.Enqueue((startID, 0), 0);

            // Perform the Dijkstra algorithm
            while (openNodes.Count > 0)
            {
                (int nodeID, byte currentToll) = openNodes.Dequeue();
                int currentDistance = (int)tollsXDistances[nodeID, currentToll]!;

                // Loop through all the connected nodes
                for (int i = 0; i < CityCount; i++)
                {
                    int? distance = CityDistances[nodeID, i];
                    byte? nextToll = Tolls[nodeID, i];
                    if (distance == null || nextToll == null)
                        continue;   // The nodes are not connected

                    byte totalToll = (byte)(currentToll + nextToll);
                    int totalDistance = (int)(currentDistance + distance);

                    if (totalToll > maxTolls)
                        continue;   // The maximum number of tolls would be overstepped

                    int? currentBestDistance = tollsXDistances[i, totalToll];

                    if (currentBestDistance != null && currentBestDistance <= totalDistance)
                        continue;   // The distance would not get better

                    // If the nodes are connected, the tolls is not too high, and the distance may improve,
                    // update the next node's info and enqueue it
                    tollsXDistances[i, totalToll] = totalDistance;
                    tollsXParents[i, totalToll] = nodeID;
                    openNodes.Enqueue((i, totalToll), totalDistance);
                }
            }

            int? bestDistance = null;
            int? bestToll = null;

            for (int i = 0; i <= maxTolls; i++)
            {
                int? distance = tollsXDistances[goalID, i];
                if (distance != null && (bestDistance == null || distance < bestDistance))
                {
                    bestDistance = distance;
                    bestToll = i;
                }
            }

            if (bestDistance == null)
                return null;    // No path was found

            List<int> path = [goalID];

            int prevNode = goalID;
            int? parent = tollsXParents[prevNode, (int)bestToll!];
            int remainingToll = (int)bestToll;
            while (parent != null)
            {
                int? toll = Tolls[prevNode, (int)parent];

                if (toll != null)
                    remainingToll -= (int)toll;

                prevNode = (int)parent;
                parent = tollsXParents[prevNode, remainingToll];

                path.Add(prevNode);
            }

            path.Reverse();

            return (path.ToArray(), (int)bestDistance);
        }

        public static void PrettyPrintPath((int[] Nodes, int Distance)? path)
        {
            if (path == null)
            {
                Console.WriteLine("Mezi městy nevede žádná cesta splňující dané podmínky.");
                return;
            }

            string pathString = string.Join(" -> ", path.Value.Nodes);
            Console.WriteLine(pathString);
            Console.WriteLine($"vzdálenost: {path.Value.Distance}");
        }

        public static Map? LoadFromInput()
        {
            string? graphDimensions = Console.ReadLine();

            if (graphDimensions == null)
                return null;

            string[] graphDimensionsSplit = graphDimensions.Split(" ");

            if (graphDimensionsSplit.Length != 2)
                return null;

            bool cityLoadSuccess = int.TryParse(graphDimensionsSplit[0], out int numCities);
            bool roadLoadSuccess = int.TryParse(graphDimensionsSplit[1], out int numRoads);

            if (!cityLoadSuccess || !roadLoadSuccess)
                return null;

            if (numCities <= 0 || numRoads < 0)
                return null;

            int?[,] cityDistances = new int?[numCities, numCities];
            byte?[,] tolls = new byte?[numCities, numCities];

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

                tolls[city1, city2] = (byte)toll;
                tolls[city2, city1] = (byte)toll;
            }

            Map newMap = new Map(cityDistances, tolls);

            return newMap;
        }
    }
}
