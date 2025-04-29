namespace SimArena.Core.Serialization.Results
{
    public interface ISimulationResult
    {
        /// <summary>
        /// Returns the simulation result as a string.
        /// </summary>
        /// <returns>The result.</returns>
        string Read();
        
        /// <summary>
        /// Returns a POCO that can be serialized.
        /// </summary>
        object ToSerializable();
    }
}