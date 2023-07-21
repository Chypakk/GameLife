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
            SetSettings(20);

            while (Settings.StartGame)
            {
                Console.SetCursorPosition(Settings.Width / 2 - 10, Settings.Height / 2 - 5);
                Console.Write("Введите размер поля: ");
                var size = Console.ReadLine();
                Console.Clear();

                if (int.Parse(size) >= 10 && int.Parse(size) <= 63)
                {
                    SetSettings(int.Parse(size));

                    Console.CursorVisible = false;

                    Console.SetCursorPosition(Settings.Width / 2 - 10, Settings.Height / 2 - 5);
                    Console.Write("Выберите режим:");

                    Console.SetCursorPosition(Settings.Width / 2 - 15, Settings.Height / 2 - 4);
                    Console.Write("1 - Обычные правила (S3_B23)");

                    Console.SetCursorPosition(Settings.Width / 2 - 15, Settings.Height / 2 - 3);
                    Console.Write("2 - \"День и Ночь\" (B3678_S34678)");

                    var mode = Console.ReadKey();

                    switch (mode.KeyChar)
                    {
                        case '1':

                            Game_B3_S23 game = new Game_B3_S23();
                            Load();
                            GameFrame(game);

                            break;

                        case '2':

                            Game_B3678_S34678 gameDay = new Game_B3678_S34678();
                            Load();
                            GameFrame(gameDay);

                            break;

                        default:
                            break;
                    }

                }

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
            Console.Clear();

            int left = Settings.Width / 2;
            int i = 0;
            int sleep = 100;
            int count = 3; // end - 5

            Console.SetCursorPosition(left - 4, Settings.Height / 2 - 5);
            Console.Write("Загрузка:");

            while (true)
            {
                Console.SetCursorPosition(left, Settings.Height / 2 - 4);
                Console.Write('.');

                Thread.Sleep(50);
                Console.CursorVisible = false;

                Console.SetCursorPosition(left, Settings.Height / 2 - 4);
                Console.Write('.');

                Thread.Sleep(sleep);
                Console.CursorVisible = false;

                i++;

                left = i % 2 == 0 ? left += i : left -= i;

                if (i == 11)
                {
                    Console.Clear();
                    i = 0;

                    left = Settings.Width / 2;

                    Console.SetCursorPosition(left - 4, Settings.Height / 2 - 5);
                    Console.ForegroundColor = GetRandomColor();
                    Console.Write("Загрузка:");
                    count++;
                }
                if (count == 5)
                {
                    Settings.FirstLaunch = true;
                    Console.Clear();

                    Console.SetCursorPosition(left - 3, Settings.Height / 2 - 5);

                    //string str = "Начинаем";
                    //foreach (var ch in str)
                    //{
                    //    Console.Write(ch);
                    //    Thread.Sleep(50);
                    //}

                    Thread.Sleep(500);

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Clear();

                    break;
                }
            }


        }

        private static void GameFrame(Game game)
        {
            game.DrawBoard();
            Thread.Sleep(1000);
            while (true)
            {
                game.Frame();
                Thread.Sleep(125);
                if (Settings.Finish)
                {
                    game.EndGame();
                    break;
                }
            }

            var repeat = Console.ReadKey();
            if (repeat.KeyChar == '2')
            {
                Settings.StartGame = false;
            }
            Console.Clear();
        }

        private static void SetSettings(int size)
        {
            int width = size * 2 + 40;
            int height = size;

            var oldWidth = Console.WindowWidth;

            if (oldWidth > width)
            {
                Console.SetWindowSize(width, height);
                Console.SetBufferSize(width, height);
            }
            else
            {
                Console.SetBufferSize(width, height);
                Console.SetWindowSize(width, height);
            }

            Console.CursorVisible = false;

            Settings.FirstLaunch = false;
            Settings.StartGame = true;
            Settings.Size = size;
            Settings.Width = width;
            Settings.Height = height;
        }

    }

    public static class Settings
    {
        public static bool FirstLaunch { get; set; }

        public static bool StartGame { get; set; }

        public static bool Finish { get; set; }

        public static int Size { get; set; }

        public static int Height { get; set; }

        public static int Width { get; set; }
    }

    public abstract class Game
    {
        internal int height = Settings.Size;
        internal int width = Settings.Size * 2;

        internal int count = 0;
        internal int generation = 0;

        internal Status[,] gameBoard;
        internal Status[,] oldBoard;
        internal Status[,] olderBoard;

        internal Random rnd = new Random();

        public Game()
        {
            gameBoard = new Status[height, width];
            Init();
        }

        private void Init()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    gameBoard[i, j] = rnd.Next(1, 3) % 2 == 0 ? Status.Live : Status.Dead;
                }
            }

            count = 0;
            oldBoard = new Status[height, width];
            olderBoard = new Status[height, width];
            Settings.Finish = false;
        }

        public void EndGame()
        {
            //Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(Settings.Width / 2 - 5, Settings.Height / 2 - 5);
            string str = "Игра закончена.";
            foreach (var ch in str)
            {
                Console.Write(ch);
                Thread.Sleep(50);
            }
            Thread.Sleep(500);

            Console.SetCursorPosition(Settings.Width / 2 - 11, Settings.Height / 2 - 4);
            Console.Write("Сгенерировать всё ещё раз?");
            Console.SetCursorPosition(Settings.Width / 2 - 7, Settings.Height / 2 - 3);
            Console.Write("1 - Да\t2 - Нет");

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(Settings.Width / 2 - 8, Settings.Height / 2 - 1);
            Console.Write($"Всего поколений: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(GetGeneration() - 1);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public int GetGeneration()
        {
            return generation;
        }

        public void DrawBoard()
        {
            string board = "";
            for (int i = 0; i < height; i++)
            {
                //Console.SetCursorPosition(0, /*(*/i /*+ 1) * 2*/);
                for (int j = 0; j < width; j++)
                {
                    if (gameBoard[i, j] == Status.Dead)
                    {
                        //Console.ForegroundColor = ConsoleColor.Gray;
                        //Console.Write("-");
                        board += " ";
                    }
                    else
                    {
                        //Console.ForegroundColor = ConsoleColor.Green;
                        //Console.Write("*");
                        board += "*";
                    }
                }
                board += "\n";
            }

            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(board);

            Console.SetCursorPosition(Settings.Width - 20, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Generation: ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(generation);

            generation++;
        }

        internal int GetLiveAroundCell(int x, int y)
        {
            int result = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (x + i < 0 || x + i >= height)
                    {
                        continue;
                    }
                    if (y + j < 0 || y + j >= width)
                    {
                        continue;
                    }
                    if (x + i == x && y + j == y)
                    {
                        continue;
                    }

                    if (gameBoard[x + i, j + y] == Status.Live)
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        internal Status ReverseStatus(Status status)
        {
            return status == Status.Dead ? Status.Live : Status.Dead;
        }

        public abstract void Frame();

    }

    public class Game_B3_S23 : Game
    {
        public override void Frame()
        {
            Status[,] newBoard = new Status[height, width];
            oldBoard = gameBoard;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var currentCell = gameBoard[i, j];

                    var countLive = GetLiveAroundCell(i, j);

                    //check rules
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

            if (generation % 2 == 0)
            {
                olderBoard = gameBoard;
            }

            if (count == 1)
            {
                Settings.Finish = true;
            }

            Console.Clear();
            DrawBoard();

        }

        private void CheckBoard()
        {
            int olderCount = 0;
            int oldCount = 0;
            int size = height * width;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (gameBoard[i, j] != oldBoard[i, j])
                    {
                        oldCount++;
                    }
                    if (gameBoard[i, j] == olderBoard[i, j])
                    {
                        olderCount++;
                    }
                }
            }
            if (olderCount == size || oldCount == size)
            {
                count++;
                return;
            }

        }

    }

    public class Game_B3678_S34678 : Game
    {

        string countForBirth = "3, 6, 7, 8 ";
        string countForSurv = "3, 4, 6, 7, 8 ";

        public override void Frame()
        {
            Status[,] newBoard = new Status[height, width];
            oldBoard = gameBoard;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var currentCell = gameBoard[i, j];

                    var countLive = GetLiveAroundCell(i, j);

                    //check rules
                    if (currentCell == Status.Dead && countForBirth.Contains(countLive.ToString()))
                    {
                        newBoard[i, j] = ReverseStatus(currentCell);
                    }
                    else if (currentCell == Status.Live && !countForSurv.Contains(countLive.ToString()))
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

            if (generation % 2 == 0)
            {
                olderBoard = gameBoard;
            }

            if (count == 10)
            {
                Settings.Finish = true;
            }

            Console.Clear();
            DrawBoard();
        }

        private void CheckBoard()
        {
            int olderCount = 0;
            int oldCount = 0;
            int size = height * width;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (gameBoard[i, j] != oldBoard[i, j])
                    {
                        oldCount++;
                    }
                    if (gameBoard[i, j] == olderBoard[i, j])
                    {
                        olderCount++;
                    }
                }
            }
            if (olderCount == size || oldCount == size)
            {
                count++;
                return;
            }
        }

    }

    public enum Status
    {
        Live = 1,
        Dead = 2
    }
}
