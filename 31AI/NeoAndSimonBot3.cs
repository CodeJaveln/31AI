namespace TrettioEtt
{
    class NeoAndSimonBot3 : Player
    {
        public CardData Cards;

        public NeoAndSimonBot3()
        {
            Cards = new CardData(ref this.Hand);

            Name = "NASConsole2.0";
        }

        public override bool Knacka(int round) //Round ökas varje runda. T.ex är spelare 2's andra runda = 4.
        {

            double percentageBarrier = 20; //At what percentage chance of winning we should knock
            Updatera();
            if (GenerateWinProbability() < percentageBarrier * Math.Sqrt(round))
            {
                return true;
            }
            return false;
        }
        double GenerateWinProbability() // Returns the percentage chance of the AI having a winning hand
        {
            int?[] IDs = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };

            Dictionary<Suit, int?[]> possibleCards = new Dictionary<Suit, int?[]>() { { Suit.Hjärter, IDs }, { Suit.Spader, IDs }, { Suit.Ruter, IDs }, { Suit.Klöver, IDs } };


            foreach (Card card in Cards.DiscardPile)
            {
                possibleCards[card.Suit][card.Id - 1] = null; // means it is not possible
            }
            foreach (Card card in Hand)
            {
                possibleCards[card.Suit][card.Id - 1] = null;
            }
            foreach (Card? card in Cards.EnemyHand)
            {
                if (card != null)
                {
                    possibleCards[card.Suit][card.Id - 1] = null;
                }
            }



            List<Card?> possibleEnemyHand = Cards.EnemyHand;
            List<int> pointerList = new List<int>();

            for (int a = 0; a < possibleEnemyHand.Count; a++)
            {
                if (possibleEnemyHand[a] != null)
                {
                    pointerList.Add(a);
                }
            }
            List<Card> possibleCardList = new List<Card>();

            foreach (Suit suit in possibleCards.Keys)
            {

                foreach (int? id in possibleCards[suit])
                {
                    if (id != null)
                    {
                        possibleCardList.Add(new Card(id ?? 0, suit));
                    }

                }
            }

            int i = 0;
            Random rng = new Random();
            bool isRunning = true;
            int totalHands = 0;
            int winningHands = 0;

            while (isRunning)
            {
                for (int u = 0; u < pointerList.Count; u++)
                {
                    try
                    {
                        int index = i % possibleCardList.Count;
                        int o = u;
                        while (o > 0)
                        {
                            index = index % possibleCardList.Count;
                            o -= 1;
                        }
                        possibleEnemyHand[pointerList[u]] = possibleCardList[rng.Next(0, possibleCardList.Count)];
                    }
                    catch
                    {
                        isRunning = false; break;
                    }
                }
                if (isRunning)
                {
                    int score = Game.HandScore(possibleEnemyHand, null);
                    totalHands++;
                    if (totalHands > 10000) { isRunning = false; }
                    if (score > Game.HandScore(this.Hand, null))
                    {
                        winningHands++;
                    }
                }
            }





            return (1 - (Convert.ToDouble(winningHands) / Convert.ToDouble(totalHands))) * 100;


        }
        public void Updatera() //Körs varje runda
        {
            if (OpponentsLatestCard != null)
            {
                if (Cards.DiscardPile.Count > 0)
                {
                    Cards.DiscardPile[0] = Game.GetTopCard();
                }
                else
                {
                    Cards.DiscardPile.Add(Game.GetTopCard());
                }
            }
        }

        private Suit BästaFärgen()
        {
            int[] färger = new int[4];

            foreach (Card card in Hand)
            {
                färger[(int)card.Suit]++;
            }

            int maxKort = färger.Max();

            for (int i = 0; i < färger.Length; i++)
            {
                if (färger[i] == maxKort)
                {
                    return (Suit)färger[i];
                }
            }
            return Suit.Hjärter;
        }

        public override bool TaUppKort(Card card)
        {
            if (card == SämstaKortet(card, Hand[0], Hand[1], Hand[2]))
            {
                return false;
            }
            if (card.Suit == BästaFärgen())
            {
                return true;
            }
            for (int i = 0; i < Hand.Count; i++)
            {
                if (card.Suit == Hand[i].Suit)
                {
                    return true;
                }
            }
            return false;
        }

        private Card SämstaKortet(params Card[] hand)
        {
            int worstValue = 1000;
            Card worstCard = null;
            bool wrongAttack = false;

            for (int i = 0; i < hand.Length; i++)
            {
                if (hand[i].Value < worstValue)
                {
                    for (int j = 0; j < hand.Length; j++)
                    {
                        if (j != i && hand[j].Suit == hand[i].Suit)
                        {
                            wrongAttack = true;
                            break;
                        }
                    }
                    if (wrongAttack == false)
                    {
                        worstValue = CardValue(hand[i]);
                        worstCard = hand[i];
                    }
                    wrongAttack = false;
                }
            }
            if (worstCard == null)
            {
                for (int i = 0; i < hand.Length; i++)
                {
                    if (hand[i].Value < worstValue)
                    {
                        worstValue = CardValue(hand[i]);
                        worstCard = hand[i];
                    }
                }
            }
            return worstCard;
        }

        public override Card KastaKort()
        {
            return SämstaKortet(Hand[0], Hand[1], Hand[2], Hand[3]);
        }

        private int CardValue(Card card)
        {
            return card.Value;
        }

        public override void SpelSlut(bool wonTheGame)
        {
            if (wonTheGame)
            {
                Wongames++;
            }
        }

        public struct CardData
        {
            public List<Card> DiscardPile = new List<Card>();
            public List<Card> Hand = new List<Card>();
            public List<Card> EnemyHand = new List<Card>() { null, null, null }; // null står för "Vet inte"
            public int DeckAmount;
            public CardData(ref List<Card> hand)
            {
                Hand = hand;
                DeckAmount = 46 - DiscardPile.Count;
            }
        }
    }
}