using System;
using Newtonsoft.Json;

namespace VisionTrainer.Common.Models
{
	public class GeoLocation
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		double latitude;

		[JsonProperty("latitude")]
		public double Latitude
		{
			get { return latitude; }
			set
			{
				if (value < -90 || value > 90)
					throw new ArgumentOutOfRangeException("Latitude", value, "Value must be between -90 and 90 inclusive.");

				if (double.IsNaN(value))
					throw new ArgumentException("Latitude must be a valid number.", "Latitude");

				latitude = value;
			}
		}

		double longitude;
		[JsonProperty("longitude")]
		public double Longitude
		{
			get { return longitude; }
			set
			{
				if (value < -180 || value > 180)
					throw new ArgumentOutOfRangeException("Longitude", value, "Value must be between -180 and 180 inclusive.");

				if (double.IsNaN(value))
					throw new ArgumentException("Longitude must be a valid number.", "Longitude");

				longitude = value;
			}
		}

		public GeoLocation() : this(0, 0)
		{
		}

		public GeoLocation(double latitude, double longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
		}

		protected virtual double ToRadian(double val)
		{
			return (Math.PI / 180.0) * val;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as GeoLocation);
		}

		public bool Equals(GeoLocation coor)
		{
			if (coor == null)
				return false;

			return (this.Latitude == coor.Latitude && this.Longitude == coor.Longitude);
		}

		public override int GetHashCode()
		{
			return Latitude.GetHashCode() ^ Latitude.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0},{1}", latitude, longitude);
		}

		public static GeoLocation Parse(string value)
		{
			var values = value.Split(new char[] { ',' });
			if (values.Length != 2)
				throw new Exception("Unable To Parse GeoLocation String");

			return new GeoLocation(double.Parse(values[0]), double.Parse(values[1])); ;
		}
	}
}
