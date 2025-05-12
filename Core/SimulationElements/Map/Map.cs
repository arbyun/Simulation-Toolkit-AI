using System;
using System.Collections.Generic;
using System.Text;

namespace SimArena.Core.SimulationElements.Map
{
  public class Map : Map<Cell>, IMap<Cell>
  {
    public Map()
    {
    }

    public Map(int width, int height)
      : base(width, height)
    {
    }
  }

  public class Map<TCell> : IMap<TCell> where TCell : ICell
  {
    private TCell[,] _cells;

    public Map()
    {
    }

    public Map(int width, int height) => this.Init(width, height);

    public TCell this[int x, int y]
    {
      get => this._cells[x, y];
      set => this._cells[x, y] = value;
    }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public virtual void Initialize(int width, int height) => this.Init(width, height);

    private void Init(int width, int height)
    {
      this.Width = width;
      this.Height = height;
      this._cells = new TCell[width, height];
      for (int index1 = 0; index1 < width; ++index1)
      {
        for (int index2 = 0; index2 < height; ++index2)
        {
          this._cells[index1, index2] = Activator.CreateInstance<TCell>();
          this._cells[index1, index2].X = index1;
          this._cells[index1, index2].Y = index2;
        }
      }
    }

    public bool IsTransparent(int x, int y) => this._cells[x, y].IsTransparent;

    public bool IsWalkable(int x, int y) => this._cells[x, y].IsWalkable;

    public void SetCellProperties(int x, int y, bool isTransparent, bool isWalkable)
    {
      this._cells[x, y].IsTransparent = isTransparent;
      this._cells[x, y].IsWalkable = isWalkable;
    }

    public void Clear() => this.Clear(true, true);

    public void Clear(bool isTransparent, bool isWalkable)
    {
      foreach (TCell allCell in this.GetAllCells())
        this.SetCellProperties(allCell.X, allCell.Y, isTransparent, isWalkable);
    }

    public virtual TMap Clone<TMap>() where TMap : IMap<TCell>, new()
    {
      TMap map = new TMap();
      map.Initialize(this.Width, this.Height);
      map.Clear(true, true);
      foreach (TCell allCell in this.GetAllCells())
        map.SetCellProperties(allCell.X, allCell.Y, allCell.IsTransparent, allCell.IsWalkable);
      return map;
    }

    public void Copy(IMap<TCell> sourceMap) => this.Copy(sourceMap, 0, 0);

    public void Copy(IMap<TCell> sourceMap, int left, int top)
    {
      if (sourceMap == null)
        throw new ArgumentNullException(nameof (sourceMap), "Source map cannot be null");
      if (sourceMap.Width + left > this.Width)
        throw new ArgumentException("Source map 'width' + 'left' cannot be larger than the destination map width");
      if (sourceMap.Height + top > this.Height)
        throw new ArgumentException("Source map 'height' + 'top' cannot be larger than the destination map height");
      foreach (TCell allCell in sourceMap.GetAllCells())
        this.SetCellProperties(allCell.X + left, allCell.Y + top, allCell.IsTransparent, allCell.IsWalkable);
    }

    public IEnumerable<TCell> GetAllCells()
    {
      for (int y = 0; y < this.Height; ++y)
      {
        for (int x = 0; x < this.Width; ++x)
          yield return this.GetCell(x, y);
      }
    }

    public IEnumerable<TCell> GetCellsAlongLine(
      int xOrigin,
      int yOrigin,
      int xDestination,
      int yDestination)
    {
      xOrigin = this.ClampX(xOrigin);
      yOrigin = this.ClampY(yOrigin);
      xDestination = this.ClampX(xDestination);
      yDestination = this.ClampY(yDestination);
      int dx = Math.Abs(xDestination - xOrigin);
      int dy = Math.Abs(yDestination - yOrigin);
      int sx = xOrigin < xDestination ? 1 : -1;
      int sy = yOrigin < yDestination ? 1 : -1;
      int err = dx - dy;
      while (true)
      {
        int num;
        do
        {
          yield return this.GetCell(xOrigin, yOrigin);
          if (xOrigin != xDestination || yOrigin != yDestination)
          {
            num = 2 * err;
            if (num > -dy)
            {
              err -= dy;
              xOrigin += sx;
            }
          }
          else
            goto label_1;
        }
        while (num >= dx);
        err += dx;
        yOrigin += sy;
      }
      label_1:;
    }

