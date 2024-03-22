using CustomCoord;

// string filePath = @"..\Day18\example1.txt";
string filePath = @"..\Day18\input.txt";

Span<string> input = File.ReadAllLines(filePath);

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

Console.WriteLine($"In part one, the lagoon can hold {PartOne(input)} cubic meters of lava.");

int PartOne(Span<string> input)
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

    int lowestRow = coordinates.Select(c => c.Row).Min();
    int lowestCol = coordinates.Select(c => c.Col).Min();
    int rows = coordinates.Select(c => c.Row).Max() - lowestRow + 1;
    int cols = coordinates.Select(c => c.Col).Max() - lowestCol + 1;

    Console.WriteLine();
    char[][] grid = new char[rows][];
    for (int i = 0; i < rows; i++)
    {
        grid[i] = new char[cols];
        for (int j = 0; j < cols; j++)
        {
            Coord correctedCoord = new(i - lowestRow, j - lowestCol);

            // if (coloredCoords.TryGetValue(correctedCoord, out ConsoleColor storedColor))
            // {
            //     Console.ForegroundColor = storedColor;
            //     Console.Write('#');
            //     Console.ResetColor();

            //     grid[i][j] = '#';
            // }
            // else
            // {
            //     Console.Write('.');
            //     grid[i][j] = '.';
            // }

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