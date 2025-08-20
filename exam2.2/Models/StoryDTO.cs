
using System.Text.Json.Serialization;


namespace Foody_exam_project.Models
{
    internal class StoryDTO
    {

        [JsonPropertyName("title")]
        public string? Title { get; set; }



        [JsonPropertyName("description")]
        public string? Description { get; set; }


        [JsonPropertyName("url")]
        public string? Url { get; set; }

    }
}
