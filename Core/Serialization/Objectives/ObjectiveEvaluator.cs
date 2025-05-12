using System.Collections.Generic;
using System.Linq;
using SimArena.Core.Entities;
using SimArena.Core.Serialization.Configuration;

namespace SimArena.Core.Configuration
{
    /// <summary>
    /// Helper class for evaluating objectives.
    /// </summary>
    public static class ObjectiveEvaluator
    {
        /// <summary>
        /// Evaluates an objective that requires reaching a maximum number of steps.
        /// </summary>
        /// <param name="objective">The objective to evaluate.</param>
        /// <param name="currentStepCount">Current simulation step.</param>
        /// <returns>True if max steps were taken. False otherwise.</returns>
        public static bool EvaluateObjective(StepsObjective objective, int currentStepCount)
        {
            return currentStepCount >= objective.MaxSteps;
        }
    
        /// <summary>
        /// Evaluates an objective that requires capturing a point within a certain amount of time.
        /// </summary>
        /// <param name="objective">The objective to evaluate.</param>
        /// <param name="timeElapsed">The capture time so far.</param>
        /// <returns>True if the objective was captured. False otherwise.</returns>
        public static bool EvaluateObjective(CapturePointObjective objective, float timeElapsed)
        {
            return timeElapsed >= objective.CaptureTime;
        }
        
        /// <summary>
        /// Evaluates an objective that requires capturing a point within a certain amount of time.
        /// </summary>
        /// <param name="objective">The objective to evaluate.</param>
        /// <param name="timeElapsed">The capture time so far.</param>
        /// <returns>True if the objective was captured. False otherwise.</returns>
        public static bool EvaluateObjective(CapturePointObjective objective, double timeElapsed)
        {
            return timeElapsed >= objective.CaptureTime;
        }
    
        /// <summary>
        /// Evaluates an objective that requires defending a point until the timer runs out.
        /// </summary>
        /// <param name="objective">The objective to evaluate.</param>
        /// <param name="timeElapsed">Time since the beggining of the match.</param>
        /// <param name="objectiveState">The objective's health points or state.</param>
        /// <returns>True if either the timer ran out or the objective was destroyed. False otherwise.</returns>
        public static bool EvaluateObjective(DefendObjective objective, float timeElapsed, float objectiveState)
        {
            if (timeElapsed >= objective.MaxMatchTime)
            {
                return true;
            }

            return objectiveState <= objective.ObjectiveThreshold;
        }
    
        /// <summary>
        /// Evaluates if there is only one team with living players.
        /// </summary>
        /// <param name="objective">The objective to evaluate.</param>
        /// <param name="teams">The teams to check.</param>
        /// <returns>True if the objective was met. False otherwise.</returns>
        public static bool EvaluateObjective(DeathmatchObjective objective, params List<Character>[] teams)
        {
            //check only one team has someone alive
            int teamsWithLiving = 0;

            foreach (var team in teams)
            {
                // does this team have at least one alive player?
                if (team.Any(e => e.IsAlive))
                {
                    teamsWithLiving++;

                    if (teamsWithLiving > 1)
                        return false;
                }
            }

            // objective completed only if exactly one team has living members
            return teamsWithLiving == 1;
        }
        
        /// <summary>
        /// This method is used for testing purposes only.
        /// It accepts a list of integers instead of characters.
        /// </summary>
        /// <param name="objective"></param>
        /// <param name="teams"></param>
        /// <returns></returns>
        public static bool EvaluateObjective(DeathmatchObjective objective, params List<int>[] teams)
        {
            //check only one team has someone alive
            int teamsWithLiving = 0;

            foreach (var team in teams)
            {
                // does this team have at least one alive player? 0 = dead, 1 = alive
                if (team.Any(e => e == 1))
                {
                    teamsWithLiving++;

                    if (teamsWithLiving > 1)
                        return false;
                }
            }

            // objective completed only if exactly one team has living members
            return teamsWithLiving == 1;
        }
    }
}