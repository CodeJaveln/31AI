using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Threading;
using System.Xml.Linq;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Data;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using _31AI;

namespace TrettioEtt
{
    #region ImportantCode
    public enum Suit { Hjärter, Ruter, Spader, Klöver };

    class Program
    {

        static void Main(string[] args)
        {
            Console.WindowWidth = 120;
            Game game = new Game();

            List<Player> players = new List<Player>();
            players.Add(new BasicPlayer());
            players.Add(new xx_ProPlayer_xx());
            players.Add(new SimonsSuperiorAI());
            players.Add(new SimonsSuperiorAI());
            players.Add(new NeoAndSimonBot3());
            players.Add(new RandomAI());
            players.Add(new Gulagen());
            players.Add(new WorstBot());
            players.Add(new NeoAndSimonConsole2());
            players.Add(new NeoAndSimonConsole2());

            Console.WriteLine("Vilka två spelare skall mötas?");
            for (int i = 1; i <= players.Count; i++)
            {
                Console.WriteLine(i + ": {0}", players[i - 1].Name);
            }
            int p1 = int.Parse(Console.ReadLine());
            int p2 = int.Parse(Console.ReadLine());
            Player player1 = players[p1 - 1];
            Player player2 = players[p2 - 1];
            player1.Game = game;
            player1.PrintPosition = 0;
            player2.Game = game;
            player2.PrintPosition = 9;
            game.Player1 = player1;
            game.Player2 = player2;
            Console.WriteLine("Hur många spel skall spelas?");
            int numberOfGames = int.Parse(Console.ReadLine());
            Console.WriteLine("Skriva ut första spelet? (y/n)");
            string print = Console.ReadLine();
            Console.Clear();
            if (print == "y")
                game.Printlevel = 2;
            else
                game.Printlevel = 0;
            game.Initialize(true);
            game.PlayAGame(true);
            Console.Clear();
            bool player1starts = true;

            for (int i = 1; i < numberOfGames; i++)
            {
                game.Printlevel = 0;
                player1starts = !player1starts;
                game.Initialize(false);
                game.PlayAGame(player1starts);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(0, 3);
                Console.Write(player1.Name + ":");
                Console.ForegroundColor = ConsoleColor.Green;

                Console.SetCursorPosition((player1.Wongames * 100 / numberOfGames) + 15, 3);
                Console.Write("█");
                Console.SetCursorPosition((player1.Wongames * 100 / numberOfGames) + 16, 3);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(player1.Wongames);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(0, 5);
                Console.Write(player2.Name + ":");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition((player2.Wongames * 100 / numberOfGames) + 15, 5);
                Console.Write("█");
                Console.SetCursorPosition((player2.Wongames * 100 / numberOfGames) + 16, 5);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(player2.Wongames);
            }
            Console.SetCursorPosition(25, 7);
            Console.Write(player1.Name);
            Console.SetCursorPosition(45, 7);
            Console.WriteLine(player2.Name);
            Console.WriteLine("          Vunna spel:");
            Console.WriteLine("            Skillnad:");
            Console.WriteLine("Antal ronder i snitt:");
            Console.WriteLine("    Egna knackningar:");
            Console.WriteLine("         Andel vunna:");
            Console.WriteLine("  Knackpoäng i snitt:");
            Console.WriteLine("            Antal 31:");
            Console.SetCursorPosition(25 + player1.Name.Length / 2, 8);
            Console.Write(player1.Wongames);
            Console.SetCursorPosition(25 + player1.Name.Length / 2, 9);
            int diff = player1.Wongames - player2.Wongames;
            if (diff > 0)
            {
                Console.Write("+" + diff);
            }
            Console.SetCursorPosition(25 + player1.Name.Length / 2, 10);
            double avgRounds1 = Math.Round((double)player1.StoppedRounds / (double)player1.StoppedGames, 2);
            Console.Write(avgRounds1);
            Console.SetCursorPosition(25 + player1.Name.Length / 2, 11);
            Console.Write(player1.KnackWins + player2.DefWins);
            Console.SetCursorPosition(25 + player1.Name.Length / 2, 12);
            double winPercent1 = Math.Round((double)player1.KnackWins * 100 / (player1.KnackWins + player2.DefWins), 1);
            Console.Write(winPercent1 + " %");
            Console.SetCursorPosition(25 + player1.Name.Length / 2, 13);
            double knackAvg = Math.Round((double)player1.KnackTotal / (player1.KnackWins + player2.DefWins), 1);
            Console.Write(knackAvg);
            Console.SetCursorPosition(25 + player1.Name.Length / 2, 14);
            Console.Write(player1.TrettiettWins);

            Console.SetCursorPosition(45 + player2.Name.Length / 2, 8);
            Console.Write(player2.Wongames);
            Console.SetCursorPosition(45 + player2.Name.Length / 2, 9);
            int diff2 = player2.Wongames - player1.Wongames;
            if (diff2 > 0)
            {
                Console.Write("+" + diff2);
            }
            Console.SetCursorPosition(45 + player2.Name.Length / 2, 10);
            double avgRounds2 = Math.Round((double)player2.StoppedRounds / (double)player2.StoppedGames, 2);
            Console.Write(avgRounds2);

            Console.SetCursorPosition(45 + player2.Name.Length / 2, 11);
            Console.Write(player2.KnackWins + player1.DefWins);
            Console.SetCursorPosition(45 + player2.Name.Length / 2, 12);
            double winPercent2 = Math.Round((double)player2.KnackWins * 100 / (player2.KnackWins + player1.DefWins), 1);
            Console.Write(winPercent2 + " %");
            Console.SetCursorPosition(45 + player2.Name.Length / 2, 13);
            double knackAvg2 = Math.Round((double)player2.KnackTotal / (player2.KnackWins + player1.DefWins), 1);
            Console.Write(knackAvg2);
            Console.SetCursorPosition(45 + player2.Name.Length / 2, 14);
            Console.Write(player2.TrettiettWins);
            Console.ReadLine();
        }
    }
    class Card
    {
        public int Value { get; private set; } //Kortets värde enligt reglerna i Trettioett, t.ex. dam = 10
        public Suit Suit { get; private set; }
        public int Id { get; private set; }  //Typ av kort, t.ex dam = 12


