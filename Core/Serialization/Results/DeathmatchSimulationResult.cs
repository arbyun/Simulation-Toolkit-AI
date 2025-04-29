using System.Text;
using SimArena.Core.Serialization.Results;

namespace SimArena.Core.Configuration
{
    public class DeathmatchSimulationResult : ISimulationResult
    {
        public record TeamMember(string Name, KDA Kda);
        public record KDA(int Kills, int Deaths, int Assists);
        public record Team(int Number, KDA CollectiveKda, TeamMember[] TeamMembers);

        public int WinnerTeam { get; set; }
        public int TotalTeams { get; set; }
        public Team[]? Teams { get; set; }

        public DeathmatchSimulationResult(int winnerTeam, int totalTeams, Team[]? teams)
        {
            WinnerTeam = winnerTeam;
            TotalTeams = totalTeams;
            Teams = teams;
        }

        public string Read()
        {
            if (Teams == null) return $"Winner: Team {WinnerTeam}\nTotal Teams: {TotalTeams}";

            var sb = new StringBuilder();
            sb.AppendLine($"Winner: Team {WinnerTeam}");
            sb.AppendLine($"Total Teams: {TotalTeams}");

            foreach (var team in Teams)
            {
                sb.AppendLine($"\n[TEAM {team.Number}]");
                sb.AppendLine($"Team KDA: {team.CollectiveKda.Kills}/{team.CollectiveKda.Deaths}/{team.CollectiveKda.Assists}");
                sb.AppendLine("[TEAM MEMBERS]");

                foreach (var member in team.TeamMembers)
                {
                    sb.AppendLine($"Name: {member.Name}");
                    sb.AppendLine($"KDA: {member.Kda.Kills}/{member.Kda.Deaths}/{member.Kda.Assists}");
                    sb.AppendLine("--------------------");
                }

                sb.AppendLine("----------------");
            }

            return sb.ToString();
        }

        public object ToSerializable() => new
        {
            WinnerTeam,
            TotalTeams,
            Teams
        };
    }
}