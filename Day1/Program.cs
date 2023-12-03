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
        int firstDigit = line.First(Char.IsDigit) - '0';
        int lastDigit = line.Last(Char.IsDigit) - '0';

        result += firstDigit * 10 + lastDigit;
    }

    return result;
}

int PartTwo(ReadOnlySpan<string> input)
{
    Dictionary<string, int> digits = new()
    {
        ["one"] = 1,
        ["two"] = 2,
        ["three"] = 3,
        ["four"] = 4,
        ["five"] = 5,
        ["six"] = 6,
        ["seven"] = 7,
        ["eight"] = 8,
        ["nine"] = 9
    };

    for (int i = 1; i < 10; i++)
    {
        digits.Add(i.ToString(), i);
    }

    int result = 0;
    foreach (var line in input)
    {
        int minIndex = line.Length;
        int maxIndex = -1;
        int firstDigit = 0;
        int lastDigit = 0;

        foreach (var kvp in digits)
        {
            int firstIndex = line.IndexOf(kvp.Key);
            int lastIndex = line.LastIndexOf(kvp.Key);

            if (firstIndex >= 0 && firstIndex < minIndex)
            {
                minIndex = firstIndex;
                firstDigit = kvp.Value;
            }

            if (lastIndex > maxIndex)
            {
                maxIndex = lastIndex;
                lastDigit = kvp.Value;
            }
        }

        result += firstDigit * 10 + lastDigit;
    }

    return result;
}