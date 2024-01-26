using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrettioEtt;

namespace _31AI
{
    class GusTheSus : Player
    {
        //List<Card> UnavailableCards = new List<Card>();
        CardData Cards;
        public GusTheSus()
        {
            Name = "Gus the Sus";
        }



        public override bool Knacka(int round) //Round ökas varje runda. T.ex är spelare 2's andra runda = 4.
        {
            Update(round);

            double percentageThreshold = 60;

            if (GetWinProbability(1000) > percentageThreshold)
            {
                return true;
            }
            return false;
        }


        public double GetWinProbability(int sampleSize)
        {
            Dictionary<Suit, int> totalValues = new Dictionary<Suit, int>() { { Suit.Ruter, 0 }, { Suit.Spader, 0 }, { Suit.Klöver, 0 }, { Suit.Hjärter, 0 } };
            Dictionary<Suit, int> suitAmounts = totalValues.ToDictionary();


            foreach (Card card in Cards.UnknownCards) //get the stats of all the unknown cards
            {
                if (card != null)
                {
                    totalValues[card.Suit] += card.Value;
                    suitAmounts[card.Suit] += 1;
                }
            }

            Dictionary<Suit, double> averageValues = new Dictionary<Suit, double>();


            foreach (Suit suit in totalValues.Keys)
            {
                averageValues[suit] = Convert.ToDouble(totalValues[suit]) / Convert.ToDouble(suitAmounts[suit]);

            }

            List<int> PointerList = new List<int>(); //points to all indices where the enemy card is unknown

            for (int i = 0; i < Cards.EnemyHand.Count; i++)
            {
                if (Cards.EnemyHand[i] == null)
                {
                    PointerList.Add(i);
                }
            }

            Random rng = new Random();



            Suit[] suitArray = new Suit[4] { Suit.Klöver, Suit.Ruter, Suit.Spader, Suit.Hjärter };
            double playerScore = Game.HandScore(Cards.Hand, null);
            int wins = 0;

            for (int i = 0; i < sampleSize; i++) //make random combinations of averagevalues
            {
                Dictionary<Suit, double> possibleEnemyHand = new Dictionary<Suit, double>() { { Suit.Hjärter, 0 }, { Suit.Klöver, 0 }, { Suit.Ruter, 0 }, { Suit.Spader, 0 } };
                for (int j = 0; j < PointerList.Count; j++) // for every unknown card in the enemyhand
                {
                    Suit randomSuit;
                    double averageValue;
                    do
                    {
                        randomSuit = suitArray[rng.Next(0, 4)];
                        averageValue = averageValues[randomSuit];
                    }
                    while (averageValue == 0.0);

                    //Debug.WriteLine(averageValue, randomSuit.ToString());
                    possibleEnemyHand[randomSuit] += averageValue;
                }

                for (int a = 0; a < Cards.EnemyHand.Count; a++)
                {
                    Card tempCard = Cards.EnemyHand[a];
                    if (tempCard != null)
                    {
                        //Debug.WriteLine(tempCard.Value, tempCard.Suit.ToString());
                        possibleEnemyHand[tempCard.Suit] += tempCard.Value;
                    }
                }


                double HighestValue = 0;

                foreach (Suit suit in possibleEnemyHand.Keys) // get the highest value in possibleEnemyHand
                {
                    if (possibleEnemyHand[suit] > HighestValue)
                    {

                        HighestValue = possibleEnemyHand[suit];
                    }
                }
                if (playerScore > HighestValue) { wins++; }

            }
            double percentage = wins / sampleSize * 100;
            return percentage;

        }


        void Update(int round)
        {

            if (round <= 2)
            {
                Cards = new CardData(Hand);

            }

            Cards.Update(OpponentsLatestCard, Game.GetTopCard(), Hand);
        }





        public override bool TaUppKort(Card card)
        {
            // Om tänker ta upp kort, kolla om det är större chans att dra ett kort istället
            if (card == SämstaKortet(card, Hand[0], Hand[1], Hand[2])) // VIKTIGT ändra inte om du vet vad du gör
            {
                return false;
            }
            if (card.Value != SämstaKortet(card, Hand[0], Hand[1], Hand[2]).Value && card.Value > 5)
            {
                return true;
            }
            if (card.Value != SämstaKortet(card, Hand[0], Hand[1], Hand[2]).Value && card.Suit == BestSuit)
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
            //UnavailableCards = new List<Card>();
            if (wonTheGame)
            {
                Wongames++;
            }
        }
        public struct CardData
        {
            public List<Card> DiscardPile = new List<Card>();
            public List<Card> Hand = new List<Card>();
            public List<Card> UnknownCards = new List<Card>();
            public List<Card> AllPossibleCards = new List<Card>();
            public List<Card> EnemyHand = new List<Card>() { null, null, null }; // null står för "Vet inte"
            public enum CardCollection
            {
                Discard,
                Enemy,
                Unkown
            }
            Dictionary<CardCollection, List<Card>> CCDict;

            public CardData(List<Card> hand)
            {
                List<Suit> allSuits = new List<Suit>() { Suit.Hjärter, Suit.Spader, Suit.Ruter, Suit.Klöver };
                foreach (Suit suit in allSuits)
                {
                    for (int i = 0; i < 13; i++)
                    {
                        AllPossibleCards.Add(new Card(i + 1, suit));
                    }
                }
                UnknownCards = AllPossibleCards.ToList();

                CCDict = new Dictionary<CardCollection, List<Card?>>() { { CardCollection.Discard, DiscardPile }, { CardCollection.Enemy, EnemyHand }, { CardCollection.Unkown, UnknownCards } };
                Hand = hand;
                foreach (Card card in Hand)
                {
                    RemoveCard(card, CardCollection.Unkown);
                }






            }

            public void Update(Card? opponentsLatestCard, Card? topCard, List<Card> hand)
            {
                this.Hand = hand;

                if (opponentsLatestCard != null) //om de tar från slänghög
                {
                    RemoveCard(topCard, CardCollection.Enemy);
                    AddCard(topCard, CardCollection.Discard);
                    AddCard(opponentsLatestCard, CardCollection.Enemy);
                    RemoveCard(opponentsLatestCard, CardCollection.Discard);

                }
                else // om de tar från kortleken
                {
                    RemoveCard(topCard, CardCollection.Enemy);
                    AddCard(topCard, CardCollection.Discard);
                }


            }
            public void RemoveCard(Card card, CardCollection cardCollection)
            {



                for (int i = 0; i < CCDict[cardCollection].Count(); i++)
                {
                    Card card2 = CCDict[cardCollection][i];
                    if (card2 != null && card2.Suit == card.Suit && card2.Id == card.Id)
                    {
                        if (cardCollection == CardCollection.Discard)
                        {
                            DiscardPile.Remove(card);
                            return;
                        }
                        else
                        {
                            CCDict[cardCollection][i] = null;
                            return;
                        }

                    }
                }

            }
            public void AddCard(Card card, CardCollection cardCollection)
            {
                for (int i = 0; i < CCDict[cardCollection].Count(); i++)
                {
                    if (CCDict[cardCollection][i] == null)
                    {
                        CCDict[cardCollection][i] = card;
                        if (cardCollection != CardCollection.Unkown)
                        {
                            RemoveCard(card, CardCollection.Unkown);
                        }
                        return;
                    }
                }
                if (cardCollection == CardCollection.Discard)
                {
                    CCDict[cardCollection].Add(card);
                    RemoveCard(card, CardCollection.Unkown);
                }
            }
        }




    }
}