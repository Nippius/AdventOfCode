namespace AdventOfCode2023;

public static class Day04
{
    private record Card(int Id, IList<int> ExpectedNumbers, IList<int> ActualNumbers) { }

    private class CardParser
    {
        private const char CardInfoSeparator = ':';
        private const char ExpectedVsActualNumbersSeparator = '|';
        private const char NumbersSeparator = ' ';

        private static IList<int> ParseCardNumbers(string inputNumbers)
        {
            List<int> numbers = [];
            string[] numbersToParse = inputNumbers.Split(NumbersSeparator);
            foreach (string numberToParse in numbersToParse)
            {
                if (numberToParse != "")
                {
                    numbers.Add(int.Parse(numberToParse));
                }
            }
            return numbers;
        }

        private static int ParseGameId(string inputCard)
        {
            string[] temp = inputCard.Split(' ');
            return int.Parse(temp[temp.Length - 1]);
        }

        private static Card ParseCard(string inputCard)
        {
            string[] cardInfo = inputCard.Split(CardInfoSeparator);
            int cardId = ParseGameId(cardInfo[0]);

            string[] cardNumbers = cardInfo[1].Split(ExpectedVsActualNumbersSeparator);
            IList<int> winningNumbers = ParseCardNumbers(cardNumbers[0]);
            IList<int> selectedNumbers = ParseCardNumbers(cardNumbers[1]);

            return new Card(cardId, winningNumbers, selectedNumbers);
        }

        public static IList<Card> ParseCards(StringReader sr)
        {
            IList<Card> cards = [];
            string? line = sr.ReadLine();
            while (line != null)
            {
                if (line != string.Empty)
                {
                    cards.Add(ParseCard(line));
                }
                line = sr.ReadLine();
            }
            return cards;
        }
    }

    private static int TotalPointsPerCard(Card card)
    {
        int totalNumbersThatMatch = GetAmmountOfMatchingNumbers(card);
        if(totalNumbersThatMatch > 1){
            // -1 because we "start" at 2
            return (int)Math.Pow(2, totalNumbersThatMatch-1);
        }
        return totalNumbersThatMatch;
    }

    private static void AddCardCopies(int[] cardsEarned, int startCard, int endCard)
    {
        // Parte 2 says: Cards will never make you copy a card past the end of the table.
        // So i don't care about bounds checking here
        for (int i = startCard; i < endCard; i++)
        {
            cardsEarned[i] += 1;
        }
    }

    private static int GetAmmountOfMatchingNumbers(Card card)
    {
        int matchingNumbers = 0;
        foreach (int number in card.ActualNumbers)
        {
            if (card.ExpectedNumbers.Contains(number))
            {
                matchingNumbers++;
            }
        }

        return matchingNumbers;
    }

    private static int TotalPoints(IList<Card> cards)
    {
        int totalPoints = 0;
        foreach (Card card in cards)
        {
            totalPoints += TotalPointsPerCard(card);
        }
        return totalPoints;
    }

    private static int TotalScratchcards(IList<Card> cards)
    {
        int[] cardsEarned = new int[cards.Count];
        foreach (Card card in cards)
        {
            int lastCardToCopyIndex = card.Id + GetAmmountOfMatchingNumbers(card);

            // For each copy we already own add the copies of the other cards we earned
            for (int i = 0; i < cardsEarned[card.Id - 1]; i++)
            {
                // Note: card.Id is used instead of card.Id + 1 because arrays start at 0
                // but card.Ids already start at one so no need to incremnt.
                AddCardCopies(cardsEarned, card.Id, lastCardToCopyIndex);
            }

            // Add the copies from the current card
            AddCardCopies(cardsEarned, card.Id, lastCardToCopyIndex);

            // Add the card itself
            cardsEarned[card.Id - 1] += 1;
        }
        return cardsEarned.Sum();
    }

    public static void Execute()
    {
        IList<Card> cards = [];
        StringReader sr = new(File.ReadAllText("./day04/input.txt"));

        cards = CardParser.ParseCards(sr);

        Console.WriteLine($"[AoC 2023 - Day 04 - Part 1] Result: {TotalPoints(cards)}");
        Console.WriteLine($"[AoC 2023 - Day 04 - Part 2] Result: {TotalScratchcards(cards)}");
    }
}
