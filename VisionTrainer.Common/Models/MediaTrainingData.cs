using System;
using Newtonsoft.Json;

namespace VisionTrainer.Common.Models
{
	public class MediaTrainingData : MediaData
	{
		[JsonProperty("submissionDate")]
		public DateTime SubmissionDate { get; set; }

		[JsonProperty("fileName")]
		public string FileName { get; set; }

		[JsonProperty("tags")]
		public TagArea[] Tags { get; set; }
	}
}
