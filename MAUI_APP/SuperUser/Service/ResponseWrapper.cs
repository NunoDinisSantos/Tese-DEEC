using System.Text.Json.Serialization;

namespace SuperUser.Service
{
    internal class ResponseWrapper<T>
    {
        [JsonPropertyName("items")]
        public List<T> Items { get; set; }
    }
}