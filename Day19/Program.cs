using System;
using System.Drawing;
using System.Globalization;
using System.Numerics;

namespace Day19
{
    static class Program
    {
        private class Rating
        {
            public int X;
            public int M;
            public int A;
            public int S;

            public int GetSumOfRatings()
            {
                return X + M + A + S;
            }
        }

        private enum RatingType
        {
            X, M, A, S
        }

        private class Workflow
        {
            public bool IsEndpoint;
            public RatingType Rating;
            public bool IsGreater;
            public int Value;
            public string NextStep;
        }

        private static Dictionary<string, List<Workflow>> _workflows = new Dictionary<string, List<Workflow>>();
        private static List<Rating> _ratings = new List<Rating>();

        static void Main(string[] args)
        {
            var filename = "PuzzleInput.txt";
            var lines = File.ReadAllLines(filename);

            GetWorkflowsAndRatings(lines);

            var sumOfAcceptedWorkflows = 0;
            foreach (var rating in _ratings)
            {
                var accepted = IterateRatingThroughWorkflow(rating);

                if (accepted)
                {
                    var sum = rating.GetSumOfRatings();
                    Console.WriteLine($"Sum for accepted rating = {sum}");
                    sumOfAcceptedWorkflows += sum;
                }
            }

            Console.WriteLine($"Sum for all accepted rating = {sumOfAcceptedWorkflows}");
        }

        static void GetWorkflowsAndRatings(string[] lines)
        {
            var allWorkflowsFound = false;

            foreach (var line in lines)
            {
                if (!allWorkflowsFound)
                {
                    if (line == "")
                    {
                        allWorkflowsFound = true;
                    }
                    else
                    {
                        // format is px{a<2006:qkq,m>2090:A,rfg}
                        var split = line.Split('{');
                        var key = split[0];

                        var str = split[1].Remove(split[1].Length - 1); // get rid of '}'
                        var workflowsAsStr = str.Split(',');

                        var workflows = new List<Workflow>();
                        foreach (var workflow in workflowsAsStr)
                        {
                            if (workflow.Contains(':'))
                            {
                                // is a sequence
                                var seq = workflow.Split(':');
                                var condition = seq[0];
                                workflows.Add(new Workflow()
                                {
                                    IsEndpoint = false,
                                    Rating = GetRatingType(condition[0]),
                                    IsGreater = condition[1] == '>',
                                    Value = Convert.ToInt16(condition.Substring(2, seq[0].Length - 2)),
                                    NextStep = seq[1],
                                });

                            }
                            else
                            {
                                workflows.Add(new Workflow(){ IsEndpoint = true, NextStep = workflow});
                            }
                        }

                        _workflows.Add(key, workflows);
                    }
                }
                else
                {
                    // format is : {x=787,m=2655,a=1222,s=2876}
                    var bracesRemoved = line.Substring(1, line.Length - 2);
                    var values = bracesRemoved.Split(',');

                    _ratings.Add(new Rating()
                    {
                        X = Convert.ToInt16((values[0].Split('='))[1]),
                        M = Convert.ToInt16((values[1].Split('='))[1]),
                        A = Convert.ToInt16((values[2].Split('='))[1]),
                        S = Convert.ToInt16((values[3].Split('='))[1])
                    });
                }
            }
        }

        static RatingType GetRatingType(char c)
        {
            if (c == 'x') return RatingType.X;
            if (c == 'm') return RatingType.M;
            if (c == 'a') return RatingType.A;
            if (c == 's') return RatingType.S;
            throw new Exception("Rating type not found");
        }

        static bool IterateRatingThroughWorkflow(Rating rating)
        {
            bool accepted = false;
            bool endPointFound = false;
            string key = "in";

            while (!endPointFound)
            {
               var workflows = _workflows[key];

               foreach (var workflow in workflows)
               {
                   if (workflow.IsEndpoint
                       || IsWorkflowConditionMet(rating, workflow))
                   {
                       if (workflow.NextStep == "R")
                       {
                           endPointFound = true;
                           break;
                       }
                       else if (workflow.NextStep == "A")
                       {
                           accepted = true;
                           endPointFound = true;
                           break;
                       }
                       else
                       {
                           key = workflow.NextStep;
                           break;
                       }
                   }
               }
            }

            return accepted;
        }

        static bool IsWorkflowConditionMet(Rating rating, Workflow workflow)
        {
            switch (workflow.Rating)
            {
                case RatingType.X:
                    return workflow.IsGreater 
                        ? rating.X > workflow.Value 
                        : rating.X < workflow.Value;

                    break;
                case RatingType.M:
                    return workflow.IsGreater
                        ? rating.M > workflow.Value
                        : rating.M < workflow.Value;
                    break;
                case RatingType.A:
                    return workflow.IsGreater
                        ? rating.A > workflow.Value
                        : rating.A < workflow.Value;
                    break;
                case RatingType.S:
                    return workflow.IsGreater
                        ? rating.S > workflow.Value
                        : rating.S < workflow.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}