using SimToolAI.Core.Entities;
using SimToolAI.Utilities;
using UnityEngine;

namespace SimToolAI.Core.Rendering.RenderStrategies
{
    /// <summary>
    /// Renderable strategy for Unity-based rendering (stub for future implementation)
    /// </summary>
    public class UnityEntityRenderable : RenderableBase
    {
        /// <summary>
        /// Creates a new Unity renderable with the specified settings
        /// </summary>
        /// <param name="settings">Settings for the renderable</param>
        public UnityEntityRenderable(Data settings) : base(settings)
        {
        }

        /// <summary>
        /// Creates a new Unity renderable with default settings
        /// </summary>
        public UnityEntityRenderable(Transform entityTransform, Entity entity) : base()
        {
            Settings.Set("transform", entityTransform);
            Settings.Set("entity", entity);
        }

        /// <summary>
        /// Renders the object using Unity
        /// TODO: Gotta find a better way to do this... Relying on unsure data in the settings is not ideal.
        /// </summary>
        public override void Render()
        {
            if (Settings.Has("grid") && Settings.Has("entity") && Settings.Has("transform"))
            {
                Grid grid = Settings.Get<Grid>("grid");
                Entity entity = Settings.Get<Entity>("entity");
                Transform transform = Settings.Get<Transform>("transform");
                
                Vector3 targetPosition = grid.GetCellCenterWorld(new Vector3Int(entity.X, entity.Y));
                
                float step = entity.Speed * Time.deltaTime;

                // Move towards the target position
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

                // Snap to exact position when very close
                if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                {
                    transform.position = targetPosition;
                }
            }
            
            if (Settings.Has("entity") && Settings.Has("transform"))
            {
                Transform transform = Settings.Get<Transform>("transform");
                Entity entity = Settings.Get<Entity>("entity");
                
                float targetRotation = 0f;
                
                switch (entity.FacingDirection)
                {
                    case Direction.Up:
                        targetRotation = 270f;  // Rotate 90 degrees counter-clockwise from right
                        break;
                    case Direction.Right:
                        targetRotation = 0f;   // No rotation (sprite starts facing right)
                        break;
                    case Direction.Down:
                        targetRotation = 90f; // Rotate 270 degrees counter-clockwise from right
                        break;
                    case Direction.Left:
                        targetRotation = 180f; // Rotate 180 degrees counter-clockwise from right
                        break;
                }

                // Smoothly rotate
                transform.rotation = Quaternion.Lerp(transform.rotation, 
                    Quaternion.Euler(0, 0, targetRotation), Time.deltaTime * 16f);
            }
        }

        public void Destroy()
        {
            if (Settings.Has("transform"))
            {
                Object.Destroy(Settings.Get<Transform>("transform").gameObject);
            }
        }
    }
}