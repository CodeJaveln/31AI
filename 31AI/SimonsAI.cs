using System.Diagnostics;

namespace TrettioEtt
{
    class SimonsAI : Player
    {
        int[] ScoreOfWonGames = new int[10];
        int AverageWinScore = 23;
        int CurrentGame = 0;
        int WonGamesDuringCalculating = 0;
        bool TestScoreAverage = false;
        //List<Card> UnavailableCards = new List<Card>();
        public SimonsAI()
        {
            Name = "SimonsAI_0.6";
        }

        public override bool Knacka(int round) //Round ökas varje runda. T.ex är spelare 2's andra runda = 4.
        {
            //Ta average av motståndarens knackningar
            //for (int i = 0; i < Hand.Count; i++)
            //{
            //    if (!UnavailableCards.Find(Hand[i]))
            //    {

            //    }
            //}
            if (Game.Score(this) >= AverageWinScore)
            {
                return true;
            }
            else if (TaUppKort(Game.GetTopCard()))
            {
                return false;
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
            CurrentGame++;

            if (wonTheGame)
            {
                Wongames++;
            }

            if (CurrentGame % 100 == 0)
            {
                //////////////////////////// Kolla om den håller på att evaluera
                // kolla om den borde sänka average
                TestScoreAverage = true;
                WonGamesDuringCalculating = 0;
                ScoreOfWonGames = new int[10];
            }
            if (TestScoreAverage && wonTheGame)
            {
                WonGamesDuringCalculating++;
                ScoreOfWonGames[WonGamesDuringCalculating - 1] = Game.Score(this);
                if (WonGamesDuringCalculating == ScoreOfWonGames.Length)
                {
                    AverageWinScore = (int)ScoreOfWonGames.Average();
                    Debug.WriteLine("AverageWinScore: " + AverageWinScore);
                    TestScoreAverage = false; 
                }
            }
        }
    }
}