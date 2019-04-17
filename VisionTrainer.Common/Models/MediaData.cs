using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VisionTrainer.Common.Enums;

namespace VisionTrainer.Common.Models
{
	public class MediaData
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("submissionId")]
		public string UserId { get; set; }

		[JsonProperty("location")]
		public GeoLocation Location { get; set; }

		[JsonProperty("tags")]
		public TagArea[] Tags { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public MediaFileType MediaType { get; set; }
	}
}
