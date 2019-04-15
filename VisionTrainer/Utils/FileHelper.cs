using System;
using System.IO;
using VisionTrainer.Common.Enums;

namespace VisionTrainer.Utils
{
	public static class FileHelper
	{
		public static string GetUniquePath(MediaFileType type, string path, string name)
		{
			string ext = Path.GetExtension(name);
			if (ext == string.Empty)
				ext = ((type == MediaFileType.Image) ? ".jpg" : ".mp4");

			name = Path.GetFileNameWithoutExtension(name);

			string nname = name + ext;
			int i = 1;
			while (File.Exists(Path.Combine(path, nname)))
				nname = name + "_" + (i++) + ext;

			return Path.Combine(path, nname);
		}


		public static string GetOutputPath(MediaFileType type, string path, string name)
		{
			var fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), path);
			Directory.CreateDirectory(fullPath);

			if (string.IsNullOrWhiteSpace(name))
			{
				string timestamp = DateTime.Now.ToString("yyyMMdd_HHmmss");
				if (type == MediaFileType.Image)
					name = "IMG_" + timestamp + ".jpg";
				else
					name = "VID_" + timestamp + ".mp4";
			}

			return GetUniquePath(type, path, name);
		}

		public static string GetFullPath(string path)
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), path);
		}

		public static string GetOutputPath(object image, string directoryName, string v)
		{
			throw new NotImplementedException();
		}
	}
}