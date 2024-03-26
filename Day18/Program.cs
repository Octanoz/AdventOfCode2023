// #define TEST
// #define PART1
#define PART2

using AdventUtilities;
using CoordXL;
using CustomCoord;
using Lagoon;
using StandardDirections;

Dictionary<string, string> filePaths = new()
{
    ["example1"] = @"..\Day18\example1.txt",
    ["example2"] = @"..\Day18\example2.txt",
    ["challenge"] = @"..\Day18\input.txt"
};

Span<string> input = File.ReadAllLines(filePaths["challenge"]);

#if PART1
Console.WriteLine($"In part one, the lagoon can hold {PartOne(input)} cubic meters of lava.");
#endif

#if PART2
Console.WriteLine($"In part two, the lagoon can hold {PartTwo(input)} cubic meters of lava.");
#endif

int PartOne(Span<string> input)
{
    Coord currentPosition = new(0, 0);
    List<Coord> coordinates = [];

    foreach (var line in input)
    {
        string[] parts = line.Split();
        char direction = parts[0][0];
        int steps = int.Parse(parts[1]);
        Coord directionCoord = GetDirection(direction);

        for (int i = 0; i < steps; i++)
        {
            currentPosition += directionCoord;
            coordinates.Add(currentPosition);
        }
    }

    //Correcting for out of bounds coordinates
    var (lowestRow, lowestCol, highestRow, highestCol) = (int.MinValue, int.MinValue, int.MaxValue, int.MaxValue);
    foreach (var coord in coordinates)
    {
        lowestRow = Math.Min(lowestRow, coord.Row);
        highestRow = Math.Max(highestRow, coord.Row);
        lowestCol = Math.Min(lowestCol, coord.Col);
        highestCol = Math.Max(highestCol, coord.Col);
    }

    Coord correctiveCoord = new(Math.Max(0, -lowestRow), Math.Max(0, -lowestCol));
    for (int i = 0; i < coordinates.Count; i++)
    {
        coordinates[i] += correctiveCoord;
    }

    //Determine dimensions, initialize grid and populate it
    int rows = highestRow + correctiveCoord.Row + 1;
    int cols = highestCol + correctiveCoord.Col + 1;
    char[,] grid = new char[rows, cols];
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            grid[i, j] = coordinates.Contains(new(i, j)) ? '#' : '.';
        }
    }

#if TEST
    //Print grid to console
    Console.WriteLine();
    int midCols = cols / 2;
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < midCols; j++)
        {
            Console.Write($"{grid[i, j]}");
        }
        Console.WriteLine();
    }
    Console.WriteLine();

    for (int i = 0; i < rows; i++)
    {
        for (int j = midCols; j < cols; j++)
        {
            Console.Write($"{grid[i, j]}");
        }
        Console.WriteLine();
    }
    Console.WriteLine();
#endif

    Coord minRow = coordinates.OrderBy(c => c.Row).First(); //Find the first coordinate at the top of the grid that is not empty
    Coord fillStart = LagoonMethods.FindFillerStart(grid, minRow); //Navigate to the first empty space inside the lagoon
    LagoonMethods.FillGrid(grid, fillStart, rows, cols);

    return LagoonMethods.CalculateVolumeFilled(grid);
}

long PartTwo(Span<string> input)
{
    //In part 2, create tuple ranges instead of separate coordinates
    List<(CoordL, CoordL)> horizontalRawRanges = [];
    List<(CoordL, CoordL)> verticalRawRanges = [];
    CoordL baseCoord = new(0, 0);

    foreach (var line in input)
    {
        string[] parts = line.Split();
        long steps = Convert.ToInt32(parts[2][2..^2], 16);
        int direction = parts[2][^2] - '0';

        (CoordL, CoordL) currentRange = LagoonMethods.CalculateRange(baseCoord, (Direction)direction, steps, out CoordL newBaseCoord);
        if (direction is 0 or 2)
        {
            horizontalRawRanges.Add(currentRange);
        }
        else verticalRawRanges.Add(currentRange);

        baseCoord = new(newBaseCoord.Row, newBaseCoord.Col);
    }

    //Correcting for negative coordinates
    long lowestRow = long.MaxValue;
    long lowestCol = long.MaxValue;
    foreach ((CoordL start, CoordL end) in horizontalRawRanges)
    {
        lowestRow = Math.Min(lowestRow, start.Row);
        lowestCol = Math.Min(lowestCol, start.Col);
    }
    CoordL correctiveCoord = new(Math.Max(0, -lowestRow), Math.Max(0, -lowestCol));

    List<(CoordL, CoordL)> horizontalRanges = LagoonMethods.CorrectCoordinates(horizontalRawRanges, correctiveCoord);
    List<(CoordL, CoordL)> verticalRanges = LagoonMethods.CorrectCoordinates(verticalRawRanges, correctiveCoord);

    return LagoonMethods.CalculateVolume2(horizontalRanges, verticalRanges);
}

static Coord GetDirection(char direction)
{
    return direction switch
    {
        'U' => new(-1, 0),
        'R' => new(0, 1),
        'D' => new(1, 0),
        'L' => new(0, -1),
        _ => throw new ArgumentException($"Unknown character [{direction}] used as direction.")
    };
}
