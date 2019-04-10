using System;
using System.ComponentModel.DataAnnotations;
using SQLite;

namespace VisionTrainer.Models
{
	public enum MediaFileType
	{
		Image,
		Video
	}

	public class MediaFile
	{
		#region New additions
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int GroupId { get; set; }
		public bool Complete { get; set; }
		public string Tag { get; set; }
		#endregion

		public string PreviewPath { get; set; }
		public string Path { get; set; }
		public bool Processed { get; set; }


		[System.ComponentModel.DataAnnotations.Required]
		public virtual int MediaFileTypeId
		{
			get { return (int)this.Type; }
			set { Type = (MediaFileType)value; }
		}
		[EnumDataType(typeof(MediaFileType))]
		public MediaFileType Type { get; set; }
		//public MediaFileType Type { get; set; }
	}
}
