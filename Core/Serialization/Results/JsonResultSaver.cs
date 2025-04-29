using System.Text.Json;
using SimArena.Core.Configuration;

namespace SimArena.Core.Serialization.Results
{
    public class JsonResultSaver : IResultSaver
    {
        public void Save(ISimulationResult result, string path)
        {
            string json = Serialize(result);
            File.AppendAllText(path, json);
        }

        public void Save(ISimulationResult result)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "results");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string baseFileName = "SimulationResult";
            string timestamp = DateTime.Now.ToString("yyyyMMdd");
            int fileIndex = 1;
            string fileName;

            do
            {
                fileName = Path.Combine(folderPath, $"{baseFileName}{fileIndex:D2}_{timestamp}.json");
                fileIndex++;
            }
            while (File.Exists(fileName));

            File.WriteAllText(fileName, Serialize(result));
        }

        private string Serialize(ISimulationResult result)
        {
            return JsonSerializer.Serialize(result.ToSerializable(), new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }
}