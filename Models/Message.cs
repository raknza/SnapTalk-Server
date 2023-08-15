
using System.Text.Json.Serialization;

namespace android_backend.Models;

public class Message
{
    [JsonPropertyName("id")]
    public int id { get; set; }
    
    [JsonPropertyName("senderId")]
    public int senderId { get; set; }
    
    [JsonPropertyName("recipientId")]
    public int recipientId { get; set; }
    
    [JsonPropertyName("content")]
    public string content { get; set; }
    
    [JsonPropertyName("messageType")]
    public MessageType messageType { get; set; }
    
    [JsonPropertyName("timestamp")]
    public String timestamp { get; set; }
    
    [JsonPropertyName("isRead")]
    public bool isRead { get; set; }
    
    [JsonPropertyName("isReceived")]
    public bool isReceived { get; set; }
}

public enum MessageType
{
    TEXT,
    IMAGE,
    FILE,
    LINK
}