using VisionTrainer.Common.Enums;
using VisionTrainer.Common.Models;
using VisionTrainer.Utils;

namespace VisionTrainer.Models
{
	public class MediaDetails
	{
		public int Id { get; set; }
		public int GroupId { get; set; }
		public bool Complete { get; set; }
		public string PreviewPath { get; set; }
		public string Path { get; set; }
		public TagArea[] Tags { get; set; }
		public GeoLocation Location { get; set; }
		public MediaFileType Type { get; set; }

		public string FullPath { get { return FileHelper.GetFullPath(this.Path); } }
		public string FullPreviewPath { get { return FileHelper.GetFullPath(this.PreviewPath); } }
	}
}
