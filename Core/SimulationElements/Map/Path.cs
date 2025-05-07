using System;
using System.Collections.Generic;

namespace SimArena.Core.SimulationElements.Map
{

   /// <summary>
   /// Exception that is thrown when attempting to move along a Path when there are no more Steps in that direction
   /// </summary>
   public class NoMoreStepsException : Exception
   {
      /// <summary>
      /// Initializes a new instance of the NoMoreStepsException class
      /// </summary>
      public NoMoreStepsException()
      {
      }
      /// <summary>
      /// Initializes a new instance of the NoMoreStepsException class with a specified error message.
      /// </summary>
      /// <param name="message">The error message that explains the reason for the exception.</param>
      public NoMoreStepsException( string message )
         : base( message )
      {
      }
      /// <summary>
      /// Initializes a new instance of the NoMoreStepsException class with a specified error message and a reference to the inner exception that is the cause of this exception.
      /// </summary>
      /// <param name="message">The error message that explains the reason for the exception.</param>
      /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
      public NoMoreStepsException( string message, Exception innerException )
         : base( message, innerException )
      {
      }
   }

   /// <summary>
   /// Exception that is thrown when attempting to find a Path from a Source to a Destination but one does not exist
   /// </summary>
   public class PathNotFoundException : Exception
   {
      /// <summary>
      /// Initializes a new instance of the PathNotFoundException class
      /// </summary>
      public PathNotFoundException()
      {
      }
      /// <summary>
      /// Initializes a new instance of the PathNotFoundException class with a specified error message.
      /// </summary>
      /// <param name="message">The error message that explains the reason for the exception.</param>
      public PathNotFoundException( string message )
         : base( message )
      {
      }
      /// <summary>
      /// Initializes a new instance of the PathNotFoundException class with a specified error message and a reference to the inner exception that is the cause of this exception.
      /// </summary>
      /// <param name="message">The error message that explains the reason for the exception.</param>
      /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
      public PathNotFoundException( string message, Exception innerException )
         : base( message, innerException )
      {
      }
   }
}
