using System;
using System.Globalization;
using System.Numerics;

namespace Day15
{
    static class Program
    {
        static void Main(string[] args)
        {
            var filename = "PuzzleInput.txt";
            var lines = File.ReadAllLines(filename);
            if (lines.Length != 1) throw new Exception("There should only be 1 line in the file");

            var seq = lines[0].Split(',');
            Part1(seq);
        }

        static void Part1(string[] seq)
        {
            //var hashValue = RunHashAlgorithm("HASH");

            var seqHash = 0;
            foreach (var step in seq)
            {
                seqHash += RunHashAlgorithm(step);
            }

            Console.WriteLine($"Sum of HASH for sequence is : {seqHash}");
        }

        static void Part2(string[] seq)
        {
            
        }

        static int RunHashAlgorithm(string str)
        {
            // Determine the ASCII code for the current character of the string.
            // Increase the current value by the ASCII code you just determined.
            // Set the current value to itself multiplied by 17.
            // Set the current value to the remainder of dividing itself by 256.

            var currentValue = 0;

            foreach (var c in str)
            {
                //Console.WriteLine($"ASCII value of {c} is {(int)c}");
                currentValue += (int)c; // casting to int should get ascii value
                currentValue = currentValue * 17;
                currentValue = currentValue % 256;
                //Console.WriteLine($"Result of HASH algorithm for {c} is {currentValue}");
            }

            //Console.WriteLine($"Result of HASH algorithm for {str} is {currentValue}");
            return currentValue;
        }
    }
}