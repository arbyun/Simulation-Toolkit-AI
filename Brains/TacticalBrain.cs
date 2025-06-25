using RogueSharp;
using SimArena.Core;
using SimArena.Core.Analysis;

namespace SimArena.Brains
{
    /// <summary>
    /// An advanced brain that uses death density information to make tactical decisions
    /// Demonstrates how to leverage the DeathAnalysis for strategic gameplay
    /// </summary>
    public class TacticalBrain : Brain
    {
        private int _lastAnalysisStep = 0;
        private const int ANALYSIS_INTERVAL = 10;
        
        public TacticalBrain(IMap map, int team, int tickIntervalMs = 500) : base(map, team, tickIntervalMs)
        {
        }

        protected override void ExecuteThink()
        {
            if (Simulation == null)
            {
                // Fallback to random behavior if no simulation reference
                MoveRandomly();
                return;
            }

            var deathAnalysis = Simulation.GetMapAnalyzer<DeathAnalysis>();
            if (deathAnalysis == null || !deathAnalysis.IsInitialized)
            {
                // Fallback to random behavior if no death analysis 
                MoveRandomly();
                return;
            }

            // Periodically analyze the battlefield
            _lastAnalysisStep++;
            if (_lastAnalysisStep >= ANALYSIS_INTERVAL)
            {
                AnalyzeBattlefield(deathAnalysis);
                _lastAnalysisStep = 0;
            }

            // Make tactical decisions based on current position and death data
            MakeTacticalDecision(deathAnalysis);
        }

        private void AnalyzeBattlefield(DeathAnalysis deathAnalysis)
        {
            var dangerousAreas = deathAnalysis.GetMostDenseAreas(radius: 3, topCount: 3);
            
            if (dangerousAreas.Count > 0)
            {
                var mostDangerous = dangerousAreas[0];
                
                // We aren't doing anything with this, this is just an example of how you could store
                // this information for decision making (or for a heatmap)
            }
        }

        private void MakeTacticalDecision(DeathAnalysis deathAnalysis)
        {
            var currentX = Agent.X;
            var currentY = Agent.Y;
            
            // Assess risk of current position
            var deathsNearby = deathAnalysis.GetDataInRadius(currentX, currentY, 2);
            var riskLevel = CalculateLocalRisk(deathsNearby.Count);

            // Decision making based on risk level
            if (riskLevel > 0.8) // High risk area
            {
                // Try to move to a safer area
                MoveToSaferArea(deathAnalysis);
            }
            else if (riskLevel < 0.3) // Low risk area
            {
                // Move towards areas with potential action but not too dangerous
                MoveToOpportunityArea(deathAnalysis);
            }
            else
            {
                // Move randomly but avoid high-risk areas
                MoveRandomlyAvoidingDanger(deathAnalysis);
            }
        }

        private void MoveToSaferArea(DeathAnalysis deathAnalysis)
        {
            var currentX = Agent.X;
            var currentY = Agent.Y;
            var bestX = currentX;
            var bestY = currentY;
            var lowestRisk = double.MaxValue;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    var newX = currentX + dx;
                    var newY = currentY + dy;

                    if (IsValidMove(newX, newY))
                    {
                        var deathsInArea = deathAnalysis.GetDataInRadius(newX, newY, 2);
                        var risk = CalculateLocalRisk(deathsInArea.Count);

                        if (risk < lowestRisk)
                        {
                            lowestRisk = risk;
                            bestX = newX;
                            bestY = newY;
                        }
                    }
                }
            }

            if (bestX != currentX || bestY != currentY)
            {
                MoveTo(bestX, bestY);
            }
        }

        private void MoveToOpportunityArea(DeathAnalysis deathAnalysis)
        {
            // Look for areas with moderate death density (indicates action but not too dangerous)
            var densityAreas = deathAnalysis.GetMostDenseAreas(radius: 4, topCount: 10);
            
            if (densityAreas.Count > 0)
            {
                // Find a moderately active area that's not too close to avoid the highest risk
                var targetArea = densityAreas
                    .Where(area => area.DataCount >= 2 && area.DataCount <= 8) // Moderate activity
                    .OrderBy(area => GetDistance(Agent.X, Agent.Y, area.X, area.Y))
                    .FirstOrDefault();

                if (targetArea != null)
                {
                    MoveTowards(targetArea.X, targetArea.Y);
                    return;
                }
            }

            // Fallback to random movement
            MoveRandomly();
        }

        private void MoveRandomlyAvoidingDanger(DeathAnalysis deathAnalysis)
        {
            var currentX = Agent.X;
            var currentY = Agent.Y;
            var validMoves = new List<(int x, int y)>();

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    var newX = currentX + dx;
                    var newY = currentY + dy;

                    if (IsValidMove(newX, newY))
                    {
                        var deathsInArea = deathAnalysis.GetDataInRadius(newX, newY, 2);
                        var risk = CalculateLocalRisk(deathsInArea.Count);

                        // Only consider moves with acceptable risk
                        if (risk < 0.8)
                        {
                            validMoves.Add((newX, newY));
                        }
                    }
                }
            }

            if (validMoves.Count > 0)
            {
                var randomMove = validMoves[_random.Next(validMoves.Count)];
                MoveTo(randomMove.x, randomMove.y);
            }
            else
            {
                // If no safe moves, move randomly (emergency situation)
                MoveRandomly();
            }
        }

        private void MoveTowards(int targetX, int targetY)
        {
            var currentX = Agent.X;
            var currentY = Agent.Y;

            var dx = Math.Sign(targetX - currentX);
            var dy = Math.Sign(targetY - currentY);

            var newX = currentX + dx;
            var newY = currentY + dy;

            if (IsValidMove(newX, newY))
            {
                MoveTo(newX, newY);
            }
            else
            {
                // Try alt moves
                if (IsValidMove(currentX + dx, currentY))
                {
                    MoveTo(currentX + dx, currentY);
                }
                else if (IsValidMove(currentX, currentY + dy))
                {
                    MoveTo(currentX, currentY + dy);
                }
            }
        }

        private void MoveRandomly()
        {
            var currentX = Agent.X;
            var currentY = Agent.Y;

            var dx = _random.Next(-1, 2);
            var dy = _random.Next(-1, 2);

            var newX = currentX + dx;
            var newY = currentY + dy;

            if (IsValidMove(newX, newY))
            {
                MoveTo(newX, newY);
            }
        }

        private bool IsValidMove(int x, int y)
        {
            if (x < 0 || x >= _map.Width || y < 0 || y >= _map.Height)
                return false;

            return _map.IsWalkable(x, y);
        }

        private double CalculateLocalRisk(int deathCount)
        {
            // Convert death count to risk score? (0.0 to 1.0)
            return Math.Min(1.0, deathCount / 10.0);
        }

        private double GetDistance(int x1, int y1, int x2, int y2)
        {
            var dx = x1 - x2;
            var dy = y1 - y2;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}