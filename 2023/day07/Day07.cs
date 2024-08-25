namespace AdventOfCode2023;

public static class Day07
{
    enum HandType { HighCard, OnePair, TwoPair, ThreeOfAKind, FullHouse, FourOfAKind, FiveOfAKind }

    private record Hand(string Cards, int Bid, HandType HandType) { }

    private static HandType GetHandType(string hand, bool handleJokers)
    {
        Dictionary<char, int> cards = new()
        {
            { '2', 0 },
            { '3', 0 },
            { '4', 0 },
            { '5', 0 },
            { '6', 0 },
            { '7', 0 },
            { '8', 0 },
            { '9', 0 },
            { 'T', 0 },
            { 'J', 0 },
            { 'Q', 0 },
            { 'K', 0 },
            { 'A', 0 }
        };

        foreach (char c in hand)
        {
            cards[c]++;
        }

        int[] cardCombinations = new int[5];
        foreach (var card in cards)
        {
            if (card.Value != 0)
            {
                // -1 to avoid ArrayIndexOutOfBoundsException
                cardCombinations[card.Value - 1]++;
            }
        }

        HandType handType;

        // Five cards (4+1) of 1 type (eg: AAAAA)
        if (cardCombinations[4] == 1)
        {
            handType = HandType.FiveOfAKind;
        }
        // 1 (0+1) card of one type and 4 [3+1] cards of another type (eg: AA8AA)
        else if (cardCombinations[0] == 1 && cardCombinations[3] == 1)
        {
            handType = HandType.FourOfAKind;
        }
        // 2 (1+1) cards (pair) of one type and 3 (2+1) cards of another type (eg: 23332)
        else if (cardCombinations[1] == 1 && cardCombinations[2] == 1)
        {
            handType = HandType.FullHouse;
        }
        // 1 (0+1) card for two different types and 3 (2+1) cards of another type (eg: TTT98)
        else if (cardCombinations[0] == 2 && cardCombinations[2] == 1)
        {
            handType = HandType.ThreeOfAKind;
        }
        // 1 (0+1) card of one type and 2 (1+1) card pairs of 2 other types (eg: 23432)
        else if (cardCombinations[0] == 1 && cardCombinations[1] == 2)
        {
            handType = HandType.TwoPair;
        }
        // 1 (0+1) card for 3 types and 2 (1+1) cards of another type (eg: A23A4)
        else if (cardCombinations[0] == 3 && cardCombinations[1] == 1)
        {
            handType = HandType.OnePair;
        }
        else // five cards of one type each (eg: 23456)
        {
            handType = HandType.HighCard;
        }

        // Recalculated the hand type if jokers are used
        if (handleJokers && cards['J'] > 0)
        {
            switch (handType)
            {
                case HandType.HighCard: handType = HandType.OnePair; break;
                case HandType.OnePair: handType = HandType.ThreeOfAKind; break;
                case HandType.TwoPair:
                    if (cards['J'] == 1)
                    {
                        handType = HandType.FullHouse;
                    }
                    else if (cards['J'] == 2)
                    {
                        handType = HandType.FourOfAKind;
                    }
                    break;
                case HandType.ThreeOfAKind: handType = HandType.FourOfAKind; break;
                case HandType.FullHouse: handType = HandType.FiveOfAKind; break;
                case HandType.FourOfAKind: handType = HandType.FiveOfAKind; break;
                default: break;
            }
        }

        return handType;
    }

    private class HandComparer(bool HandleJokers) : IComparer<Hand>
    {
        public int Compare(Hand x, Hand y)
        {
            if (x.HandType != y.HandType)
            {
                return x.HandType - y.HandType;
            }
            for (int i = 0; i < x.Cards.Length; i++)
            {
                int valX = _cardToStrengthConverter(x, i);
                int valY = _cardToStrengthConverter(y, i);
                if (valX != valY)
                {
                    return valX - valY;
                }
            }
            return 0;

            int _cardToStrengthConverter(Hand hand, int i)
            {
                if (char.IsDigit(hand.Cards[i]))
                {
                    return hand.Cards[i] - '0'; // atoi()
                }
                else
                {
                    return hand.Cards[i] switch
                    {
                        'A' => 14,
                        'K' => 13,
                        'Q' => 12,
                        'J' => HandleJokers ? 1 : 11, // In part2, it's a joker and it's the weakest
                        _ => 10
                    };
                }
            }
        }
    }

    private static int SumRankOfEveryHand(string input, bool handleJokers)
    {
        int sum = 0;
        SortedList<Hand, int> hands = new(new HandComparer(handleJokers));
        StringReader sr = new(input);
        string line = sr.ReadLine();
        while (line != null)
        {
            if (line != string.Empty)
            {
                string[] lineData = line.Split(' ');
                string cards = lineData[0];
                int bid = int.Parse(lineData[1]);
                HandType handType = GetHandType(cards, handleJokers);
                hands.Add(new(cards, bid, handType), bid);
            }
            line = sr.ReadLine();
        }

        int rank = 1;
        foreach (var hand in hands)
        {
            sum += rank++ * hand.Value;
        }

        return sum;
    }

    public static void Execute()
    {
        string input = File.ReadAllText("./day07/input.txt");

        Console.WriteLine($"[AoC 2023 - Day 07 - Part 1] Result: {SumRankOfEveryHand(input, false)}");
        Console.WriteLine($"[AoC 2023 - Day 07 - Part 2] Result: {SumRankOfEveryHand(input, true)}");
    }
}