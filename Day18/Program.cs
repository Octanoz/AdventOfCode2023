// #define TEST
#define PART1

using AdventUtilities;
using CoordXL;
using CustomCoord;
using StandardDirections;

// string filePath = @"..\Day18\example1.txt";
// string filePath = @"..\Day18\example2.txt";
string filePath = @"..\Day18\input.txt";

Span<string> input = File.ReadAllLines(filePath);

#if PART1
Console.WriteLine($"In part one, the lagoon can hold {PartOne(input)} cubic meters of lava.");
#endif

//? 38188

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

    // Coord minRow = coordinates.OrderBy(c => c.Row).First(); //Find the first coordinate at the top of the grid that is not empty
    // Coord fillStart = FindFillerStart(grid, minRow); //Navigate to the first empty space inside the lagoon
    // FillGrid(grid, fillStart, rows, cols);

    // return CalculateVolumeFilled(grid);
    // return CalculateVolume(grid);
    int result = CalculateVolumeTest(coordinates, rows, cols);
    return result;
}

int PartTwo(Span<string> input)
{
    Coord currentPosition = new(0, 0);
    List<Coord> coordinates = [];

    foreach (var line in input)
    {
        string[] parts = line.Split();
        string hex = parts[2][2..^2];
        int direction = parts[2][^1] - '0';
        Coord directionCoord = GetDirection2(direction);

        long steps = Convert.ToInt32(hex, 16);
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

ulong CalculateVolume2(List<CoordUL> coordinates, ulong rows, ulong cols)
{
    ulong volume = 0;

    // Sort coordinates by row to ensure we are scanning row by row
    coordinates.Sort((a, b) => a.Row.CompareTo(b.Row));

    bool insideLagoon = false;
    ulong rowNumber = 0;
    var (firstNeighbour, lastNeighbour) = (0, 0);
    CoordUL? prevCoord = null;

    for (int i = 0; i < coordinates.Count; i++)
    {
        volume++;
        CoordUL coord = coordinates[i];

        //Top and bottom row coordinates don't need further check 
        //since the state before and after will always be the same
        if (coord.Row is 0 || coord.Row == rows - 1)
        {
            continue;
        }

        //Reset bool and previous coordinate when scanning a new row
        if (coord.Row != rowNumber)
        {
            rowNumber = coord.Row;
            insideLagoon = false;
            prevCoord = null;
        }

        if (prevCoord is not null && insideLagoon)
        {
            volume += prevCoord.Col + coord.Col - 1;
        }

        //If the first neighbour is not set and the current coordinate has a neighbour
        if (coordinates.Contains(coord.Right) && firstNeighbour is 0)
        {
            firstNeighbour = coordinates.Contains(coord.Up) ? -1 : 1;
            while (coordinates.Contains(coord.Right))
            {
                coord = coordinates[++i];
                volume++;
            }

            lastNeighbour = coordinates.Contains(coord.Up) ? -1 : 1;
            if (firstNeighbour == lastNeighbour)
                insideLagoon = !insideLagoon;

            (firstNeighbour, lastNeighbour) = (0, 0);
            prevCoord = new(coord.Row, coord.Col);
        }

        if (!coordinates.Contains(coord.Right) && firstNeighbour is 0)
        {
            insideLagoon = !insideLagoon;
            prevCoord = new(coord.Row, coord.Col);
        }
    }

    return volume;
}

int CalculateVolumeTest(List<Coord> inputCoordinates, int rows, int cols)
{
    int volume = 0;

    // Sort coordinates by row to ensure we are scanning row by row
    // coordinates.Sort((a, b) => a.Row.CompareTo(b.Row));

    List<Coord> coordinates = inputCoordinates.OrderBy(c => c.Row).ThenBy(c => c.Col).ToList();

    bool insideLagoon = false;
    int rowNumber = 0;
    var (firstNeighbour, lastNeighbour) = (0, 0);
    Coord? prevCoord = null;

    for (int i = 0; i < coordinates.Count; i++)
    {
        volume++;
        Coord coord = coordinates[i];

        //Top and bottom row coordinates don't need further check 
        //since the state before and after will always be the same
        if (coord.Row is 0 || coord.Row == rows - 1)
        {
            continue;
        }

        //Reset bool and previous coordinate when scanning a new row
        if (coord.Row != rowNumber)
        {
            rowNumber = coord.Row;
            insideLagoon = false;
            prevCoord = null;
        }
        else if (prevCoord is not null && insideLagoon)
        {
            volume += coord.Col - prevCoord.Col;
        }

        //If the first neighbour is not set and the current coordinate has a horizontal neighbour
        if (coordinates.Contains(coord.Right) && firstNeighbour is 0)
        {
            firstNeighbour = coordinates.Contains(coord.Up) ? -1 : 1;
            while (coordinates.Contains(coord.Right))
            {
                coord = coordinates[++i];
                volume++;
            }

            lastNeighbour = coordinates.Contains(coord.Up) ? -1 : 1;
            if (firstNeighbour != lastNeighbour)
                insideLagoon = !insideLagoon;

            (firstNeighbour, lastNeighbour) = (0, 0);
            prevCoord = new(coord.Row, coord.Col + 1);
        }
        else if (!coordinates.Contains(coord.Right) && firstNeighbour is 0)
        {
            insideLagoon = !insideLagoon;
            prevCoord = new(coord.Row, coord.Col + 1);
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

static Coord GetDirection2(int direction)
{
    return (Direction)direction switch
    {
        Direction.Up => new(-1, 0),
        Direction.Right => new(0, 1),
        Direction.Down => new(1, 0),
        Direction.Left => new(0, -1),
        _ => throw new ArgumentException($"Unknown character [{direction}] used as direction.")
    };
}


