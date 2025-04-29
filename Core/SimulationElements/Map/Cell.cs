using SimArena.Core.Entities;

namespace SimArena.Core.SimulationElements.Map
{
    public class Cell : ICell
    {
        public Cell()
        {
        }

        public Cell(int x, int y, bool isTransparent, bool isWalkable)
        {
            X = x;
            Y = y;
            IsTransparent = isTransparent;
            IsWalkable = isWalkable;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public bool IsTransparent { get; set; }

        public bool IsWalkable { get; set; }
        public Entity Entity { get; set; }

        public override string ToString()
        {
            return IsWalkable ? (IsTransparent ? "." : "s") : (IsTransparent ? "o" : "#");
        }

        public bool Equals(ICell? other)
        {
            if (other == null)
                return false;
            if (this == (object) other)
                return true;
            return X == other.X && Y == other.Y && IsTransparent == other.IsTransparent && IsWalkable == other.IsWalkable;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == (object) GetType() && Equals((ICell) obj);
        }

        public static bool operator ==(Cell left, Cell right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Cell left, Cell right)
        {
            return !Equals(left, right);
        }

        public override int GetHashCode()
        {
            return ((X * 397 ^ Y) * 397 ^ IsTransparent.GetHashCode()) * 397 ^ IsWalkable.GetHashCode();
        }
    }
}