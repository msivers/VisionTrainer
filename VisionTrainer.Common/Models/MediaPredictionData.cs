using System;
using Newtonsoft.Json;

namespace VisionTrainer.Common.Models
{
	public class MediaPredictionData : MediaData
	{
		[JsonProperty("targetModelName")]
		public string TargetModelName { get; set; }
	}
}
