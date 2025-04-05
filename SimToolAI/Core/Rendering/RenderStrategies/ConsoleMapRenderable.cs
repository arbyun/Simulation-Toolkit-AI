using System;
using SimToolAI.Utilities;

namespace SimToolAI.Core.Rendering.RenderStrategies
{
    public class ConsoleMapRenderable: IRenderable
    {
        private readonly Data _settings;

        public ConsoleMapRenderable(Data settings)
        {
            _settings = settings;
        }
        
        public ConsoleMapRenderable(char[,] mapGrid, int height, int width)
        {
            _settings = new Data();
            _settings.Set("map", mapGrid);
            _settings.Set("height", height);
            _settings.Set("width", width);
        }

        public void Render()
        {
            char[,] mapGrid = _settings.Get<char[,]>("map");
            int height = _settings.Get<int>("height");
            int width = _settings.Get<int>("width");
            
            Console.Clear();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.SetCursorPosition(x,y);
                    
                    if (mapGrid[x, y] == '#') Console.ForegroundColor = ConsoleColor.DarkGray;
                    else if (mapGrid[x, y] == '.') Console.ForegroundColor = ConsoleColor.Green;
                    else if (mapGrid[x, y] == '&') Console.ForegroundColor = ConsoleColor.DarkMagenta;

                    Console.Write(mapGrid[x, y]);
                    Console.ResetColor();
                }
            }
        }
    }
}