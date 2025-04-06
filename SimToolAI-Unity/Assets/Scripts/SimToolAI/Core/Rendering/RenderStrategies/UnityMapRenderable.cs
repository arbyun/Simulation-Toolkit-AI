using System;
using SimToolAI.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SimToolAI.Core.Rendering.RenderStrategies
{
    public class UnityMapRenderable: RenderableBase
    {
        /// <summary>
        /// Gets the rendering priority (maps are rendered before entities)
        /// </summary>
        public override int RenderPriority => 0;

        /// <summary>
        /// Creates a new console map renderable with the specified settings
        /// </summary>
        /// <param name="settings">Settings for the renderable</param>
        public UnityMapRenderable(Data settings) : base(settings)
        {
        }

        /// <summary>
        /// Creates a new unity map renderable with the specified parameters
        /// </summary>
        /// <param name="mapGrid">2D array of characters representing the map</param>
        /// <param name="height">Height of the map</param>
        /// <param name="width">Width of the map</param>
        /// <param name="wall"></param>
        /// <param name="floor"></param>
        /// <param name="tilemap"></param>
        public UnityMapRenderable(char[,] mapGrid, int height, int width, Tilemap tilemap, 
            TileBase wall, TileBase floor)
        {
            if (mapGrid == null)
                throw new ArgumentNullException(nameof(mapGrid));

            if (height <= 0 || width <= 0)
                throw new ArgumentException("Height and width must be positive");

            Settings.Set("map", mapGrid);
            Settings.Set("height", height);
            Settings.Set("width", width);
            Settings.Set("tilemap", tilemap);
            Settings.Set("wallTile", wall);
            Settings.Set("floorTile", floor);
        }

        /// <summary>
        /// Renders the map to the console
        /// </summary>
        public override void Render()
        {
            char[,] mapGrid = Settings.Get<char[,]>("map");
            int height = Settings.Get<int>("height");
            int width = Settings.Get<int>("width");
            Tilemap tilemap = Settings.Get<Tilemap>("tilemap");
            TileBase wall = Settings.Get<TileBase>("wallTile");
            TileBase floor = Settings.Get<TileBase>("floorTile");

            if (mapGrid == null)
                return;

            try
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Set the appropriate color based on the map cell
                        switch (mapGrid[x, y])
                        {
                            case '#': // Wall
                                tilemap.SetTile(new Vector3Int(x, y), wall);
                                break;
                            case '.': // Floor
                                tilemap.SetTile(new Vector3Int(x, y), floor);
                                break;
                            case '&': // Door; for now, let's treat it as floor
                                tilemap.SetTile(new Vector3Int(x, y), floor);
                                break;
                            case 'O': // Window; for now, let's treat it as a wall
                                tilemap.SetTile(new Vector3Int(x, y), wall);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Ignore exceptions related to console buffer size changes
                if (ex is not (ArgumentOutOfRangeException or System.IO.IOException))
                    throw;
            }
        }
    }
}