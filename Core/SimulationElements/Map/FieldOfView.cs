using System.Collections.ObjectModel;

namespace SimArena.Core.SimulationElements.Map
{
    public class FieldOfView : FieldOfView<Cell>
    {
        public FieldOfView(IMap<Cell> map)
            : base(map)
        {
        }
    }

    public class FieldOfView<TCell> where TCell : ICell
    {
        private readonly IMap<TCell> _map;
        private readonly HashSet<int> _inFov;

        public FieldOfView(IMap<TCell> map)
        {
            _map = map;
            _inFov = new HashSet<int>();
        }

        internal FieldOfView(IMap<TCell> map, HashSet<int> inFov)
        {
            _map = map;
            _inFov = inFov;
        }

        public FieldOfView<TCell> Clone()
        {
            var inFov = new HashSet<int>();
            foreach (var num in _inFov)
                inFov.Add(num);
            return new FieldOfView<TCell>(_map, inFov);
        }

        public bool IsInFov(int x, int y)
        {
            return _inFov.Contains(_map.IndexFor(x, y));
        }

        public ReadOnlyCollection<TCell> ComputeFov(
            int xOrigin,
            int yOrigin,
            int radius,
            bool lightWalls)
        {
            ClearFov();
            return AppendFov(xOrigin, yOrigin, radius, lightWalls);
        }

        public ReadOnlyCollection<TCell> AppendFov(
            int xOrigin,
            int yOrigin,
            int radius,
            bool lightWalls)
        {
            foreach (var cell1 in _map.GetBorderCellsInSquare(xOrigin, yOrigin, radius))
            foreach (var cell2 in _map.GetCellsAlongLine(xOrigin, yOrigin, cell1.X, cell1.Y))
                if (Math.Abs(cell2.X - xOrigin) + Math.Abs(cell2.Y - yOrigin) <= radius)
                {
                    if (cell2.IsTransparent)
                    {
                        _inFov.Add(_map.IndexFor(cell2));
                    }
                    else
                    {
                        if (lightWalls) _inFov.Add(_map.IndexFor(cell2));
                        break;
                    }
                }
                else
                {
                    break;
                }

            if (lightWalls)
                foreach (var cell in _map.GetCellsInSquare(xOrigin, yOrigin, radius))
                    if (cell.X > xOrigin)
                    {
                        if (cell.Y > yOrigin)
                            PostProcessFovQuadrant(cell.X, cell.Y, Quadrant.SE);
                        else if (cell.Y < yOrigin)
                            PostProcessFovQuadrant(cell.X, cell.Y, Quadrant.NE);
                    }
                    else if (cell.X < xOrigin)
                    {
                        if (cell.Y > yOrigin)
                            PostProcessFovQuadrant(cell.X, cell.Y, Quadrant.SW);
                        else if (cell.Y < yOrigin)
                            PostProcessFovQuadrant(cell.X, cell.Y, Quadrant.NW);
                    }

            return CellsInFov();
        }

        private ReadOnlyCollection<TCell> CellsInFov()
        {
            var list = new List<TCell>();
            foreach (var index in _inFov)
                list.Add(_map.CellFor(index));
            return new ReadOnlyCollection<TCell>(list);
        }

        private void ClearFov()
        {
            _inFov.Clear();
        }

        private void PostProcessFovQuadrant(int x, int y, Quadrant quadrant)
        {
            var x1 = x;
            var y1 = y;
            var x2 = x;
            var y2 = y;
            switch (quadrant)
            {
                case Quadrant.NE:
                    y1 = y + 1;
                    x2 = x - 1;
                    break;
                case Quadrant.SE:
                    y1 = y - 1;
                    x2 = x - 1;
                    break;
                case Quadrant.SW:
                    y1 = y - 1;
                    x2 = x + 1;
                    break;
                case Quadrant.NW:
                    y1 = y + 1;
                    x2 = x + 1;
                    break;
            }

            if (IsInFov(x, y) || _map.IsTransparent(x, y) || ((!_map.IsTransparent(x1, y1) || !IsInFov(x1, y1)) &&
                                                              (!_map.IsTransparent(x2, y2) || !IsInFov(x2, y2)) &&
                                                              (!_map.IsTransparent(x2, y1) || !IsInFov(x2, y1))))
                return;
            _inFov.Add(_map.IndexFor(x, y));
        }

        private enum Quadrant
        {
            NE = 1,
            SE = 2,
            SW = 3,
            NW = 4
        }
    }
}