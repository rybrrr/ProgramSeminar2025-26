using System.Numerics;

namespace ZadaniPisemnePrace
{
    struct Position2D
    {
        public int X;
        public int Y;

        public Position2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        // TODO: make a class out of this and implement Add & Compare
    }

    struct BoardReturns
    {
        public Board Board;
        public Position2D Start;
        public Position2D Goal;

        public BoardReturns(Board board, Position2D start, Position2D goal)
        {
            Board = board;
            Start = start;
            Goal = goal;
        }
    }

    enum Field
    {
        Empty = 0,
        Obstacle = 1,
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 4; i++)
            {
                string[] input = File.ReadAllLines(@$"vstupni_soubory\{i+1}.txt");
                BoardReturns? boardReturns = LoadBoard(input);
                if (boardReturns == null)
                    throw new Exception("Invalid input!");

                Board board = ((BoardReturns)boardReturns).Board;
                Position2D start = ((BoardReturns)boardReturns).Start;
                Position2D goal = ((BoardReturns)boardReturns).Goal;

                int? moveCount = SolveBoard(board, start, goal);
                Console.WriteLine(moveCount == null ? "Do cíle se koněm nejde dostat." : moveCount);
            }
        }

        static int? SolveBoard(Board board, Position2D start, Position2D goal)
        {
            Position2D[] activePositions = new Position2D[1] { start };
            bool[,] visitedPositions = new bool[board.Width,board.Height];
            visitedPositions[start.X, start.Y] = true;

            int moveNum = 1;
            while (activePositions.Length > 0)
            {
                List<Position2D> nextActivePositionsBatch = new List<Position2D>();

                foreach (Position2D activePosition in activePositions)
                {
                    foreach (Position2D moveDir in Horse.MoveDirections)
                    {
                        Position2D nextPosition = new Position2D(
                            activePosition.X + moveDir.X,
                            activePosition.Y + moveDir.Y
                        );

                        if (nextPosition.X == goal.X && nextPosition.Y == goal.Y)
                            return moveNum;

                        if (board.IsPositionOnBoard(nextPosition) && !visitedPositions[nextPosition.X,nextPosition.Y])
                        {
                            nextActivePositionsBatch.Add(nextPosition);
                            visitedPositions[nextPosition.X, nextPosition.Y] = true;
                        }
                    }
                }

                activePositions = nextActivePositionsBatch.ToArray();
                moveNum += 1;
            }

            return null;
        }

        static Position2D? LoadPosition2D(string position)
        {
            string[] positionSplit = position.Split(' ');
            if (positionSplit.Length != 2)
                return null;

            bool succX = int.TryParse(positionSplit[0], out int X);
            bool succY = int.TryParse(positionSplit[1], out int Y);
            if (!succX || !succY)
                return null;

            return new Position2D(X, Y);
        }


        // Returns null if the input was invalid
        static BoardReturns? LoadBoard(string[] input)
        {
            if (input.Length == 0)
                return null;

            int width = 8;
            int height = 8;

            // obstacleCount
            // obstacle1_X, obstacle1_Y
            // obstacle2_X, obstacle2_Y
            // ...
            // start_X, start_Y
            // goal_Y, goal_Y

            bool succObstCnt = int.TryParse(input[0], out int obstacleCount);
            if (!succObstCnt)
                return null;

            if (input.Length != obstacleCount + 3)
                return null;

            Position2D[] obstacles = new Position2D[obstacleCount];
            for (int i=0; i<obstacleCount; i++)
            {
                Position2D? obstaclePosition = LoadPosition2D(input[i+1]);
                if (obstaclePosition == null)
                    return null;

                obstacles[i] = (Position2D) obstaclePosition;
            }

            Position2D? start = LoadPosition2D(input[input.Length - 2]);
            Position2D? goal = LoadPosition2D(input[input.Length - 1]);
            if (start == null || goal == null)
                return null;

            Board board = new Board(width, height);
            board.AddObstacles(obstacles);

            return new BoardReturns(board, (Position2D) start, (Position2D) goal);
        }
    }

    class Board
    {
        public int Width;
        public int Height;

        public Field[,] Fields { get; protected set; }

        public Board(int width, int height)
        {
            Width = width;
            Height = height;
            Fields = new Field[Width, Height];
        }

        public void AddObstacles(Position2D[] obstacles)
        {
            foreach (Position2D obstacle in obstacles)
            {
                Fields[obstacle.X, obstacle.Y] = Field.Obstacle;
            }
        }

        public bool IsPositionOnBoard(Position2D position)
        {
            return (position.X > 0 && position.X < Width)
                && (position.Y > 0 && position.Y < Height);
        }
    }

    interface IChessPiece
    {
        public static Position2D[] MoveDirections;
    }

    class Horse : IChessPiece
    {
        public static Position2D[] MoveDirections = new Position2D[]
        {
            new Position2D(-2, -1),
            new Position2D(-2, 1),
            new Position2D(-1, -2),
            new Position2D(-1, 2),
            new Position2D(1, -2),
            new Position2D(1, 2),
            new Position2D(2, -1),
            new Position2D(2, 1),
        };
    }
}
