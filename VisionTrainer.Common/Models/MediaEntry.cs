using System;
using Newtonsoft.Json;

namespace VisionTrainer.Common.Models
{
	public class MediaEntry : MediaData
	{
		[JsonProperty("submissionDate")]
		public DateTime SubmissionDate { get; set; }

		[JsonProperty("fileName")]
		public string FileName { get; set; }
	}
}
