// string filePath = @"..\Day15\example1.txt";
string filePath = @"..\Day15\input.txt";
string[] input = File.ReadAllText(filePath)
                        .Split(new char[] { ',', '\n' }, StringSplitOptions.RemoveEmptyEntries);

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
        string[] parts = input[i].Split(new char[] { '=', '-' }, StringSplitOptions.RemoveEmptyEntries);
        string labelString = parts[0];
        char operation = input[i].Contains('-') ? '-' : '=';

        int strength = 0;
        int boxNumber = labelString.Select(c => c).Aggregate(0, (acc, ascii) => (acc + ascii) * 17 % 256);
        int index = boxes[boxNumber].IndexOf(boxes[boxNumber]
                                        .Find(item => item.label == labelString));

        if (operation is '-' && index is not -1)
        {
            boxes[boxNumber].RemoveAt(index);
        }
        else if (operation is '=')
        {
            strength = input[i][^1] - '0';

            if (index is not -1)
            {
                boxes[boxNumber][index] = (labelString, strength);
            }
            else boxes[boxNumber].Add((labelString, strength));
        }
    }

    return boxes.SelectMany((box, i) => box
                    .Select((lens, j) => (Box: i + 1, Slot: j + 1, Strength: lens.strength)))
                .Sum(lens => lens.Box * lens.Slot * lens.Strength);
}
