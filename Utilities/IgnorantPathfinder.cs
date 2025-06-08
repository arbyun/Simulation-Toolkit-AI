using System;
using System.Collections.Generic;
using System.Linq;
using RogueSharp;
using RogueSharp.Algorithms;

namespace SimArena.Utilities
{
    public class IgnorantPathfinder
    {
        private readonly EdgeWeightedDigraph _graph;
        private readonly IMap _map;

        public IgnorantPathfinder(IMap map, ICell[] cellsToIgnore)
        {
            _map = map != null ? map : throw new ArgumentNullException(nameof(map), "Map cannot be null");
            _graph = new EdgeWeightedDigraph(_map.Width * _map.Height);

            foreach (var allCell in _map.GetAllCells())
                
                if (allCell.IsWalkable || cellsToIgnore.Contains(allCell))
                {
                    var num1 = IndexFor(allCell);
                    foreach (var cell in _map.GetBorderCellsInDiamond(allCell.X, allCell.Y, 1))
                        if (cell.IsWalkable)
                        {
                            var num2 = IndexFor(cell);
                            _graph.AddEdge(new DirectedEdge(num1, num2, 1.0));
                            _graph.AddEdge(new DirectedEdge(num2, num1, 1.0));
                        }
                }
        }

        public IgnorantPathfinder(IMap map)
        {
            _map = map != null ? map : throw new ArgumentNullException(nameof(map), "Map cannot be null");
            _graph = new EdgeWeightedDigraph(_map.Width * _map.Height);
            foreach (var allCell in _map.GetAllCells())
                if (allCell.IsWalkable)
                {
                    var num1 = IndexFor(allCell);
                    foreach (var cell in _map.GetBorderCellsInDiamond(allCell.X, allCell.Y, 1))
                        if (cell.IsWalkable)
                        {
                            var num2 = IndexFor(cell);
                            _graph.AddEdge(new DirectedEdge(num1, num2, 1.0));
                            _graph.AddEdge(new DirectedEdge(num2, num1, 1.0));
                        }
                }
        }

        public IgnorantPathfinder(IMap map, double diagonalCost)
        {
            _map = map != null ? map : throw new ArgumentNullException(nameof(map), "Map cannot be null");
            _graph = new EdgeWeightedDigraph(_map.Width * _map.Height);
            foreach (var allCell in _map.GetAllCells())
                if (allCell.IsWalkable)
                {
                    var num1 = IndexFor(allCell);
                    foreach (var cell in _map.GetBorderCellsInSquare(allCell.X, allCell.Y, 1))
                        if (cell.IsWalkable)
                        {
                            var num2 = IndexFor(cell);
                            if (cell.X != allCell.X && cell.Y != allCell.Y)
                            {
                                _graph.AddEdge(new DirectedEdge(num1, num2, diagonalCost));
                                _graph.AddEdge(new DirectedEdge(num2, num1, diagonalCost));
                            }
                            else
                            {
                                _graph.AddEdge(new DirectedEdge(num1, num2, 1.0));
                                _graph.AddEdge(new DirectedEdge(num2, num1, 1.0));
                            }
                        }
                }
        }

        public Path ShortestPath(ICell source, ICell destination)
        {
            return TryFindShortestPath(source, destination);
        }

        public Path TryFindShortestPath(ICell source, ICell destination)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            var steps = destination != null
                ? ShortestPathCells(source, destination).ToList()
                : throw new ArgumentNullException(nameof(destination));
            return steps[0] == null ? null : new Path(steps);
        }

        private IEnumerable<ICell> ShortestPathCells(ICell source, ICell destination)
        {
            var path = DijkstraShortestPath.FindPath(_graph, IndexFor(source), IndexFor(destination));
            if (path == null)
            {
                yield return null;
            }
            else
            {
                yield return source;
                foreach (var directedEdge in path)
                    yield return CellFor(directedEdge.To);
            }
        }

        private int IndexFor(ICell cell)
        {
            return cell.Y * _map.Width + cell.X;
        }

        private ICell CellFor(int index)
        {
            return _map.GetCell(index % _map.Width, index / _map.Width);
        }
    }
}