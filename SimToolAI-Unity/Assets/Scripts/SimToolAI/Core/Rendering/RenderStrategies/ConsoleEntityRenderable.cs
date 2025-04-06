using System;
using SimToolAI.Core.Entities;
using SimToolAI.Utilities;

namespace SimToolAI.Core.Rendering.RenderStrategies
{
    /// <summary>
    /// Renderable strategy for console-based entity rendering
    /// </summary>
    public class ConsoleEntityRenderable : RenderableBase
    {
        /// <summary>
        /// Gets the rendering priority (entities are rendered after maps)
        /// </summary>
        public override int RenderPriority => 10;

        /// <summary>
        /// Creates a new console entity renderable with the specified settings
        /// </summary>
        /// <param name="settings">Settings for the renderable</param>
        public ConsoleEntityRenderable(Data settings) : base(settings)
        {
        }

        /// <summary>
        /// Creates a new console entity renderable with the specified parameters
        /// </summary>
        /// <param name="character">Character to represent the entity</param>
        /// <param name="foregroundColor">Foreground color of the character</param>
        /// <param name="backgroundColor">Background color of the character</param>
        /// <param name="entity">Entity to render</param>
        public ConsoleEntityRenderable(char character, ConsoleColor foregroundColor,
            ConsoleColor backgroundColor, Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Settings.Set("char", character);
            Settings.Set("fcolor", foregroundColor);
            Settings.Set("bcolor", backgroundColor);
            Settings.Set("entity", entity);
        }

        /// <summary>
        /// Renders the entity to the console
        /// </summary>
        public override void Render()
        {
            if (!NeedsRendering)
                return;

            char symbol = Settings.Get<char>("char");
            ConsoleColor foregroundColor = Settings.Get<ConsoleColor>("fcolor");
            ConsoleColor backgroundColor = Settings.Get<ConsoleColor>("bcolor");
            Entity entity = Settings.Get<Entity>("entity");

            if (entity == null)
                return;

            try
            {
                // Only render if the entity is within the console buffer
                if (entity.X >= 0 && entity.Y >= 0 &&
                    entity.X < Console.BufferWidth && entity.Y < Console.BufferHeight)
                {
                    Console.SetCursorPosition(entity.X, entity.Y);

                    Console.ForegroundColor = foregroundColor;
                    Console.BackgroundColor = backgroundColor;
                    Console.Write(symbol);
                    Console.ResetColor();
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