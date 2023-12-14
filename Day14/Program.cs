using System;
using System.Globalization;
using System.Numerics;

namespace Day11
{
    static class Program
    {
        private enum Direction
        {
            North,
            East, 
            South,
            West
        }

        private class Point
        {
            public int X;
            public int Y;
        }

        static void Main(string[] args)
        {
            string filename = "PuzzleInput.txt";
            var lines = File.ReadAllLines(filename);
            var matrix = ConvertLinesToMatrix(lines);
            PrintMatrix(matrix);
            
            var (barriers, balls) = GetLocationOfBarriersAndBalls(matrix);
            balls = RollBoard(matrix, Direction.North, barriers, balls);
            
            // get sum of balls
            var sum = 0;
            foreach (var ball in balls)
            {
                sum += lines.Length - ball.X;
            }
            Console.WriteLine($"Sum = {sum}");
        }

        static char[,] ConvertLinesToMatrix(string[] lines)
        {
            var rows = lines.Length;
            var cols = lines[0].Length;

            char[,] matrix = new char[rows, cols];

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    matrix[row, col] = (lines[row])[col];
                }
            }
            return matrix;
        }

        // first Item is Barriers. Second item is balls
        static Tuple<List<Point>, List<Point>> GetLocationOfBarriersAndBalls(char[,] matrix)
        {
            var barriers = new List<Point>();
            var balls = new List<Point>();

            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] == '#')
                    {
                        barriers.Add(new Point{X = i, Y = j});
                    }
                    else if (matrix[i, j] == 'O')
                    {
                        balls.Add(new Point { X = i, Y = j });
                    }
                }
            }

            return new Tuple<List<Point>, List<Point>>(barriers, balls);
        }

        static List<Point> RollBoard(char[,] matrix, Direction direction, List<Point> barriers, List<Point> balls)
        {
            var numRows = matrix.GetLength(0);

            switch (direction)
            {
                case Direction.North:
                    // sort balls in ascending Row order
                    balls.Sort(ComparePointsByX);

                    foreach (var ball in balls)
                    {
                        if (ball.X > 0) // exclude the top row as they won't shift when the board is tilted north
                        {
                            // decrement the row until we hit a barrier, ball
                            for (var row = ball.X - 1; row >= 0; row--)
                            {
                                if (IsCellOccupied(new Point { X = row, Y = ball.Y }, barriers, balls))
                                {
                                    // found an end point so stop rolling ball
                                    break;
                                }
                                else
                                {
                                    // no ball or barrier so update position
                                    matrix[ball.X, ball.Y] = '.';
                                    matrix[row, ball.Y] = 'O';
                                    ball.X = row;
                                }
                            }
                        }
                    }

                    break;
                case Direction.East:
                    break;
                case Direction.South:
                    break;
                case Direction.West:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            Console.WriteLine("Balls rolled north");
            PrintMatrix(matrix);
            return balls;
        }

        static int ComparePointsByX(Point a, Point b)
        {
            if (a.X == b.X) return 0;
            if (a.X > b.X) return 1;
            return -1; // a.X < b.X
        }

        static bool IsCellOccupied(Point point, List<Point> barriers, List<Point> balls)
        {
            return barriers.Any(barrier => barrier.X == point.X && barrier.Y == point.Y) 
                   || balls.Any(ball => ball.X == point.X && ball.Y == point.Y);
        }

        static void PrintMatrix(char[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write($"{matrix[i, j]}");
                }
                Console.Write("\n");
            }
        }

    }
}