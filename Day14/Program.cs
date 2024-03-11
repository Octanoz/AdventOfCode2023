using System.Text;

// string filePath = @"..\Day14\example1.txt";
string filePath = @"..\Day14\input.txt";
ReadOnlySpan<string> input = File.ReadAllLines(filePath);

Console.WriteLine($"The total load on the northern support beams is {PartOne(input)}");
Console.WriteLine($"After 1_000_000_000 cycles the load on the north beams is {PartTwo(input)}");

int PartOne(ReadOnlySpan<string> input)
{
    int rows = input.Length;
    int cols = input[0].Length;
    char[,] grid = PopulateGrid(input, rows, cols);
    char[,] tiltedNorthGrid = TiltGridNorth(grid, rows, cols);

    return CalculateNorthLoad(tiltedNorthGrid, rows, cols);
}

int PartTwo(ReadOnlySpan<string> input)
{
    int loops = 1_000_000_000;
    int rows = input.Length;
    int cols = input[0].Length;
    char[,] grid = PopulateGrid(input, rows, cols);
    Dictionary<int, int> gridHashes = [];

    int currentLoop = 0;
    int gridHash = GridHasher(grid, rows, cols);
    while (gridHashes.TryAdd(gridHash, currentLoop))
    {
        grid = TiltAllDirections(grid, rows, cols);
        gridHash = GridHasher(grid, rows, cols);
        currentLoop++;
    }

    int loopSize = currentLoop - gridHashes[gridHash];
    while (currentLoop + loopSize < loops)
        currentLoop += loopSize;

    while (currentLoop++ < loops)
        grid = TiltAllDirections(grid, rows, cols);

    return CalculateNorthLoad(grid, rows, cols);
}

#region Alternative Part 2 method

/// <summary>
/// Converts grids to strings and stores those so will use significantly more memory.
/// With the size of the loop in this challenge the *potential* speed gain for not having to loop over the grid anymore is negligible.
/// Using char[][] instead of char[,] would've made string conversions a little more straightforward.
/// 
int PartTwoStrings(ReadOnlySpan<string> input)
{
    int rows = input.Length;
    int cols = input[0].Length;
    char[,] grid = PopulateGrid(input, rows, cols);
    Dictionary<string, int> gridTransitions = [];

    int loops = 1_000_000_000;
    string gridString = ConvertGridToString(grid, rows, cols);
    for (int i = 0; i < loops; i++)
    {
        if (!gridTransitions.TryGetValue(gridString, out int storedLoop))
        {
            gridTransitions.Add(gridString, i);
            grid = TiltAllDirections(grid, rows, cols);
            gridString = ConvertGridToString(grid, rows, cols);
        }
        else
        {
            int loopSize = i - storedLoop;
            while (i + loopSize < loops)
                i += loopSize;

            int storedValue = storedLoop + loops - i;
            if (gridTransitions.ContainsValue(storedValue))
            {
                grid = ConvertStringToGrid(gridTransitions.Keys.First(k => gridTransitions[k] == storedValue), rows, cols);
                i = loops;
            }

            while (i < loops)
            {
                grid = TiltAllDirections(grid, rows, cols);
                i++;
            }
        }
    }

    return CalculateNorthLoad(grid, rows, cols);
}

string ConvertGridToString(char[,] grid, int rows, int cols)
{
    StringBuilder sb = new();

    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            sb.Append(grid[i, j]);
        }
        sb.Append('\n');
    }

    return sb.ToString();
}

char[,] ConvertStringToGrid(string storedString, int rows, int cols)
{
    char[,] grid = new char[rows, cols];
    string[] gridRows = storedString.Trim().Split('\n');

    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            grid[i, j] = gridRows[i][j];
        }
    }

    return grid;
}

#endregion

char[,] PopulateGrid(ReadOnlySpan<string> input, int rows, int cols)
{
    char[,] resultGrid = new char[rows, cols];

    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            resultGrid[i, j] = input[i][j];
        }
    }

    return resultGrid;
}

void DrawGrid(char[,] grid, int rows, int cols)
{
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            Console.Write(grid[i, j]);
        }
        Console.WriteLine();
    }
}

int GridHasher(char[,] grid, int rows, int cols)
{
    int hash = 17;
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            hash = hash * 31 + grid[i, j].GetHashCode();
        }
    }

    return hash;
}

char[,] TiltAllDirections(char[,] grid, int rows, int cols)
{
    grid = TiltGridNorth(grid, rows, cols);
    grid = TiltGridWest(grid, rows, cols);
    grid = TiltGridSouth(grid, rows, cols);
    grid = TiltGridEast(grid, rows, cols);

    return grid;
}

char[,] TiltGridNorth(char[,] grid, int rows, int cols)
{
    var (empty, rock, cube) = ('.', 'O', '#');

    for (int i = 1; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            if (grid[i, j] == rock)
            {
                int newRow = i - 1;
                while (newRow >= 0 && grid[newRow, j] == empty)
                {
                    newRow--;
                }
                newRow++;

                if (newRow != i)
                {
                    grid[newRow, j] = rock;
                    grid[i, j] = empty;
                }
            }
        }
    }

    return grid;
}

char[,] TiltGridWest(char[,] grid, int rows, int cols)
{
    var (empty, rock, cube) = ('.', 'O', '#');

    for (int i = 0; i < rows; i++)
    {
        for (int j = 1; j < cols; j++)
        {
            if (grid[i, j] == rock)
            {
                int newCol = j - 1;
                while (newCol >= 0 && grid[i, newCol] == empty)
                {
                    newCol--;
                }
                newCol++;

                if (newCol != j)
                {
                    grid[i, newCol] = rock;
                    grid[i, j] = empty;
                }
            }
        }
    }

    return grid;
}

char[,] TiltGridSouth(char[,] grid, int rows, int cols)
{
    var (empty, rock, cube) = ('.', 'O', '#');

    for (int i = rows - 2; i >= 0; i--)
    {
        for (int j = 0; j < cols; j++)
        {
            if (grid[i, j] == rock)
            {
                int newRow = i + 1;
                while (newRow < rows && grid[newRow, j] == empty)
                {
                    newRow++;
                }
                newRow--;

                if (newRow != i)
                {
                    grid[newRow, j] = rock;
                    grid[i, j] = empty;
                }
            }
        }
    }

    return grid;
}

char[,] TiltGridEast(char[,] grid, int rows, int cols)
{
    var (empty, rock, cube) = ('.', 'O', '#');

    for (int i = 0; i < rows; i++)
    {
        for (int j = cols - 2; j >= 0; j--)
        {
            if (grid[i, j] == rock)
            {
                int newCol = j + 1;
                while (newCol < cols && grid[i, newCol] == empty)
                {
                    newCol++;
                }
                newCol--;

                if (newCol != j)
                {
                    grid[i, newCol] = rock;
                    grid[i, j] = empty;
                }
            }
        }
    }

    return grid;
}

int CalculateNorthLoad(char[,] tiltedGrid, int rows, int cols)
{
    int load = 0;
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            load += tiltedGrid[i, j] == 'O' ? rows - i : 0;
        }
    }

    return load;
}
