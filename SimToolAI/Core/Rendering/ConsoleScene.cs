using System;
using System.Linq;
using SimToolAI.Core.Entities;
using SimToolAI.Core.Map;

namespace SimToolAI.Core.Rendering
{
    public class ConsoleScene: Scene
    {
        private bool _renderRequired = true;
        
        public ConsoleScene(ISimMap map) : base(map)
        {
        }

        public override void Update(float deltaTime)
        {
            foreach (Entity entity in Entities.ToList())
            {
                entity.Update(deltaTime);
            }
        }

        public override void Render()
        {
            if (!_renderRequired)
                return;

            // Clear the console to remove any old entities
            Console.Clear();

            // Render the map
            Map.Renderable.Render();

            // Render all entities
            foreach (Entity entity in Entities)
            {
                entity.Avatar.Render();
            }

            // Reset the render flag
            _renderRequired = false;
        }

        public override T QueryScene<T>(string query, params object[] parameters)
        {
            switch (query)
            {
                case "GetMap":
                    return (T)Map;
                case "SetRenderRequired":
                    _renderRequired = (bool)parameters[0];
                    return default;
                case "IsRenderRequired":
                    return (T)(object)_renderRequired;
                default:
                    throw new ArgumentException($"Unknown query: {query}");
            }
        }
    }
}