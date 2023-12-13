// string filePath = @"..\Day9\example1.txt";
string filePath = @"..\Day9\input.txt";
ReadOnlySpan<string> input = File.ReadAllLines(filePath);

int resultOne = PartOne(input);
Console.WriteLine($"Total histories in part one: {resultOne}");

int resultTwo = PartTwo(input);
Console.WriteLine($"Total histories in part two: {resultTwo}");

int PartOne(ReadOnlySpan<string> input)
{
    int total = 0;
    foreach (var line in input)
    {
        List<List<int>> histories = [];
        List<int> origin = line.Split(' ').Select(int.Parse).ToList();
        histories.Add(origin);

        for (int i = 0; i < histories.Count; i++)
        {
            List<int> currentList = histories[i];
            int len = currentList.Count;
            List<int> newList = [];

            for (int j = 0; j < len - 1; j++)
            {
                newList.Add(currentList[j + 1] - currentList[j]);
            }

            if (newList.All(i => i == 0))
                break;

            histories.Add(newList);
        }

        for (int i = histories.Count - 1; i >= 1; i--)
        {
            histories[i - 1].Add(histories[i][^1] + histories[i - 1][^1]);
        }

        total += histories[0][^1];
    }

    return total;
}

int PartTwo(ReadOnlySpan<string> input)
{
    int total = 0;
    foreach (var line in input)
    {
        List<List<int>> histories = [];
        List<int> origin = line.Split(' ').Select(int.Parse).ToList();
        histories.Add(origin);

        for (int i = 0; i < histories.Count; i++)
        {
            List<int> currentList = histories[i];
            int len = currentList.Count;
            List<int> newList = [];

            for (int j = 0; j < len - 1; j++)
            {
                newList.Add(currentList[j + 1] - currentList[j]);
            }

            if (newList.All(i => i == 0))
                break;

            histories.Add(newList);
        }

        for (int i = histories.Count - 1; i >= 1; i--)
        {
            histories[i - 1].Insert(0, histories[i - 1][0] - histories[i][0]);
        }

        total += histories[0][0];
    }

    return total;
}