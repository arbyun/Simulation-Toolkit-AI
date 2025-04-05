using RogueSharp;
using SimToolAI.Core.Entities;
using SimToolAI.Core.Map;

namespace SimToolAI.Utilities
{
    public class CommandSystem
    {
        public static bool MovePlayer(Direction direction, Entity player, ISimMap map)
        {
            int x = player.X;
            int y = player.Y;

            switch (direction)
            {
                case Direction.Up:
                {
                    y = player.Y - 1;
                    break;
                }
                case Direction.Down:
                {
                    y = player.Y + 1;
                    break;
                }
                case Direction.Left:
                {
                    x = player.X - 1;
                    break;
                }
                case Direction.Right:
                {
                    x = player.X + 1;
                    break;
                }
                default:
                {
                    return false;
                }
            }

            if (map.SetEntityPosition(player, x, y))
            {
                return true;
            }

            return false;
        }
    
        public static void MoveAgent(Entity actor, ICell cell, ISimMap map)
        {
            map.SetEntityPosition(actor, cell.X, cell.Y);
        }
    }
}