namespace SimArena.Core.Serialization.Objectives
{
    /// <summary>
    /// Interface for objective trackers that need to interact with simulation events
    /// </summary>
    public interface IEventInteractor
    {
        /// <summary>
        /// Initializes the event handlers for the tracker
        /// </summary>
        /// <param name="events">The simulation events</param>
        void InitializeEvents(SimulationEvents events);
    }
}