        public Card(int id, Suit suit)
        {
            Id = id;
            Suit = suit;
            if (id == 1)
            {
                Value = 11;
            }
            else if (id > 9)
            {
                Value = 10;
            }
            else
            {
                Value = id;
            }
        }

        public void PrintCard()
        {
            string cardname = "";
            if (Id == 1)
            {
                cardname = "ess ";
            }
            else if (Id == 10)
            {
                cardname = "tio ";
            }
            else if (Id == 11)
            {
                cardname = "knekt ";
            }
            else if (Id == 12)
            {
                cardname = "dam ";
            }
            else if (Id == 13)
            {
                cardname = "kung ";
            }
            if (Suit == Suit.Hjärter)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (Suit == Suit.Ruter)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (Suit == Suit.Spader)
                Console.ForegroundColor = ConsoleColor.Gray;
            else if (Suit == Suit.Klöver)
                Console.ForegroundColor = ConsoleColor.Green;

            Console.Write(" " + Suit + " " + cardname);
            if (cardname == "")
            {
                Console.Write(Id + " ");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            Card card = (Card)obj;
            if (card.Id == Id && card.Suit == Suit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


    class Game
    {
        List<Card> CardDeck = new List<Card>();
        List<Card> DiscardPile = new List<Card>();
        public bool Lastround;
        int Cardnumber;
        public int Printlevel;
        public int Discardnumber;
        public Player Player1 { private get; set; }
        public Player Player2 { private get; set; }
        Random RNG = new Random();
        public int NbrOfRounds;

        public Game()
        {

        }

        public void Initialize(bool firstGame)
        {
            Lastround = false;
            Player1.lastTurn = false;
            Player2.lastTurn = false;
            Cardnumber = -1;
            Discardnumber = 52;
            CardDeck = new List<Card>();
            DiscardPile = new List<Card>();
            Player1.Hand = new List<Card>();
            Player2.Hand = new List<Card>();

            int id;
            int suit;
            for (int i = 0; i < 52; i++)
            {
                id = i % 13 + 1;
                suit = i % 4;
                CardDeck.Add(new Card(id, (Suit)suit));
            }
            Shuffle();
            for (int i = 0; i < 3; i++)
            {
                Player1.Hand.Add(DrawCard());
                Player2.Hand.Add(DrawCard());
            }
            if (firstGame)
            {
                if (Score(Player1) > 10 || Score(Player2) > 10)
                {
                    //Console.WriteLine("Omgiv. Scores: " + Score(Player1) + " , " + Score(Player2));
                    //Console.ReadKey();
                    Initialize(true);
                }
            }
            Discard(DrawCard());
        }

        public void PrintHand(Player player)
        {
            Console.SetCursorPosition(0, player.PrintPosition);
            Console.WriteLine(player.Name + " har ");
            for (int i = 0; i < player.Hand.Count; i++)
            {
                player.Hand[i].PrintCard();
                Console.WriteLine();
            }
        }

        private bool CheckDeck()
        {
            List<Card> mockDeck = new List<Card>();
            int id;
            int suit;
            for (int i = 0; i < 52; i++)
            {
                id = i % 13 + 1;
                suit = i % 4;
                mockDeck.Add(new Card(id, (Suit)suit));
            }
            foreach (var card in CardDeck)
            {
                if (!mockDeck.Remove(card))
                    return false;
            }
            foreach (var card in DiscardPile)
            {
                if (!mockDeck.Remove(card))
                    return false;
            }
            foreach (var card in Player1.Hand)
            {
                if (!mockDeck.Remove(card))
                    return false;
            }
            foreach (var card in Player2.Hand)
            {
                if (!mockDeck.Remove(card))
                    return false;
            }
            if (mockDeck.Count != 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsHandOK(List<Card> hand)
        {
            if (hand[0].Equals(hand[1]) || hand[1].Equals(hand[2]) || hand[0].Equals(hand[2]))
                return false;
            else
                return true;
        }


        private int PlayARound(Player player, Player otherPlayer)
        {
            if (Printlevel > 1)
            {
                PrintHand(player);
                Console.SetCursorPosition(4, 6);
                Console.Write("På skräphögen ligger ");
                DiscardPile.Last().PrintCard();
            }
            otherPlayer.OpponentsLatestCard = null;
            if (NbrOfRounds <= 1)
                player.Knacka(NbrOfRounds);
            if (NbrOfRounds > 1 && !Lastround && player.Knacka(NbrOfRounds))
            {
                if (Printlevel > 1)
                {
                    Console.SetCursorPosition(20, player.PrintPosition + 2);
                    Console.Write(player.Name + " knackar!");
                    Console.ReadKey();
                }
                return Score(player);
            }
            else if (player.TaUppKort(DiscardPile.Last()))
            {
                player.Hand.Add(PickDiscarded());
                otherPlayer.OpponentsLatestCard = player.Hand.Last();
                if (Printlevel > 1)
                {
                    Console.SetCursorPosition(20, player.PrintPosition + 2);
                    Console.Write(player.Name + " plockar ");
                    player.Hand.Last().PrintCard();
                    Console.Write(" från skräphögen.");
                }
            }
            else
            {
                player.Hand.Add(DrawCard());
                if (Printlevel > 1)
                {
                    Console.SetCursorPosition(20, player.PrintPosition + 2);
                    Console.Write(player.Name + " drar ");
                    player.Hand.Last().PrintCard();
                }
            }
            Card discardcard = player.KastaKort();

            UpdateHand(player, discardcard);
            if (Printlevel > 1)
            {
                Console.SetCursorPosition(20, player.PrintPosition + 3);
                Console.Write(player.Name + " kastar bort ");
                discardcard.PrintCard();
                Console.Write("       Tryck ENTER");
                Console.ReadLine();
                Console.Clear();
                PrintHand(player);
            }
            Discard(discardcard);
            if (Score(player) == 31)
            {
                return 31;
            }
            else
            {
                return 0;
            }
        }

        private void UpdateHand(Player player, Card discardcard)
        {
            player.Hand.Remove(discardcard);
        }

        public Card GetTopCard()
        {
            return DiscardPile.Last();
        }

        public int Score(Player player) //Reurnerar spelarens poäng av bästa färg. Uppdaterar player.bestsuit.
        {
            int[] suitScore = new int[4];
            if (player.Hand[0].Value == 11 && player.Hand[1].Value == 11 && player.Hand[2].Value == 11)
            {
                return 31;
            }

            for (int i = 0; i < player.Hand.Count; i++)
            {
                if (player.Hand[i] != null)
                    suitScore[(int)player.Hand[i].Suit] += player.Hand[i].Value;
            }
            int max = 0;

            for (int i = 0; i < 4; i++)
            {
                if (suitScore[i] > max)
                {
                    max = suitScore[i];
                    player.BestSuit = (Suit)i;
                }
            }
            return max;

        }

        public int SuitScore(List<Card> hand, Suit suit) //Reurnerar handens poäng av en viss färg
        {
            int sum = 0;
            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i] != null && hand[i].Suit == suit)
                {
                    sum += hand[i].Value;
                }
            }
            return sum;


        }

        public int HandScore(List<Card> hand, Card excluded) //Returnerar handens poäng av bästa färg. Undantar ett kort från beräkningen (null för att ta med alla kort)
        {
            int[] suitScore = new int[4];
            int aces = 0;
            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i] != null && hand[i] != excluded)
                {
                    suitScore[(int)hand[i].Suit] += hand[i].Value;
                    if (hand[i].Value == 11)
                    {
                        aces++;
                    }
                }
            }
            if (aces == 3)
                return 31;
            int max = 0;

            for (int i = 0; i < 4; i++)
            {
                if (suitScore[i] > max)
                {
                    max = suitScore[i];
                }
            }
            return max;
        }