    private int ClampX(int x)
    {
      if (x < 0)
        return 0;
      return x <= this.Width - 1 ? x : this.Width - 1;
    }

    private int ClampY(int y)
    {
      if (y < 0)
        return 0;
      return y <= this.Height - 1 ? y : this.Height - 1;
    }

    public IEnumerable<TCell> GetCellsInCircle(int xCenter, int yCenter, int radius)
    {
      HashSet<int> discovered = new HashSet<int>();
      int d = (5 - radius * 4) / 4;
      int x = 0;
      int y = radius;
      do
      {
        foreach (TCell cell in this.GetCellsAlongLine(xCenter + x, yCenter + y, xCenter - x, yCenter + y))
        {
          if (this.AddToHashSet(discovered, cell))
            yield return cell;
        }
        foreach (TCell cell in this.GetCellsAlongLine(xCenter - x, yCenter - y, xCenter + x, yCenter - y))
        {
          if (this.AddToHashSet(discovered, cell))
            yield return cell;
        }
        foreach (TCell cell in this.GetCellsAlongLine(xCenter + y, yCenter + x, xCenter - y, yCenter + x))
        {
          if (this.AddToHashSet(discovered, cell))
            yield return cell;
        }
        foreach (TCell cell in this.GetCellsAlongLine(xCenter + y, yCenter - x, xCenter - y, yCenter - x))
        {
          if (this.AddToHashSet(discovered, cell))
            yield return cell;
        }
        if (d < 0)
        {
          d += 2 * x + 1;
        }
        else
        {
          d += 2 * (x - y) + 1;
          --y;
        }
        ++x;
      }
      while (x <= y);
    }

    public IEnumerable<TCell> GetCellsInDiamond(int xCenter, int yCenter, int distance)
    {
      HashSet<int> discovered = new HashSet<int>();
      int xMin = Math.Max(0, xCenter - distance);
      int xMax = Math.Min(this.Width - 1, xCenter + distance);
      int yMin = Math.Max(0, yCenter - distance);
      int yMax = Math.Min(this.Height - 1, yCenter + distance);
      for (int i = 0; i <= distance; ++i)
      {
        for (int j = distance; j >= i; --j)
        {
          TCell cell;
          if (this.AddToHashSet(discovered, Math.Max(xMin, xCenter - i), Math.Min(yMax, yCenter + distance - j), out cell))
            yield return cell;
          if (this.AddToHashSet(discovered, Math.Max(xMin, xCenter - i), Math.Max(yMin, yCenter - distance + j), out cell))
            yield return cell;
          if (this.AddToHashSet(discovered, Math.Min(xMax, xCenter + i), Math.Min(yMax, yCenter + distance - j), out cell))
            yield return cell;
          if (this.AddToHashSet(discovered, Math.Min(xMax, xCenter + i), Math.Max(yMin, yCenter - distance + j), out cell))
            yield return cell;
        }
      }
    }

    public IEnumerable<TCell> GetCellsInSquare(int xCenter, int yCenter, int distance)
    {
      int xMin = Math.Max(0, xCenter - distance);
      int xMax = Math.Min(this.Width - 1, xCenter + distance);
      int num = Math.Max(0, yCenter - distance);
      int yMax = Math.Min(this.Height - 1, yCenter + distance);
      for (int y = num; y <= yMax; ++y)
      {
        for (int x = xMin; x <= xMax; ++x)
          yield return this.GetCell(x, y);
      }
    }

    public IEnumerable<TCell> GetCellsInRectangle(int top, int left, int width, int height)
    {
      int xMin = Math.Max(0, left);
      int xMax = Math.Min(this.Width, left + width);
      int num = Math.Max(0, top);
      int yMax = Math.Min(this.Height, top + height);
      for (int y = num; y < yMax; ++y)
      {
        for (int x = xMin; x < xMax; ++x)
          yield return this.GetCell(x, y);
      }
    }

