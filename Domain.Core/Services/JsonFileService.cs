using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Core.Exceptions;

namespace Domain.Core.Services
{
    /// <summary>
    /// Узагальнений сервіс серіалізації списку об'єктів у JSON-файл і назад.
    /// </summary>
    public class JsonFileService<T>
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter() }
        };

        private readonly string _filePath;

        public JsonFileService(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Шлях до файлу не може бути порожнім.", nameof(filePath));

            _filePath = filePath;
        }

        public void Save(List<T> data)
        {
            try
            {
                string? directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                string json = JsonSerializer.Serialize(data, Options);
                string tempPath = _filePath + ".tmp";
                File.WriteAllText(tempPath, json);
                File.Move(tempPath, _filePath, true);
            }
            catch (Exception ex) when (
                ex is IOException or UnauthorizedAccessException or NotSupportedException)
            {
                throw new DataStorageException($"Не вдалося зберегти файл «{_filePath}».", ex);
            }
        }

        public List<T> Load()
        {
            if (!File.Exists(_filePath))
                return new List<T>();

            try
            {
                string json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<T>>(json, Options) ?? new List<T>();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException
                or JsonException or ArgumentException or NotSupportedException)
            {
                throw new DataStorageException($"Не вдалося прочитати файл «{_filePath}»: {ex.Message}", ex);
            }
        }
    }
}
