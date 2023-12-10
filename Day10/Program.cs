using System;
using System.Numerics;

namespace Day9
{
    static class Program
    {
        private enum Direction
        {
            Up,
            Right,
            Down,
            Left
        }

        static void Main(string[] args)
        {
            var filename = "PuzzleInput.txt";
            var lines = File.ReadAllLines(filename);

            var startPoint = GetStartPoint(lines);

            // from the start position, there are multiple places you can go (up, right, down, left)
            var upRoute = GetLoop(lines, startPoint, Direction.Up);
            var rightRoute = GetLoop(lines, startPoint, Direction.Right);
            var downRoute = GetLoop(lines, startPoint, Direction.Down);
            var leftRoute = GetLoop(lines, startPoint, Direction.Left);

            var allRoutes = new List<List<char>>
            {
                upRoute,
                rightRoute,
                downRoute,
                leftRoute
            };

            foreach (var route in allRoutes)
            {
                var length = route.Count;

                if (length > 1) // all will start with 'S' at the beginning
                {
                    if (route[length - 1] == 'S')
                    {
                        // found the route that loops.furthest away point is the midpoint.
                        int midpoint;
                        if (length % 2 == 1)
                        {
                            // odd number
                            midpoint = (length - 1) / 2;
                        }
                        else
                        {
                            // even number
                            midpoint = length / 2;
                        }
                        Console.WriteLine($"Number of steps to midpoint in loop is {midpoint}");
                        break;
                    }
                }
            }
        }

        static Tuple<int, int> GetStartPoint(string[] lines)
        {
            for (var i = 0; i < lines.Length; i++)
            {
                var startCol = lines[i].IndexOf('S');
                if (startCol != -1)
                {
                    return new Tuple<int, int>(i, startCol);
                }
            }

            throw new Exception("Start Point not found in file");
        }

        static List<char> GetLoop(string[] lines, Tuple<int, int> startPoint, Direction startDirection)
        {
            // create loop and initialise with starting point
            var path = new List<char> { 'S' };
            
            var currentRow = startPoint.Item1;
            var currentCol = startPoint.Item2;
            var direction = startDirection;
            var endOfLoop = false;

            while (!endOfLoop)
            {
                // look at the character in the next position
                Tuple<int, int> nextPos;
                string validChars;

                switch (direction)
                {
                    case Direction.Up:
                        nextPos = new Tuple<int, int>(currentRow - 1, currentCol);
                        validChars = "|7F";
                        break;
                    case Direction.Right:
                        nextPos = new Tuple<int, int>(currentRow, currentCol + 1);
                        validChars = "-J7";
                        break;
                    case Direction.Down:
                        nextPos = new Tuple<int, int>(currentRow + 1, currentCol);
                        validChars = "|JL";
                        break;
                    case Direction.Left:
                        nextPos = new Tuple<int, int>(currentRow, currentCol - 1);
                        validChars = "-LF";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(startDirection), startDirection, null);
                }
                
                // get next character
                char nextChar;
                try
                {
                    var line = lines[nextPos.Item1];
                    nextChar = line[nextPos.Item2];
                }
                catch (Exception e)
                {
                    // the next character to go to may be outside the edge of the board
                    endOfLoop = true;
                    break;
                }

                // check if the character in the next position is valid
                if (validChars.Contains(nextChar))
                {
                    path.Add(nextChar);
                    direction = GetNextDirection(nextChar, direction);
                    currentRow = nextPos.Item1;
                    currentCol = nextPos.Item2;
                }
                else
                {
                    if (nextChar == 'S')
                    {
                        path.Add(nextChar);
                    }
                    endOfLoop = true;
                }
            }
            return path;
        }

        static Direction GetNextDirection(char c, Direction currentDirection)
        {
            switch (currentDirection)
            {
                case Direction.Up:
                    // valid chars are: "|7F"
                    if (c == '|') return Direction.Up;
                    if (c == '7') return Direction.Left;
                    if (c == 'F') return Direction.Right;
                    break;
                case Direction.Right:
                    // valid chars are: "-J7"
                    if (c == '-') return Direction.Right;
                    if (c == 'J') return Direction.Up;
                    if (c == '7') return Direction.Down;
                    break;
                case Direction.Down:
                    // valid chars are: "|JL"
                    if (c == '|') return Direction.Down;
                    if (c == 'J') return Direction.Left;
                    if (c == 'L') return Direction.Right;
                    break;
                case Direction.Left:
                    // valid chars are: "-LF"
                    if (c == '-') return Direction.Left;
                    if (c == 'L') return Direction.Up;
                    if (c == 'F') return Direction.Down;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentDirection), currentDirection, null);
            }
            throw new ArgumentOutOfRangeException(nameof(currentDirection), currentDirection, null);
        }
    }
}