using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using GMImagePicker;
using Photos;
using UIKit;
using VisionTrainer.Common.Enums;
using VisionTrainer.Interfaces;
using VisionTrainer.Models;
using VisionTrainer.Utils;

namespace VisionTrainer.iOS.Services
{
	public class MultiMediaPickerService : IMultiMediaPickerService
	{

		//Events
		public event EventHandler<MediaDetails> OnMediaPicked;
		public event EventHandler<IList<MediaDetails>> OnMediaPickedCompleted;

		GMImagePickerController currentPicker;
		TaskCompletionSource<IList<MediaDetails>> mediaPickTcs;

		public async Task<IList<MediaDetails>> PickPhotosAsync()
		{
			return await PickMediaAsync("Select Images", PHAssetMediaType.Image);
		}

		public async Task<IList<MediaDetails>> PickVideosAsync()
		{
			return await PickMediaAsync("Select Videos", PHAssetMediaType.Video);
		}

		async Task<IList<MediaDetails>> PickMediaAsync(string title, PHAssetMediaType type)
		{

			mediaPickTcs = new TaskCompletionSource<IList<MediaDetails>>();
			currentPicker = new GMImagePickerController()
			{
				Title = title,
				MediaTypes = new[] { type }
			};

			currentPicker.FinishedPickingAssets += FinishedPickingAssets;

			var window = UIApplication.SharedApplication.KeyWindow;
			var vc = window.RootViewController;
			while (vc.PresentedViewController != null)
			{
				vc = vc.PresentedViewController;
			}

			await vc.PresentViewControllerAsync(currentPicker, true);

			var results = await mediaPickTcs.Task;

			currentPicker.FinishedPickingAssets -= FinishedPickingAssets;
			OnMediaPickedCompleted?.Invoke(this, results);
			return results;
		}

		async void FinishedPickingAssets(object sender, MultiAssetEventArgs args)
		{
			IList<MediaDetails> results = new List<MediaDetails>();
			TaskCompletionSource<IList<MediaDetails>> tcs = new TaskCompletionSource<IList<MediaDetails>>();

			var options = new PHImageRequestOptions()
			{
				NetworkAccessAllowed = true
			};

			options.Synchronous = false;
			options.ResizeMode = PHImageRequestOptionsResizeMode.Fast;
			options.DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat;

			bool completed = false;
			for (var i = 0; i < args.Assets.Length; i++)
			{
				var asset = args.Assets[i];

				string fileName = string.Empty;
				if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
				{
					fileName = PHAssetResource.GetAssetResources(asset).FirstOrDefault().OriginalFilename;
				}

				switch (asset.MediaType)
				{
					case PHAssetMediaType.Video:

						PHImageManager.DefaultManager.RequestImageForAsset(asset, new SizeF(150.0f, 150.0f),
							PHImageContentMode.AspectFill, options, async (img, info) =>
							{
								var startIndex = fileName.IndexOf(".", StringComparison.CurrentCulture);

								string relativePath = "";
								if (startIndex != -1)
								{
									relativePath = FileHelper.GetOutputPath(MediaFileType.Image, $"{fileName.Substring(0, startIndex)}-THUMBNAIL.JPG");
								}
								else
								{
									relativePath = FileHelper.GetOutputPath(MediaFileType.Image, string.Empty);
								}

								if (!File.Exists(relativePath))
								{
									var fullPath = FileHelper.GetFullPath(relativePath);
									img.AsJPEG().Save(fullPath, true);
								}

								TaskCompletionSource<string> tvcs = new TaskCompletionSource<string>();

								var vOptions = new PHVideoRequestOptions();
								vOptions.NetworkAccessAllowed = true;
								vOptions.Version = PHVideoRequestOptionsVersion.Original;
								vOptions.DeliveryMode = PHVideoRequestOptionsDeliveryMode.FastFormat;

								PHImageManager.DefaultManager.RequestAvAsset(asset, vOptions, (avAsset, audioMix, vInfo) =>
								{
									var vPath = FileHelper.GetOutputPath(MediaFileType.Video, fileName);

									if (!File.Exists(vPath))
									{
										AVAssetExportSession exportSession = new AVAssetExportSession(avAsset, AVAssetExportSession.PresetHighestQuality);

										exportSession.OutputUrl = NSUrl.FromFilename(vPath);
										exportSession.OutputFileType = AVFileType.QuickTimeMovie;

										exportSession.ExportAsynchronously(() =>
										{
											Console.WriteLine(exportSession.Status);

											tvcs.TrySetResult(vPath);
											//exportSession.Dispose();
										});
									}
								});

								var videoUrl = await tvcs.Task;
								var meFile = new MediaDetails()
								{
									Type = MediaFileType.Video,
									Path = videoUrl,
									PreviewPath = relativePath,
									Date = DateTime.Now
								};
								results.Add(meFile);
								OnMediaPicked?.Invoke(this, meFile);

								if (args.Assets.Length == results.Count && !completed)
								{
									completed = true;
									tcs.TrySetResult(results);
								}
							});

						break;
					default:

						PHImageManager.DefaultManager.RequestImageData(asset, options, (data, dataUti, orientation, info) =>
						{
							if (fileName.EndsWith(".HEIC", StringComparison.CurrentCulture))
								fileName = Path.GetFileNameWithoutExtension(fileName) + ".jpg";

							var relativePath = FileHelper.GetOutputPath(MediaFileType.Image, fileName);
							var fullPath = FileHelper.GetFullPath(relativePath);

							if (!File.Exists(fullPath))
							{
								Debug.WriteLine(dataUti);

								var image = UIImage.LoadFromData(data);
								var imageData = image.AsJPEG();

								imageData?.Save(fullPath, true);
							}

							var meFile = new MediaDetails()
							{
								Type = MediaFileType.Image,
								Path = relativePath,
								PreviewPath = relativePath,
								Date = DateTime.Now
							};

							results.Add(meFile);
							OnMediaPicked?.Invoke(this, meFile);
							if (args.Assets.Length == results.Count && !completed)
							{
								completed = true;
								tcs.TrySetResult(results);
							}

						});

						break;
				}
			}


			mediaPickTcs?.TrySetResult(await tcs.Task);
		}

	}
}
