using System;
using SimToolAI.Utilities;

namespace SimToolAI.Core.Rendering.RenderStrategies
{
    /// <summary>
    /// Renderable strategy for console-based map rendering
    /// </summary>
    public class ConsoleMapRenderable : RenderableBase
    {
        /// <summary>
        /// Gets the rendering priority (maps are rendered before entities)
        /// </summary>
        public override int RenderPriority => 0;

        /// <summary>
        /// Creates a new console map renderable with the specified settings
        /// </summary>
        /// <param name="settings">Settings for the renderable</param>
        public ConsoleMapRenderable(Data settings) : base(settings)
        {
        }

        /// <summary>
        /// Creates a new console map renderable with the specified parameters
        /// </summary>
        /// <param name="mapGrid">2D array of characters representing the map</param>
        /// <param name="height">Height of the map</param>
        /// <param name="width">Width of the map</param>
        public ConsoleMapRenderable(char[,] mapGrid, int height, int width)
        {
            if (mapGrid == null)
                throw new ArgumentNullException(nameof(mapGrid));

            if (height <= 0 || width <= 0)
                throw new ArgumentException("Height and width must be positive");

            Settings.Set("map", mapGrid);
            Settings.Set("height", height);
            Settings.Set("width", width);
        }

        /// <summary>
        /// Renders the map to the console
        /// </summary>
        public override void Render()
        {
            if (!NeedsRendering)
                return;

            char[,] mapGrid = Settings.Get<char[,]>("map");
            int height = Settings.Get<int>("height");
            int width = Settings.Get<int>("width");

            if (mapGrid == null)
                return;

            try
            {
                Console.Clear();

                // Determine the maximum dimensions to render based on console buffer size
                int maxWidth = Math.Min(width, Console.BufferWidth);
                int maxHeight = Math.Min(height, Console.BufferHeight);

                for (int y = 0; y < maxHeight; y++)
                {
                    for (int x = 0; x < maxWidth; x++)
                    {
                        Console.SetCursorPosition(x, y);

                        // Set the appropriate color based on the map cell
                        Console.ForegroundColor = mapGrid[x, y] switch
                        {
                            '#' => // Wall
                                ConsoleColor.DarkGray,
                            '.' => // Floor
                                ConsoleColor.Green,
                            '&' => // Door
                                ConsoleColor.DarkMagenta,
                            'O' => // Window
                                ConsoleColor.Cyan,
                            _ => ConsoleColor.White
                        };

                        Console.Write(mapGrid[x, y]);
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                // Ignore exceptions related to console buffer size changes
                if (ex is not (ArgumentOutOfRangeException or System.IO.IOException))
                    throw;
            }

            MarkAsRendered();
        }
    }
}