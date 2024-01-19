using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrettioEtt
{
    internal class RandomAI : Player
    {
        Random rng = new Random();
        public RandomAI()
        {
            Name = "RandomAI";
        }
        public override bool Knacka(int round) //Round ökas varje runda. T.ex är spelare 2's andra runda = 4.
        {
            if (!(round == 1))
            {
                if (rng.Next(0, 2) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override bool TaUppKort(Card card)
        {
            if (rng.Next(0, 2) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public override Card KastaKort()
        {
            return Hand[rng.Next(0, Hand.Count)];

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
