using System;
using System.Globalization;
using System.Numerics;

namespace Day16
{
    static class Program
    {
        private enum DirectionOfTravel
        {
            Up, Down, Left, Right
        }

        private enum MirrorTypes
        {
            ForwardSlash,   //      '/'
            Backslash,      //      '\'
            Vertical,       //      '|'
            Horizontal      //      '-'
        }

        private class Mirror // mirror is going to include both mirror and splitters
        {
            public MirrorTypes Type;
            public Tuple<int, int> Position;
        }

        public class EnergizedCell
        {
            public Tuple<int, int> Position;
            public bool ContainsMirror;
            public bool HorizontalPassage;
            public bool VerticalPassage;
        }

        static void Main(string[] args)
        {
            string filename = "Test1.txt";
            var lines = File.ReadAllLines(filename);
            var rows = lines.Length;
            var cols = lines[0].Length;
            var energizedCells = new List<EnergizedCell>();

            var mirrors = GetMirrorPositions(lines);

            energizedCells = FollowLightPath(new Tuple<int, int>(rows, cols), energizedCells, mirrors, 
                new Tuple<int, int>(0, 0), DirectionOfTravel.Right);

            Console.WriteLine($"Number of energized cells are: {energizedCells.Count}");
        }

        static List<Mirror> GetMirrorPositions(string[] lines)
        {
            var mirrors = new List<Mirror>();
            var row = 0;

            foreach (var line in lines)
            {
                var col = 0;
                foreach (var c in line)
                {
                    if (c == '/')
                    {
                        mirrors.Add(new Mirror(){Type = MirrorTypes.ForwardSlash, Position = new Tuple<int, int>(row,col)});
                    }
                    else if (c == '\\')
                    {
                        mirrors.Add(new Mirror() { Type = MirrorTypes.Backslash, Position = new Tuple<int, int>(row, col) });
                    }
                    else if (c == '|')
                    {
                        mirrors.Add(new Mirror() { Type = MirrorTypes.Vertical, Position = new Tuple<int, int>(row, col) });
                    }
                    else if (c == '-')
                    {
                        mirrors.Add(new Mirror() { Type = MirrorTypes.Horizontal, Position = new Tuple<int, int>(row, col) });
                    }
                    col++;
                }

                row++;
            }

            return mirrors;
        }

