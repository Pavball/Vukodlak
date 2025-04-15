public class JsonRoleReader
{
    public static List<Role> LoadRolesFromFile()
    {
        private string filePath = Path.Combine(Application.persistentDataPath, "Roles.json");

        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found: {filePath}");
            return new List<Role>();
        }

        string json = File.ReadAllText(filePath);

        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new Color32Converter() }
        };

        List<Role> roles = JsonConvert.DeserializeObject<List<Role>>(json, settings);
        return roles;
    }
}