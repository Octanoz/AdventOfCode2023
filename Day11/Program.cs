// string filePath = @"..\Day11\example1.txt";
string filePath = @"..\Day11\input.txt";
ReadOnlySpan<string> input = File.ReadAllLines(filePath);

int resultOne = PartOne(input);
Console.WriteLine($"The sum of all distances in part one is: {resultOne}.");

long resultTwo = PartTwo(input);
Console.WriteLine($"The sum of all distances in part two is: {resultTwo}.");

int PartOne(ReadOnlySpan<string> input)
{
    List<List<char>> universeLines = ExpandUniverse(input);
    Dictionary<int, (int, int)> galaxies = [];

    int maxRow = universeLines.Count;
    int maxCol = universeLines[0].Count;
    int galaxyNumber = 0;
    for (int row = 0; row < maxRow; row++)
    {
        for (int col = 0; col < maxCol; col++)
        {
            if (universeLines[row][col] == '#')
                galaxies.Add(++galaxyNumber, (row, col));
        }
    }

    List<int> distances = [];
    int skipGalaxies = 1;
    foreach (var kvp in galaxies)
    {
        foreach (var otherKvp in galaxies.Skip(skipGalaxies))
        {
            distances.Add(CalculateDistance(kvp, otherKvp));
        }

        skipGalaxies++;
    }

    return distances.Sum();
}

long PartTwo(ReadOnlySpan<string> input)
{
    int maxRow = input.Length;
    int maxCol = input[0].Length;

    int additionFactor = 999_999;
    int[] extraRows = ExpandedUniverseRows(input);
    int[] extraCols = ExpandedUniverseCols(input);

    Dictionary<int, (int, int)> galaxies = [];
    int galaxyNumber = 0;
    for (int row = 0; row < maxRow; row++)
    {
        for (int col = 0; col < maxCol; col++)
        {
            if (input[row][col] == '#')
            {
                int totalRows = (extraRows.Where(r => r < row).Count() * additionFactor) + row;
                int totalCols = (extraCols.Where(r => r < col).Count() * additionFactor) + col;

                galaxies.Add(++galaxyNumber, (totalRows, totalCols));
            }
        }
    }

    List<long> distances = [];
    int skipGalaxies = 1;
    foreach (var kvp in galaxies)
    {
        foreach (var otherKvp in galaxies.Skip(skipGalaxies))
        {
            distances.Add(CalculateDistance(kvp, otherKvp));
        }

        skipGalaxies++;
    }

    return distances.Sum();
}

int CalculateDistance(KeyValuePair<int, (int, int)> galaxy, KeyValuePair<int, (int, int)> otherGalaxy)
{
    return Math.Abs(otherGalaxy.Value.Item1 - galaxy.Value.Item1) + Math.Abs(otherGalaxy.Value.Item2 - galaxy.Value.Item2);
}

List<List<char>> ExpandUniverse(ReadOnlySpan<string> input)
{
    int maxRow = input.Length;
    List<List<char>> universeLines = [];

    for (int row = 0; row < maxRow; row++)
    {
        if (input[row].All(c => c == '.'))
            universeLines.Add([.. input[row]]);

        universeLines.Add([.. input[row]]);
    }

    for (int col = 0; col < universeLines[0].Count; col++)
    {
        int foundGalaxies = 0;
        for (int row = 0; row < universeLines.Count; row++)
        {
            if (universeLines[row][col] != '.')
                foundGalaxies++;
        }

        if (foundGalaxies == 0)
        {
            foreach (var list in universeLines)
                list.Insert(col, '.');

            col++;
        }
    }

    return universeLines;
}

int[] ExpandedUniverseRows(ReadOnlySpan<string> input)
{
    int maxRow = input.Length;
    int maxCol = input[0].Length;
    List<int> emptyRows = [];

    for (int row = 0; row < maxRow; row++)
    {
        if (input[row].All(c => c == '.'))
            emptyRows.Add(row);
    }

    return [.. emptyRows];
}

int[] ExpandedUniverseCols(ReadOnlySpan<string> input)
{
    int maxRow = input.Length;
    int maxCol = input[0].Length;
    List<int> emptyCols = [];

    for (int col = 0; col < maxCol; col++)
    {
        int foundGalaxies = 0;
        for (int row = 0; row < maxRow; row++)
        {
            if (input[row][col] == '#')
                foundGalaxies++;
        }

        if (foundGalaxies == 0)
        {
            emptyCols.Add(col);
        }
    }

    return [.. emptyCols];
}