        static List<EnergizedCell> FollowLightPath(Tuple<int, int> gridSize, List<EnergizedCell> energizedCells, List<Mirror> mirrors,
            Tuple<int, int> startingPos, DirectionOfTravel startingDirection)
        {
            // follow light path until we hit a wall or path of light traveling in the same direction

            var position = startingPos;
            var direction = startingDirection;
            bool endPointFound = false;

            while (!endPointFound)
            {
                // check if we have hit a mirror
                bool posHasMirror = false;

                foreach (var mirror in mirrors)
                {
                    if (mirror.Position.Item1 == position.Item1
                        && mirror.Position.Item2 == position.Item2)
                    {
                        posHasMirror = true;

                        // add to energised list if it isn't already
                        if (!HasCellBeenEnergized(position, energizedCells))
                        {
                            energizedCells.Add(new EnergizedCell()
                            {
                                Position = new Tuple<int, int>(position.Item1, position.Item2), 
                                ContainsMirror = true
                            });
                        }

                        // based on mirror type, decide what to do next and update next position and direction
                        switch (direction)
                        {
                            case DirectionOfTravel.Up:
                                switch (mirror.Type)
                                {
                                    case MirrorTypes.ForwardSlash:
                                        // go right (increment the column)
                                        direction = DirectionOfTravel.Right;
                                        position = new Tuple<int, int>(position.Item1, position.Item2 + 1);
                                        if (position.Item2 >= gridSize.Item2) endPointFound = true;
                                        break;

                                    case MirrorTypes.Backslash:
                                        // go left (decrement the column)
                                        direction = DirectionOfTravel.Left;
                                        position = new Tuple<int, int>(position.Item1, position.Item2 - 1);
                                        if (position.Item2 < 0) endPointFound = true;
                                        break;

                                    case MirrorTypes.Vertical:
                                        // keep going up (decrement the row)
                                        position = new Tuple<int, int>(position.Item1 - 1, position.Item2);
                                        if (position.Item1 < 0) endPointFound = true;
                                        break;

                                    case MirrorTypes.Horizontal:
                                        // split so one goes left and one goes right with the current position as the start position
                                        energizedCells = FollowLightPath(gridSize, energizedCells, mirrors, position, DirectionOfTravel.Left);
                                        energizedCells = FollowLightPath(gridSize, energizedCells, mirrors, position, DirectionOfTravel.Right);
                                        break;

                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;

                            case DirectionOfTravel.Down:
                                switch (mirror.Type)
                                {
                                    case MirrorTypes.ForwardSlash:
                                        // go left (decrement the column)
                                        direction = DirectionOfTravel.Left;
                                        position = new Tuple<int, int>(position.Item1, position.Item2 - 1);
                                        if (position.Item2 < 0) endPointFound = true;
                                        break;

                                    case MirrorTypes.Backslash:
                                        // go right (increment the column)
                                        direction = DirectionOfTravel.Right;
                                        position = new Tuple<int, int>(position.Item1, position.Item2 + 1);
                                        if (position.Item2 >= gridSize.Item2) endPointFound = true;
                                        break;

                                    case MirrorTypes.Vertical:
                                        // keep going down (increment the row)
                                        position = new Tuple<int, int>(position.Item1 + 1, position.Item2);
                                        if (position.Item1 >= gridSize.Item1) endPointFound = true;
                                        break;

                                    case MirrorTypes.Horizontal:
                                        // split so one goes left and one goes right with the current position as the start position
                                        energizedCells = FollowLightPath(gridSize, energizedCells, mirrors, position, DirectionOfTravel.Left);
                                        energizedCells = FollowLightPath(gridSize, energizedCells, mirrors, position, DirectionOfTravel.Right);
                                        break;

                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;

                            case DirectionOfTravel.Left:
                                switch (mirror.Type)
                                {
                                    case MirrorTypes.ForwardSlash:
                                        // go down (increment the row)
                                        direction = DirectionOfTravel.Down;
                                        position = new Tuple<int, int>(position.Item1 + 1, position.Item2);
                                        if (position.Item1 >= gridSize.Item1) endPointFound = true;
                                        break;

                                    case MirrorTypes.Backslash:
                                        // go up (decrement the row)
                                        direction = DirectionOfTravel.Up;
                                        position = new Tuple<int, int>(position.Item1 - 1, position.Item2);
                                        if (position.Item1 < 0) endPointFound = true;
                                        break;

                                    case MirrorTypes.Vertical:
                                        // split so one goes up and one goes down with the current position as the start position
                                        energizedCells = FollowLightPath(gridSize, energizedCells, mirrors, position, DirectionOfTravel.Up);
                                        energizedCells = FollowLightPath(gridSize, energizedCells, mirrors, position, DirectionOfTravel.Down);
                                        break;

                                    case MirrorTypes.Horizontal:
                                        // keep going left (decrement the col)
                                        position = new Tuple<int, int>(position.Item1, position.Item2 - 1);
                                        if (position.Item2 < 0) endPointFound = true;
                                        break;

                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;

                            case DirectionOfTravel.Right:
                                switch (mirror.Type)
                                {
                                    case MirrorTypes.ForwardSlash:
                                        // go up (decrement the row)
                                        direction = DirectionOfTravel.Up;
                                        position = new Tuple<int, int>(position.Item1 - 1, position.Item2);
                                        if (position.Item1 < 0) endPointFound = true;
                                        break;

                                    case MirrorTypes.Backslash:
                                        // go down (increment the row)
                                        direction = DirectionOfTravel.Down;
                                        position = new Tuple<int, int>(position.Item1 + 1, position.Item2);
                                        if (position.Item1 >= gridSize.Item1) endPointFound = true;
                                        break;

                                    case MirrorTypes.Vertical:
                                        // split so one goes up and one goes down with the current position as the start position
                                        energizedCells = FollowLightPath(gridSize, energizedCells, mirrors, position, DirectionOfTravel.Up);
                                        energizedCells = FollowLightPath(gridSize, energizedCells, mirrors, position, DirectionOfTravel.Down);
                                        break;

                                    case MirrorTypes.Horizontal:
                                        // keep going right (increment the col)
                                        position = new Tuple<int, int>(position.Item1, position.Item2 + 1);
                                        if (position.Item2 >= gridSize.Item2) endPointFound = true;
                                        break;

                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        
                        break;
                    }
                }

                if (!posHasMirror)
                {
                    var foundExistingEnergizedCell = false;

                    foreach (var cell in energizedCells)
                    {
                        if (cell.Position.Item1 == position.Item1
                            || cell.Position.Item2 == position.Item2)
                        {
                            foundExistingEnergizedCell = true;

                            // determine if we are going over the same path and if we are end looping
                            if (direction == DirectionOfTravel.Up || direction == DirectionOfTravel.Down)
                            {
                                if (cell.VerticalPassage)
                                {
                                    endPointFound = true;
                                }
                                else
                                {
                                    cell.VerticalPassage = true;
                                }
                            }
                            else
                            {
                                if (cell.HorizontalPassage)
                                {
                                    endPointFound = true;
                                }
                                else
                                {
                                    cell.HorizontalPassage = true;
                                }
                            }
                            break;
                        }
                    }

                    if (!foundExistingEnergizedCell)
                    {
                        energizedCells.Add(new EnergizedCell()
                        {
                            Position = new Tuple<int, int>(position.Item1, position.Item2),
                            VerticalPassage = direction == DirectionOfTravel.Up || direction == DirectionOfTravel.Down,
                            HorizontalPassage = direction == DirectionOfTravel.Right || direction == DirectionOfTravel.Left,
                        });
                    }

                    if (!endPointFound)
                    {
                        // find next position and direction
                        switch (direction)
                        {
                            case DirectionOfTravel.Up:
                                // go up (decrement the row)
                                direction = DirectionOfTravel.Up;
                                position = new Tuple<int, int>(position.Item1 - 1, position.Item2);
                                if (position.Item1 < 0) endPointFound = true;
                                break;

                            case DirectionOfTravel.Down:
                                // go down (increment the row)
                                direction = DirectionOfTravel.Down;
                                position = new Tuple<int, int>(position.Item1 + 1, position.Item2);
                                if (position.Item1 >= gridSize.Item1) endPointFound = true;
                                break;

                            case DirectionOfTravel.Left:
                                // go left (decrement the column)
                                direction = DirectionOfTravel.Left;
                                position = new Tuple<int, int>(position.Item1, position.Item2 - 1);
                                if (position.Item2 < 0) endPointFound = true;
                                break;

                            case DirectionOfTravel.Right:
                                // go right (increment the column)
                                direction = DirectionOfTravel.Right;
                                position = new Tuple<int, int>(position.Item1, position.Item2 + 1);
                                if (position.Item2 >= gridSize.Item2) endPointFound = true;
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }

            return energizedCells;
        }

        static bool HasCellBeenEnergized(Tuple<int, int> position, List<EnergizedCell> energizedCells)
        {
            var cellEnergized = false;

            foreach (var cell in energizedCells)
            {
                if (cell.Position.Item1 == position.Item1
                    && cell.Position.Item2 == position.Item2)
                {
                    cellEnergized = true;
                    break;
                }
            }
            return cellEnergized;
        }
    }
}