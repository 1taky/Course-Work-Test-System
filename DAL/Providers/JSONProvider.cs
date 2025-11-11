using System.Text.Json;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Providers
{
    public class JsonProvider<T> : IDataProvider<T>
    {
        private readonly string _file;
        public JsonProvider(string file) { _file = file; }

        public void Save(List<T> data)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var dir = Path.GetDirectoryName(_file);

            // якщо шлях не вказано — нічого не створюємо
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(_file, JsonSerializer.Serialize(data, options));
        }

        public List<T> Load()
        {
            if (!File.Exists(_file)) return new List<T>();
            var text = File.ReadAllText(_file);
            return JsonSerializer.Deserialize<List<T>>(text) ?? new List<T>();
        }
    }
}
