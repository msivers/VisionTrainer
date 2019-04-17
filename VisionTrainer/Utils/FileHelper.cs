﻿using System;
using System.IO;
using VisionTrainer.Common.Enums;

namespace VisionTrainer.Utils
{
	public static class FileHelper
	{
		public static string GetUniqueName(MediaFileType type, string name)
		{
			string ext = Path.GetExtension(name);
			if (ext == string.Empty)
				ext = ((type == MediaFileType.Image) ? ".jpg" : ".mp4");

			name = Path.GetFileNameWithoutExtension(name);

			string nname = name + ext;
			int i = 1;
			while (File.Exists(nname))
				nname = name + "_" + (i++) + ext;

			return nname;
		}


		public static string GetOutputPath(MediaFileType type, string name)
		{
			var fullPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			if (string.IsNullOrWhiteSpace(name))
			{
				string timestamp = DateTime.Now.ToString("yyyMMdd_HHmmss");
				if (type == MediaFileType.Image)
					name = "IMG_" + timestamp + ".jpg";
				else
					name = "VID_" + timestamp + ".mp4";
			}

			return GetUniqueName(type, name);
		}

		public static string GetFullPath(string path)
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), path);
		}
	}
}