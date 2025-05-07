using System;
using System.Collections.Generic;

namespace SimArena.Core.SimulationElements.Map

{
   /// <summary>
   /// A class which can be used to find shortest path from a source to a destination in a Map
   /// </summary>
   public class PathFinder : PathFinder<Cell>
   {
      /// <summary>
      /// Constructs a new PathFinder instance for the specified Map that will not consider diagonal movements to be valid.
      /// </summary>
      /// <param name="map">The Map that this PathFinder instance will run shortest path algorithms on</param>
      /// <exception cref="ArgumentNullException">Thrown when a null map parameter is passed in</exception>
      public PathFinder( IMap<Cell> map )
         : base( map )
      {
      }

      /// <summary>
      /// Constructs a new PathFinder instance for the specified Map that will consider diagonal movement by using the specified diagonalCost
      /// </summary>
      /// <param name="map">The Map that this PathFinder instance will run shortest path algorithms on</param>
      /// <param name="diagonalCost">
      /// The cost of diagonal movement compared to horizontal or vertical movement.
      /// Use 1.0 if you want the same cost for all movements.
      /// On a standard cartesian map, it should be sqrt(2) (1.41)
      /// </param>
      /// <exception cref="ArgumentNullException">Thrown when a null map parameter is passed in</exception>
      public PathFinder( IMap<Cell> map, double diagonalCost )
         : base( map, diagonalCost )
      {
      }

      public PathFinder(IMap map) : this(map as IMap<Cell>)
      {
      }
   }

   /// <summary>
   /// A class which can be used to find shortest path from a source to a destination in a Map
   /// </summary>
   public class PathFinder<TCell> where TCell : ICell
   {
      private readonly double? _diagonalCost;
      private readonly IMap<TCell> _map;

      /// <summary>
      /// Constructs a new PathFinder instance for the specified Map that will not consider diagonal movements to be valid.
      /// </summary>
      /// <param name="map">The Map that this PathFinder instance will run shortest path algorithms on</param>
      /// <exception cref="ArgumentNullException">Thrown when a null map parameter is passed in</exception>
      public PathFinder( IMap<TCell> map )
      {
         _map = map ?? throw new ArgumentNullException( nameof( map ), "Map cannot be null" );
      }

      /// <summary>
      /// Constructs a new PathFinder instance for the specified Map that will consider diagonal movement by using the specified diagonalCost
      /// </summary>
      /// <param name="map">The Map that this PathFinder instance will run shortest path algorithms on</param>
      /// <param name="diagonalCost">
      /// The cost of diagonal movement compared to horizontal or vertical movement.
      /// Use 1.0 if you want the same cost for all movements.
      /// On a standard cartesian map, it should be sqrt(2) (1.41)
      /// </param>
      /// <exception cref="ArgumentNullException">Thrown when a null map parameter is passed in</exception>
      public PathFinder( IMap<TCell> map, double diagonalCost )
      {
         _map = map ?? throw new ArgumentNullException( nameof( map ), "Map cannot be null" );
         _diagonalCost = diagonalCost;
      }

      /// <summary>
      /// Returns a shortest Path containing a list of Cells from a specified source Cell to a destination Cell using the AStar search algorithm.
      /// </summary>
      /// <param name="source">The Cell which is at the start of the path</param>
      /// <param name="destination">The Cell which is at the end of the path</param>
      /// <exception cref="ArgumentNullException">Thrown when source or destination is null</exception>
      /// <exception cref="PathNotFoundException">Thrown when there is not a path from the source to the destination</exception>
      /// <returns>Returns a shortest Path containing a list of Cells from a specified source Cell to a destination Cell</returns>
      public MapPath ShortestPath( ICell source, ICell destination )
      {
         MapPath shortestPath = TryFindShortestPath( source, destination );

         if ( shortestPath == null )
         {
            throw new PathNotFoundException( $"Path from ({source.X}, {source.Y}) to ({destination.X}, {destination.Y}) not found" );
         }

         return shortestPath;
      }

