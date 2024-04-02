#define TEST
// #define PART1
// #define PART2

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
#elif PART1
Console.WriteLine($"The elf can reach {PartOne(filePaths["challenge"])} unique locations in 64 steps.");
#elif PART2
    Console.WriteLine($"The elf can reach {PartTwo(filePaths["challenge"])} unique locations in 64 steps.");
#endif


long PartOne(string filePath)
{
    Span<string> input = File.ReadAllLines(filePath);
    char[,] grid = GridExtensions.New2DGridWithDimensions<char>(input, out int rows, out int cols);
    Coord startingPoint = Coord.Zero;

    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            if (grid[i, j] is 'S')
            {
                startingPoint = new(i, j);
                break;
            }
        }
    }

#if TEST
    const int requestedSteps = 6;
#elif PART1
    const int requestedSteps = 64;
#endif


    HashSet<Coord> evenStepsCoords = [];
    HashSet<Coord> endPoints = [];

    Queue<(Coord, int)> possibleCoords = [];
    possibleCoords.Enqueue((startingPoint, 0)); // (Coord, steps)

    while (possibleCoords.Count is not 0)
    {
        (Coord currentCoord, int stepsTaken) = possibleCoords.Dequeue();

        if (stepsTaken is requestedSteps)
        {
            endPoints.Add(currentCoord);
#if TEST
            grid[currentCoord.Row, currentCoord.Col] = 'O';
#endif
            continue;
        }

        //As we're allowed to backtrack, every even numbered step can be stored as an end point.
        if (stepsTaken % 2 is 0)
        {
            evenStepsCoords.Add(currentCoord);
            endPoints.Add(currentCoord);
#if TEST
            grid[currentCoord.Row, currentCoord.Col] = 'O';
#endif
        }
        else if (currentCoord.Neighbours.Where(c => c.Row >= 0 && c.Row < rows &&
                                                    c.Col >= 0 && c.Col < cols)
                                        .All(c => grid[c.Row, c.Col] == '#' || grid[c.Row, c.Col] == 'O'))
        {
            continue;
        }

        var validNeighbours = currentCoord.Neighbours.Where(c => c.Row >= 0 && c.Row < rows &&
                                                                    c.Col >= 0 && c.Col < cols &&
                                                                    grid[c.Row, c.Col] is not '#')
                                                        .Where(c => !evenStepsCoords.Contains(c));

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

    return endPoints.Count;
}

bool CoordWithinBounds(Coord coord, int rows, int cols) => coord.Row >= 0 && coord.Row < rows &&
                                                            coord.Col >= 0 && coord.Col < cols;