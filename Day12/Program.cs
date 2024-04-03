using System.Text;
using System.Text.RegularExpressions;

Dictionary<string, string> filePaths = new()
{
    ["example1"] = @"..\Day12\example1.txt",
    ["example2"] = @"..\Day12\example2.txt",
    ["challenge"] = @"..\Day12\input.txt"
};

string[] input = File.ReadAllLines(filePaths["challenge"]);

int resultOne = PartOne(input);
Console.WriteLine($"Result for part one: {resultOne}");

long resultTwo = partTwo(input);
Console.WriteLine($"Result for part two: {resultTwo}");

// Takes a few seconds to process on part 1 so obviously too slow for five times the pattern size with 5 extra ?'s
int PartOne(string[] input)
{
    int result = 0;
    foreach (var line in input)
    {
        string[] parts = line.Split();
        string pattern = parts[0];
        int[] groups = parts[1].Split(',').Select(int.Parse).ToArray();
        string expression = CreateExpression(groups);
        string[] permutations = GeneratePermutations(pattern);
        result += MatchesFound(pattern, expression, permutations);
    }

    return result;
}

long partTwo(string[] input)
{
    long result = 0;
    foreach (var line in input)
    {
        string[] parts = line.Split();

        string pattern = "";
        for (int i = 0; i < 5; i++)
        {
            pattern += parts[0];

            if (i is not 4)
                pattern += '?';
        }

        string quintString = "";
        for (int i = 0; i < 5; i++)
        {
            quintString += parts[1];

            if (i is not 4)
                quintString += ',';
        }

        int[] groups = quintString.Split(',').Select(int.Parse).ToArray();
        result += PossibleIterations(pattern, groups);
    }

    return result;
}

string CreateExpression(int[] groups)
{
    int groupsLen = groups.Length;

    StringBuilder sb = new(@"^\.*");
    sb.Append(@"(#){");
    sb.Append($"{groups[0]}");
    sb.Append('}');

    for (int i = 1; i < groupsLen; i++)
    {
        sb.Append(@"\.+(#){");
        sb.Append($"{groups[i]}");
        sb.Append('}');
    }

    sb.Append(@"\.*$");

    return sb.ToString();
}

string[] GeneratePermutations(string pattern)
{
    int questionMarks = pattern.Select(c => c).Count(c => c == '?');

    var permutations = Enumerable.Range(0, 1 << questionMarks)
                                                        .Select(i => new string(Enumerable.Range(0, questionMarks)
                                                            .Select(j => (i & (1 << j)) == 0 ? '.' : '#')
                                                        .ToArray()));

    return permutations.ToArray();
}

int MatchesFound(string pattern, string expression, string[] permutations)
{
    int patternLen = pattern.Length;
    int matching = 0;
    foreach (var combo in permutations)
    {
        StringBuilder sb = new(pattern);
        int index = 0;
        for (int i = 0; i < patternLen; i++)
        {
            if (sb[i] == '?')
            {
                sb[i] = combo[index++];
            }
        }

        string result = sb.ToString();

        if (Regex.IsMatch(result, expression))
            matching++;
    }

    return matching;
}


long PossibleIterations(string pattern, int[] groups)
{
    long[,] permutations = new long[pattern.Length + 1, groups.Length + 1];
    permutations[0, 0] = 1;

    for (int patternLength = 1; patternLength <= pattern.Length; patternLength++)
    {
        for (int groupCount = 0; groupCount <= groups.Length; groupCount++)
        {
            int patternIndex = patternLength - 1;
            char character = pattern[patternIndex];
            if (character == '.' || character == '?')
            {
                permutations[patternLength, groupCount] += permutations[patternLength - 1, groupCount];
            }

            if (groupCount == 0)
            {
                continue;
            }

            //The pattern can't be shorter than the group it's checked against
            int groupSize = groups[groupCount - 1];
            if (patternLength < groupSize)
            {
                continue;
            }

            bool canPlaceGroup = true;
            for (int endIndex = patternIndex; endIndex >= patternLength - groupSize; endIndex--)
            {
                // Any '.' will make it impossible for a group to exist there
                if (pattern[endIndex] == '.')
                {
                    canPlaceGroup = false;
                    break;
                }
            }

            // There needs to be a separator between groups, anything that's not #
            if (patternIndex - groupSize >= 0 && pattern[patternIndex - groupSize] is '#')
            {
                canPlaceGroup = false;
            }

            if (canPlaceGroup)
            {
                if (patternLength == groupSize)
                {
                    if (groupCount == 1)
                        permutations[patternLength, groupCount] += 1;
                }
                else
                {
                    permutations[patternLength, groupCount] += permutations[patternLength - groupSize - 1, groupCount - 1];
                }
            }
        }
    }

    return permutations[pattern.Length, groups.Length];
}
