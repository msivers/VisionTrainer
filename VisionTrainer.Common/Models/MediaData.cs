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
		public string SubmissionId { get; set; }

		[JsonProperty("location")]
		public GeoLocation Location { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public MediaFileType MediaType { get; set; }
	}
}
