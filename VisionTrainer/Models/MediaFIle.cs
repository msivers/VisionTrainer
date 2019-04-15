using System.ComponentModel.DataAnnotations;
using SQLite;
using VisionTrainer.Common.Enums;
using VisionTrainer.Utils;

namespace VisionTrainer.Models
{
	public class MediaFile
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int GroupId { get; set; }
		public bool Complete { get; set; }
		public string PreviewPath { get; set; }
		public string Path { get; set; }
		public string Tags { get; set; }
		public double LocationLatitude { get; set; }
		public double LocationLongitude { get; set; }

		[EnumDataType(typeof(MediaFileType))]
		public MediaFileType Type { get; set; }

		[Required]
		public virtual int MediaFileTypeId
		{
			get { return (int)this.Type; }
			set { Type = (MediaFileType)value; }
		}

		[Ignore]
		public string FullPath { get { return FileHelper.GetFullPath(Path); } }
		[Ignore]
		public string FullPreviewPath { get { return FileHelper.GetFullPath(PreviewPath); } }

		#region Helper Methods
		//public static MediaFile FromMediaData()
		//{

		//}

		//public static MediaData ToMediaData()
		//{

		//}
		#endregion
	}
}
