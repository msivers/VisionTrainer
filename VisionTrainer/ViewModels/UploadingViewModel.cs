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

		bool uploadButtonEnabled;
		public bool UploadButtonEnabled
		{
			get { return uploadButtonEnabled; }
			set { SetProperty(ref uploadButtonEnabled, value); }
		}

		public ICommand StartUploadCommand { get; set; }
		public ICommand StopUploadCommand { get; set; }

		public UploadingViewModel(INavigation navigation)
		{
			database = ServiceContainer.Resolve<IDatabase>();
			var remainingItemsCount = database.GetItemsNotDone().Count();
			UploadButtonText = "Upload";
			UploadButtonEnabled = (remainingItemsCount > 0);
			RemainingItems = "Remaining:" + remainingItemsCount;

			StartUploadCommand = new Command(async (obj) =>
			{
				UploadButtonText = "Uploading";
				UploadButtonEnabled = false;
				shouldUpload = true;
				await Start();

				if (database.GetItemsNotDone().Count() == 0)
					await navigation.PopAsync();
			});

			StopUploadCommand = new Command((obj) =>
			{
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
