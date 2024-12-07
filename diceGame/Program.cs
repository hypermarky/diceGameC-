using System;

namespace DiceGameSimulation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();

            game.Start();
        }
    }

    public class Game
    {
        private Player Player1;
        private Player Player2;
        private bool PlayAgain;

        public void Start()
        {


            Console.WriteLine("Enter username for player 1:");
            string player1Name = Console.ReadLine();

            Console.WriteLine("Enter username for player 2:");
            string player2Name = Console.ReadLine();
            Console.Clear();

            Player1 = new Player(player1Name);
            Player2 = new Player(player2Name);

            PlayAgain = true;
            while (PlayAgain)
            {
                PlayRound();
            }

            DisplayFinalResults();
        }

        private bool CashChecker()
        {
            if (Player1.Cash <= 0 || Player2.Cash <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CashCheckerResult()
        {
            if (Player1.Cash <= 0)
            {
                Console.WriteLine($"{Player1.Name} ran out of money game over!");
                Console.WriteLine("Press any key to continue...");
                PlayAgain = false;
                Console.ReadKey();
            }
            else if (Player2.Cash <= 0)
            {
                Console.WriteLine($"{Player2.Name} ran out of money game over!");
                Console.WriteLine("Press any key to continue...");
                PlayAgain = false;
                Console.ReadKey();
            }

        }


        private void PlayRound()
        {
            bool CheckCash = CashChecker();
            if (CheckCash)
            {
                CashCheckerResult();
            }
            else
            {
                Player1.Reset();
                Player2.Reset();



                bool wager = AskToWager();
                if (wager)
                {
                    Player1.PlaceWager();
                    Player2.PlaceWager();
                }

                for (int i = 0; i < 3; i++)
                {
                    PlayTurn();
                }

                DetermineWinner(wager);
                PlayAgain = AskToPlayAgain();
            }
        }

        private void PlayTurn()
        {
            int player1Roll = Player1.RollDice();
            int player2Roll = Player2.RollDice();

            Console.WriteLine($"{Player1.Name} rolled a {player1Roll}");
            Console.WriteLine($"{Player2.Name} rolled a {player2Roll}");

            if (player1Roll > player2Roll)
            {
                Console.WriteLine($"{Player1.Name} wins this turn!");
                Player1.Score++;
            }
            else if (player1Roll < player2Roll)
            {
                Console.WriteLine($"{Player2.Name} wins this turn!");
                Player2.Score++;
            }
            else
            {
                Console.WriteLine("It's a draw!");
            }

            Thread.Sleep(1000);
        }

        private void DetermineWinner(bool wager)
        {
            Console.Clear();
            if (Player1.Score > Player2.Score)
            {
                Console.WriteLine($"{Player1.Name} wins the game!");
                Player1.GameWins++;
                if (wager)
                {
                    Player1.CollectWinnings(Player2);
                }
            }
            else if (Player1.Score < Player2.Score)
            {
                Console.WriteLine($"{Player2.Name} wins the game!");
                Player2.GameWins++;
                if (wager)
                {
                    Player2.CollectWinnings(Player1);
                }
            }
            else
            {
                Console.WriteLine("It's a draw!");
                if (wager)
                {
                    Player1.RefundWager();
                    Player2.RefundWager();
                }
            }
        }

        private bool AskToWager()
        {
            Console.WriteLine("Would you like to wager cash? (Y/N)");
            char input = Console.ReadLine().ToUpper()[0];
            return input == 'Y';
        }

        private bool AskToPlayAgain()
        {
            Console.WriteLine("Would you like to play again? (Y/N)");
            char input = Console.ReadLine().ToUpper()[0];
            return input == 'Y';
        }

        private void DisplayFinalResults()
        {
            Console.Clear();
            Console.WriteLine("Game Over! Final Results:");
            Console.WriteLine($"{Player1.Name} won {Player1.GameWins} game(s) with ${Player1.Cash} left.");
            Console.WriteLine($"{Player2.Name} won {Player2.GameWins} game(s) with ${Player2.Cash} left.");
        }
    }

    public class Player
    {
        public string Name { get; }
        public int Score { get; set; }
        public int GameWins { get; set; }
        public int Cash { get; private set; }
        private int CurrentWager { get; set; }

        public Player(string name)
        {
            Name = name;
            Cash = 1000;
        }

        public void Reset()
        {
            Score = 0;
            CurrentWager = 0;
        }

        public void PlaceWager()
        {
            Console.WriteLine($"{Name}'s Balance: ${Cash}");
            Console.WriteLine($"Enter wager amount for {Name}: ");
            CurrentWager = int.Parse(Console.ReadLine());

            while (CurrentWager > Cash || CurrentWager < 0)
            {
                Console.WriteLine("Insufficient funds! Enter a valid wager amount:");
                CurrentWager = int.Parse(Console.ReadLine());
            }

            Cash -= CurrentWager;
            Console.WriteLine($"{Name} wagered ${CurrentWager}.");
        }

        public void CollectWinnings(Player opponent)
        {
            Cash += CurrentWager + opponent.CurrentWager;
            Console.WriteLine($"{Name} won ${CurrentWager + opponent.CurrentWager}!");
        }

        public void RefundWager()
        {
            Cash += CurrentWager;
            Console.WriteLine($"{Name} had their wager refunded.");
        }

        public int RollDice()
        {
            Random random = new Random();
            return random.Next(1, 7);
        }
    }
}
