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
		public RectangleF Area { get; set; } // To be sent as percentages

		public override string ToString()
		{
			return string.Format("{0},{1},{2},{3},{4}", Id, Area.X, Area.Y, Area.Width, Area.Height);
		}

		public static TagArea Parse(string value)
		{
			var values = value.Split(new char[] { ',' });
			if (values.Length < 5)
				throw new Exception("Unable To Parse TagArea String");

			var result = new TagArea();
			result.Id = values[0];

			result.Area = new RectangleF()
			{
				X = float.Parse(values[1]),
				Y = float.Parse(values[2]),
				Width = float.Parse(values[3]),
				Height = float.Parse(values[4])
			};

			return result;
		}
	}
}
