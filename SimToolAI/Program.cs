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
    /// <summary>
    /// Main program class for the SimToolAI demo
    /// </summary>
    public static class Program
    {
        #region Static Fields

        /// <summary>
        /// Map parser for loading maps from files
        /// </summary>
        private static ConsoleMapParser<GridMap> _mapParser;

        /// <summary>
        /// Current map
        /// </summary>
        private static GridMap _map;

        /// <summary>
        /// Current scene
        /// </summary>
        private static ConsoleScene _scene;

        /// <summary>
        /// Player entity
        /// </summary>
        private static Player _player;

        /// <summary>
        /// Game timer for continuous updates
        /// </summary>
        private static System.Timers.Timer _updateTimer;

        /// <summary>
        /// Last update time for calculating delta time
        /// </summary>
        private static DateTime _lastUpdateTime;

        /// <summary>
        /// Whether the game is running
        /// </summary>
        private static bool _running;

        #endregion

        #region Main Method

        /// <summary>
        /// Entry point for the application
        /// </summary>
        /// <param name="args">Command line arguments</param>
        private static void Main(string[] args)
        {
            try
            {
                InitializeGame(args);
                RunGameLoop();
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            finally
            {
                CleanupGame();
            }
        }

        #endregion

        #region Game Initialization

        /// <summary>
        /// Initializes the game
        /// </summary>
        /// <param name="args">Command line arguments</param>
        private static void InitializeGame(string[] args)
        {
            Console.WriteLine("SimToolAI Console Demo");
            Console.WriteLine("----------------------");

            Console.Clear();
            Console.Title = "SimToolAI Console Demo";

            // Load the map
            if (args.Length > 0)
            {
                LoadMap(Path.Exists(args[0]) ? args[0] : "NULL");
            }
            else
            {
                LoadMap("NULL");
            }

            // Create entities and set up the scene
            CreateEntities();
            SetupScene();

            // Initialize the update timer
            _lastUpdateTime = DateTime.Now;
            _updateTimer = new System.Timers.Timer(50); // 50ms = 20 updates per second
            _updateTimer.Elapsed += OnUpdateTimerElapsed;

            // Display instructions
            DisplayInstructions();
        }

        /// <summary>
        /// Loads a map from a file
        /// </summary>
        /// <param name="path">Path to the map file</param>
        private static void LoadMap(string path)
        {
            _mapParser = new ConsoleMapParser<GridMap>();
            _map = _mapParser.LoadMapFromFile(path);
            _map.Initialize(new ConsoleMapRenderable(_mapParser.GetMapGrid(), _map.Height, _map.Width));
        }

        /// <summary>
        /// Creates game entities
        /// </summary>
        private static void CreateEntities()
        {
            // Create the player
            _player = new Player("Player", 5, 2, 15)
            {
                Health = 100,
                MaxHealth = 100,
                AttackPower = 10,
                Defense = 5,
                Speed = 1.0f,
                FacingDirection = Direction.Right
            };

            // Create the player's avatar
            _player.Avatar = new ConsoleEntityRenderable('@', ConsoleColor.Yellow, ConsoleColor.Black, _player);
        }

        /// <summary>
        /// Sets up the scene
        /// </summary>
        private static void SetupScene()
        {
            _scene = new ConsoleScene(_map);
            _scene.AddEntity(_player);
            _map.ToggleFieldOfView(_player);
        }

        /// <summary>
        /// Displays game instructions
        /// </summary>
        private static void DisplayInstructions()
        {
            Console.WriteLine("Controls:");
            Console.WriteLine("WASD / Arrow Keys: Move player");
            Console.WriteLine("Spacebar: Fire bullet");
            Console.WriteLine("Q / Escape: Quit");
            Thread.Sleep(2000);
        }

        #endregion

        #region Game Loop

        /// <summary>
        /// Runs the main game loop
        /// </summary>
        private static void RunGameLoop()
        {
            _running = true;
            _updateTimer.Start();

            while (_running)
            {
                ProcessInput();
                Thread.Sleep(10); // Small delay to prevent CPU hogging
            }
        }

        /// <summary>
        /// Processes user input
        /// </summary>
        private static void ProcessInput()
        {
            // Check if a key is available to avoid blocking
            if (!Console.KeyAvailable)
                return;

            ConsoleKey key = Console.ReadKey(true).Key;
            bool didPlayerAct = false;

            switch (key)
            {
                case ConsoleKey.Escape:
                case ConsoleKey.Q:
                    _running = false;
                    break;

                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Up, _player, _map);
                    break;

                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Down, _player, _map);
                    break;

                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Left, _player, _map);
                    break;

                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Right, _player, _map);
                    break;

                case ConsoleKey.Spacebar:
                    // Fire a bullet using the command system
                    CommandSystem.FireBullet(_player, _scene, 50);
                    didPlayerAct = true;
                    break;
            }

            if (didPlayerAct)
            {
                _scene.QueryScene<bool>("SetRenderRequired", true);
            }
        }

        /// <summary>
        /// Called when the update timer elapses
        /// </summary>
        private static void OnUpdateTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Calculate deltaTime in seconds
            DateTime currentTime = DateTime.Now;
            float deltaTime = (float)(currentTime - _lastUpdateTime).TotalSeconds;
            _lastUpdateTime = currentTime;

            // Update the scene with the proper deltaTime
            _scene.Update(deltaTime);

            // Only render if needed
            if ((bool)_scene.QueryScene<object>("IsRenderRequired"))
            {
                _scene.Render();
            }
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Cleans up game resources
        /// </summary>
        private static void CleanupGame()
        {
            // Stop the update timer
            if (_updateTimer != null)
            {
                _updateTimer.Stop();
                _updateTimer.Dispose();
            }

            // Display exit message
            Console.Clear();
            Console.WriteLine("Demo ended. Thanks for playing!");
        }

        #endregion
    }
}
