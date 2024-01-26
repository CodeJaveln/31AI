using System.Data;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TrettioEtt
{
    class NeoOchSimonsBot : Player
    {
        CardData Cards;
        List<RAD> RADs = new List<RAD>();
        bool Knackade = false;
        int KnackRound;
        public NeoOchSimonsBot()
        {
            Name = "NASKonsolen";
            
        }

        //Neo:
        //Får RAD för round och sen tar dens threshold och sen jämför det med win probability
        public override bool Knacka(int round) //Round ökas varje runda. T.ex är spelare 2's andra runda = 4.
        {
            Update(round);

            
            RAD rad = SearchRADs(round);
            


            double percentageThreshold;
            if (rad != null)
            {
                percentageThreshold = rad.Threshold;
            }
            else
            {
                percentageThreshold = 50;
            }

            

            if (GetWinProbability(800) > percentageThreshold)
            {
                Knackade = true;
                KnackRound = round;
                return true;


            }
            else
            {
                return false;
            }

        }
        //Neo:
        //returnerar chansen att spelaren har en vinnande hand. 1-100%. Hittar average värdet av alla suits från kor en som vi inte vet
        public double GetWinProbability(int sampleSize)
        {
            Dictionary<Suit, int> totalValues = new Dictionary<Suit, int>() { {Suit.Ruter, 0}, {Suit.Spader, 0}, {Suit.Klöver, 0}, {Suit.Hjärter, 0} };
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
                
                foreach(Suit suit in possibleEnemyHand.Keys) // get the highest value in possibleEnemyHand
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

        //Neo:
        //Körs varje runda
        //används för att uppdatera Cards
        void Update(int round)
        {
            
            if (round == 1 || round == 2)
            {
                Cards = new CardData(Hand);
                
            }


            Cards.Update(OpponentsLatestCard, Game.GetTopCard(), Hand);

        }
        //Neo:
        //Letar efter den RAD som motsvarar roundnum i RADs listan
        public RAD SearchRADs(int roundNum)
        {
            foreach(RAD rad in RADs)
            {
                if (rad.RoundNum == roundNum)
                {
                    return rad;
                }
            }
            return null;
        }

        // Kodskrivare Simon:
        public override bool TaUppKort(Card card)
        {
            // Det här är så att den inte tar upp ett kort bara för att kasta det direkt efter.
            if (card == SämstaKortet(card, Hand[0], Hand[1], Hand[2])) // VIKTIGT ändra inte om du vet vad du gör
            {
                return false;
            }

            // Det här är om kortet inte är sämsta korten samt har ett högre värde än fem för annars verkar det som om då kan det bli större chans att få ett bättre kort om man drar.
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
        // Kodskrivare Simon:
        private Card SämstaKortet(params Card[] hand)
        {
            // Deklarerar variablarna, är bara default values för koden.
            int worstValue = 1000;
            Card worstCard = null;
            bool wrongAttack = false;

            // Går igenom varje kort på handen, hittar det med lägst värde och har det som sämsta kortet.
            for (int i = 0; i < hand.Length; i++)
            {
                if (hand[i].Value < worstValue)
                {
                    // Går igenom igen för att kolla om kortets färg är vanlig så borde den inte kasta den, därmed "wrongAttack" boolean.
                    // Jag kan inte göra den här koden bättre utan att försämra funktionaliteten.
                    for (int j = 0; j < hand.Length; j++)
                    {
                        if (j != i && hand[j].Suit == hand[i].Suit)
                        {
                            wrongAttack = true;
                            break;
                        }
                    }
                    // Så den tar kortet som är det sämsta kortet om den färgen inte är vanligast.
                    if (wrongAttack == false)
                    {
                        worstValue = hand[i].Value;
                        worstCard = hand[i];
                    }
                    wrongAttack = false;
                }
            }
            // Det här är ifall alla färger är samma och inte har hittat ett "sämsta kort".
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
        // Kodskrivare Simon:
        public override Card KastaKort()
        {
            // Kasta sämsta kortet på handen
            return SämstaKortet(Hand[0], Hand[1], Hand[2], Hand[3]);
        }

        // När spelet tar slut
        public override void SpelSlut(bool wonTheGame)
        {
            // Hålla reda på hur många spel man har vunnit.
            if (wonTheGame)
            {
                Wongames++;
            }
                
            //Neo
            //Updaterar Rad information eller skapar en ny RAD om det inte finns någon för den rundan
            if (Knackade)
            {
                Knackade = false;
                RAD rad = SearchRADs(KnackRound);
                if (rad == null)
                {
                    rad = new RAD(KnackRound);
                    rad.Update(wonTheGame);
                    RADs.Add(rad);
            
                }
                else
                {
                    rad.Update(wonTheGame);
                }
            }
        }
        
        //Neo:
        //Håller information om både kort som vi vet och som vi inte vet. I de flesta listor så står null för "vet inte vad det är för kort"
        public class CardData
        {
            public List<Card> DiscardPile = new List<Card>();
            public List<Card> Hand = new List<Card>(); //Spelarens hand
            public List<Card> UnknownCards = new List<Card>(); //alla kort där vi inte vet var den är 
            public List<Card> AllPossibleCards = new List<Card>(); //Alla 52 kort i en kortlek
            public List<Card> EnemyHand = new List<Card>() { null, null, null }; //om vi vet något kort som finns i Enemyhand så kommer det stå här
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
                foreach (Card card in Hand) //ta bort alla kort i hand från Unkown 
                {
                    RemoveCard(card, CardCollection.Unkown);
                }

                


                
                
            }
            //uppdaterar 
            public void Update(Card? opponentsLatestCard, Card? topCard, List<Card> hand)
            {
                this.Hand = hand;

                if(opponentsLatestCard != null) //om de tar från slänghög
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
        //Neo:
        public class RAD // RAD stands for round average data. Det är typ en 
        {
            public int RoundNum; //vilken runda den trackar
            public double Threshold; //vilken threshold GetWinProbability har för den rundan
            public double Change; //hur mycket threshold ändras med varje gång den vinner eller förlorar
            public RAD(int roundNum)
            {
                this.RoundNum = roundNum; 
                this.Threshold = 50;
                this.Change = 25;
            }

            //Neo
            //Halverar change och ändrar Threshold baserat på om den vann eller inte
            public void Update(bool won)
            {
                double localChange = this.Change;
                if (!won)
                {
                    localChange *= -1;
                }

                this.Threshold += localChange;
                this.Change /= 2.0;
            }
        }
    }
}