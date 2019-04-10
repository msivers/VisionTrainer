using System;
using System.Collections.Generic;
using SQLite;

namespace VisionTrainer.Models
{
	public class BatchGroup
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }
		public string Tag { get; set; }
		public List<MediaFile> Media { get; set; }
		public bool Complete { get; set; }
	}
}
