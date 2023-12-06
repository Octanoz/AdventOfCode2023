// string filePath = @"..\Day4\example1.txt";
string filePath = @"..\Day4\input.txt";
ReadOnlySpan<string> input = File.ReadAllLines(filePath);

int resultOne = PartOne(input);
Console.WriteLine($"Total points in part one: {resultOne}");

int resultTwo = PartTwo(input);
Console.WriteLine($"Total cards in part one: {resultTwo}");

int PartOne(ReadOnlySpan<string> input)
{
    int points = 0;
    foreach (var line in input)
    {
        string[] parts = line.Split([':', '|']);
        var winners = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
        var givenNumbers = parts[2].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);

        int[] winningNumbers = winners.Intersect(givenNumbers).ToArray();

        points += winningNumbers.Length == 0 ? 0 : 1 << winningNumbers.Length - 1;
    }

    return points;
}

int PartTwo(ReadOnlySpan<string> input)
{
    int[] totalCards = new int[input.Length];
    int index = 0;
    while (index < input.Length)
    {
        totalCards[index]++;
        string[] parts = input[index].Split([':', '|']);
        var winners = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
        var givenNumbers = parts[2].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);

        int cardPoints = winners.Intersect(givenNumbers).Count();
        int copies = totalCards[index];

        for (int i = 1; i <= cardPoints; i++)
        {
            totalCards[index + i] += copies;
        }

        index++;
    }

    return totalCards.Sum();
}



