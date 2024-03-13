using Day16;

// string filePath = @"..\Day16\example1.txt";
string filePath = @"..\Day16\input.txt";

char[][] input = File.ReadAllLines(filePath).Select(s => s.ToCharArray()).ToArray();
int rows = input.Length;
int cols = input[0].Length;

Dictionary<(int row, int col), char> mirrors = [];
Dictionary<(int row, int col), char> splitters = [];

for (int i = 0; i < rows; i++)
{
    for (int j = 0; j < cols; j++)
    {
        char c = input[i][j];

        if (c is '|' or '-')
            splitters.Add((i, j), c);
        else if (c is '/' or '\\')
            mirrors.Add((i, j), c);
    }
}

Console.WriteLine($"The number of energized cells is {PartOne(input, rows, cols)}");
Console.WriteLine($"The highest possible number of energized cells is {PartTwo(input, rows, cols)}");

int PartOne(char[][] grid, int rows, int cols)
{
    HashSet<(int, int, Direction direction)> visited = [];
    Point startPoint = new(0, 0, Direction.Right);

    Queue<Point> beams = [];
    beams.Enqueue(startPoint);
    while (beams.Count is not 0)
    {
        Point current = beams.Dequeue();
        if (!PointIsValid(current, rows, cols) ||
            !visited.Add((current.Row, current.Column, current.Direction)))
        {
            continue;
        }

        char c = grid[current.Row][current.Column];
        switch (c)
        {
            case '.':
            case '|' when current.Direction is Direction.Up or Direction.Down:
            case '-' when current.Direction is Direction.Left or Direction.Right:
                Point nextPoint = Move(current);
                beams.Enqueue(nextPoint);
                break;
            case '\\' or '/':
                nextPoint = MirrorReflect(c, current);
                beams.Enqueue(nextPoint);
                break;
            case '|' or '-':
                List<Point> nextPoints = BeamSplits(c, current);
                nextPoints.ForEach(beams.Enqueue);
                break;
        }
    }

    EnergizedGrid(grid, visited);

    return grid.SelectMany(arr => arr).Count(c => c is '#');
}

int PartTwo(char[][] grid, int rows, int cols)
{
    var leftStarters = Enumerable.Range(0, rows)
                                    .Select(n => new Point(n, 0, Direction.Right));
    var rightStarters = Enumerable.Range(0, rows)
                                    .Select(n => new Point(n, cols - 1, Direction.Left));
    var topStarters = Enumerable.Range(0, cols)
                                    .Select(n => new Point(0, n, Direction.Down));
    var bottomStarters = Enumerable.Range(0, cols)
                                    .Select(n => new Point(rows - 1, n, Direction.Up));

    IEnumerable<Point> startPositions = topStarters.Concat(rightStarters.Concat(bottomStarters.Concat(leftStarters)));
    List<int> energizedTiles = [];

    foreach (var startPoint in startPositions)
    {
        ResetGrid(grid, rows, cols);
        HashSet<(int, int, Direction direction)> visited = [];
        Queue<Point> beams = [];
        beams.Enqueue(startPoint);

        while (beams.Count is not 0)
        {
            Point current = beams.Dequeue();
            if (!PointIsValid(current, rows, cols) ||
                !visited.Add((current.Row, current.Column, current.Direction)))
            {
                continue;
            }

            char c = grid[current.Row][current.Column];
            switch (c)
            {
                case '.':
                case '|' when current.Direction is Direction.Up or Direction.Down:
                case '-' when current.Direction is Direction.Left or Direction.Right:
                    Point nextPoint = Move(current);
                    beams.Enqueue(nextPoint);
                    break;
                case '\\' or '/':
                    nextPoint = MirrorReflect(c, current);
                    beams.Enqueue(nextPoint);
                    break;
                case '|' or '-':
                    List<Point> nextPoints = BeamSplits(c, current);
                    nextPoints.ForEach(beams.Enqueue);
                    break;
            }
        }

        EnergizedGrid(grid, visited);

        int energized = grid.SelectMany(arr => arr).Count(c => c is '#');
        energizedTiles.Add(energized);
    }

    return energizedTiles.Max();
}

void EnergizedGrid(char[][] grid, HashSet<(int row, int col, Direction direction)> energized)
{
    foreach ((int row, int col, _) in energized)
    {
        grid[row][col] = '#';
    }
}

