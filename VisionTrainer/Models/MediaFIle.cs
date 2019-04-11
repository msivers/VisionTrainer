using System;
using System.ComponentModel.DataAnnotations;
using SQLite;
using VisionTrainer.Utils;

namespace VisionTrainer.Models
{
	public enum MediaFileType
	{
		Image,
		Video
	}

	public class MediaFile
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int GroupId { get; set; }
		public bool Complete { get; set; }
		public string Tag { get; set; }
		public string PreviewPath { get; set; }
		public string Path { get; set; }
		public bool Processed { get; set; }

		[EnumDataType(typeof(MediaFileType))]
		public MediaFileType Type { get; set; }

		[System.ComponentModel.DataAnnotations.Required]
		public virtual int MediaFileTypeId
		{
			get { return (int)this.Type; }
			set { Type = (MediaFileType)value; }
		}

		[Ignore]
		public string FullPath { get { return FileHelper.GetFullPath(Path); } }
		[Ignore]
		public string FullPreviewPath { get { return FileHelper.GetFullPath(PreviewPath); } }
	}
}
