using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using SimArena;
using SimArena.Core.Configuration;
using SimArena.Core.Entities;
using SimArena.Core.Serialization.Configuration;

public static class Program
{
    /// <summary>
    /// Entry point for the application
    /// </summary>
    /// <param name="args">Command line arguments</param>
    private static void Main(string[] args)
    {
        // Run the simulation tests
        RunSimulationTests();
    }
    
    /// <summary>
    /// Runs all simulation tests
    /// </summary>
    private static void RunSimulationTests()
    {
        Console.WriteLine("=== SIMULATION TESTS ===\n");
        
        // Test with Steps objective
        SimulationTests.TestSimulationWithStepsObjective();
        
        // Test with CapturePoint objective
        //SimulationTests.TestSimulationWithCapturePointObjective();
        
        // Test with Defend objective
        SimulationTests.TestSimulationWithDefendObjective();
        
        // Test with human agents in offline mode (should throw an exception)
        SimulationTests.TestSimulationWithHumanAgentsInOfflineMode();
        
        // Test with Deathmatch objective
        //SimulationTests.TestSimulationWithDeathmatchObjective();
        
        Console.WriteLine("All simulation tests completed!");
    }

    private static void JsonTest()
    {
        //test
        string json = "{\n\"Type\": \"DefendObjective\",\n" +
                      "\"Teams\": 2,\n" +
                      "\"PlayersPerTeam\": 5,\n" +
                      "\"MaxMatchTime\": 120.0\n}";
        
        Console.WriteLine("Serialized JSON:\n\n");
        Console.WriteLine(json);
        
        //now let's try to deserialize it back to an Objective
        try
        {
            ObjectiveConfig deserializedObj = JsonSerializer.Deserialize<ObjectiveConfig>(json);
            Console.WriteLine("\n\nDeserialized Objective:\n");
            Console.WriteLine(deserializedObj is DefendObjective);
            Console.WriteLine(deserializedObj.GetType());

            if (deserializedObj is DefendObjective stepsObjective)
            {
                Console.WriteLine("Max Teams: " + stepsObjective.Teams);
                Console.WriteLine("Max Match Time: " + stepsObjective.MaxMatchTime);
            }
            
            Console.WriteLine(deserializedObj.TypeEnum);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static void TestEvaluationSteps()
    {
        StepsObjective objective = new StepsObjective(SimulationObjective.Steps, 10);

        Console.WriteLine("Starting Evaluation...");
        Console.WriteLine("Objective Type: " + objective.TypeEnum);
        Console.WriteLine("Max Steps: " + objective.MaxSteps);
        Console.WriteLine();
        
        int stepsTaken = 0;
        
        while (!ObjectiveEvaluator.EvaluateObjective(objective, stepsTaken))
        {
            Console.WriteLine($"Steps: {stepsTaken}/{objective.MaxSteps}");
            stepsTaken++;
        }
        
        Console.WriteLine($"Steps: {stepsTaken}/{objective.MaxSteps}");
        
        Console.WriteLine();
        Console.WriteLine("Objective Reached!");
        Console.WriteLine("Ending Evaluation...");
    }

    private static void TestEvaluationCapture()
    {
        CapturePointObjective objective = new CapturePointObjective(SimulationObjective.CapturePoint, 2, 
            5, 5f, 10f);

        bool ended = false;
        var timeElapsed = 0;
        var captureElapsed = 0;
        Random random = new Random(Guid.NewGuid().GetHashCode());
        
        Console.WriteLine("Starting Evaluation...");
        Console.WriteLine("Objective Type: " + objective.TypeEnum);
        Console.WriteLine("Max Capture Time: " + objective.CaptureTime);
        Console.WriteLine();
        
        while (ended == false)
        {
            //using a very simple random to decide whether or not we're in range of the capture point
            bool inRange = random.Next(1, 3) == 2;
            
            if (inRange)
            {
                Console.WriteLine("We're in capture range...");
                captureElapsed += 1; //needs to be in seconds
                Console.WriteLine($"Capture Elapsed: {captureElapsed}s\n");
            }
            else
            {
                Console.WriteLine("Not in capture range...\n");
            }

            timeElapsed += 1;
            
            ended = ObjectiveEvaluator.EvaluateObjective(objective, captureElapsed);
            
            Thread.Sleep(1000);
        }
        
        Console.WriteLine("Objective Reached!");
        Console.WriteLine($"Total Time Elapsed: {timeElapsed}s");
        Console.WriteLine("Ending Evaluation...");
    }

    private static void TestEvaluationDeathmatch()
    {
        string GetStateText(int state) => state == 1 ? "Alive" : "Dead";

        DeathmatchObjective objective = new DeathmatchObjective(SimulationObjective.TeamDeathmatch, 2,
            2);
        
        //create the teams and players, pretty simply
        //0 = dead, 1 = alive

        Dictionary<string, int> teamOne = new Dictionary<string, int>
        {
            { "Ana", 1 },
            { "Baptiste", 1 }
        };

        Dictionary<string, int> teamTwo = new Dictionary<string, int>
        {
            { "Jett", 1 },
            { "Omen", 1 }
        };

        List<int> teamOneState = teamOne.Select(pair => pair.Value).ToList();
        List<int> teamTwoState = teamTwo.Select(pair => pair.Value).ToList();
        
        Random random = new Random();
        Random randPlayer = new Random(Guid.NewGuid().GetHashCode());
        Random randTarget = new Random(51);
        
        Console.WriteLine("Starting Evaluation...");
        Console.WriteLine("Objective Type: " + objective.TypeEnum);
        Console.WriteLine("Only one team can be alive!\n");
        
        Console.WriteLine($"Teams:\n[TEAM ONE]\n" +
                          $"{teamOne.ElementAt(0).Key}\n" +
                          $"{teamOne.ElementAt(1).Key}\n" +
                          $"\n[TEAM TWO]\n" +
                          $"{teamTwo.ElementAt(0).Key}\n" +
                          $"{teamTwo.ElementAt(1).Key}\n" +
                          $"\n-------------------------\n");

        while (!ObjectiveEvaluator.EvaluateObjective(objective, teamOneState, teamTwoState))
        {
            //choose a random team
            bool isTeamOneAttacking = random.Next(0, 2) == 0;
            
            var attackingTeam = isTeamOneAttacking ? teamOne : teamTwo;
            var defendingTeam = isTeamOneAttacking ? teamTwo : teamOne;
            
            //only alive characters can kill or die
            string[] aliveAttackers = attackingTeam.Where(player => player.Value == 1)
                .Select(player => player.Key).ToArray(); 
                
            string[] aliveDefenders = defendingTeam.Where(player => player.Value == 1)
                .Select(player => player.Key).ToArray();
            
            if (aliveAttackers.Any() && aliveDefenders.Any())
            {
                // Randomly pick an attacker and a target
                var attacker = aliveAttackers[randPlayer.Next(aliveAttackers.Length)];
                var target = aliveDefenders[randTarget.Next(aliveDefenders.Length)];

                Console.WriteLine($"{attacker} from {(isTeamOneAttacking ? "Team 1" : "Team 2")} kills {target} from " +
                                  $"{(isTeamOneAttacking ? "Team 2" : "Team 1")}!");

                defendingTeam[target] = 0; // Mark target as dead
            }
            
            //update list with new states
            teamOneState = teamOne.Select(pair => pair.Value).ToList();
            teamTwoState = teamTwo.Select(pair => pair.Value).ToList();
        }
        
        Console.WriteLine("\n-------------------------\n");
        Console.WriteLine("Objective Reached!");
        Console.WriteLine($"Winning Team: {(teamOneState.Contains(1) ? "Team One" : "Team Two")}!");
        
        Console.WriteLine($"\nCharacter States:\n[TEAM ONE]\n" +
                          $"{teamOne.ElementAt(0).Key} : {GetStateText(teamOne.ElementAt(0).Value)}\n" +
                          $"{teamOne.ElementAt(1).Key} : {GetStateText(teamOne.ElementAt(1).Value)}\n" +
                          $"\n[TEAM TWO]\n" +
                          $"{teamTwo.ElementAt(0).Key} : {GetStateText(teamTwo.ElementAt(0).Value)}\n" +
                          $"{teamTwo.ElementAt(1).Key} : {GetStateText(teamTwo.ElementAt(1).Value)}");
        
        Console.WriteLine("\nEnding Evaluation...");
    }

    private static void TestEvaluationDefense()
    {
        DefendObjective objective = new DefendObjective(SimulationObjective.DefendObjective, 2, 
            5, 10f);
        
        bool ended = false;
        var timeElapsed = 0;
        var objectiveHealth = 100;
        Random random = new Random(Guid.NewGuid().GetHashCode());
        
        Console.WriteLine("Starting Evaluation...");
        Console.WriteLine("Objective Type: " + objective.TypeEnum);
        Console.WriteLine("Countdown Time: " + objective.MaxMatchTime);
        Console.WriteLine();

        while (ended == false)
        {
            //using a very simple random to decide whether or not the enemy team has damaged our objective
            bool attack = random.Next(1, 4) == 2;
            
            if (attack)
            {
                Console.WriteLine("Offense hits the objective.");
                objectiveHealth -= 20;
                Console.WriteLine($"Defense Objective Health: {objectiveHealth}\n");
            }
            else
            {
                Console.WriteLine("Defense holds steadfast!\n");
            }

            timeElapsed += 1;
            
            ended = ObjectiveEvaluator.EvaluateObjective(objective, timeElapsed, objectiveHealth);
            
            Thread.Sleep(1000);
        }
        
        Console.WriteLine("Objective Reached!");
        Console.WriteLine($"Total Time Elapsed: {timeElapsed}s\n");

        if (objectiveHealth <= objective.ObjectiveThreshold)
        {
            Console.WriteLine("Offense won by destroying the objective!");
        }
        else if (timeElapsed >= objective.MaxMatchTime)
        {
            Console.WriteLine("Defense won by holding off the offense!");
        }
        
        Console.WriteLine("\nEnding Evaluation...");
    }
}