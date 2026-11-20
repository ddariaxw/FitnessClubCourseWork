using System.Text.Json;

namespace FitnessClub.Core.Services
{

    public class JsonFileService<T>
    {
        private readonly string _filePath;

        public string FilePath => _filePath;

        public JsonFileService(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Шлях до файлу не може бути порожнім.", nameof(filePath));

            _filePath = filePath;
        }

        public void Save(List<T> data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "Список даних не може бути null.");

            string? directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(_filePath, json);
        }

        public List<T> Load()
        {
            if (!File.Exists(_filePath))
                return new List<T>();

            string json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json))
                return new List<T>();

            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
    }
}