﻿
Dictionary<string, string> filePaths = new()
{
    ["example1"] = @"..\Day15\example1.txt",
    ["challenge"] = @"..\Day15\input.txt"
};

string[] input = File.ReadAllText(filePaths["challenge"])
                        .Split(new[] { ',', '\n' }, StringSplitOptions.RemoveEmptyEntries);

Console.WriteLine($"The sum of the results in part one is {PartOne(input)}");
Console.WriteLine($"The sum of the results in part two is {PartTwo(input)}");

int PartOne(string[] input) => input.Select(s => s
                                        .Select(c => c)
                                        .Aggregate(0, (acc, ascii) => (acc + ascii) * 17 % 256))
                                    .Sum();

int PartTwo(string[] input)
{
    List<(string label, int strength)>[] boxes = new List<(string label, int strength)>[256];
    for (int i = 0; i < 256; i++)
    {
        boxes[i] = [];
    }

    for (int i = 0; i < input.Length; i++)
    {
        string[] parts = input[i].Split(new[] { '=', '-' }, StringSplitOptions.RemoveEmptyEntries);
        string labelString = parts[0];
        int boxNumber = labelString.Select(c => c).Aggregate(0, (acc, ascii) => (acc + ascii) * 17 % 256);

        int index = boxes[boxNumber].IndexOf(boxes[boxNumber].Find(item => item.label == labelString));
        char operation = input[i][^2] is '=' ? '=' : '-';

        if (operation is '=')
        {
            int strength = input[i][^1] - '0';

            if (index is not -1)
            {
                boxes[boxNumber][index] = (labelString, strength);
            }
            else boxes[boxNumber].Add((labelString, strength));
        }
        else if (index is not -1)
        {
            boxes[boxNumber].RemoveAt(index);
        }
    }

    return boxes.SelectMany((box, i) => box
                    .Select((lens, j) => (Box: i + 1, Slot: j + 1, Strength: lens.strength)))
                .Sum(lens => lens.Box * lens.Slot * lens.Strength);
}
