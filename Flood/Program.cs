using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Flood
{
    class Program
    {
        static void Main(string[] args)
        {
            SetSettings();
            bool startGame = true;
            while (startGame)
            {
                Load();

                Game game = new Game();
                game.DrawBoard();
                Thread.Sleep(2000);
                while (true)
                {
                    game.Frame();
                    Thread.Sleep(1000);
                    if (Settings.Finish)
                    {
                        EndGame();
                        break;
                    }
                }
                Console.SetCursorPosition(35, 13);
                Console.Write("Сгенерировать всё ещё раз?");
                Console.SetCursorPosition(39, 14);
                Console.Write("1 - Да\t2 - Нет");
                var repeat = Console.ReadKey();
                if (repeat.KeyChar == '2')
                {
                    startGame = false;
                }
                Console.Clear();
            }

        }

        private static ConsoleColor GetRandomColor()
        {
            Random rnd = new Random();
            var colorNum = rnd.Next(0, 12);

            switch (colorNum)
            {
                case 1:
                    return ConsoleColor.Red;

                case 2:
                    return ConsoleColor.Yellow;

                case 3:
                    return ConsoleColor.Green;

                case 4:
                    return ConsoleColor.Cyan;

                case 5:
                    return ConsoleColor.Gray;

                case 6:
                    return ConsoleColor.Blue;

                case 7:
                    return ConsoleColor.Magenta;

                case 8:
                    return ConsoleColor.DarkBlue;

                case 9:
                    return ConsoleColor.DarkCyan;

                case 10:
                    return ConsoleColor.DarkRed;

                case 11:
                    return ConsoleColor.DarkMagenta;

                case 12:
                    return ConsoleColor.DarkYellow;

                default:
                    return ConsoleColor.White;
            }
        }

        private static void Load()
        {
            int left = 50;
            int i = 0;
            int sleep = 100;
            int count = 3; // end - 5

            Console.SetCursorPosition(left - 4, 12);
            Console.Write("Загрузка:");

            while (true)
            {
                Console.SetCursorPosition(left, 13);
                Console.Write('.');

                Thread.Sleep(50);
                Console.CursorVisible = false;

                Console.SetCursorPosition(left, 13);
                Console.Write('.');

                Thread.Sleep(sleep);
                Console.CursorVisible = false;

                i++;

                left = i % 2 == 0 ? left += i : left -= i;

                if (i == 11)
                {
                    Console.Clear();
                    i = 0;

                    left = 50;

                    Console.SetCursorPosition(left - 4, 12);
                    Console.ForegroundColor = GetRandomColor();
                    Console.Write("Загрузка:");
                    count++;
                }
                if (count == 5)
                {
                    Settings.FirstLaunch = true;
                    Console.Clear();

                    Console.SetCursorPosition(left - 3, 12);
                    string str = "Начинаем";
                    foreach (var ch in str)
                    {
                        Console.Write(ch);
                        Thread.Sleep(50);
                    }
                    Thread.Sleep(500);

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Clear();

                    break;
                }
            }


        }

        private static void EndGame()
        {
            Console.Clear();
            Console.SetCursorPosition(40, 12);
            string str = "Игра закончена.";
            foreach (var ch in str)
            {
                Console.Write(ch);
                Thread.Sleep(300);
            }
            Thread.Sleep(500);
        }

        private static void SetSettings()
        {
            Console.SetWindowSize(100, 30);
            Console.SetBufferSize(100, 30);
            Settings.FirstLaunch = false;
        }

    }

    public static class Settings
    {
        public static bool FirstLaunch { get; set; }

        public static bool Finish { get; set; }
    }

    public class Game
    {
        int size = 5;
        int count = 0;

        Status[,] gameBoard;
        Status[,] oldBoard;

        Random rnd = new Random();

        public Game()
        {
            gameBoard = new Status[size, size];
            Init();
        }

        public void Init()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    gameBoard[i, j] = rnd.Next(1, 3) % 2 == 0 ? Status.Live : Status.Dead;
                }
            }
            count = 0;
            oldBoard = new Status[size, size];
            Settings.Finish = false;
        }

        public void DrawBoard()
        {

            for (int i = 0; i < size; i++)
            {
                Console.SetCursorPosition(0, (i + 1) * 2);
                for (int j = 0; j < size; j++)
                {
                    if (gameBoard[i, j] == Status.Dead)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("-\t");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("*\t");
                    }
                }
            }

        }

        public void Frame()
        {
            Status[,] newBoard = new Status[size, size];
            oldBoard = gameBoard;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var currentCell = gameBoard[i, j];

                    var aroundCells = GetAroundCell(i, j);

                    int countDead = 0;
                    int countLive = 0;

                    foreach (var item in aroundCells)
                    {
                        int.TryParse(item.Key.Split(',')[0], out int x);
                        int.TryParse(item.Key.Split(',')[1], out int y);

                        if (gameBoard[x, y] == Status.Dead)
                        {
                            countDead++;
                        }
                        else
                        {
                            countLive++;
                        }

                    }

                    if (currentCell == Status.Dead && countLive == 3)
                    {
                        newBoard[i, j] = ReverseStatus(currentCell);
                    }
                    else if (currentCell == Status.Live && (countLive < 2 || countLive > 3))
                    {
                        newBoard[i, j] = ReverseStatus(currentCell);
                    }
                    else
                    {
                        newBoard[i, j] = currentCell;
                    }

                }
            }

            gameBoard = newBoard;
            CheckBoard();

            if (count == 2)
            {
                Settings.Finish = true;
            }

            Console.Clear();
            DrawBoard();

        }

        private Dictionary<string, Status> GetAroundCell(int i, int j)
        {
            var result = new Dictionary<string, Status>();

            //previous row
            if (i - 1 >= 0)
            {
                if (j - 1 >= 0)
                {
                    result.Add($"{i - 1}, {j - 1}", gameBoard[i - 1, j - 1]);
                }

                if (j + 1 < size)
                {
                    result.Add($"{i - 1}, {j + 1}", gameBoard[i - 1, j + 1]);
                }

                result.Add($"{i - 1}, {j}", gameBoard[i - 1, j]);
            }

            //next row
            if (i + 1 < size)
            {
                if (j - 1 >= 0)
                {
                    result.Add($"{i + 1}, {j - 1}", gameBoard[i + 1, j - 1]);
                }

                if (j + 1 < size)
                {
                    result.Add($"{i + 1}, {j + 1}", gameBoard[i + 1, j + 1]);
                }

                result.Add($"{i + 1}, {j}", gameBoard[i + 1, j]);
            }

            if (j - 1 >= 0)
            {
                result.Add($"{i}, {j - 1}", gameBoard[i, j - 1]);
            }

            if (j + 1 < size)
            {
                result.Add($"{i}, {j + 1}", gameBoard[i, j + 1]);
            }

            return result;
        }

        private Status ReverseStatus(Status status)
        {
            return status == Status.Dead ? Status.Live : Status.Dead;
        }

        private void CheckBoard()
        {

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (gameBoard[i, j] != oldBoard[i, j])
                    {
                        return;
                    }
                }
            }

            count++;
        }
    }

    public enum Status
    {
        Live,
        Dead
    }
}
