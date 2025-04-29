namespace SimArena.Core.SimulationElements.Map
{
    public class MapState
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public CellProperties[] Cells { get; set; }

        [Flags]
        public enum CellProperties
        {
            None = 0,
            Walkable = 1,
            Transparent = 2,
        }
    }
}