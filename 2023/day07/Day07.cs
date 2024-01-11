namespace AdventOfCode2023;

public static class Day07
{
    enum HandType { HighCard, OnePair, TwoPair, ThreeOfAKind, FullHouse, FourOfAKind, FiveOfAKind }
    private record Hand(string Cards, int Bid, HandType HandType) { }
    private static HandType GetHandType(string hand)
    {
        Dictionary<char, int> cards = [];
        cards.Add('2', 0);
        cards.Add('3', 0);
        cards.Add('4', 0);
        cards.Add('5', 0);
        cards.Add('6', 0);
        cards.Add('7', 0);
        cards.Add('8', 0);
        cards.Add('9', 0);
        cards.Add('T', 0);
        cards.Add('J', 0);
        cards.Add('Q', 0);
        cards.Add('K', 0);
        cards.Add('A', 0);

        foreach (char c in hand)
        {
            cards[c]++;
        }

        int[] cardCombinations = new int[5];
        foreach (var card in cards)
        {
            if (card.Value != 0)
            {
                cardCombinations[card.Value - 1]++;
            }
        }

        // Five cards (4+1) of 1 type (eg: AAAAA)
        if (cardCombinations[4] == 1)
        {
            return HandType.FiveOfAKind;
        }
        // 1 (0+1) card of one type and 4 [3+1] cards of another type (eg: AA8AA)
        else if (cardCombinations[0] == 1 && cardCombinations[3] == 1)
        {
            return HandType.FourOfAKind;
        }
        // 2 (1+1) cards (pair) of one type and 3 (2+1) cards of another type (eg: 23332)
        else if (cardCombinations[1] == 1 && cardCombinations[2] == 1)
        {
            return HandType.FullHouse;
        }
        // 1 (0+1) card for two different types and 3 (2+1) cards of another type (eg: TTT98)
        else if (cardCombinations[0] == 2 && cardCombinations[2] == 1)
        {
            return HandType.ThreeOfAKind;
        }
        // 1 (0+1) card of one type and 2 (1+1) card pairs of 2 other types (eg: 23432)
        else if (cardCombinations[0] == 1 && cardCombinations[1] == 2)
        {
            return HandType.TwoPair;
        }
        // 1 (0+1) card for 3 types and 2 (1+1) cards of another type (eg: A23A4)
        else if (cardCombinations[0] == 3 && cardCombinations[1] == 1)
        {
            return HandType.OnePair;
        }
        else // five cards of one type each (eg: 23456)
        {
            return HandType.HighCard;
        }
    }

    private class HandComparer : IComparer<Hand>
    {
        public int Compare(Hand x, Hand y)
        {
            if (x.HandType != y.HandType)
            {
                return x.HandType - y.HandType;
            }
            for (int i = 0; i < x.Cards.Length; i++)
            {
                int valX = CardToStrength(x, i);
                int valY = CardToStrength(y, i);
                if (valX != valY)
                {
                    return valX - valY;
                }
            }
            return 0;

            static int CardToStrength(Hand hand, int i)
            {
                if (char.IsDigit(hand.Cards[i]))
                {
                    return hand.Cards[i] - '0';
                }
                else
                {
                    return hand.Cards[i] switch
                    {
                        'A' => 14,
                        'K' => 13,
                        'Q' => 12,
                        'J' => 11,
                        _ => 10
                    };
                }
            }
        }
    }

    public static void Execute()
    {
        SortedList<Hand, int> hands = new SortedList<Hand, int>(new HandComparer());
        int sum = 0;
        StringReader sr = new(File.ReadAllText("./day07/input.txt"));
        string line = sr.ReadLine();
        while (line != null)
        {
            if (line != string.Empty)
            {
                string[] lineData = line.Split(' ');
                string cards = lineData[0];
                int bid = int.Parse(lineData[1]);
                HandType handType = GetHandType(cards);
                hands.Add(new(cards, bid, handType), bid);
            }
            line = sr.ReadLine();
        }

        int rank = 1;
        foreach (var hand in hands)
        {
            sum += rank++ * hand.Value;
        }

        Console.WriteLine($"[AoC 2023 - Day 07 - Part1] Result: {sum}");
    }
}