    public IEnumerable<TCell> GetBorderCellsInCircle(int xCenter, int yCenter, int radius)
    {
      HashSet<int> discovered = new HashSet<int>();
      int d = (5 - radius * 4) / 4;
      int x = 0;
      int y = radius;
      TCell centerCell = this.GetCell(xCenter, yCenter);
      do
      {
        TCell cell;
        if (this.AddToHashSet(discovered, this.ClampX(xCenter + x), this.ClampY(yCenter + y), centerCell, out cell))
          yield return cell;
        if (this.AddToHashSet(discovered, this.ClampX(xCenter + x), this.ClampY(yCenter - y), centerCell, out cell))
          yield return cell;
        if (this.AddToHashSet(discovered, this.ClampX(xCenter - x), this.ClampY(yCenter + y), centerCell, out cell))
          yield return cell;
        if (this.AddToHashSet(discovered, this.ClampX(xCenter - x), this.ClampY(yCenter - y), centerCell, out cell))
          yield return cell;
        if (this.AddToHashSet(discovered, this.ClampX(xCenter + y), this.ClampY(yCenter + x), centerCell, out cell))
          yield return cell;
        if (this.AddToHashSet(discovered, this.ClampX(xCenter + y), this.ClampY(yCenter - x), centerCell, out cell))
          yield return cell;
        if (this.AddToHashSet(discovered, this.ClampX(xCenter - y), this.ClampY(yCenter + x), centerCell, out cell))
          yield return cell;
        if (this.AddToHashSet(discovered, this.ClampX(xCenter - y), this.ClampY(yCenter - x), centerCell, out cell))
          yield return cell;
        if (d < 0)
        {
          d += 2 * x + 1;
        }
        else
        {
          d += 2 * (x - y) + 1;
          --y;
        }
        ++x;
      }
      while (x <= y);
    }

    public IEnumerable<TCell> GetBorderCellsInDiamond(int xCenter, int yCenter, int distance)
    {
      HashSet<int> discovered = new HashSet<int>();
      int xMin = Math.Max(0, xCenter - distance);
      int xMax = Math.Min(this.Width - 1, xCenter + distance);
      int yMin = Math.Max(0, yCenter - distance);
      int yMax = Math.Min(this.Height - 1, yCenter + distance);
      TCell centerCell = this.GetCell(xCenter, yCenter);
      TCell cell;
      if (this.AddToHashSet(discovered, xCenter, yMin, centerCell, out cell))
        yield return cell;
      if (this.AddToHashSet(discovered, xCenter, yMax, centerCell, out cell))
        yield return cell;
      for (int i = 1; i <= distance; ++i)
      {
        if (this.AddToHashSet(discovered, Math.Max(xMin, xCenter - i), Math.Min(yMax, yCenter + distance - i), centerCell, out cell))
          yield return cell;
        if (this.AddToHashSet(discovered, Math.Max(xMin, xCenter - i), Math.Max(yMin, yCenter - distance + i), centerCell, out cell))
          yield return cell;
        if (this.AddToHashSet(discovered, Math.Min(xMax, xCenter + i), Math.Min(yMax, yCenter + distance - i), centerCell, out cell))
          yield return cell;
        if (this.AddToHashSet(discovered, Math.Min(xMax, xCenter + i), Math.Max(yMin, yCenter - distance + i), centerCell, out cell))
          yield return cell;
      }
    }

    public IEnumerable<TCell> GetBorderCellsInSquare(int xCenter, int yCenter, int distance)
    {
      int x1 = Math.Max(0, xCenter - distance);
      int x2 = Math.Min(this.Width - 1, xCenter + distance);
      int y1 = Math.Max(0, yCenter - distance);
      int y2 = Math.Min(this.Height - 1, yCenter + distance);
      List<TCell> borderCellsInSquare = new List<TCell>();
      for (int x3 = x1; x3 <= x2; ++x3)
      {
        borderCellsInSquare.Add(this.GetCell(x3, y1));
        borderCellsInSquare.Add(this.GetCell(x3, y2));
      }
      for (int y3 = y1 + 1; y3 <= y2 - 1; ++y3)
      {
        borderCellsInSquare.Add(this.GetCell(x1, y3));
        borderCellsInSquare.Add(this.GetCell(x2, y3));
      }
      TCell cell = this.GetCell(xCenter, yCenter);
      borderCellsInSquare.Remove(cell);
      return (IEnumerable<TCell>) borderCellsInSquare;
    }

    public IEnumerable<TCell> GetCellsInRows(params int[] rowNumbers)
    {
      int[] numArray = rowNumbers;
      for (int index = 0; index < numArray.Length; ++index)
      {
        int y = numArray[index];
        for (int x = 0; x < this.Width; ++x)
          yield return this.GetCell(x, y);
      }
      numArray = (int[]) null;
    }

