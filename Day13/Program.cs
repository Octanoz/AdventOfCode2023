
// string filePath = @"..\Day13\example1.txt";
string filePath = @"..\Day13\input.txt";

int resultOne = PartOne(filePath);
Console.WriteLine($"Adding the total lines left of the vertical reflections and 100x the total lines above the horizontal reflections gave: {resultOne}");

int resultTwo = PartTwo(filePath);
Console.WriteLine($"After fixing the smudge calculating the total reflections gave: {resultTwo}");

int PartOne(string filePath)
{
    int totalReflections = 0;
    using StreamReader sr = new(filePath);
    string currentLine = "";
    while (!sr.EndOfStream)
    {
        currentLine = sr.ReadLine()!;
        int lineLength = currentLine.Length;
        List<string> horizontalGrid = [];

        List<char>[] verticalGrid = new List<char>[lineLength];
        for (int i = 0; i < lineLength; i++)
        {
            verticalGrid[i] = [];
        }

        while (!String.IsNullOrEmpty(currentLine))
        {
            for (int i = 0; i < lineLength; i++)
            {
                verticalGrid[i].Add(currentLine[i]);
            }
            horizontalGrid.Add(currentLine);

            currentLine = sr.ReadLine()!;
        }

        totalReflections += FindVerticalReflection(verticalGrid);
        totalReflections += FindHorizontalReflection(horizontalGrid) * 100;
    }

    return totalReflections;
}

int PartTwo(string filePath)
{
    int totalReflections = 0;
    using StreamReader sr = new(filePath);
    string currentLine = "";
    while (!sr.EndOfStream)
    {
        currentLine = sr.ReadLine()!;
        int lineLength = currentLine.Length;
        List<List<char>> horizontalGrid = [];

        List<char>[] verticalGrid = new List<char>[lineLength];
        for (int i = 0; i < lineLength; i++)
        {
            verticalGrid[i] = [];
        }

        while (!String.IsNullOrEmpty(currentLine))
        {
            for (int i = 0; i < lineLength; i++)
            {
                verticalGrid[i].Add(currentLine[i]);
            }
            horizontalGrid.Add([.. currentLine]);

            currentLine = sr.ReadLine()!;
        }

        int originalVertical = FindVerticalReflection(verticalGrid);
        int originalHorizontal = FindHorizontalReflectionChar(horizontalGrid);

        int postSmudgeVertical = 0;
        int postSmudgeHorizontal = 0;
        int rows = horizontalGrid.Count;
        for (int i = 0; i < rows; i++)
        {
            bool newResultFound = false;

            for (int j = 0; j < lineLength; j++)
            {
                if (postSmudgeVertical == 0)
                {
                    verticalGrid[j][i] = verticalGrid[j][i] == '.' ? '#' : '.';
                    postSmudgeVertical = FindVerticalReflection(verticalGrid, originalVertical);
                    verticalGrid[j][i] = verticalGrid[j][i] == '.' ? '#' : '.';
                }

                if (postSmudgeHorizontal == 0)
                {
                    horizontalGrid[i][j] = horizontalGrid[i][j] == '.' ? '#' : '.';
                    postSmudgeHorizontal = FindHorizontalReflectionChar(horizontalGrid, originalHorizontal);
                    horizontalGrid[i][j] = horizontalGrid[i][j] == '.' ? '#' : '.';
                }

                if (postSmudgeVertical > 0 || postSmudgeHorizontal > 0)
                {
                    newResultFound = true;
                    break;
                }
            }

            if (newResultFound)
            {
                totalReflections += postSmudgeVertical + (100 * postSmudgeHorizontal);
                break;
            }
        }
    }

    return totalReflections;
}

int FindVerticalReflection(List<char>[] grid, int previousResult = 0)
{
    int left = 0;
    int right = 1;

    // Rows and columns in this grid represent columns and rows, respectively, in the original grid 
    int gridRows = grid.Length;

    while (right < gridRows)
    {
        string gridLeft = new(String.Join("", grid[left]));
        string gridRight = new(String.Join("", grid[right]));

        if (gridLeft == gridRight)
        {
            int outerLeft = left - 1;
            int outerRight = right + 1;
            bool isValidReflection = true;

            while (outerLeft >= 0 && outerRight < gridRows)
            {
                string gridOuterLeft = new(String.Join("", grid[outerLeft]));
                string gridOuterRight = new(String.Join("", grid[outerRight]));

                if (gridOuterLeft != gridOuterRight)
                {
                    isValidReflection = false;
                    break;
                }

                outerLeft--;
                outerRight++;
            }

            if (isValidReflection && right != previousResult)
                return right;
        }

        left++;
        right++;
    }

    return 0;
}

int FindHorizontalReflection(List<string> grid)
{
    int upper = 0;
    int lower = 1;
    int gridRows = grid.Count;

    while (lower < gridRows)
    {
        if (grid[upper] == grid[lower])
        {
            int outerUpper = upper - 1;
            int outerLower = lower + 1;
            bool isValidReflection = true;

            while (outerUpper >= 0 && outerLower < gridRows)
            {
                if (grid[outerUpper] != grid[outerLower])
                {
                    isValidReflection = false;
                    break;
                }

                outerUpper--;
                outerLower++;
            }

            if (isValidReflection)
                return lower;
        }

        upper++;
        lower++;
    }

    return 0;
}

int FindHorizontalReflectionChar(List<List<char>> grid, int previousResult = 0)
{
    int upper = 0;
    int lower = 1;
    int gridRows = grid.Count;

    while (lower < gridRows)
    {
        string gridUpper = new(String.Join("", grid[upper]));
        string gridLower = new(String.Join("", grid[lower]));
        if (gridUpper == gridLower)
        {
            int outerUpper = upper - 1;
            int outerLower = lower + 1;
            bool isValidReflection = true;

            while (outerUpper >= 0 && outerLower < gridRows)
            {
                string gridOuterUpper = new(String.Join("", grid[outerUpper]));
                string gridOuterLower = new(String.Join("", grid[outerLower]));
                if (gridOuterUpper != gridOuterLower)
                {
                    isValidReflection = false;
                    break;
                }

                outerUpper--;
                outerLower++;
            }

            if (isValidReflection && lower != previousResult)
                return lower;
        }

        upper++;
        lower++;
    }

    return 0;
}

