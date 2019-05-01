using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using VisionTrainer.Services;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public class UploadingViewModel : BaseViewModel
	{
		IDatabase database;
		IUploadManager uploadManager;
		bool shouldUpload;

		string remainingItems;
		public string RemainingItems
		{
			get { return remainingItems; }
			set { SetProperty(ref remainingItems, value); }
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

		bool uploadButtonEnabled;
		public bool UploadButtonEnabled
		{
			get { return uploadButtonEnabled; }
			set { SetProperty(ref uploadButtonEnabled, value); }
		}

		bool showAnimation;
		public bool ShowAnimation
		{
			get { return showAnimation; }
			set { SetProperty(ref showAnimation, value); }
		}

		public ICommand StartUploadCommand { get; set; }
		public ICommand StopUploadCommand { get; set; }

		public UploadingViewModel(INavigation navigation)
		{
			database = ServiceContainer.Resolve<IDatabase>();
			var remainingItemsCount = database.GetItemsNotDone().Count();
			UploadButtonText = "Upload";

			var hasItems = (remainingItemsCount > 0);
			if (hasItems)
			{
				UploadButtonEnabled = (remainingItemsCount > 0);
				RemainingItems = remainingItemsCount + " items remaining";
				Animation = "spinner.json";
			}
			else
			{
				Animation = "complete.json";
				ShowAnimation = true;
			}

			StartUploadCommand = new Command(async (obj) =>
			{
				UploadButtonText = "Uploading";
				UploadButtonEnabled = false;
				shouldUpload = true;
				ShowAnimation = true;
				await Start();

				if (database.GetItemsNotDone().Count() == 0)
				{
					ShowAnimation = false;
					await navigation.PopAsync();
				}
			});

			StopUploadCommand = new Command((obj) =>
			{
				ShowAnimation = false;
				shouldUpload = false;
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

				RemainingItems = string.Format("Remaining Items {0} / {1}", itemCount, totalItems);

				var result = await AzureService.UploadTrainingMedia(item);
				if (result)
				{
					database.DeleteItem(item);
					itemCount--;
				}
			}
		}
	}
}