      /// <summary>
      /// Returns a shortest Path containing a list of Cells from a specified source Cell to a destination Cell using the AStar search algorithm.
      /// </summary>
      /// <param name="source">The Cell which is at the start of the path</param>
      /// <param name="destination">The Cell which is at the end of the path</param>
      /// <exception cref="ArgumentNullException">Thrown when source or destination is null</exception>
      /// <returns>Returns a shortest Path containing a list of Cells from a specified source Cell to a destination Cell. If no path is found null will be returned</returns>
      public MapPath TryFindShortestPath( ICell source, ICell destination )
      {
         if ( source == null )
         {
            throw new ArgumentNullException( nameof( source ) );
         }

         if ( destination == null )
         {
            throw new ArgumentNullException( nameof( destination ) );
         }

         if ( !source.IsWalkable )
         {
            return null;
         }

         if ( !destination.IsWalkable )
         {
            return null;
         }

         AStarShortestPath<TCell> aStarShortestPath;
         if ( _diagonalCost.HasValue )
         {
            aStarShortestPath = new AStarShortestPath<TCell>( (int) _diagonalCost.Value );
         }
         else
         {
            aStarShortestPath = new AStarShortestPath<TCell>();
         }
         List<TCell> cells = aStarShortestPath.FindPath( (TCell) source, (TCell) destination, _map );
         if ( cells == null )
         {
            return null;
         }
         return new MapPath( (IEnumerable<ICell>) cells );
      }
   }
   
   /// <summary>
   /// A class representing an ordered list of Cells from Start to End
   /// The path can be traversed by the StepForward and StepBackward methods
   /// Implemented by a doubly linked list
   /// </summary>
   public class MapPath
   {
      private readonly LinkedList<ICell> _steps;
      private LinkedListNode<ICell> _currentStep;

      /// <summary>
      /// Construct a new Path from the specified ordered list of Cells
      /// </summary>
      /// <param name="steps">An IEnumerable of Cells that represent the ordered steps along this Path from Start to End</param>
      /// <exception cref="ArgumentNullException">Thrown when the specified steps parameter is null</exception>
      /// <exception cref="ArgumentException">Thrown when the specified steps parameter is empty</exception>
      public MapPath( IEnumerable<ICell> steps )
      {
         _steps = new LinkedList<ICell>( steps );

         if ( _steps.Count < 1 )
         {
            throw new ArgumentException( "Path must have steps", nameof( steps ) );
         }

         _currentStep = _steps.First;
      }

      /// <summary>
      /// The Cell representing the first step or Start of this Path
      /// </summary>
      public ICell Start => _steps.First.Value;

      /// <summary>
      /// The Cell representing the last step or End of this Path
      /// </summary>
      public ICell End => _steps.Last.Value;

      /// <summary>
      /// The number of steps in this Path
      /// </summary>
      public int Length => _steps.Count;

      /// <summary>
      /// The Cell representing the step that is currently occupied in this Path
      /// </summary>
      public ICell CurrentStep => _currentStep.Value;

      /// <summary>
      /// All of the Cells representing the Steps in this Path from Start to End as an IEnumerable
      /// </summary>
      public IEnumerable<ICell> Steps => _steps;

      /// <summary>
      /// Move forward along this Path and advance the CurrentStep to the next Step in the Path
      /// </summary>
      /// <returns>A Cell representing the Step that was moved to as we advanced along the Path</returns>
      /// <exception cref="NoMoreStepsException">Thrown when attempting to move forward along a Path on which we are currently at the End</exception>
      public ICell StepForward()
      {
         ICell cell = TryStepForward();

         if ( cell == null )
         {
            throw new NoMoreStepsException( "Cannot take a step forward when at the end of the path" );
         }

         return cell;
      }

      /// <summary>
      /// Move forward along this Path and advance the CurrentStep to the next Step in the Path
      /// </summary>
      /// <returns>A Cell representing the Step that was moved to as we advanced along the Path. If there is not another Cell in the path to advance to null is returned</returns>
      public ICell TryStepForward()
      {
         LinkedListNode<ICell> nextStep = _currentStep.Next;
         if ( nextStep == null )
         {
            return null;
         }
         _currentStep = nextStep;
         return nextStep.Value;
      }

      /// <summary>
      /// Move backwards along this Path and rewind the CurrentStep to the previous Step in the Path
      /// </summary>
      /// <returns>A Cell representing the Step that was moved to as we back up along the Path</returns>
      /// <exception cref="NoMoreStepsException">Thrown when attempting to move backward along a Path on which we are currently at the Start</exception>
      public ICell StepBackward()
      {
         ICell cell = TryStepBackward();

         if ( cell == null )
         {
            throw new NoMoreStepsException( "Cannot take a step backward when at the start of the path" );
         }

         return cell;
      }

      /// <summary>
      /// Move backwards along this Path and rewind the CurrentStep to the next Step in the Path
      /// </summary>
      /// <returns>A Cell representing the Step that was moved to as we back up along the Path. If there is not another Cell in the path to back up to null is returned</returns>
      public ICell TryStepBackward()
      {
         LinkedListNode<ICell> previousStep = _currentStep.Previous;
         if ( previousStep == null )
         {
            return null;
         }
         _currentStep = previousStep;
         return previousStep.Value;
      }
   }
}