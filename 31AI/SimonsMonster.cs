namespace TrettioEtt
{
    class SimonsMonster : Player
    {
        int[] ScoreOfWonGames = new int[10];
        int AverageWonScore = 18;
        int CurrentGame = 0;
        int WonGamesDuringCalculating = 0;
        bool TestScoreAverage;
        //List<Card> UnavailableCards = new List<Card>();
        public SimonsMonster()
        {
            Name = "SimonsAI_1.2";
        }

        public override bool Knacka(int round) //Round ökas varje runda. T.ex är spelare 2's andra runda = 4.
        {
            //for (int i = 0; i < Hand.Count; i++)
            //{
            //    if (!UnavailableCards.Find(Hand[i]))
            //    {

            //    }
            //}
            if (TaUppKort(Game.GetTopCard()))
            {
                return false;
            }
            if (Game.Score(this) >= AverageWonScore)
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
            if (card.Value > SämstaKortet(card, Hand[0], Hand[1], Hand[2]).Value && card.Value > 5)
            {
                return true;
            }
            if (card.Suit == BestSuit)
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
                        worstValue = hand[i].Value;
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
                        worstValue = hand[i].Value;
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

        public override void SpelSlut(bool wonTheGame)
        {
            // If game % 1000 = 0 then recount average!!!! fix this
            CurrentGame++;

            if (wonTheGame)
            {
                Wongames++;
            }

            if (CurrentGame % 100 == 0)
            {
                TestScoreAverage = true;
                WonGamesDuringCalculating = 0;
                ScoreOfWonGames = new int[10];
            }
            if (TestScoreAverage && wonTheGame)
            {
                WonGamesDuringCalculating++;
                if (WonGamesDuringCalculating <= ScoreOfWonGames.Length)
                {
                    ScoreOfWonGames[WonGamesDuringCalculating - 1] = Game.Score(this);
                    if (WonGamesDuringCalculating == ScoreOfWonGames.Length)
                    {
                        AverageWonScore = (int)ScoreOfWonGames.Average();
                    }
                }
            }
        }
    }
}