using System;
using SimToolAI.Core.Rendering;
using SimToolAI.Utilities;

namespace SimToolAI.Core.Entities
{
    /// <summary>
    /// Base class for all entities in the simulation
    /// </summary>
    public abstract class Entity : IEquatable<Entity>
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
        /// Awareness radius of the entity (for field of view calculations)
        /// </summary>
        public int Awareness { get; }

        /// <summary>
        /// Name of the entity
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Visual representation of the entity
        /// </summary>
        public IRenderable Avatar { get; set; }

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
        public Direction FacingDirection { get; set; } = Direction.Right;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new entity with the specified parameters
        /// </summary>
        /// <param name="name">Name of the entity</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="awareness">Awareness radius</param>
        protected Entity(string name, int x, int y, int awareness = 0)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            X = x;
            Y = y;
            Awareness = awareness;
        }

        /// <summary>
        /// Creates a new unnamed entity at the specified position
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        protected Entity(int x, int y) : this("null", x, y, 0)
        { }

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
        public override bool Equals(object obj)
        {
            return Equals(obj as Entity);
        }

        /// <summary>
        /// Determines whether the specified entity is equal to the current entity
        /// </summary>
        /// <param name="other">The entity to compare with the current entity</param>
        /// <returns>True if the specified entity is equal to the current entity, false otherwise</returns>
        public bool Equals(Entity other)
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