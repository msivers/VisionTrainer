using System;
using Newtonsoft.Json;

namespace VisionTrainer.Common.Messages
{
	public class PredictionModelResponse : BaseResponse
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("isAvailable")]
		public bool IsAvailable { get; set; }

		[JsonProperty("path")]
		public string Path { get; set; }
	}
}
