using System.Text.RegularExpressions;

// string filePath = @"..\Day8\example1.txt";
// string filePath = @"..\Day8\example2.txt";
// string filePath = @"..\Day8\example3.txt";
string filePath = @"..\Day8\input.txt";
ReadOnlySpan<string> input = File.ReadAllLines(filePath);

// int resultOne = PartOne(input);
// Console.WriteLine($"It took {resultOne} steps to get to 'ZZZ' in part one.");

ulong resultTwo = PartTwo(input, []);
Console.WriteLine($"It took {resultTwo} steps to travel from all 'xxA' nodes to all 'xxZ' nodes in part two.");

//? 105537351152520 was too high

int PartOne(ReadOnlySpan<string> input)
{
    List<MapNode> nodes = [];
    string pattern = @"(\w+)";
    string directions = input[0];

    int len = input.Length;
    for (int i = 2; i < len; i++)
    {
        var parts = Regex.Matches(input[i], pattern)
                                                .Cast<Match>()
                                                .Select(m => m.Value)
                                                .ToArray();

        MapNode newNode = new(parts[0], parts[1] == parts[0] ? null : parts[1], parts[2] == parts[0] ? null : parts[2]);
        nodes.Add(newNode);
    }

    MapNode currentNode = nodes.First(n => n.Name == "AAA");
    int steps = 0;
    int dirLen = directions.Length;
    while (currentNode.Name != "ZZZ")
    {
        int index = steps % dirLen;
        char direction = directions[index];

        if (direction == 'L')
            currentNode = nodes.First(n => n.Name == currentNode.Left);
        else currentNode = nodes.First(n => n.Name == currentNode.Right);

        steps++;
    }

    return steps;
}

ulong PartTwo(ReadOnlySpan<string> input, List<MapNode> travellingNodes)
{
    List<MapNode> nodes = [];
    string pattern = @"(\w+)";
    string directions = input[0];

    int len = input.Length;
    for (int i = 2; i < len; i++)
    {
        var parts = Regex.Matches(input[i], pattern)
                                                .Cast<Match>()
                                                .Select(m => m.Value)
                                                .ToArray();

        MapNode newNode = new(parts[0], parts[1] == parts[0] ? null : parts[1], parts[2] == parts[0] ? null : parts[2]);
        nodes.Add(newNode);

        if (parts[0].EndsWith('A'))
            travellingNodes.Add(newNode);
    }

    int nodesCount = travellingNodes.Count;
    ulong[] stepsTaken = new ulong[nodesCount];

    int dirLen = directions.Length;
    for (int i = 0; i < nodesCount; i++)
    {
        int steps = 0;
        while (!travellingNodes[i].Name.EndsWith('Z'))
        {
            int index = steps % dirLen;
            char direction = directions[index];
            if (direction == 'L')
                travellingNodes[i] = nodes.First(n => n.Name == travellingNodes[i].Left);
            else travellingNodes[i] = nodes.First(n => n.Name == travellingNodes[i].Right);

            steps++;
        }

        stepsTaken[i] = (ulong)steps;
    }

    ulong lcm = stepsTaken[0];
    for (int i = 1; i < stepsTaken.Length; i++)
    {
        lcm = LowestCommonMultiple(lcm, stepsTaken[i]);
    }

    return lcm;
}

ulong LowestCommonMultiple(ulong a, ulong b) => a * b / GreatestCommonDivisor(a, b);

ulong GreatestCommonDivisor(ulong a, ulong b)
{
    while (b != 0)
    {
        ulong temp = b;
        b = a % b;
        a = temp;
    }

    return a;
}

record MapNode(string Name, string? Left, string? Right);