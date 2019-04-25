namespace VisionTrainer.Common.Models
{
	using Newtonsoft.Json;

	public class MediaRecognition
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("project")]
		public string Project { get; set; }

		[JsonProperty("iteration")]
		public string Iteration { get; set; }

		[JsonProperty("created")]
		public string Created { get; set; }

		[JsonProperty("predictions")]
		public Prediction[] Predictions { get; set; }
	}

	public class Prediction
	{
		[JsonProperty("probability")]
		public long Probability { get; set; }

		[JsonProperty("tagId")]
		public string TagId { get; set; }

		[JsonProperty("tagName")]
		public string TagName { get; set; }

		[JsonProperty("boundingBox")]
		public BoundingBox BoundingBox { get; set; }
	}

	public class BoundingBox
	{
		[JsonProperty("left")]
		public long Left { get; set; }

		[JsonProperty("top")]
		public long Top { get; set; }

		[JsonProperty("width")]
		public long Width { get; set; }

		[JsonProperty("height")]
		public long Height { get; set; }
	}
}
