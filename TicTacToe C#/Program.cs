using System;
using System.IO;
using System.Linq;

namespace TicTacToe_C_
{
    internal class Program
    {
        static char[,] board = {
        {' ', ' ', ' '},
        {' ', ' ', ' '},
        {' ', ' ', ' '}
    };

        static string[] playerNames = new string[2];
        static char currentPlayer = 'X';

        static void Main()
        {
            Console.WriteLine("Welcome to my Tic Tac Toe game!");
            Console.Write("Enter Player 1's name: ");
            playerNames[0] = Console.ReadLine();
            Console.Write("Enter Player 2's name: ");
            playerNames[1] = Console.ReadLine();

            bool playAgain = true;

            do
            {
                bool gameOver = false;
                int k = 1;
                do
                {
                    Console.Clear();
                    DrawBoard();
                    MakeMove();

                    /*if (k++ == 3)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                board[i, j] = 'X';
                            }
                        }
                    }*/

                    // Check for a win or a draw
                    if (CheckWin() || CheckDraw())
                    {
                        Console.Clear();
                        DrawBoard();
                        gameOver = true;

                        if (CheckWin())
                        {
                            Console.WriteLine($"{playerNames[currentPlayer == 'X' ? 0 : 1]} wins!");
                            UpdateWinHistory(playerNames[0], playerNames[1], playerNames[currentPlayer == 'X' ? 0 : 1]);
                        }

                        else
                            Console.WriteLine("It's a draw!");
                    }

                    // Switch players
                    currentPlayer = (currentPlayer == 'X') ? 'O' : 'X';

                } while (!gameOver);
                Console.WriteLine("Game Over!");

                //Console.ReadLine();

                // Ask if the players want to play again
                Console.Write("Play again? (Y/N): ");
                char playAgainChoice = char.ToUpper(Console.ReadKey().KeyChar);

                if (playAgainChoice != 'Y')
                    playAgain = false;

                // Reset the board for a new game
                InitializeBoard();
                currentPlayer = 'X';

            } while (playAgain);

        }

        static void DrawBoard()
        {
            Console.WriteLine($"   0   1   2");
            Console.WriteLine("0  " + board[0, 0] + " | " + board[0, 1] + " | " + board[0, 2]);
            Console.WriteLine("  ---|---|---");
            Console.WriteLine("1  " + board[1, 0] + " | " + board[1, 1] + " | " + board[1, 2]);
            Console.WriteLine("  ---|---|---");
            Console.WriteLine("2  " + board[2, 0] + " | " + board[2, 1] + " | " + board[2, 2]);
        }

        static void MakeMove()
        {
            bool validMove = false;

            do
            {
                int row = -1;
                int col = -1;

                Console.WriteLine($"{playerNames[currentPlayer == 'X' ? 0 : 1]}'s turn ({currentPlayer}):");
                do
                {
                    Console.Write("Enter row (0, 1, or 2): ");
                    try
                    { row = int.Parse(Console.ReadLine()); }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number.");
                        continue;
                    }

                    if (row < 0 || row > 2)
                    { Console.WriteLine("Invalid row. Please enter a valid row (0, 1, or 2)."); }
                }
                while (row < 0 || row > 2);

                do
                {
                    Console.Write("Enter column (0, 1, or 2): ");
                    try
                    { col = int.Parse(Console.ReadLine()); }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number.");
                        continue;
                    }

                    if (col < 0 || col > 2)
                    { Console.WriteLine("Invalid column. Please enter a valid column (0, 1, or 2)."); }
                }
                while (col < 0 || col > 2);

                if (board[row, col] == ' ')
                {
                    board[row, col] = currentPlayer;
                    validMove = true;
                }
                else
                {
                    Console.WriteLine("Invalid move. Try again.");
                }

            } while (!validMove);
        }

        static bool CheckWin()
        {
            //check for a win
            for (int i = 0; i <= 2; i++)
            {
                //check each row and if any row has the same value in all cells return true
                if (board[i, 0] == currentPlayer && board[i, 1] == currentPlayer && board[i, 2] == currentPlayer)
                    return true;
                //check each column and if any column has the same value in all cells return true
                if (board[0, i] == currentPlayer && board[1, i] == currentPlayer && board[2, i] == currentPlayer)
                    return true;
            }

            //check to see if either diagonal has the same value in all cells and return true if it does
            if ((board[0, 0] == currentPlayer && board[1, 1] == currentPlayer && board[2, 2] == currentPlayer) ||
                (board[0, 2] == currentPlayer && board[1, 1] == currentPlayer && board[2, 0] == currentPlayer))
                return true;

            return false;
        }

        static bool CheckDraw()
        {
            //check for draw condition and return true if all spots are full
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == ' ')
                        return false;
                }
            }
            return true;
        }

        static void InitializeBoard()
        {
            // Clear the board for a new game
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    board[i, j] = ' ';
                }
            }
        }

        static void UpdateWinHistory(string player1, string player2, string winner)
        {
            int winnersWins = 0;
            bool newWinner = true;
            string winHistoryFilePath = "win_history.csv";

            // Read all lines from the CSV file
            string[] lines = File.ReadAllLines(winHistoryFilePath);

            // Find winner's record (if exists)
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(winner + ","))
                {
                    newWinner = false;
                    string[] values = lines[i].Split(',');
                    try
                    {
                        winnersWins = int.Parse(values[1]);
                        lines[i] = winner + "," + ++winnersWins;
                        break;
                    }
                    //If the winners win count could not be read, set it to 1
                    catch
                    {
                        lines[i] = winner + ", 1";
                        break;
                    }
                }
            }

            if (newWinner)
            // "The winner doesn't have a record yet, so create a new record with 1 win
            {
                lines = lines.Append($"{winner}, 1").ToArray();
            }

            // Write updated records back to the CSV file
            File.WriteAllLines(winHistoryFilePath, lines);
        }
    }
}
