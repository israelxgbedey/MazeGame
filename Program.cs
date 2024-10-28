using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MazeGame
{
    class Player
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool HasKey { get; private set; }
        public int Score { get; private set; }

        public Player(int startX, int startY)
        {
            X = startX;
            Y = startY;
            Score = 0;
            HasKey = false;
        }

        public void Move(int dx, int dy, Maze maze)
        {
            int newX = X + dx;
            int newY = Y + dy;

            if (maze.IsWalkable(newX, newY, this))
            {
                X = newX;
                Y = newY;

                // Check for items (like key or collectible) after moving
                maze.CheckForItem(this);
            }
        }

        public void CollectKey()
        {
            HasKey = true;
            Console.WriteLine("You found a key!");
        }

        public void AddScore(int points)
        {
            Score += points;
        }
    }

    class Maze
    {
        private char[,] grid;
        private Dictionary<char, int> itemPoints = new Dictionary<char, int>
        {
            { 'k', 0 }, // Key (use lowercase 'k')
            { 'D', 0 }, // Door
            { '*', 10 } // Collectible item
        };

        public Maze()
        {
            string mazeLayout = @"
            ##########
            #C #   #E#
            #  # #   #
            # ## # # #
            #      # #
            # # ## # #
            # #      #
            ## ## ## #
            #      #k#
            ##########";

            InitializeMaze(mazeLayout);
        }

        public bool IsKey(int x, int y)
        {
            return grid[x, y] == 'k'; // Check for lowercase 'k'
        }

        public void RemoveKey(int x, int y)
        {
            if (grid[x, y] == 'k')
            {
                grid[x, y] = ' '; // Replace key with an empty space
            }
        }

        private void InitializeMaze(string mazeLayout)
        {
            var lines = mazeLayout.Trim().Split('\n');
            int rows = lines.Length;
            int cols = lines[0].Trim().Length;

            grid = new char[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                var line = lines[i].Trim();
                for (int j = 0; j < cols; j++)
                {
                    grid[i, j] = j < line.Length ? line[j] : '#';
                }
            }
        }

        public bool IsWalkable(int x, int y, Player player)
        {
            if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1))
                return false;

            char cell = grid[x, y];
            return cell != '#' && (cell != 'E' || player.HasKey); // Requires a key to open 'E'
        }

        public bool IsExit(int x, int y)
        {
            return grid[x, y] == 'E';
        }

        public void CheckForItem(Player player)
        {
            char cell = grid[player.X, player.Y];
            if (itemPoints.ContainsKey(cell))
            {
                if (cell == 'k') 
                {
                    player.CollectKey(); 
                }
                else if (cell == '*')
                {
                    player.AddScore(itemPoints[cell]);
                    Console.WriteLine($"Collected an item! Current score: {player.Score}");
                }
                grid[player.X, player.Y] = ' '; // Clear collected item from maze
            }
        }

        public void Draw(Player player, int remainingTime)
        {
            Console.Clear();
            Console.WriteLine($"Score: {player.Score} | Has Key: {player.HasKey} | Time Left: {remainingTime} seconds");

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (i == player.X && j == player.Y)
                        Console.Write('C'); // Draw player at their position
                    else
                        Console.Write(grid[i, j]);
                }
                Console.WriteLine();
            }
        }
    }

    class Game
    {
        private Player player;
        private Maze maze;
        private Stopwatch stopwatch;
        private int maxTimeInSeconds = 60;

        public Game()
        {
            maze = new Maze();
            player = new Player(1, 1); // Starting position for player 'C'
            stopwatch = new Stopwatch();
        }

        public void Run()
        {
            Console.WriteLine("Navigate the maze to reach the endpoint 'E'. Use W A S D to move. Collect items (*) for points.");

            stopwatch.Start();

            while (true)
            {
                int remainingTime = maxTimeInSeconds - (int)stopwatch.Elapsed.TotalSeconds;
                if (remainingTime <= 0)
                {
                    Console.Clear();
                    Console.WriteLine("Time's up! Game over.");
                    break;
                }

                maze.Draw(player, remainingTime);

                ConsoleKeyInfo input = Console.ReadKey(true);
                switch (input.Key)
                {
                    case ConsoleKey.W: player.Move(-1, 0, maze); break; // Up
                    case ConsoleKey.S: player.Move(1, 0, maze); break;  // Down
                    case ConsoleKey.A: player.Move(0, -1, maze); break; // Left
                    case ConsoleKey.D: player.Move(0, 1, maze); break;  // Right
                }
                if (maze.IsExit(player.X, player.Y) && player.HasKey)
                {
                    Console.Clear();
                    Console.WriteLine("Congratulations! You reached the endpoint and unlocked the door.");
                    Console.WriteLine($"Final Score: {player.Score}");
                    break;
                }
                else if (maze.IsExit(player.X, player.Y) && !player.HasKey)
                {
                    Console.WriteLine("The door is locked. You need a key to exit!");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Run();
        }
    }
}
