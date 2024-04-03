using System.Text.RegularExpressions;

// string filePath = @"..\Day3\example1.txt";
string filePath = @"..\Day3\input.txt";
string[] grid = File.ReadAllLines(filePath);

int resultOne = PartOne(grid);
Console.WriteLine($"Sum of all partnumbers in part one: {resultOne}");

long resultTwo = PartTwo(grid);
Console.WriteLine($"Sum of all gear ratios in part two: {resultTwo}");

int PartOne(ReadOnlySpan<string> grid)
{
    List<int> partNumbers = [];
    string numberPattern = @"(\d+)";
    string symbolPattern = @"([^\d.])";
    for (int i = 0; i < grid.Length; i++)
    {
        Match match = Regex.Match(grid[i], numberPattern);

        while (match.Success)
        {
            int start = Math.Max(0, match.Index - 1);
            int end = match.Index == 0 ? match.Length + 1 : Math.Min(grid[i].Length - start, match.Length + 2);

            if (i > 0 && Regex.IsMatch(grid[i - 1].AsSpan(start, end), symbolPattern))
            {
                partNumbers.Add(int.Parse(match.Value));
            }
            else if (Regex.IsMatch(grid[i].AsSpan(start, end), symbolPattern))
            {
                partNumbers.Add(int.Parse(match.Value));
            }
            else if (i < grid.Length - 1 && Regex.IsMatch(grid[i + 1].AsSpan(start, end), symbolPattern))
            {
                partNumbers.Add(int.Parse(match.Value));
            }

            match = match.NextMatch();
        }
    }

    return partNumbers.Sum();
}

long PartTwo(ReadOnlySpan<string> grid)
{
    List<long> gearRatios = [];
    string numberPattern = @"(\d+)";
    string gearPattern = @"(\*)";
    for (int i = 0; i < grid.Length; i++)
    {
        Match gearMatch = Regex.Match(grid[i], gearPattern);

        while (gearMatch.Success)
        {
            int start = Math.Max(0, gearMatch.Index - 1);
            int end = Math.Min(grid[i].Length, gearMatch.Index + gearMatch.Length);
            List<int> adjacentNumbers = [];
            Match numberMatch;

            if (i > 0)
            {
                numberMatch = Regex.Match(grid[i - 1], numberPattern);

                while (numberMatch.Success)
                {
                    int firstNum = numberMatch.Index;
                    int lastNum = firstNum + numberMatch.Length - 1;

                    if (firstNum >= start && firstNum <= end)
                        adjacentNumbers.Add(int.Parse(numberMatch.Value));
                    else if (lastNum >= start && lastNum <= end)
                        adjacentNumbers.Add(int.Parse(numberMatch.Value));

                    numberMatch = numberMatch.NextMatch();
                }
            }

            numberMatch = Regex.Match(grid[i], numberPattern);

            while (numberMatch.Success)
            {
                int firstNum = numberMatch.Index;
                int lastNum = firstNum + numberMatch.Length - 1;

                if (firstNum >= start && firstNum <= end)
                    adjacentNumbers.Add(int.Parse(numberMatch.Value));
                else if (lastNum >= start && lastNum <= end)
                    adjacentNumbers.Add(int.Parse(numberMatch.Value));

                numberMatch = numberMatch.NextMatch();
            }

            if (i < grid.Length - 1)
            {
                numberMatch = Regex.Match(grid[i + 1], numberPattern);

                while (numberMatch.Success)
                {
                    int firstNum = numberMatch.Index;
                    int lastNum = firstNum + numberMatch.Length - 1;

                    if (firstNum >= start && firstNum <= end)
                        adjacentNumbers.Add(int.Parse(numberMatch.Value));
                    else if (lastNum >= start && lastNum <= end)
                        adjacentNumbers.Add(int.Parse(numberMatch.Value));

                    numberMatch = numberMatch.NextMatch();
                }
            }

            if (adjacentNumbers.Count == 2)
                gearRatios.Add(adjacentNumbers[0] * adjacentNumbers[1]);

            gearMatch = gearMatch.NextMatch();
        }
    }

    return gearRatios.Sum();
}