        public void PlayAGame(bool player1starts)
        {
            NbrOfRounds = 0;
            Player playerInTurn, playerNotInTurn, temp;
            if (player1starts)
            {
                playerInTurn = Player1;
                playerNotInTurn = Player2;
            }
            else
            {
                playerInTurn = Player2;
                playerNotInTurn = Player1;
            }
            while (Cardnumber < 51 && NbrOfRounds < 100)
            {
                if (DiscardPile.Count + CardDeck.Count != 46)
                {
                    Console.WriteLine("FEL, kortantal inte 52 ");
                    Console.ReadLine();
                }
                NbrOfRounds++;
                int result = PlayARound(playerInTurn, playerNotInTurn);
                if (result == 31)
                {
                    if (Printlevel > 1)
                        PrintHand(playerNotInTurn);
                    playerInTurn.SpelSlut(true);
                    playerInTurn.TrettiettWins++;
                    playerInTurn.StoppedGames++;
                    playerInTurn.StoppedRounds += NbrOfRounds;
                    playerNotInTurn.SpelSlut(false);
                    if (Printlevel > 0)
                    {
                        Console.SetCursorPosition(15, playerInTurn.PrintPosition + 5);
                        Console.Write(playerInTurn.Name + " fick 31 och vann spelet!");
                        Console.ReadLine();
                    }
                    break;
                }
                else if (result > 0)
                {
                    playerInTurn.KnackTotal += result;
                    Lastround = true;
                    playerNotInTurn.lastTurn = true;
                    PlayARound(playerNotInTurn, playerInTurn);
                    if (Printlevel > 1)
                    {
                        PrintHand(playerInTurn);
                        PrintHand(playerNotInTurn);
                    }
                    if (Printlevel > 0)
                    {
                        Console.SetCursorPosition(15, playerInTurn.PrintPosition + 5);
                        Console.Write(playerInTurn.Name + " knackade och har " + Score(playerInTurn) + " poäng");
                        Console.SetCursorPosition(15, playerNotInTurn.PrintPosition + 5);
                        Console.Write(playerNotInTurn.Name + " har " + Score(playerNotInTurn) + " poäng");
                    }

                    if (Score(playerInTurn) > Score(playerNotInTurn))
                    {
                        playerInTurn.SpelSlut(true);
                        playerInTurn.KnackWins++;
                        playerInTurn.StoppedGames++;
                        playerInTurn.StoppedRounds += NbrOfRounds;
                        playerNotInTurn.SpelSlut(false);
                        if (Printlevel > 0)
                        {
                            Console.SetCursorPosition(15, playerInTurn.PrintPosition + 6);
                            Console.WriteLine(playerInTurn.Name + " vann!");
                            Console.ReadLine();
                        }
                        break;
                    }
                    else
                    {
                        playerInTurn.SpelSlut(false);
                        playerNotInTurn.SpelSlut(true);
                        playerNotInTurn.DefWins++;
                        playerInTurn.StoppedGames++;
                        playerInTurn.StoppedRounds += NbrOfRounds;
                        //playerNotInTurn.WinRounds += NbrOfRounds;
                        if (Printlevel > 0)
                        {
                            Console.SetCursorPosition(15, playerNotInTurn.PrintPosition + 6);
                            Console.WriteLine(playerNotInTurn.Name + " vann!");
                            Console.ReadLine();
                        }
                        break;

                    }

                }

                else

                {

                    temp = playerNotInTurn;

                    playerNotInTurn = playerInTurn;

                    playerInTurn = temp;

                }



            }

            if (Cardnumber >= 51 || NbrOfRounds >= 100)

            {

                if (Printlevel > 0)

                {

                    Console.SetCursorPosition(0, 20);

                    Console.WriteLine("Korten tog slut utan att någon spelare vann.");

                    Console.ReadLine();

                }

                playerInTurn.SpelSlut(false);

                playerNotInTurn.SpelSlut(false);



            }



        }

