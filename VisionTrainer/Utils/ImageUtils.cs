using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using SkiaSharp;

namespace VisionTrainer.Utils
{
	public enum ExifOrientation
	{
		[Description("Top, left side (Horizontal / normal)")]
		Normal,

		[Description("Top, right side (Mirror horizontal)")]
		MirrorHorizontal,

		[Description("Right side, top (Rotate 90 CW)")]
		Rotate90,

		[Description("Right side, bottom (Mirror horizontal and rotate 90 CW)")]
		Rotate90MirrorHorizontal,

		[Description("Bottom, right side (Rotate 180)")]
		Rotate180,

		[Description("Bottom, left side (Mirror vertical)")]
		MirrorVertical,

		[Description("Left side, bottom (Rotate 270 CW)")]
		Rotate270,

		[Description("Left side, top (Mirror horizontal and rotate 270 CW)")]
		Rotate270MirrorHorizontal
	}

	public static class ImageUtils
	{
		public static ExifOrientation GetImageOrientation(string filePath)
		{
			// Handle Exif information
			IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(filePath);
			var subIfdDirectory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
			var orientation = subIfdDirectory?.GetDescription(ExifDirectoryBase.TagOrientation);
			Console.WriteLine("Orientation:" + orientation);
			return ParseExifDescription(orientation);
		}

		public static ExifOrientation ParseExifDescription(string value)
		{
			switch (value)
			{
				case "Top, right side (Mirror horizontal)":
					return ExifOrientation.MirrorHorizontal;

				case "Right side, top (Rotate 90 CW)":
					return ExifOrientation.Rotate90;

				case "Right side, bottom (Mirror horizontal and rotate 90 CW)":
					return ExifOrientation.Rotate90MirrorHorizontal;

				case "Bottom, right side (Rotate 180)":
					return ExifOrientation.Rotate180;

				case "Bottom, left side (Mirror vertical)":
					return ExifOrientation.MirrorVertical;

				case "Left side, bottom (Rotate 270 CW)":
					return ExifOrientation.Rotate270;

				case "Left side, top (Mirror horizontal and rotate 270 CW)":
					return ExifOrientation.Rotate270MirrorHorizontal;

				// Top, left side (Horizontal / normal)
				default:
					return ExifOrientation.Normal;
			}
		}

		public static SKBitmap HandleOrientation(SKBitmap bitmap, ExifOrientation exifOrientation)
		{
			SKBitmap rotated;
			switch (exifOrientation)
			{
				case ExifOrientation.Rotate180:

					using (var surface = new SKCanvas(bitmap))
					{
						surface.RotateDegrees(180, bitmap.Width / 2, bitmap.Height / 2);
						surface.DrawBitmap(bitmap.Copy(), 0, 0);
					}

					return bitmap;

				case ExifOrientation.MirrorHorizontal:
					rotated = new SKBitmap(bitmap.Height, bitmap.Width);

					using (var surface = new SKCanvas(rotated))
					{
						surface.Translate(rotated.Width, 0);
						surface.RotateDegrees(90);
						surface.DrawBitmap(bitmap, 0, 0);
					}

					return rotated;

				case ExifOrientation.MirrorVertical:
					rotated = new SKBitmap(bitmap.Height, bitmap.Width);

					using (var surface = new SKCanvas(rotated))
					{
						surface.Translate(0, rotated.Height);
						surface.RotateDegrees(270);
						surface.DrawBitmap(bitmap, 0, 0);
					}

					return rotated;

				case ExifOrientation.Rotate90:

					rotated = new SKBitmap(bitmap.Height, bitmap.Width);

					using (var surface = new SKCanvas(rotated))
					{
						surface.Translate(rotated.Width, 0);
						surface.RotateDegrees(90);
						surface.DrawBitmap(bitmap, 0, 0);
					}

					return rotated;

				default:
					return bitmap;
			}
		}
	}
}