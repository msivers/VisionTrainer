using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Media;
using Plugin.Media.Abstractions;
using VisionTrainer.Common.Enums;
using VisionTrainer.Models;
using VisionTrainer.Pages;
using VisionTrainer.Resources;
using VisionTrainer.Services;
using VisionTrainer.Utils;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public delegate void PageStateEventHandler(object sender, PageStateEventArgs e);

	public class PageStateEventArgs : EventArgs
	{
		public PredictionPageState State { get; private set; }

		public PageStateEventArgs(PredictionPageState state)
		{
			State = state;
		}
	}

	public enum PredictionPageState
	{
		None,
		NoModelAvailable,
		NoCameraReady,
		CameraReady,
		Uploading
	}

	public class PredictionInputViewModel : BaseViewModel
	{
		IDatabase database;
		MediaDetails predictionMedia;
		PredictionPageState pageState = PredictionPageState.None;

		public INavigation Navigation { get; set; }
		public ICommand RefreshViewCommand { get; set; }
		public ICommand BrowseMediaCommand { get; set; }
		public event PageStateEventHandler UpdatePageState;

		string titleLabel;
		public string TitleLabel
		{
			get { return titleLabel; }
			set { SetProperty(ref titleLabel, value); }
		}

		string messageLabel;
		public string MessageLabel
		{
			get { return messageLabel; }
			set { SetProperty(ref messageLabel, value); }
		}

		ImageSource heroImage;
		public ImageSource HeroImage
		{
			get { return heroImage; }
			set { SetProperty(ref heroImage, value); }
		}

		bool showAnimation;
		public bool ShowAnimation
		{
			get { return showAnimation; }
			set { SetProperty(ref showAnimation, value); }
		}

		public PredictionInputViewModel(INavigation navigation)
		{
			this.Navigation = navigation;
			database = ServiceContainer.Resolve<IDatabase>();

			RefreshViewCommand = new Command(async () =>
			{
				var hasActiveModel = await AzureService.RemoteModelAvailable();
				var hasModelName = !string.IsNullOrEmpty(Settings.PublishedModelName);
				bool isAvilable = (hasActiveModel || hasModelName);

				PredictionPageState state = isAvilable ?
					(CrossMedia.Current.IsCameraAvailable ? PredictionPageState.CameraReady : PredictionPageState.NoCameraReady)
					: PredictionPageState.NoModelAvailable;

				SetPageState(state);
			});

			BrowseMediaCommand = new Command(async () => await PickPhoto());
		}

		void SetPageState(PredictionPageState state)
		{
			pageState = state;

			switch (pageState)
			{
				case PredictionPageState.CameraReady:
					{
						HeroImage = null;
					}
					break;
				case PredictionPageState.NoCameraReady:
					{
						HeroImage = ImageSource.FromFile("CameraMissing");
						TitleLabel = ApplicationResource.CameraNotSupportedTitle;
						MessageLabel = ApplicationResource.CameraNotSupportedMessage;
					}
					break;
				case PredictionPageState.NoModelAvailable:
					{
						HeroImage = ImageSource.FromFile("CloudMissing");
						TitleLabel = ApplicationResource.PredictionModelUnavailableTitle;
						MessageLabel = ApplicationResource.PredictionModelUnavailableMessage;
					}
					break;
				case PredictionPageState.Uploading:
					{
						HeroImage = ImageSource.FromFile("CameraUpload");
						TitleLabel = ApplicationResource.PagePredictionInputUploadingTitle;
						MessageLabel = ApplicationResource.PagePredictionInputUploadingMessage;
					}
					break;
				default:
					break;
			}

			var eventData = new PageStateEventArgs(pageState);
			UpdatePageState?.Invoke(this, eventData);
		}

		public async Task SaveBytes(byte[] bytes)
		{
			SetPageState(PredictionPageState.Uploading);

			var fileName = Guid.NewGuid() + ".jpg";
			predictionMedia = new Models.MediaDetails()
			{
				Path = fileName,
				PreviewPath = fileName,
				Type = MediaFileType.Image,
				Date = DateTime.Now
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
				Type = MediaFileType.Image,
				Date = DateTime.Now
			};
			database.SaveItem(predictionMedia);

			SetPageState(PredictionPageState.Uploading);

			await UploadMedia(predictionMedia.FullPath);
		}

		async Task UploadMedia(string filePath)
		{
			var result = await AzureService.UploadPredictionMedia(filePath);

			var success = (result != null);

			if (success && App.CurrentTabPage.GetType() == typeof(PredictionInputPage))
			{
				SetPageState(PredictionPageState.None);
				await Navigation.PushAsync(new PredictionResultsPage(predictionMedia, result));
			}
			else
			{
				var state = (CrossMedia.Current.IsCameraAvailable) ? PredictionPageState.CameraReady : PredictionPageState.NoCameraReady;
				SetPageState(state);
			}
		}
	}
}