        private void Discard(Card card)
        {
            Discardnumber--;
            DiscardPile.Add(card);
        }

        private Card DrawCard()
        {
            Cardnumber++;
            Card card = CardDeck.First();
            CardDeck.RemoveAt(0);
            return card;
        }

        private Card PickDiscarded()
        {
            Card card = DiscardPile.Last();
            DiscardPile.RemoveAt(DiscardPile.Count - 1);
            Discardnumber++;
            return card;
        }



        private void Shuffle()
        {
            for (int i = 0; i < 200; i++)
            {
                SwitchCards();
            }
        }

        private void SwitchCards()
        {
            int card1 = RNG.Next(CardDeck.Count);
            int card2 = RNG.Next(CardDeck.Count);
            Card temp = CardDeck[card1];
            CardDeck[card1] = CardDeck[card2];
            CardDeck[card2] = temp;
        }
    }

    #endregion
    #region player Class
    abstract class Player
    {
        public string Name;
        // Dessa variabler får ej ändras

        public List<Card> Hand = new List<Card>();  // Lista med alla kort i handen. Bara de tre första platserna har kort i sig när rundan börjar, ett fjärde läggs till när man tar ett kort
        public Game Game;
        public Suit BestSuit; // Den färg med mest poäng. Uppdateras varje gång game.Score anropas
        public int Wongames;
        public int TrettiettWins;
        public int KnackWins;
        public int DefWins;
        public int KnackTotal;
        public int StoppedGames;
        public int StoppedRounds;
        public int PrintPosition;
        public bool lastTurn; // True om motståndaren har knackat, annars false.
        public Card? OpponentsLatestCard; // Det senaste kortet motståndaren tog. Null om kortet drogs från högen.

        public abstract bool Knacka(int round);
        public abstract bool TaUppKort(Card card);
        public abstract Card KastaKort();
        public abstract void SpelSlut(bool wonTheGame);
    }

    class BasicPlayer : Player
    {

        public BasicPlayer()
        {
            Name = "BasicPlayer";
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

    #endregion

   
