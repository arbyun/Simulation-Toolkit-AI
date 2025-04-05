using System;
using SimToolAI.Core.Entities;
using SimToolAI.Utilities;

namespace SimToolAI.Core.Rendering.RenderStrategies
{
    public class ConsoleEntityRenderable: IRenderable
    {
        private readonly Data _settings;
    
        public ConsoleEntityRenderable(Data settings)
        {
            _settings = settings;
        }
        
        public ConsoleEntityRenderable(char character, ConsoleColor foregroundColor, 
            ConsoleColor backgroundColor, Entity entity)
        {
            _settings = new Data();
            
            _settings.Set("char", character);
            _settings.Set("fcolor", foregroundColor);
            _settings.Set("bcolor", backgroundColor);
            _settings.Set("entity", entity);
        }

        public void Render()
        {
            char symbol = _settings.Get<char>("char");
            ConsoleColor foregroundColor = _settings.Get<ConsoleColor>("fcolor");
            ConsoleColor backgroundColor = _settings.Get<ConsoleColor>("bcolor");
            Entity entity = _settings.Get<Entity>("entity");
            
            Console.SetCursorPosition(entity.X, entity.Y);
            
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.Write(symbol);
            Console.ResetColor();
        }
    }
}