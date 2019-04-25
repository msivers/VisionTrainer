using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using VisionTrainer.Common.Enums;
using VisionTrainer.Models;
using VisionTrainer.Services;
using VisionTrainer.Utils;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public class PredictionInputViewModel : BaseViewModel
	{
		IDatabase database;

		public INavigation Navigation { get; set; }
		public ICommand RefreshViewCommand { get; set; }

		bool displayCameraControls;
		public bool DisplayCameraControls
		{
			get { return displayCameraControls; }
			set { SetProperty(ref displayCameraControls, value); }
		}

		public PredictionInputViewModel(INavigation navigation)
		{
			this.Navigation = navigation;
			database = ServiceContainer.Resolve<IDatabase>();

			RefreshViewCommand = new Command(() =>
			{
				var hasActiveModel = Settings.ActivePublishedModel;
				var hasModelName = !string.IsNullOrEmpty(Settings.PublishedModelName);

				DisplayCameraControls = (hasActiveModel || hasModelName);
			});
		}

		public async Task SaveBytes(byte[] bytes)
		{
			var fileName = Guid.NewGuid() + ".jpg";
			var media = new MediaFile()
			{
				Path = fileName,
				PreviewPath = fileName,
				Type = MediaFileType.Image
			};
			File.WriteAllBytes(media.FullPath, bytes);
			database.SaveItem(media);

			//var result = await AzureService.UploadTestMedia(media);
			// TODO submit image through to service for checking
		}
	}
}
