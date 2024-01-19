namespace TrettioEtt
{
    class NeoAndSimonConsole2 : Player
    {
        List<Card> UnavailableCards = new List<Card>();
        public NeoAndSimonConsole2()
        {
            Name = "NASConsole2";
        }

        public override bool Knacka(int round) //Round ökas varje runda. T.ex är spelare 2's andra runda = 4.
        {
            //for (int i = 0; i < Hand.Count; i++)
            //{
            //    if (!UnavailableCards.Find(Hand[i]))
            //    {

            //    }
            //}
            if (TaUppKort(Game.GetTopCard()) && Game.GetTopCard().Suit == BästaFärgen())
            {
                return false;
            }
            if (Game.Score(this) >= 21)
            {
                return true;
            }
            return false;
        }

        public override bool TaUppKort(Card card)
        {
            // Om tänker ta upp kort, kolla om det är större chans att dra ett kort istället


            if (card == SämstaKortet(card, Hand[0], Hand[1], Hand[2])) // VIKTIGT ändra inte om du vet vad du gör
            {
                return false;
            }
            if (card.Value > SämstaKortet(card, Hand[0], Hand[1], Hand[2]).Id && card.Value > 5)
            {
                return true;
            }
            if (card.Suit == BästaFärgen())
            {
                return true;
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
            UnavailableCards = new List<Card>();
            if (wonTheGame)
            {
                Wongames++;
            }
        }
    }
}