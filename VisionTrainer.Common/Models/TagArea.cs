using System;
using System.Drawing;
using Newtonsoft.Json;

namespace VisionTrainer.Common.Models
{
	public class TagArea
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("area")]
		public Rectangle Area { get; set; } // To be sent as percentages
	}
}
