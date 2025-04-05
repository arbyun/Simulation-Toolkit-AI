using System;
using System.IO;
using System.Threading;
using SimToolAI.Core.Entities;
using SimToolAI.Core.Map;
using SimToolAI.Core.Rendering;
using SimToolAI.Core.Rendering.RenderStrategies;
using SimToolAI.Utilities;

namespace SimToolAI
{
    public class Program
    {
        internal static ConsoleMapParser<GridMap> mapParser;
        internal static GridMap map;
        internal static ConsoleScene scene;
        internal static Player player;
        
        private static void Main(string[] args)
        {
            Console.WriteLine("Console Demo");
            Console.WriteLine("----------------");
            
            Console.Clear();
            Console.Title = "SimToolAI Console Demo";

            if (args.Length > 0)
            {
                GetMap(Path.Exists(args[0]) ? args[0] : "NULL");
            }
            else
            {
                GetMap("NULL");
            }

            CreateEntities();
            DrawScene();
            MainLoop();
        }

        private static void CreateEntities()
        {
            player = new Player("player", 5, 2, 15);
            IRenderable playerAvatar = new ConsoleEntityRenderable(Convert.ToChar("@"), ConsoleColor.Yellow,
                ConsoleColor.Black, player);
            player.Avatar = playerAvatar;
        }

        private static void GetMap(string path)
        {
            mapParser = new ConsoleMapParser<GridMap>();
            map = mapParser.LoadMapFromFile(path);
            map.Initialize(new ConsoleMapRenderable(mapParser.GetMapGrid(),map.Height, map.Width));
        }

        private static void MainLoop()
        {
            bool running = true;

            Console.WriteLine("Use WASD to move player 1, Arrow keys to move player 2, Q to quit");
            Thread.Sleep(2000);

            map.ToggleFieldOfView(player);

            // Variables for tracking time
            DateTime lastUpdateTime = DateTime.Now;
            Direction lastDirection = Direction.Right; // Track the last direction for bullet firing

            // Set up a timer for continuous updates
            System.Timers.Timer updateTimer = new System.Timers.Timer(50); // 50ms = 20 updates per second
            updateTimer.Elapsed += (sender, e) =>
            {
                // Calculate deltaTime in seconds
                DateTime currentTime = DateTime.Now;
                float deltaTime = (float)(currentTime - lastUpdateTime).TotalSeconds;
                lastUpdateTime = currentTime;

                // Update the scene with the proper deltaTime
                scene.Update(deltaTime);

                // Only render if needed
                if ((bool)scene.QueryScene<object>("IsRenderRequired"))
                {
                    scene.Render();
                }
            };
            updateTimer.Start();

            while (running)
            {
                bool didPlayerAct = false;

                // Check if a key is available to avoid blocking
                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;

                    switch (key)
                    {
                        case ConsoleKey.Escape: case ConsoleKey.Q:
                            running = false;
                            break;
                        case ConsoleKey.UpArrow: case ConsoleKey.W:
                            didPlayerAct = CommandSystem.MovePlayer(Direction.Up, player, map);
                            lastDirection = Direction.Up;
                            break;
                        case ConsoleKey.DownArrow: case ConsoleKey.S:
                            didPlayerAct = CommandSystem.MovePlayer(Direction.Down, player, map);
                            lastDirection = Direction.Down;
                            break;
                        case ConsoleKey.LeftArrow: case ConsoleKey.A:
                            didPlayerAct = CommandSystem.MovePlayer(Direction.Left, player, map);
                            lastDirection = Direction.Left;
                            break;
                        case ConsoleKey.RightArrow: case ConsoleKey.D:
                            didPlayerAct = CommandSystem.MovePlayer(Direction.Right, player, map);
                            lastDirection = Direction.Right;
                            break;
                        case ConsoleKey.Spacebar:
                            // Create a new bullet at the player's position using the last direction
                            var bullet = new Bullet(player.X, player.Y, lastDirection, map, scene,50);
                            scene.AddEntity(bullet);
                            scene.QueryScene<bool>("SetRenderRequired", true);
                            break;
                    }

                    if (didPlayerAct)
                    {
                        scene.QueryScene<bool>("SetRenderRequired", true);
                    }
                }

                // Small delay to prevent CPU hogging
                Thread.Sleep(10);
            }

            // Stop the timer when exiting
            updateTimer.Stop();

            Console.Clear();
            Console.WriteLine("Demo ended. Thanks for playing!");
        }

        private static void DrawScene()
        {
            scene = new ConsoleScene(map);
            scene.AddEntity(player);
        }
    }
}
