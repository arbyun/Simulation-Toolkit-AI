using System;
using System.IO;

namespace SimToolAI.Core.Map
{
    public class GridMapParser<T> where T : RogueSharp.Map, ISimMap, new()
    {
        private int _width;
        private int _height;
        private char[,] _mapGrid = null!;

        private void InitializeMap()
        {
            _mapGrid = new char[_width, _height];
        
            for (int y = 0; y < _height; y++)
            for (int x = 0; x < _width; x++)
                _mapGrid[x, y] = '.';
        }
        
        private void InitializeMap(string[] lines)
        {
            if (_width == 0 || _height == 0)
            {
                (_width, _height) = FindMapBounds(lines);
            }
            
            InitializeMap();
        }

        public char[,] GetMapGrid() => _mapGrid.Clone() as char[,];
        
        public T LoadMapFromFile(string filePath = "NULL")
        {
            string[] lines;
            
            if (filePath == "NULL")
            {
                // Sample map
                lines = new []
                {
                    "#spacing X 2",
                    "#spacing Y 3", 
                    "#offset X 3",
                    "#offset Y 3",
                    "#wallrect (0,0) (35,4)",
                    "#door (35,3)",
                    "#wallrect (34,0) (39,6)",
                    "#door (34,3)"
                };
            }
            else
            {
                lines = File.ReadAllLines(filePath);
            }
            
            InitializeMap(lines);
            
            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("//")) continue;

                string[] parts = trimmed.Split(' ');

                switch (parts[0])
                {
                    case "#width":
                        _width = int.Parse(parts[1]);
                        break;
                    case "#height":
                        _height = int.Parse(parts[1]);
                        break;
                    case "#window":
                        AddWindow(parts);
                        break;
                    case "#wall":
                        DrawWall(parts);
                        break;
                    case "#wallrect":
                        DrawRectangle(parts);
                        break;
                    case "#door":
                        AddDoor(parts);
                        break;
                }
            }
            
            return CreateInstance();
        }
        
        public T LoadMapFromText(string text)
        {
            string[] lines;
            
            if (text is "NULL" or "" or null)
            {
                // Sample map
                lines = new []
                {
                    "#spacing X 2",
                    "#spacing Y 3", 
                    "#offset X 3",
                    "#offset Y 3",
                    "#wallrect (0,0) (35,4)",
                    "#door (35,3)",
                    "#wallrect (34,0) (39,6)",
                    "#door (34,3)"
                };
            }
            else
            {
                lines = text.Split('\n');
            }
            
            InitializeMap(lines);
            
            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("//")) continue;

                string[] parts = trimmed.Split(' ');

                switch (parts[0])
                {
                    case "#width":
                        _width = int.Parse(parts[1]);
                        break;
                    case "#height":
                        _height = int.Parse(parts[1]);
                        break;
                    case "#window":
                        AddWindow(parts);
                        break;
                    case "#wall":
                        DrawWall(parts);
                        break;
                    case "#wallrect":
                        DrawRectangle(parts);
                        break;
                    case "#door":
                        AddDoor(parts);
                        break;
                }
            }
            
            return CreateInstance();
        }

        private T CreateInstance()
        {
            T rogueMap = new T();
            
            rogueMap.Initialize(_width, _height);
            
            for (int y = 0; y < _mapGrid.GetLength(1); y++)
            {
                for (int x = 0; x < _mapGrid.GetLength(0); x++)
                {
                    char cell = _mapGrid[x, y];

                    switch (cell)
                    {
                        // Wall
                        case '#':
                            rogueMap.SetCellProperties(x, y, isTransparent: false, isWalkable: false);
                            break;
                        // Window
                        case 'O':
                            rogueMap.SetCellProperties(x, y, isTransparent: true, isWalkable: false);
                            break;
                        // Door
                        case '&':
                            rogueMap.SetCellProperties(x, y, isTransparent: false, isWalkable: true);
                            break;
                        case '.':
                            rogueMap.SetCellProperties(x, y, isTransparent: true, isWalkable: true);
                            break;
                    }
                }
            }
            
            return rogueMap;
        }

        private void AddWindow(string[] parts)
        {
            (int x, int y) = ParseCoordinates(parts[1]);
            _mapGrid[x, y] = 'O';
        }

        private void DrawWall(string[] parts)
        {
            (int x1, int y1) = ParseCoordinates(parts[1]);
            (int x2, int y2) = ParseCoordinates(parts[2]);
            
            DrawWall(x1, y1, x2, y2);
        }

        private void DrawWall(int x1, int y1, int x2, int y2)
        {
            if (x1 == x2) 
            {
                for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
                    _mapGrid[x1, y] = '#';
            }
            else if (y1 == y2) 
            {
                for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
                    _mapGrid[x, y1] = '#';
            }
        }

        private void DrawRectangle(string[] parts)
        {
            (int x1, int y1) = ParseCoordinates(parts[1]);
            (int x2, int y2) = ParseCoordinates(parts[2]);

            DrawWall(new []{"#wall", $"({x1},{y1})", $">({x2},{y1})"}); // Top
            DrawWall(new []{"#wall", $"({x1},{y1})", $">({x1},{y2})"}); // Left
            DrawWall(new []{"#wall", $"({x2},{y1})", $">({x2},{y2})"}); // Right
            DrawWall(new []{"#wall", $"({x1},{y2})", $">({x2},{y2})"}); // Bottom
        }

        private (int maxX, int maxY) FindMapBounds(string[] lines)
        {
            int maxX = 0, maxY = 0;

            foreach (string line in lines)
            {
                string[] parts = line.Split(' ');
                
                if (parts[0] != "#wallrect" && parts[0] != "#door") continue;
                
                (int x1, int y1) = ParseCoordinates(parts[1]);
                (int x2, int y2) = parts.Length > 2 ? ParseCoordinates(parts[2]) : (x1, y1);

                maxX = Math.Max(maxX, Math.Max(x1, x2));
                maxY = Math.Max(maxY, Math.Max(y1, y2));
            }

            return (maxX + 1, maxY + 1);
        }

        private void AddDoor(string[] parts)
        {
            (int x, int y) = ParseCoordinates(parts[1]);
            _mapGrid[x, y] = '.';
        }

        static (int, int) ParseCoordinates(string coord)
        {
            coord = coord.Replace("(", "").Replace(")", "").Replace(">", "");
            string[] split = coord.Split(',');
            return (int.Parse(split[0]), int.Parse(split[1]));
        }
    }
}