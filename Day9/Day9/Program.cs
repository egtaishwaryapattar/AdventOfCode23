using System;
using System.Numerics;

namespace Day9
{
    static class Program
    {
        // this program intends to be run every day
        static void Main(string[] args)
        {
            var sumOfNextNums = 0;
            var lines = File.ReadAllLines("PuzzleInput.txt");

            foreach (var line in lines)
            {
                var sequence = GetSequence(line);
                var nextNum = GetNextNumberInSequence(sequence);
                Console.WriteLine($"Next Number in sequence is {nextNum}");
                sumOfNextNums += nextNum;
            }

            Console.WriteLine($"Sum of all next numbers is {sumOfNextNums}");
        }

        static List<int> GetSequence(string line)
        {
            var extractedNumbers = line.Split(' ');
            var sequence = new List<int>();
            foreach (var number in extractedNumbers)
            {
                sequence.Add(Convert.ToInt32(number));
            }

            return sequence;
        }

        static int GetNextNumberInSequence(List<int> sequence)
        {
            if (AreAllNumsZero(sequence))
            {
                return 0;
            }
            else
            {
                int sequenceCount = sequence.Count;
                var diffs = new List<int>();

                for (var i = 0; i < sequenceCount - 1; i++)
                {
                    diffs.Add(sequence[i + 1] - sequence[i]);
                }

                // add last number in sequence to next number in the sequence of differences
                return sequence[sequenceCount - 1] + GetNextNumberInSequence(diffs);
            }
        }

        static bool AreAllNumsZero(List<int> diffs)
        {
            foreach(var diff in diffs)
            {
                if (diff != 0) return false;
            }

            return true;
        }

    }
}