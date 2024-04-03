
Dictionary<string, string> filePaths = new()
{
    ["example1"] = @"..\Day7\example1.txt",
    ["challenge"] = @"..\Day7\input.txt"
};

ReadOnlySpan<string> input = File.ReadAllLines(filePaths["challenge"]);

Dictionary<char, int> cardStrengths = new()
{
    ['2'] = 2,
    ['3'] = 3,
    ['4'] = 4,
    ['5'] = 5,
    ['6'] = 6,
    ['7'] = 7,
    ['8'] = 8,
    ['9'] = 9,
    ['T'] = 10,
    ['J'] = 11,
    ['Q'] = 12,
    ['K'] = 13,
    ['A'] = 14
};

Dictionary<char, int> cardStrengthsWithJoker = new()
{
    ['J'] = 1,
    ['2'] = 2,
    ['3'] = 3,
    ['4'] = 4,
    ['5'] = 5,
    ['6'] = 6,
    ['7'] = 7,
    ['8'] = 8,
    ['9'] = 9,
    ['T'] = 10,
    ['Q'] = 11,
    ['K'] = 12,
    ['A'] = 13
};

int resultOne = PartOne(input);
Console.WriteLine($"Total winnings in part one: {resultOne}");

int resultTwo = PartTwo(input);
Console.WriteLine($"Total winnings in part two: {resultTwo}");

int PartOne(ReadOnlySpan<string> input)
{
    List<string> fiveOfAKind = [];
    List<string> fourOfAKind = [];
    List<string> fullHouse = [];
    List<string> threeOfAKind = [];
    List<string> twoPair = [];
    List<string> onePair = [];
    List<string> highCard = [];

    Dictionary<string, int> bets = [];

    foreach (var line in input)
    {
        var parts = line.Split(' ');
        string currentHand = parts[0];
        int bid = int.Parse(parts[1]);

        switch (DistinctCharacters(currentHand))
        {
            case 1:
                fiveOfAKind.Add(currentHand);
                break;
            case 2:
                if (IsFourOfAKind(currentHand))
                    fourOfAKind.Add(currentHand);
                else fullHouse.Add(currentHand);
                break;
            case 3:
                if (IsThreeOfAKind(currentHand))
                    threeOfAKind.Add(currentHand);
                else twoPair.Add(currentHand);
                break;
            case 4:
                onePair.Add(currentHand);
                break;
            default:
                highCard.Add(currentHand);
                break;
        }

        bets.Add(currentHand, bid);
    }

    int handComparer(string hand1, string hand2)
    {
        for (int i = 0; i < hand1.Length; i++)
        {
            int result = cardStrengths[hand1[i]] - cardStrengths[hand2[i]];

            if (result != 0)
                return result;
        }

        return 0;
    }

    fiveOfAKind.Sort();
    fourOfAKind.Sort(handComparer);
    fullHouse.Sort(handComparer);
    threeOfAKind.Sort(handComparer);
    twoPair.Sort(handComparer);
    onePair.Sort(handComparer);
    highCard.Sort(handComparer);

    string[] allCards = [.. highCard, .. onePair.Concat(twoPair.Concat(threeOfAKind.Concat(fullHouse.Concat(fourOfAKind.Concat(fiveOfAKind)))))];

    int totalWinnings = 0;
    int len = allCards.Length;
    for (int i = 0; i < len; i++)
    {
        int rank = i + 1;
        totalWinnings += bets[allCards[i]] * rank;
    }

    return totalWinnings;
}

int PartTwo(ReadOnlySpan<string> input)
{
    List<string> fiveOfAKind = [];
    List<string> fourOfAKind = [];
    List<string> fullHouse = [];
    List<string> threeOfAKind = [];
    List<string> twoPair = [];
    List<string> onePair = [];
    List<string> highCard = [];

    Dictionary<string, int> bets = [];

    foreach (var line in input)
    {
        var parts = line.Split(' ');
        string currentHand = parts[0];
        int bid = int.Parse(parts[1]);

        switch (sameCards(currentHand))
        {
            case 1:
                highCard.Add(currentHand);
                break;
            case 2:
                if (IsOnePairWithJoker(currentHand))
                    onePair.Add(currentHand);
                else twoPair.Add(currentHand);
                break;
            case 3:
                if (IsFullHouseWithJoker(currentHand))
                    fullHouse.Add(currentHand);
                else threeOfAKind.Add(currentHand);
                break;
            case 4:
                fourOfAKind.Add(currentHand);
                break;
            case 5:
                fiveOfAKind.Add(currentHand);
                break;
            default:
                Console.WriteLine($"Invalid output for [{currentHand}] with sameCards method: {sameCards(currentHand)}");
                break;
        }

        bets.Add(currentHand, bid);
    }

    int handComparer(string hand1, string hand2)
    {
        for (int i = 0; i < hand1.Length; i++)
        {
            int result = cardStrengthsWithJoker[hand1[i]] - cardStrengthsWithJoker[hand2[i]];

            if (result != 0)
                return result;
        }

        return 0;
    }

    fiveOfAKind.Sort(handComparer);
    fourOfAKind.Sort(handComparer);
    fullHouse.Sort(handComparer);
    threeOfAKind.Sort(handComparer);
    twoPair.Sort(handComparer);
    onePair.Sort(handComparer);
    highCard.Sort(handComparer);

    string[] allCards = [.. highCard, .. onePair.Concat(twoPair.Concat(threeOfAKind.Concat(fullHouse.Concat(fourOfAKind.Concat(fiveOfAKind)))))];

    int totalWinnings = 0;
    int len = allCards.Length;
    for (int i = 0; i < len; i++)
    {
        int rank = i + 1;
        totalWinnings += bets[allCards[i]] * rank;
    }

    return totalWinnings;
}

int DistinctCharacters(string s) => s.Distinct().Count();

bool IsFourOfAKind(string s)
{
    for (int i = 0; i < s.Length - 3; i++)
    {
        int sameLabel = 1;
        for (int j = i + 1; j < s.Length; j++)
        {
            if (s[j] == s[i])
                sameLabel++;
        }

        if (sameLabel is 4)
            return true;
    }

    return false;
}

bool IsThreeOfAKind(string s)
{
    for (int i = 0; i < s.Length - 2; i++)
    {
        int sameLabel = 1;
        for (int j = i + 1; j < s.Length; j++)
        {
            if (s[j] == s[i])
                sameLabel++;
        }

        if (sameLabel is 3)
            return true;
    }

    return false;
}

int sameCards(string s)
{
    int jokers = s.Count(c => c == 'J');
    int maxSame = 0;
    for (int i = 0; i < s.Length - 1; i++)
    {
        if (s[i] is 'J')
            continue;

        int sameLabel = 1;
        for (int j = i + 1; j < s.Length; j++)
        {
            if (s[j] == s[i])
                sameLabel++;
        }

        maxSame = Math.Max(maxSame, sameLabel);
    }

    return maxSame + jokers;
}

bool IsFullHouseWithJoker(string s)
{
    return s.Distinct().Count() is 2 || s.Distinct().Count() is 3 && s.Contains('J');
}

bool IsOnePairWithJoker(string s)
{
    return s.Distinct().Count() is 4 || s.Distinct().Count() is 5 && s.Contains('J');
}



