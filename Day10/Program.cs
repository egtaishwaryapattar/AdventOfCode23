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

        private class CellInfo
        {
            public char Char;
            public Tuple<int, int> Position;
        }

        static void Main(string[] args)
        {
            var filename = "Test2.txt";
            var lines = File.ReadAllLines(filename);

            // Part 1
            var validRoute = GetValidLoop(lines);

            // Part2
            var numInside = GetTilesInsideRoute(validRoute, lines);
            Console.WriteLine($"Tiles inside route = {numInside}");
        }
        static List<CellInfo> GetValidLoop(string[] lines)
        {
            var startPoint = GetStartPoint(lines);

            // from the start position, there are multiple places you can go (up, right, down, left)
            var upRoute = GetPossibleRoute(lines, startPoint, Direction.Up);
            var rightRoute = GetPossibleRoute(lines, startPoint, Direction.Right);
            var downRoute = GetPossibleRoute(lines, startPoint, Direction.Down);
            var leftRoute = GetPossibleRoute(lines, startPoint, Direction.Left);

            var allRoutes = new List<List<CellInfo>>
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
                    if (route[length - 1].Char == 'S')
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
                        return route;
                    }
                }
            }

            throw new Exception("Valid route not found");
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

        static List<CellInfo> GetPossibleRoute(string[] lines, Tuple<int, int> startPoint, Direction startDirection)
        {
            // create loop and initialise with starting point
            var path = new List<CellInfo> { new CellInfo{Char = 'S', Position = startPoint} };
            
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
                    path.Add(new CellInfo {Char = nextChar, Position = nextPos});
                    direction = GetNextDirection(nextChar, direction);
                    currentRow = nextPos.Item1;
                    currentCol = nextPos.Item2;
                }
                else
                {
                    if (nextChar == 'S')
                    {
                        path.Add(new CellInfo { Char = nextChar, Position = nextPos});
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

        static int GetTilesInsideRoute(List<CellInfo> route, string[] lines)
        {
            var numRows = lines.Count();
            var numCols = lines[0].Length;
            var numInside = 0;

            // iterate through each cell in the file and identify whether it is inside the loop or not
            for (var row = 0; row < numRows; row++)
            {
                for (var col = 0; col < numCols; col++)
                {
                    // first check that it isn't a cell on the route
                    if (!IsCellOnRoute(route, row, col))
                    {
                        // if cell isn't on route, identify whether it is inside or not
                        // can determine this by how many times it intersects the route until it gets to the edge
                        // odd number = inside. 0 or even number = outside
                        // intersections need to be perpendicular - do not follow the length of a route
                        //      therefor exclude '|' if we are following in vertical direction  
                        //      exclude '-' if we are following in horizontal direction
                        // for ease, we are going to follow all cells vertically upwards to see how many times they intersect
                        
                        if (row != 0) // if row is 0, it will be on the outside
                        {
                            var numIntersections = 0;
                            //var numJunctions = 0;

                            // check whether rows above the cell intersect the route
                            for (int i = row - 1; i >= 0; i--)
                            {
                                if (IsCellOnRoute(route, i, col))
                                {
                                    char c = (lines[i])[col];
                                    if (c == '-')
                                    {
                                        numIntersections++;
                                    }


                                    //// confirm it is not following the path
                                    //if (c != '|')
                                    //{
                                    //    if (c == '-')
                                    //    {
                                    //        // perpendicular intersection
                                    //        numIntersections++;
                                    //    }
                                    //    else
                                    //    {
                                    //        // it has crossed a junction - need to find the pair to complete a route intersection
                                    //        numJunctions++;

                                    //        if (numJunctions == 2)
                                    //        {
                                    //            numIntersections++;
                                    //            numJunctions = 0;
                                    //        }
                                    //    }
                                    //}
                                }
                            }

                            if (numIntersections % 2 == 1)
                            {
                                numInside++;
                            }
                        }
                    }
                }
            }

            return numInside;
        }

        // row and col corresponds to the cell we want to compare and check if it exists on the route
        static bool IsCellOnRoute(List<CellInfo> route, int row, int col)
        {
            foreach (var cell in route)
            {
                if (cell.Position.Item1 == row
                    && cell.Position.Item2 == col)
                {
                    return true;
                }
            }
            return false;
        }
    }
}