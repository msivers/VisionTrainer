using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using VisionTrainer.Constants;
using VisionTrainer.Services;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public class UploadingViewModel : BaseViewModel
	{
		IDatabase database;
		bool shouldUpload;

		string statusTitle;
		public string StatusTitle
		{
			get { return statusTitle; }
			set { SetProperty(ref statusTitle, value); }
		}

		string statusMessage;
		public string StatusMessage
		{
			get { return statusMessage; }
			set { SetProperty(ref statusMessage, value); }
		}

		string uploadButtonText;
		public string UploadButtonText
		{
			get { return uploadButtonText; }
			set { SetProperty(ref uploadButtonText, value); }
		}

		string animation;
		public string Animation
		{
			get { return animation; }
			set { SetProperty(ref animation, value); }
		}

		bool uploadEnabled;
		public bool UploadEnabled
		{
			get { return uploadEnabled; }
			set { SetProperty(ref uploadEnabled, value); }
		}

		bool uploadButtonEnabled;
		public bool UploadButtonEnabled
		{
			get { return uploadButtonEnabled; }
			set { SetProperty(ref uploadButtonEnabled, value); }
		}

		Color uploadButtonTextColor;
		public Color UploadButtonTextColor
		{
			get { return uploadButtonTextColor; }
			set { SetProperty(ref uploadButtonTextColor, value); }
		}

		Color uploadButtonColor;
		public Color UploadButtonColor
		{
			get { return uploadButtonColor; }
			set { SetProperty(ref uploadButtonColor, value); }
		}

		bool showAnimation;
		public bool ShowAnimation
		{
			get { return showAnimation; }
			set { SetProperty(ref showAnimation, value); }
		}

		bool shouldLoopAnimation;
		public bool ShouldLoopAnimation
		{
			get { return shouldLoopAnimation; }
			set { SetProperty(ref shouldLoopAnimation, value); }
		}

		ImageSource heroImage;
		public ImageSource HeroImage
		{
			get { return heroImage; }
			set { SetProperty(ref heroImage, value); }
		}

		public ICommand StartUploadCommand { get; set; }
		public ICommand StopUploadCommand { get; set; }

		public UploadingViewModel(INavigation navigation)
		{
			database = ServiceContainer.Resolve<IDatabase>();
			UploadButtonTextColor = Color.White;
			UpdateStatus();

			StartUploadCommand = new Command(async (obj) =>
			{
				StatusTitle = "Uploading Media";
				UploadButtonText = "Uploading";
				UploadButtonEnabled = false;

				UploadButtonColor = AppColors.ButtonDisabledColor;
				shouldUpload = true;
				ShowAnimation = true;
				await Start();

				if (database.GetItemsNotDone().Count() == 0)
				{
					ShowAnimation = false;
					UpdateStatus();
				}
			});

			StopUploadCommand = new Command((obj) =>
			{
				ShowAnimation = false;
				shouldUpload = false;

				UpdateStatus();
			});
		}

		public async Task Start()
		{
			var entries = database.GetItemsNotDone().ToList();
			int itemCount, totalItems;
			itemCount = totalItems = entries.Count;

			foreach (var item in entries)
			{
				if (!shouldUpload)
				{
					UploadButtonEnabled = true;
					return;
				}

				StatusMessage = string.Format("Remaining Items {0} / {1}", itemCount, totalItems);

				var result = await AzureService.UploadTrainingMedia(item);
				if (result)
				{
					database.DeleteItem(item);
					itemCount--;
				}
				else
				{
					StopUploadCommand.Execute(null);
				}
			}
		}

		void UpdateStatus()
		{
			var remainingItemsCount = database.GetItemsNotDone().Count();

			var hasItems = (remainingItemsCount > 0);
			if (hasItems)
			{
				var itemDescription = (remainingItemsCount > 1) ? "items" : "item";
				UploadButtonEnabled = (remainingItemsCount > 0);
				StatusTitle = "Upload Media for Training";
				StatusMessage = remainingItemsCount + " " + itemDescription + " waiting for submission.\nUse wifi as files may be large.";
				Animation = "pulse.json";
				UploadButtonText = "Upload";
				UploadButtonColor = AppColors.ButtonEnabledColor;

				ShouldLoopAnimation = true;
				UploadEnabled = true;
				HeroImage = ImageSource.FromFile("CameraUpload");
			}
			else
			{
				StatusTitle = "Complete";
				StatusMessage = "There are no items to upload";
				Animation = "complete.json";
				ShowAnimation = false;
				HeroImage = ImageSource.FromFile("AllComplete");
			}
		}
	}
}
