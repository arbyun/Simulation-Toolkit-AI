using System;
using System.Numerics;

namespace SimArena.Core.Entities
{
    /// <summary>
    /// Base class for all entities in the game
    /// </summary>
    public abstract class Entity: IEquatable<Entity>
    {
        #region Properties

        /// <summary>
        /// Unique identifier for the entity
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// X-coordinate of the entity in the world
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y-coordinate of the entity in the world
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Position of the entity as a tuple
        /// </summary>
        public (int X, int Y) Position
        {
            get => (X, Y);
            set => (X, Y) = value;
        }

        /// <summary>
        /// Name of the entity
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Whether the entity blocks movement
        /// </summary>
        public virtual bool BlocksMovement { get; } = true;

        /// <summary>
        /// Whether the entity blocks line of sight
        /// </summary>
        public bool BlocksLineOfSight { get; set; } = false;
        
        /// <summary>
        /// Gets or sets the entity's speed
        /// </summary>
        public float Speed { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the entity's current direction
        /// </summary>
        public Vector3 FacingDirection { get; set; } = Vector3.Zero;
        
        /// <summary>
        /// The simulation that contains this entity
        /// </summary>
        public Simulation Simulation { get; internal set; }
        

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new entity with the specified parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="simulation"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected Entity(string name, int x, int y, Simulation simulation)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            X = x;
            Y = y;
            Simulation = simulation;
        }
        
        /// <summary>
        /// Creates a new entity with the specified parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="simulation"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected Entity(string name, int x, int y, Simulation simulation, int width, int height)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            X = x;
            Y = y;
            Simulation = simulation;
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Updates the entity state
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update in seconds</param>
        public virtual void Update(float deltaTime) { }

        /// <summary>
        /// Calculates the distance to another entity
        /// </summary>
        /// <param name="other">The other entity</param>
        /// <returns>The distance between this entity and the other entity</returns>
        public float DistanceTo(Entity other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            int dx = X - other.X;
            int dy = Y - other.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Calculates the distance to a position
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <returns>The distance between this entity and the position</returns>
        public float DistanceTo(int x, int y)
        {
            int dx = X - x;
            int dy = Y - y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Checks if this entity is at the specified position
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <returns>True if the entity is at the specified position, false otherwise</returns>
        public bool IsAt(int x, int y)
        {
            return X == x && Y == y;
        }

        #endregion

        #region Equality and Hashing

        /// <summary>
        /// Determines whether the specified object is equal to the current object
        /// </summary>
        /// <param name="obj">The object to compare with the current object</param>
        /// <returns>True if the specified object is equal to the current object, false otherwise</returns>
        public override bool Equals(object? obj)
        {
            return Equals(obj as Entity);
        }

        /// <summary>
        /// Determines whether the specified entity is equal to the current entity
        /// </summary>
        /// <param name="other">The entity to compare with the current entity</param>
        /// <returns>True if the specified entity is equal to the current entity, false otherwise</returns>
        public bool Equals(Entity? other)
        {
            return other != null && Id.Equals(other.Id);
        }

        /// <summary>
        /// Returns a hash code for this entity
        /// </summary>
        /// <returns>A hash code for this entity</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of this entity
        /// </summary>
        /// <returns>A string representation of this entity</returns>
        public override string ToString()
        {
            return $"{Name} at ({X}, {Y})";
        }

        #endregion
    }
}