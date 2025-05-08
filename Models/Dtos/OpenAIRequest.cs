using System;
using System.Text.Json.Serialization;

[Serializable]
public class OpenAIRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("inputs")]
    public Input[] Inputs { get; set; }

    [Serializable]
    public class Input
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public Content[] Content { get; set; }
    }

    [Serializable]
    public class Content
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }
    }
}
