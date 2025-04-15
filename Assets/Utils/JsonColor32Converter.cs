public class Color32Converter : JsonConverter<Color32>
{
	public override Color32 ReadJson(JsonReader reader, Type objectType, Color32 existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		var obj = serializer.Deserialize<Dictionary<string, byte>>(reader);
		return new Color32(obj["r"], obj["g"], obj["b"], obj["a"]);
	}
}