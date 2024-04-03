// #define TEST
// #define PART1
// #define PART2
// #define VISUALIZE
#define TEST2
// #define PLAYGROUND

using System.Diagnostics;
using AdventUtilities;
using CustomCoord;

Dictionary<string, string> filePaths = new()
{
    ["example1"] = @"..\Day21\example1.txt",
    ["challenge"] = @"..\Day21\input.txt"
};

#if TEST
long result1 = PartOne(filePaths["example1"]);
Debug.Assert(result1 == 16, $"Expected result for 'example1' is 16, not {result1}.");
#elif TEST2
long result1 = PartTwo(filePaths["example1"], 6);
Debug.Assert(result1 == 16, $"Expected result for 'example1' is 16, not {result1}.");
Console.WriteLine($"Test 1 completed.");
long result2 = PartTwo(filePaths["example1"], 10);
Debug.Assert(result2 == 50, $"Expected result for 'example1' is 50, not {result2}.");
Console.WriteLine($"Test 2 completed.");
long result3 = PartTwo(filePaths["example1"], 50);
Debug.Assert(result3 == 1594, $"Expected result for 'example1' is 1599, not {result3}.");
Console.WriteLine($"Test 3 completed.");
long result4 = PartTwo(filePaths["example1"], 100);
Debug.Assert(result4 == 6536, $"Expected result for 'example1' is 6536, not {result4}.");
Console.WriteLine($"Test 4 completed.");
long result5 = PartTwo(filePaths["example1"], 500);
Debug.Assert(result5 == 167004, $"Expected result for 'example1' is 167004, not {result5}.");
Console.WriteLine($"Test 5 completed.");
long result6 = PartTwo(filePaths["example1"], 1000);
Debug.Assert(result6 == 668697, $"Expected result for 'example1' is 668697, not {result6}.");
Console.WriteLine($"Test 6 completed.");
long result7 = PartTwo(filePaths["example1"], 5000);
Debug.Assert(result7 == 16733044, $"Expected result for 'example1' is 16733044, not {result7}.");
Console.WriteLine("All tests completed.");
#elif PART1
Console.WriteLine($"The elf can reach {PartOne(filePaths["challenge"])} unique locations in 64 steps.");
#elif PART2
Console.WriteLine($"The elf can reach {PartTwo(filePaths["challenge"])} unique locations in 26501365 steps.");
#endif

long PartOne(string filePath)
{
    Span<string> input = File.ReadAllLines(filePath);
    char[,] grid = GridExtensions.New2DGridWithDimensions<char>(input, out int rows, out int cols);
    Coord startingPoint = FindStartingPoint(grid, rows, cols);

#if TEST
    const int requestedSteps = 6;
#elif PART1
    const int requestedSteps = 64;
#endif

    HashSet<Coord> visitedCoords = [];
    long endPoints = 0;

#if VISUALIZE
    int currentFurthestStep = 0;
#endif

    Queue<(Coord, int)> possibleCoords = [];
    possibleCoords.Enqueue((startingPoint, 0)); // (Coord, steps)

    while (possibleCoords.Count is not 0)
    {
        (Coord currentCoord, int stepsTaken) = possibleCoords.Dequeue();
        if (!visitedCoords.Add(currentCoord))
            continue;

        //As we're allowed to backtrack and the requested number of steps is even
        //Instead of actually backtracking treat every even numbered step as an end point.
        if (stepsTaken % 2 is 0)
        {
            endPoints++;
            grid[currentCoord.Row, currentCoord.Col] = 'O';
#if TEST && PART1
            if (stepsTaken is requestedSteps)
                continue;
#endif
        }

#if VISUALIZE
        if (stepsTaken % 10 is 0 && stepsTaken > currentFurthestStep)
        {
            currentFurthestStep = stepsTaken;
            grid.Draw2DGrid();
            Console.WriteLine($"\n{stepsTaken} steps taken.\n");
        }
#endif

        var validNeighbours = currentCoord.Neighbours.Where(c => c.Row >= 0 && c.Row < rows &&
                                                                    c.Col >= 0 && c.Col < cols &&
                                                                    grid[c.Row, c.Col] is not '#')
                                                        .Where(c => !visitedCoords.Contains(c));


        foreach (var neighbour in validNeighbours)
        {
            possibleCoords.Enqueue((neighbour, stepsTaken + 1));
        }
    }

#if TEST
    Console.WriteLine();
    grid.Draw2DGridTight();
    Console.WriteLine();
#endif

    return endPoints;
}


long PartTwo(string filePath, int testingSteps)
{
    Span<string> input = File.ReadAllLines(filePath);
    char[,] grid = GridExtensions.New2DGridWithDimensions<char>(input, out int rows, out int cols);
    HashSet<Coord> rocks = DocumentRocks(grid, rows, cols);
    Coord startingPoint = FindStartingPoint(grid, rows, cols);

#if PART2
    //This can probably be brought down with modulo cols or rows and the result multiplied again until we reach the requestedSteps
    const int requestedSteps = 26501365;
#endif

    HashSet<Coord> visitedCoords = [];
    long endPoints = 0;

    Queue<(Coord, int)> possibleCoords = [];
    possibleCoords.Enqueue((startingPoint, 0)); // (Coord, steps)

    while (possibleCoords.Count is not 0)
    {
        (Coord currentCoord, int stepsTaken) = possibleCoords.Dequeue();
        if (!visitedCoords.Add(currentCoord))
            continue;

        //As we're allowed to backtrack and the requested number of steps is even
        //Instead of actually backtracking treat every even numbered step as an end point.
        if (stepsTaken % 2 is 0)
        {
            endPoints++;

#if TEST2
            if (stepsTaken == testingSteps)
                continue;
#elif PART2
            if (stepsTaken is requestedSteps)
                continue;
#endif
        }

        var validNeighbours = currentCoord.Neighbours.Where(c => !visitedCoords.Contains(c));


        foreach (var neighbour in validNeighbours)
        {
            if (!IsRock(neighbour, rocks, rows, cols))
                possibleCoords.Enqueue((neighbour, stepsTaken + 1));
        }
    }

    return endPoints;
}

bool CoordWithinBounds(Coord coord, int rows, int cols) => coord.Row >= 0 && coord.Row < rows &&
                                                            coord.Col >= 0 && coord.Col < cols;

static Coord FindStartingPoint(char[,] grid, int rows, int cols)
{
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            if (grid[i, j] is 'S')
            {
                return new(i, j);
            }
        }
    }

    return Coord.Zero;
}

static HashSet<Coord> DocumentRocks(char[,] grid, int rows, int cols)
{
    HashSet<Coord> rocks = [];

    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            if (grid[i, j] is '#')
                rocks.Add(new(i, j));
        }
    }

    return rocks;
}

static bool IsRock(Coord coord, HashSet<Coord> rocks, int rows, int cols)
{
    int row = coord.Row;
    while (row < 0)
        row += rows;

    int col = coord.Col;
    while (col < 0)
        col += cols;

    row %= rows;
    col %= cols;

    return rocks.Contains(new(row, col));
}

#if PLAYGROUND


#endif