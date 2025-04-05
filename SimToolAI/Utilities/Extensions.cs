using System;

namespace SimToolAI.Utilities
{
    public static class Extensions
    {
        public static Tuple<int,int> ToVector(this Direction dir)
        {
            switch (dir)
            {
                case Direction.None:
                    return new Tuple<int, int>(0, 0);
                case Direction.DownLeft:
                    return new Tuple<int, int>(-1, 1);
                case Direction.Down:
                    return new Tuple<int, int>(0, 1);
                case Direction.DownRight:
                    return new Tuple<int, int>(1, 1);
                case Direction.Left:
                    return new Tuple<int, int>(-1, 0);
                case Direction.Center:
                    return new Tuple<int, int>(0, 0);
                case Direction.Right:
                    return new Tuple<int, int>(1, 0);
                case Direction.UpLeft:
                    return new Tuple<int, int>(-1, -1);
                case Direction.Up:
                    return new Tuple<int, int>(0, -1);
                case Direction.UpRight:
                    return new Tuple<int, int>(1, -1);
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }
    }
}