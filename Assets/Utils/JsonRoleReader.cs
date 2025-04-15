public class JsonRoleReader
{
    public static List<Role> LoadRolesFromFile(string filePath)
    {
        private string filePath = "";

        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found: {filePath}");
            return new List<Role>();
        }

        string json = File.ReadAllText(filePath);

        // Custom color converter needed for Color32
        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new Color32Converter() }
        };

        List<Role> roles = JsonConvert.DeserializeObject<List<Role>>(json, settings);
        return roles;
    }
}