    public IEnumerable<TCell> GetCellsInColumns(params int[] columnNumbers)
    {
      int[] numArray = columnNumbers;
      for (int index = 0; index < numArray.Length; ++index)
      {
        int x = numArray[index];
        for (int y = 0; y < this.Height; ++y)
          yield return this.GetCell(x, y);
      }
      numArray = (int[]) null;
    }

    public IEnumerable<TCell> GetAdjacentCells(int xCenter, int yCenter)
    {
      return this.GetAdjacentCells(xCenter, yCenter, false);
    }

    public IEnumerable<TCell> GetAdjacentCells(int xCenter, int yCenter, bool includeDiagonals)
    {
      int topY = yCenter - 1;
      int bottomY = yCenter + 1;
      int leftX = xCenter - 1;
      int rightX = xCenter + 1;
      if (topY >= 0)
        yield return this.GetCell(xCenter, topY);
      if (leftX >= 0)
        yield return this.GetCell(leftX, yCenter);
      if (bottomY < this.Height)
        yield return this.GetCell(xCenter, bottomY);
      if (rightX < this.Width)
        yield return this.GetCell(rightX, yCenter);
      if (includeDiagonals)
      {
        if (rightX < this.Width && topY >= 0)
          yield return this.GetCell(rightX, topY);
        if (rightX < this.Width && bottomY < this.Height)
          yield return this.GetCell(rightX, bottomY);
        if (leftX >= 0 && topY >= 0)
          yield return this.GetCell(leftX, topY);
        if (leftX >= 0 && bottomY < this.Height)
          yield return this.GetCell(leftX, bottomY);
      }
    }

    public TCell GetCell(int x, int y) => this._cells[x, y];

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      int num = 0;
      foreach (TCell allCell in this.GetAllCells())
      {
        Cell cell = (Cell) (object) allCell;
        if (cell.Y != num)
        {
          num = cell.Y;
          stringBuilder.Append(Environment.NewLine);
        }
        stringBuilder.Append(cell.ToString());
      }
      return stringBuilder.ToString().TrimEnd('\r', '\n');
    }

    public MapState Save()
    {
      MapState mapState = new MapState()
      {
        Width = this.Width,
        Height = this.Height,
        Cells = new MapState.CellProperties[this.Width * this.Height]
      };
      foreach (TCell allCell in this.GetAllCells())
      {
        MapState.CellProperties cellProperties = MapState.CellProperties.None;
        if (allCell.IsTransparent)
          cellProperties |= MapState.CellProperties.Transparent;
        if (allCell.IsWalkable)
          cellProperties |= MapState.CellProperties.Walkable;
        mapState.Cells[allCell.Y * this.Width + allCell.X] = cellProperties;
      }
      return mapState;
    }

    public void Restore(MapState state)
    {
      if (state == null)
        throw new ArgumentNullException(nameof (state), "Map state cannot be null");
      this.Initialize(state.Width, state.Height);
      foreach (TCell allCell in this.GetAllCells())
      {
        MapState.CellProperties cell = state.Cells[allCell.Y * this.Width + allCell.X];
        this._cells[allCell.X, allCell.Y].IsTransparent = cell.HasFlag((Enum) MapState.CellProperties.Transparent);
        this._cells[allCell.X, allCell.Y].IsWalkable = cell.HasFlag((Enum) MapState.CellProperties.Walkable);
      }
    }

    public TCell CellFor(int index) => this.GetCell(index % this.Width, index / this.Width);

    public int IndexFor(int x, int y) => y * this.Width + x;

    public int IndexFor(TCell cell)
    {
      if ((object) cell == null)
        throw new ArgumentNullException(nameof (cell), "Cell cannot be null");
      return cell.Y * this.Width + cell.X;
    }

    private bool AddToHashSet(HashSet<int> hashSet, int x, int y, out TCell cell)
    {
      cell = this.GetCell(x, y);
      return hashSet.Add(this.IndexFor(cell));
    }

    private bool AddToHashSet(
      HashSet<int> hashSet,
      int x,
      int y,
      TCell centerCell,
      out TCell cell)
    {
      cell = this.GetCell(x, y);
      return !cell.Equals((ICell) centerCell) && hashSet.Add(this.IndexFor(cell));
    }

    private bool AddToHashSet(HashSet<int> hashSet, TCell cell) => hashSet.Add(this.IndexFor(cell));
  }
}