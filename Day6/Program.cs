
Dictionary<string, string> filePaths = new()
{
    ["example1"] = @"..\Day6\example1.txt",
    ["challenge"] = @"..\Day6\input.txt"
};

string[] input = File.ReadAllLines(filePaths["challenge"]);

int resultOne = PartOne(input);
Console.WriteLine($"The product of all win scenarios in part one is: {resultOne}");

long resultTwo = PartTwo(input);
Console.WriteLine($"There are {resultTwo} win scenarios in part two.");

int PartOne(ReadOnlySpan<string> input)
{
    int[] timeLimits = input[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(int.Parse).ToArray();
    int[] distanceRecords = input[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(int.Parse).ToArray();
    int[] winScenarios = new int[timeLimits.Length];

    for (int i = 0; i < timeLimits.Length; i++)
    {
        int timeLimit = timeLimits[i];
        int currentRecord = distanceRecords[i];
        int distanceTravelled = 0;

        for (int j = 1; j < timeLimit; j++)
        {
            distanceTravelled = j * (timeLimit - j);

            if (distanceTravelled > currentRecord)
            {
                winScenarios[i]++;
            }
        }
    }

    int productOfWins = winScenarios.Aggregate((acc, val) => acc * val);

    return productOfWins;
}

long PartTwo(string[] input)
{
    for (int i = 0; i < input.Length; i++)
    {
        input[i] = input[i].Replace(" ", "");
    }

    long timeLimit = input[0].Split(':').Skip(1).Select(long.Parse).First();
    long currentRecord = input[1].Split(':').Skip(1).Select(long.Parse).First();
    long winScenarios = 0;

    for (int i = 1; i < timeLimit; i++)
    {
        long distanceTravelled = i * (timeLimit - i);

        if (distanceTravelled > currentRecord)
        {
            winScenarios++;
        }
    }

    return winScenarios;
}
