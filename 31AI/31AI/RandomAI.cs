using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrettioEtt
{
    internal class RandomAI : Player
    {
        public RandomAI()
        {
            Name = "RandomAI";
        }
        public override bool Knacka(int round) //Round ökas varje runda. T.ex är spelare 2's andra runda = 4.
        {
            if (Game.Score(this) >= 30)
            {
                return true;
            }
            return false;
        }

        public override bool TaUppKort(Card card)
        {
            if (card.Value == 11 || (card.Value == 10 && card.Suit == BestSuit))
            {
                return true;
            }
            return false;

        }

        public override Card KastaKort()
        {
            int worstValue = 12;
            Card worstCard = null;
            for (int i = 0; i < Hand.Count; i++)
            {
                if (Hand[i].Value < worstValue)
                {
                    worstValue = Hand[i].Value;
                    worstCard = Hand[i];
                }
            }
            return worstCard;

        }

        public override void SpelSlut(bool wonTheGame)
        {
            if (wonTheGame)
            {
                Wongames++;
            }

        }
    }
}
