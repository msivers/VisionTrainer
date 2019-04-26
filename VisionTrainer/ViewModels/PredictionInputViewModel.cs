using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Media;
using Plugin.Media.Abstractions;
using VisionTrainer.Common.Enums;
using VisionTrainer.Services;
using VisionTrainer.Utils;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public delegate void PredictionEndpointEventHandler(object sender, PredictionEndpointEventArgs e);

	public class PredictionEndpointEventArgs : EventArgs
	{
		public bool IsAvailable { get; private set; }

		public PredictionEndpointEventArgs(bool isAvilalble)
		{
			IsAvailable = isAvilalble;
		}
	}

	public class PredictionInputViewModel : BaseViewModel
	{
		IDatabase database;
		byte[] mediaBytes;

		public INavigation Navigation { get; set; }
		public ICommand RefreshViewCommand { get; set; }
		public ICommand BrowseMediaCommand { get; set; }
		public event PredictionEndpointEventHandler EndpointAvailable;

		public PredictionInputViewModel(INavigation navigation)
		{
			this.Navigation = navigation;
			database = ServiceContainer.Resolve<IDatabase>();

			RefreshViewCommand = new Command(async () =>
			{
				var hasActiveModel = await AzureService.RemoteModelAvailable();
				var hasModelName = !string.IsNullOrEmpty(Settings.PublishedModelName);
				bool isAvilable = (hasActiveModel || hasModelName);

				var eventData = new PredictionEndpointEventArgs(isAvilable);
				EndpointAvailable?.Invoke(this, eventData);
			});

			BrowseMediaCommand = new Command(async () => await PickPhoto());
		}

		public async Task SaveBytes(byte[] bytes)
		{
			var fileName = Guid.NewGuid() + ".jpg";
			var media = new Models.MediaFile()
			{
				Path = fileName,
				PreviewPath = fileName,
				Type = MediaFileType.Image
			};
			File.WriteAllBytes(media.FullPath, bytes);
			database.SaveItem(media);

			await UploadFile(media.FullPath);

			throw new NotImplementedException();
			//var result = await AzureService.UploadTestMedia(media);
			// TODO submit image through to service for checking
		}

		async Task PickPhoto()
		{
			var hasPermission = await PermissionsCheck.PhotosAsync();
			if (!hasPermission)
				return;

			var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions { PhotoSize = PhotoSize.Full });
			if (file == null)
				return;

			await UploadFile(file.Path);
		}

		async Task UploadFile(string path)
		{

		}
	}
}
