using RogueSharp;
using RogueSharp.MapCreation;
using SimArena.Core;

namespace SimArena.Examples;

public static class MapCreationStrategiesDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== Map Creation Strategies Demonstration ===");
        Console.WriteLine("This demo shows different map creation strategies...");
        
        // 1. Test with random rooms strategy (default)
        Console.WriteLine("\nTesting with RandomRoomsMapCreationStrategy (default):");
        var randomRoomsSimulation = new Simulation(40, 30);
        SimulationTests.RunSimulationTest(randomRoomsSimulation, null, "random_rooms");
        
        // 2. Test with cellular automata strategy
        Console.WriteLine("\nTesting with CellularAutomataMapCreationStrategy:");
        var cellularStrategy = new CaveMapCreationStrategy<Map>(40, 30, 45, 2, 4);
        var cellularSimulation = new Simulation(40, 30, cellularStrategy);
        SimulationTests.RunSimulationTest(cellularSimulation, null, "cellular_automata");
        
        // 3. Test with a pre-created map
        Console.WriteLine("\nTesting with a pre-created map:");
        var mapCreator = new RandomRoomsMapCreationStrategy<Map>(40, 30, 5, 10, 8);
        var preCreatedMap = mapCreator.CreateMap();
        
        // Modify the map in some way to demonstrate it's pre-created
        preCreatedMap.SetCellProperties(20, 15, true, true);
        var preCreatedMapSimulation = new Simulation(preCreatedMap);
        SimulationTests.RunSimulationTest(preCreatedMapSimulation, null, "pre_created");
    }
}