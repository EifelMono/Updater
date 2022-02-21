using System.Text.Json;
using System.Text.Json.Serialization;
using Updater.CoreLib.udp;

public class BroadcastMessage
{
    [JsonPropertyName("C")]
    public Command Command { get; set; }
    [JsonPropertyName("M")]
    public string MachineName { get; set; }
}

public static class BroadcastMessageExtension
{
    public static string ToJson(this BroadcastMessage thisValue)
    {
        return JsonSerializer.Serialize(thisValue);
    }
    public static T FromJson<T>(this string thisValue) where T : class
    {
        return JsonSerializer.Deserialize<T>(thisValue);
    }
}
