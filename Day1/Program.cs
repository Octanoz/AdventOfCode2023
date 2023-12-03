// string filePath = @"..\Day1\example1.txt";
// string filePath = @"..\Day1\example2.txt";
string filePath = @"..\Day1\input.txt";
ReadOnlySpan<string> input = File.ReadAllLines(filePath);

int sumOne = PartOne(input);
Console.WriteLine($"Total sum part one: {sumOne}");

int sumTwo = PartTwo(input);
Console.WriteLine($"Total sum part two: {sumTwo}");

int PartOne(ReadOnlySpan<string> input)
{
    int result = 0;
    foreach (var line in input)
    {
        int lineResult = 0;

        for (int i = 0; i < line.Length; i++)
        {
            if (Char.IsDigit(line[i]))
            {
                lineResult += line[i] - '0';
                break;
            }
        }

        lineResult *= 10;

        for (int i = line.Length - 1; i >= 0; i--)
        {
            if (Char.IsDigit(line[i]))
            {
                lineResult += line[i] - '0';
                break;
            }
        }

        result += lineResult;
    }

    return result;
}

int PartTwo(ReadOnlySpan<string> input)
{
    string[] numbers = ["zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];
    int result = 0;
    foreach (var line in input)
    {
        int lineResult = 0;
        bool hasWrittenNumbers = false;
        int firstString = -1;
        int lastString = -1;

        if (numbers.Any(line.Contains))
        {
            hasWrittenNumbers = true;
            firstString = numbers.Select(n => new { Number = Array.IndexOf(numbers, n), Index = line.IndexOf(n) })
                                    .Where(n => n.Index >= 0)
                                    .OrderBy(n => n.Index)
                                    .Select(n => n.Number)
                                    .First();

            lastString = numbers.Select(n => new { Number = Array.IndexOf(numbers, n), Index = line.LastIndexOf(n) })
                                .Where(n => n.Index >= 0)
                                .OrderBy(n => n.Index)
                                .Select(n => n.Number)
                                .Last();
        }

        for (int i = 0; i < line.Length; i++)
        {
            if (Char.IsDigit(line[i]))
            {
                if (hasWrittenNumbers && line[0..i].Contains(numbers[firstString]))
                {
                    lineResult += firstString;
                }
                else lineResult += line[i] - '0';

                break;
            }
        }

        if (lineResult == 0)
        {
            lineResult += firstString;
            lineResult *= 10;
            lineResult += lastString;
        }
        else
        {
            lineResult *= 10;

            for (int i = line.Length - 1; i >= 0; i--)
            {
                if (Char.IsDigit(line[i]))
                {
                    if (hasWrittenNumbers && line[i..].Contains(numbers[lastString]))
                    {
                        lineResult += lastString;
                    }
                    else lineResult += line[i] - '0';

                    break;
                }
            }
        }

        result += lineResult;
    }

    return result;
}