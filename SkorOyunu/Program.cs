using System;
using System.IO;
using System.Threading;

class Program
{
    static int width = 30;
    static int height = 20;

    static int playerX = width / 2;
    static int playerY = height - 2;

    static int itemX = 0;
    static int itemY = 0;
    static char itemSymbol = '*';

    static int score = 0;
    static bool gameOver = false;

    static Random rnd = new Random();

    static void Log(string message)
    {
        try { File.AppendAllText("log.txt", message + "\n"); }
        catch { }
    }

    static void SpawnItem()
    {
        itemX = rnd.Next(0, width);
        itemY = 0;
        itemSymbol = rnd.Next(2) == 0 ? '*' : 'O';
        Log($"UPDATE → itemSpawned x={itemX} y={itemY}");
    }

    static void Draw(int remainingTime)
    {
        Console.Clear();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x == playerX && y == playerY)
                    Console.Write('@');
                else if (x == itemX && y == itemY)
                    Console.Write(itemSymbol);
                else
                    Console.Write(' ');
            }
            Console.WriteLine();
        }
        Console.WriteLine($"Score: {score}   Time Left: {remainingTime}s");
    }

    static void MovePlayer()
    {
        if (Console.KeyAvailable)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            Log($"INPUT → key={key.Key} playerX={playerX} playerY={playerY}");

            if (key.Key == ConsoleKey.LeftArrow) playerX--;
            if (key.Key == ConsoleKey.RightArrow) playerX++;
            if (key.Key == ConsoleKey.UpArrow) playerY--;
            if (key.Key == ConsoleKey.DownArrow) playerY++;

            playerX = Math.Max(0, Math.Min(playerX, width - 1));
            playerY = Math.Max(0, Math.Min(playerY, height - 1));

            Log($"MOVE → playerX={playerX} playerY={playerY}");
        }
    }

    static void MoveItem()
    {
        itemY++;
        Log($"UPDATE → itemMoved x={itemX} y={itemY}");
        if (itemY >= height) SpawnItem();
    }

    static void CheckCollision()
    {
        if (playerX == itemX && playerY == itemY)
        {
            score++;
            Log($"COLLISION → score={score}");
            SpawnItem();
        }
    }

    static void Main()
    {
        Console.CursorVisible = false;
        try
        {
            Console.SetWindowSize(width, height + 5);
            Console.SetBufferSize(width, height + 5);
        }
        catch { }

        SpawnItem();
        DateTime startTime = DateTime.Now;

        while (!gameOver)
        {
            int elapsed = (int)(DateTime.Now - startTime).TotalSeconds;
            int remainingTime = 60 - elapsed;
            if (remainingTime <= 0) { gameOver = true; break; }

            MovePlayer();
            MoveItem();
            CheckCollision();
            Draw(remainingTime);

            Thread.Sleep(120);
        }

        Log($"GAME OVER → finalScore={score}");
        Console.Clear();
        Console.WriteLine("Game Over!");
        Console.WriteLine("Final Score: " + score);
        Console.ReadKey();
    }
}