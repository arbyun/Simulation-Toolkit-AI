using SimToolAI.Core.Map;
using SimToolAI.Core.Rendering.RenderStrategies;
using SimToolAI.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Examples.Unity.Managers
{
    /// <summary>
    /// Manages map initialization and rendering
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        [Header("Map Settings")]
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private TileBase wallTile;
        [SerializeField] private TileBase floorTile;
        [SerializeField] private TextAsset mapText;
        [SerializeField] private Grid grid;

        /// <summary>
        /// The map instance
        /// </summary>
        public ISimMap Map { get; private set; }

        /// <summary>
        /// The grid component
        /// </summary>
        public Grid Grid => grid;

        /// <summary>
        /// Initializes the map
        /// </summary>
        public void Initialize()
        {
            ParseMap();
        }

        /// <summary>
        /// Parses the map from the text asset or file
        /// </summary>
        private void ParseMap()
        {
            GridMapParser<GridMap> map = new GridMapParser<GridMap>();

            Map = mapText ? map.LoadMapFromText(mapText.text) : map.LoadMapFromFile();

            Map.Initialize(new UnityMapRenderable(map.GetMapGrid(), Map.Height, Map.Width,
                tilemap, wallTile, floorTile));

            Map.Renderable.Render();
        }
    }
}