void DrawGrid(char[][] grid, int rows, int cols, bool isEnergized = false)
{
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            if (isEnergized)
                Console.Write(grid[i][j] is '#' ? '#' : '.');
            else Console.Write(grid[i][j]);
        }
        Console.WriteLine();
    }
}

void ResetGrid(char[][] grid, int rows, int cols)
{
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            if (mirrors.TryGetValue((i, j), out char symbol))
            {
                grid[i][j] = symbol;
            }
            else if (splitters.TryGetValue((i, j), out symbol))
            {
                grid[i][j] = symbol;
            }
            else grid[i][j] = '.';
        }
    }
}

bool PointIsValid(Point current, int rows, int cols) => current.Row >= 0 && current.Row < rows
                                                    && current.Column >= 0 && current.Column < cols;

Point Move(Point current)
{
    return current.Direction switch
    {
        Direction.Up => new(current.Row - 1, current.Column, current.Direction),
        Direction.Right => new(current.Row, current.Column + 1, current.Direction),
        Direction.Down => new(current.Row + 1, current.Column, current.Direction),
        Direction.Left => new(current.Row, current.Column - 1, current.Direction),
        _ => throw new ArgumentException($"Invalid direction found: [{current.Direction}]. Point: [{current.Row},{current.Column}]")
    };
}

List<Point> BeamSplits(char c, Point current)
{
    return c switch
    {
        '|' => current.Direction switch
        {
            Direction.Left => [new(current.Row - 1, current.Column,  (Direction)((int)(current.Direction + 1) % 4)),
                                    new(current.Row + 1, current.Column, (Direction)((int)(current.Direction - 1 + 4) % 4))],
            Direction.Right => [new(current.Row - 1, current.Column, (Direction)((int)(current.Direction - 1 + 4) % 4)),
                                    new(current.Row + 1, current.Column, (Direction)((int)(current.Direction + 1) % 4))],
            _ => throw new ArgumentException($"Invalid direction found: [{current.Direction}] Point: [{current.Row},{current.Column}]")
        },
        '-' => current.Direction switch
        {
            Direction.Up => [new(current.Row, current.Column + 1, (Direction)((int)(current.Direction + 1) % 4)),
                                    new(current.Row, current.Column - 1, (Direction)((int)(current.Direction - 1 + 4) % 4))],
            Direction.Down => [new(current.Row, current.Column + 1, (Direction)((int)(current.Direction - 1 + 4) % 4)),
                                    new(current.Row, current.Column - 1, (Direction)((int)(current.Direction + 1) % 4))],
            _ => throw new ArgumentException($"Invalid direction found: [{current.Direction}] Point: [{current.Row},{current.Column}]")
        },
        _ => throw new ArgumentException($"Non-splitter character [{c}] used in call to BeamSplits method")
    };
}

Point MirrorReflect(char c, Point current)
{
    return c switch
    {
        '/' => current.Direction switch
        {
            Direction.Left => new(current.Row + 1, current.Column, (Direction)((int)(current.Direction - 1 + 4) % 4)),
            Direction.Right => new(current.Row - 1, current.Column, (Direction)((int)(current.Direction - 1 + 4) % 4)),
            Direction.Up => new(current.Row, current.Column + 1, (Direction)((int)(current.Direction + 1) % 4)),
            Direction.Down => new(current.Row, current.Column - 1, (Direction)(int)(current.Direction + 1 % 4)),
            _ => throw new ArgumentException($"Invalid direction found: [{current.Direction}] Point: [{current.Row},{current.Column}]")
        },
        '\\' => current.Direction switch
        {
            Direction.Left => new(current.Row - 1, current.Column, (Direction)((int)(current.Direction + 1) % 4)),
            Direction.Right => new(current.Row + 1, current.Column, (Direction)((int)(current.Direction + 1) % 4)),
            Direction.Up => new(current.Row, current.Column - 1, (Direction)((int)(current.Direction - 1 + 4) % 4)),
            Direction.Down => new(current.Row, current.Column + 1, (Direction)((int)(current.Direction - 1 + 4) % 4)),
            _ => throw new ArgumentException($"Invalid direction found: [{current.Direction}] Point: [{current.Row},{current.Column}]")
        },
        _ => throw new ArgumentException($"Non-mirror character [{c}] used in MirrorReflect method")
    };
}
