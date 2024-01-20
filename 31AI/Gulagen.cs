using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrettioEtt;

namespace _31AI
{
    class Gulagen : Player
    {

        public Gulagen()
        {
            Name = "ManualLabour AB";
        }

        public override bool Knacka(int round) //Round ökas varje runda. T.ex är spelare 2's andra runda = 4.
        {
            Console.WriteLine("\n\nKnacka metoden:\n");

            PrintHand();

            Console.Write("\nHögkortet: ");
            Game.GetTopCard().PrintCard();

            if (round == 1)
            {
                Console.WriteLine("\nScore: " + Game.Score(this) + ". Du får inte knacka den första rundan!");
            }
            else
            {
                Console.Write("\nScore: " + Game.Score(this) + ". Runda: " + ((round + 1) / 2) + ". Vill du knacka (Y/N): ");
                string villKnacka = Console.ReadLine().ToLower();
                if (villKnacka == "y")
                {
                    return true;
                }
            }

            return false;
        }

        public override bool TaUppKort(Card card)
        {
            Console.WriteLine("\n\nTaUppKort metoden:\n");

            PrintHand();

            Console.Write("\nHögkortet: ");
            Game.GetTopCard().PrintCard();

            Console.Write("\nVill du ta upp kortet från högen eller dra ett kort (Y/N): ");
            string villTaKortet = Console.ReadLine().ToLower();
            if (villTaKortet == "y")
            {
                return true;
            }
            return false;
        }

        public override Card KastaKort()
        {
            Console.WriteLine("\n\nKastaKort metoden:\n");

            PrintHand();

            Console.Write("\nVilket kort vill du kasta (1-4): ");
            int kastaKort = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

            return Hand[kastaKort - 1];
        }

        private void PrintHand()
        {
            Console.Write("Hand: ");
            for (int i = 0; i < Hand.Count; i++)
            {
                this.Hand[i].PrintCard();
            }
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
