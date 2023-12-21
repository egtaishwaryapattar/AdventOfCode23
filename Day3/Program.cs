using System;
using System.Data;
using System.Text;

namespace Day3
{
    static class Program
    {
        private static char[,] _matrix;

        static void Main(string[] args)
        {
            var filename = "PuzzleInput.txt";
            var lines = File.ReadAllLines(filename);

            ConvertLinesToMatrix(lines);

            var partNumbers = FindPartNumbers();

            var sum = 0;
            foreach (var part in partNumbers)
            {
                sum += part;
            }

            Console.WriteLine($"Sum of part numbers is {sum}");

            // answer is greater than 530822
        }

        static void ConvertLinesToMatrix(string[] lines)
        {
            var rows = lines.Length;
            var cols = lines[0].Length;

            _matrix = new char[rows, cols];

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    _matrix[row, col] = (lines[row])[col];
                }
            }
        }

        static List<int> FindPartNumbers()
        {
            var partNumbers = new List<int>();
            var numRows = _matrix.GetLength(0);
            var numCols = _matrix.GetLength(1);

            for (var row = 0; row < numRows; row++)
            {
                for (var col = 0; col < numCols; col++)
                {
                    if (IsDigit(_matrix[row, col]))
                    {
                        // found the start digit of a number - find out what the number is
                        var numAsString = new StringBuilder();
                        var numFound = false;
                        var startCol = col;
                        var numDigits = 0;

                        while (!numFound)
                        {
                            if (IsDigit(_matrix[row, col]))
                            {
                                numAsString.Append(_matrix[row, col]);
                                numDigits++;
                                col++;
                                if (col == numCols) numFound = true;
                            }
                            else
                            {
                                //Console.WriteLine($"Found number: {numAsString.ToString()}");
                                numFound = true;
                            }
                        }

                        // find if there is a symbol around it
                        if (IsPartNumber(row, startCol, numDigits))
                        {
                            var number = Convert.ToInt16(numAsString.ToString());
                            partNumbers.Add(number);
                            //Console.WriteLine($"{number} is a part number");
                        }
                        else
                        {
                            Console.WriteLine($"{numAsString.ToString()} is NOT a part number");
                        }
                    }
                }
            }

            return partNumbers;
        }

        static bool IsDigit(char c)
        {
            if (c == '0') return true;
            if (c == '1') return true;
            if (c == '2') return true;
            if (c == '3') return true;
            if (c == '4') return true;
            if (c == '5') return true;
            if (c == '6') return true;
            if (c == '7') return true;
            if (c == '8') return true;
            if (c == '9') return true;
            return false;
        }

        static bool IsSymbol(char c)
        {
            return !IsDigit(c) && c != '.';
        }

        static bool IsPartNumber(int row, int startCol, int numDigits)
        {
            var symbolFound = false;

            for (var i = 0; i < numDigits; i++)
            {
                if (i == 0)
                {
                    symbolFound = CheckToLeftOfFirstDigit(row, startCol + i);
                    if (symbolFound) break;
                }
                if (i == numDigits - 1)
                {
                    symbolFound = CheckToRightOfLastDigit(row, startCol + i);
                    if (symbolFound) break;
                }

                symbolFound = CheckAboveAndBelowDigit(row, startCol + i);
                if (symbolFound) break;
            }

            return symbolFound;
        }

        static bool CheckToLeftOfFirstDigit(int row, int col)
        {
            if (col - 1 >= 0)
            {
                // check left
                var isSymbol = IsSymbol(_matrix[row, col - 1]);
                if (isSymbol) return true;

                // check north west
                if (row - 1 >= 0)
                {
                    isSymbol = IsSymbol(_matrix[row - 1, col - 1]);
                    if (isSymbol) return true;
                }

                // check south west
                if (row + 1 < _matrix.GetLength(0))
                {
                    isSymbol = IsSymbol(_matrix[row + 1, col - 1]);
                    if (isSymbol) return true;
                }
            }

            return false;
        }

        static bool CheckToRightOfLastDigit(int row, int col)
        {
            var numCols = _matrix.GetLength(1);
            if (col + 1 < numCols)
            {
                // check right
                var isSymbol = IsSymbol(_matrix[row, col + 1]);
                if (isSymbol) return true;

                // check north east
                if (row - 1 >= 0)
                {
                    isSymbol = IsSymbol(_matrix[row - 1, col + 1]);
                    if (isSymbol) return true;
                }

                // check south east
                if (row + 1 < _matrix.GetLength(0))
                {
                    isSymbol = IsSymbol(_matrix[row + 1, col + 1]);
                    if (isSymbol) return true;
                }
            }

            return false;
        }

        static bool CheckAboveAndBelowDigit(int row, int col)
        {
            // check above
            if (row - 1 >= 0)
            {
                var isSymbol = IsSymbol(_matrix[row - 1, col]);
                if (isSymbol) return true;
            }

            // check below
            if (row + 1 < _matrix.GetLength(0))
            {
                var isSymbol = IsSymbol(_matrix[row + 1, col]);
                if (isSymbol) return true;
            }

            return false;
        }
    }
}