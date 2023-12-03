using System.Text.RegularExpressions;

// string filePath = @"..\Day2\example1.txt";
string filePath = @"..\Day2\input.txt";
ReadOnlySpan<string> input = File.ReadAllLines(filePath);

int resultOne = PartOne(input);
Console.WriteLine($"Sum of all possible games for part one: {resultOne}");

int resultTwo = PartTwo(input);
Console.WriteLine($"Sum of all powers in part two: {resultTwo}");

int PartOne(ReadOnlySpan<string> input)
{
    List<int> possibleGames = [];
    string pattern = @"(?<blue>\d+ blue)|(?<red>\d+ red)|(?<green>\d+ green)";
    Dictionary<string, int> allowedAmounts = new()
    {
        ["red"] = 12,
        ["green"] = 13,
        ["blue"] = 14
    };

    int gameNumber = 0;
    foreach (var line in input)
    {
        Dictionary<string, int> gameData = new()
        {
            ["Game"] = ++gameNumber
        };

        Match match = Regex.Match(line, pattern);

        while (match.Success)
        {
            if (match.Groups["red"].Success)
            {
                string[] parts = match.Groups[groupname: "red"].Value.Split(' ');
                int amount = int.Parse(parts[0]);

                if (!gameData.TryAdd("red", amount))
                {
                    if (amount > gameData["red"])
                        gameData["red"] = amount;
                }
            }
            else if (match.Groups["green"].Success)
            {
                string[] parts = match.Groups[groupname: "green"].Value.Split(' ');
                int amount = int.Parse(parts[0]);

                if (!gameData.TryAdd("green", amount))
                {
                    if (amount > gameData["green"])
                        gameData["green"] = amount;
                }
            }
            else if (match.Groups["blue"].Success)
            {
                string[] parts = match.Groups["blue"].Value.Split(' ');
                int amount = int.Parse(parts[0]);

                if (!gameData.TryAdd("blue", amount))
                {
                    if (amount > gameData["blue"])
                        gameData["blue"] = amount;
                }
            }

            match = match.NextMatch();
        }

        if (gameData["red"] <= allowedAmounts["red"] && gameData["green"] <= allowedAmounts["green"] && gameData["blue"] <= allowedAmounts["blue"])
        {
            possibleGames.Add(gameData["Game"]);
        }
    }

    return possibleGames.Sum();
}

int PartTwo(ReadOnlySpan<string> input)
{
    List<int> powers = [];
    string pattern = @"(?<blue>\d+ blue)|(?<red>\d+ red)|(?<green>\d+ green)";

    int gameNumber = 0;
    foreach (var line in input)
    {
        Dictionary<string, int> gameData = new()
        {
            ["Game"] = ++gameNumber
        };

        Match match = Regex.Match(line, pattern);

        while (match.Success)
        {
            if (match.Groups["red"].Success)
            {
                string[] parts = match.Groups[groupname: "red"].Value.Split(' ');
                int amount = int.Parse(parts[0]);

                if (!gameData.TryAdd("red", amount))
                {
                    if (amount > gameData["red"])
                        gameData["red"] = amount;
                }
            }
            else if (match.Groups["green"].Success)
            {
                string[] parts = match.Groups[groupname: "green"].Value.Split(' ');
                int amount = int.Parse(parts[0]);

                if (!gameData.TryAdd("green", amount))
                {
                    if (amount > gameData["green"])
                        gameData["green"] = amount;
                }
            }
            else if (match.Groups["blue"].Success)
            {
                string[] parts = match.Groups["blue"].Value.Split(' ');
                int amount = int.Parse(parts[0]);

                if (!gameData.TryAdd("blue", amount))
                {
                    if (amount > gameData["blue"])
                        gameData["blue"] = amount;
                }
            }

            match = match.NextMatch();
        }

        powers.Add(gameData["red"] * gameData["green"] * gameData["blue"]);
    }

    return powers.Sum();
}

