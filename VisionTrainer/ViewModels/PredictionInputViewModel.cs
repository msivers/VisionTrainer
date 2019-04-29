using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Media;
using Plugin.Media.Abstractions;
using VisionTrainer.Common.Enums;
using VisionTrainer.Pages;
using VisionTrainer.Resources;
using VisionTrainer.Services;
using VisionTrainer.Utils;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public delegate void ResultEventHandler(object sender, ResultEventArgs e);

	public class ResultEventArgs : EventArgs
	{
		public bool Value { get; private set; }

		public ResultEventArgs(bool isAvilalble)
		{
			Value = isAvilalble;
		}
	}

	public class PredictionInputViewModel : BaseViewModel
	{
		IDatabase database;
		Models.MediaDetails predictionMedia;

		public INavigation Navigation { get; set; }
		public ICommand RefreshViewCommand { get; set; }
		public ICommand BrowseMediaCommand { get; set; }
		public event ResultEventHandler EndpointAvailable;
		public event ResultEventHandler UploadCompleted;

		string messageLabel;
		public string MessageLabel
		{
			get { return messageLabel; }
			set { SetProperty(ref messageLabel, value); }
		}

		public PredictionInputViewModel(INavigation navigation)
		{
			this.Navigation = navigation;
			database = ServiceContainer.Resolve<IDatabase>();
			MessageLabel = ApplicationResource.CameraNotSupported;

			RefreshViewCommand = new Command(async () =>
			{
				var hasActiveModel = await AzureService.RemoteModelAvailable();
				var hasModelName = !string.IsNullOrEmpty(Settings.PublishedModelName);
				bool isAvilable = (hasActiveModel || hasModelName);

				var eventData = new ResultEventArgs(isAvilable);
				EndpointAvailable?.Invoke(this, eventData);
			});

			BrowseMediaCommand = new Command(async () => await PickPhoto());
		}

		public async Task SaveBytes(byte[] bytes)
		{
			var fileName = Guid.NewGuid() + ".jpg";
			predictionMedia = new Models.MediaDetails()
			{
				Path = fileName,
				PreviewPath = fileName,
				Type = MediaFileType.Image
			};
			File.WriteAllBytes(predictionMedia.FullPath, bytes);
			database.SaveItem(predictionMedia);

			await UploadMedia(predictionMedia.FullPath);
		}

		async Task PickPhoto()
		{
			var hasPermission = await PermissionsCheck.PhotosAsync();
			if (!hasPermission)
				return;

			var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions { PhotoSize = PhotoSize.Full });
			if (file == null)
				return;

			predictionMedia = new Models.MediaDetails()
			{
				Path = file.Path,
				PreviewPath = file.Path,
				Type = MediaFileType.Image
			};
			database.SaveItem(predictionMedia);

			MessageLabel = ApplicationResource.PagePredictionInputUploading;

			await UploadMedia(predictionMedia.FullPath);

			MessageLabel = ApplicationResource.CameraNotSupported;
		}

		async Task UploadMedia(string filePath)
		{
			var result = await AzureService.UploadPredictionMedia(filePath);

			var success = (result != null);
			UploadCompleted?.Invoke(this, new ResultEventArgs(success));

			if (success)
				await Navigation.PushAsync(new PredictionResultsPage(predictionMedia, result));
		}
	}
}
