
using System.Text.Json.Serialization;


namespace Foody_exam_project.Models
{
    internal class ApiResponseDTO
    {
        [JsonPropertyName("msg")]
        public string? Msg { get; set; }
        [JsonPropertyName("storyId")]
        public string? StoryId { get; set; }
    }
}
