#define TEST
using AdventUtilities;
using CustomCoord;

// string filePath = @"..\Day18\example1.txt";
// string filePath = @"..\Day18\example2.txt";
string filePath = @"..\Day18\input.txt";

Span<string> input = File.ReadAllLines(filePath);

Console.WriteLine($"In part one, the lagoon can hold {PartOne(input)} cubic meters of lava.");

int PartOne(Span<string> input)
{
    Coord currentPosition = new(0, 0);
    List<Coord> coordinates = [];

    foreach (var line in input)
    {
        string[] parts = line.Split();
        char direction = parts[0][0];
        int steps = int.Parse(parts[1]);
        string hex = parts[2][1..^1];
        Coord directionCoord = GetDirection(direction);

        for (int i = 0; i < steps; i++)
        {
            currentPosition += directionCoord;
            coordinates.Add(currentPosition);
        }
    }

    //Correcting for out of bounds coordinates
    int lowestRow = coordinates.Min(c => c.Row);
    int lowestCol = coordinates.Min(c => c.Col);
    Coord correctiveCoord = new(Math.Max(0, -lowestRow), Math.Max(0, -lowestCol));
    for (int i = 0; i < coordinates.Count; i++)
    {
        coordinates[i] += correctiveCoord;
    }

    //Determine dimensions, initialize grid and populate it
    int rows = coordinates.Max(c => c.Row) + 1;
    int cols = coordinates.Max(c => c.Col) + 1;
    char[,] grid = new char[rows, cols];
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            grid[i, j] = coordinates.Contains(new(i, j)) ? '#' : '.';
        }
    }

    Coord minRow = coordinates.OrderBy(c => c.Row).First(); //Find the first coordinate at the top of the grid that is not empty
    Coord fillStart = FindFillerStart(grid, minRow); //Navigate to the first empty space inside the lagoon
    FillGrid(grid, fillStart, rows, cols);

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

    return CalculateVolumeFilled(grid);
}

void FillGrid(char[,] grid, Coord fillStart, int rows, int cols)
{
    Queue<Coord> coordQueue = [];
    coordQueue.Enqueue(fillStart);
    while (coordQueue.Count is not 0)
    {
        Coord current = coordQueue.Dequeue();
        grid[current.Row, current.Col] = '#';

        var emptySpaces = current.Neighbours.Where(c => IsValidCoordinate(grid, c, rows, cols) && grid[c.Row, c.Col] is '.');
        foreach (var emptySpace in emptySpaces)
        {
            if (!coordQueue.Contains(emptySpace))
                coordQueue.Enqueue(emptySpace);
        }
    }
}

bool IsValidCoordinate(char[,] grid, Coord coord, int rows, int cols) => coord.Row >= 0 && coord.Row < rows && coord.Col >= 0 && coord.Col < cols;

Coord FindFillerStart(char[,] grid, Coord minRow)
{
    Queue<Coord> coordQueue = [];
    coordQueue.Enqueue(minRow);
    while (true)
    {
        Coord current = coordQueue.Dequeue();

        if (grid[current.Row, current.Col] is '.')
            return current;
        else
        {
            coordQueue.Enqueue(current.Right);
            coordQueue.Enqueue(current.Down);
        }
    }
}

int CalculateVolume(char[,] grid)
{
    int rows = grid.GetLength(0);
    int cols = grid.GetLength(1);

    int volume = 0;
    for (int i = 0; i < rows; i++)
    {
        bool insideLagoon = false;
        for (int j = 0; j < cols; j++)
        {
            if (grid[i, j] is '#' || insideLagoon)
            {
                volume++;
            }

            //if '#' comes after a '.' or if '#' is the first character on the line, toggle the status of insideLagoon
            if (grid[i, j] is '#' && (j == 0 || grid[i, j - 1] is '.'))
                insideLagoon = !insideLagoon;
        }
    }

    return volume;
}

int CalculateVolumeFilled(char[,] grid)
{
    int rows = grid.GetLength(0);
    int cols = grid.GetLength(1);

    int volume = 0;
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            if (grid[i, j] is '#')
                volume++;
        }
    }

    return volume;
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

Dictionary<string, ConsoleColor> consoleColors = new()
{
    ["#70c710"] = ConsoleColor.Green,
    ["#0dc571"] = ConsoleColor.Green,
    ["#5713f0"] = ConsoleColor.Blue,
    ["#d2c081"] = ConsoleColor.DarkYellow,
    ["#59c680"] = ConsoleColor.Green,
    ["#411b91"] = ConsoleColor.DarkMagenta,
    ["#8ceee2"] = ConsoleColor.Cyan,
    ["#caa173"] = ConsoleColor.DarkYellow,
    ["#1b58a2"] = ConsoleColor.Blue,
    ["#caa171"] = ConsoleColor.DarkYellow,
    ["#7807d2"] = ConsoleColor.DarkMagenta,
    ["#a77fa3"] = ConsoleColor.Magenta,
    ["#015232"] = ConsoleColor.DarkGreen,
    ["#7a21e3"] = ConsoleColor.DarkMagenta
};

int PartOneVisualize(Span<string> input)
{
    Coord currentPosition = new(0, 0);

    Dictionary<Coord, ConsoleColor> coloredCoords = [];
    List<Coord> coordinates = [];


    foreach (var line in input)
    {
        string[] parts = line.Split();
        char direction = parts[0][0];
        int steps = int.Parse(parts[1]);
        string hex = parts[2][1..^1];
        Coord directionCoord = GetDirection(direction);

        for (int i = 0; i < steps; i++)
        {
            currentPosition += directionCoord;
            coloredCoords[currentPosition] = consoleColors[hex];
            coordinates.Add(currentPosition);
        }
    }

    int lowestRow = coordinates.Min(c => c.Row);
    int lowestCol = coordinates.Min(c => c.Col);
    int rows = coordinates.Max(c => c.Row) - lowestRow + 1;
    int cols = coordinates.Max(c => c.Col) - lowestCol + 1;

    Console.WriteLine();
    char[][] grid = new char[rows][];
    for (int i = 0; i < rows; i++)
    {
        grid[i] = new char[cols];
        for (int j = 0; j < cols; j++)
        {
            Coord correctedCoord = new(i - lowestRow, j - lowestCol);

            if (coloredCoords.TryGetValue(correctedCoord, out ConsoleColor storedColor))
            {
                Console.ForegroundColor = storedColor;
                Console.Write('#');
                Console.ResetColor();

                grid[i][j] = '#';
            }
            else
            {
                Console.Write('.');
                grid[i][j] = '.';
            }

            if (coordinates.Contains(correctedCoord))
            {
                Console.Write('#');
                grid[i][j] = '#';
            }
            else
            {
                Console.Write('.');
                grid[i][j] = '.';
            }
        }
        Console.WriteLine();
    }
    Console.WriteLine();

    int result = grid.Where(arr => arr
                            .Any(c => c == '#'))
                    .Sum(arr => Array
                            .LastIndexOf(arr, '#') - Array
                            .IndexOf(arr, '#') + 1);

    // Console.WriteLine(result);
    return result;
}

