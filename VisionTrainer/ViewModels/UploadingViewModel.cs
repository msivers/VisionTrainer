using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using VisionTrainer.Services;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public class UploadingViewModel
	{
		public event PropertyChangedEventHandler PropertyChanged;
		IDatabase database;
		IUploadManager uploadManager;
		bool shouldUpload;

		string remainingItems;
		public string RemainingItems
		{
			get { return remainingItems; }
			set { SetProperty(ref remainingItems, value); }
		}

		string buttonText;
		public string ButtonText
		{
			get { return buttonText; }
			set { SetProperty(ref buttonText, value); }
		}

		public ICommand StartUploadCommand { get; set; }
		public ICommand StopUploadCommand { get; set; }

		public UploadingViewModel(INavigation navigation)
		{
			database = ServiceContainer.Resolve<IDatabase>();
			ButtonText = "Upload";
			RemainingItems = "Remaining:";

			StartUploadCommand = new Command(async (obj) =>
			{
				ButtonText = "Uploading";
				shouldUpload = true;
				await Start();
				await navigation.PopModalAsync();
			});

			StopUploadCommand = new Command((obj) =>
			{
				shouldUpload = false;
			});
		}

		public async Task Start()
		{
			var entries = database.GetItemsNotDone().ToList();
			int itemCount = entries.Count;

			foreach (var item in entries)
			{
				if (!shouldUpload)
					return;

				Console.WriteLine("Remaining Items {0}", itemCount);
				RemainingItems = itemCount.ToString();
				//Device.BeginInvokeOnMainThread(() =>
				//{
				//	RemainingItems = itemCount.ToString();
				//});

				var result = await AzureService.UploadTrainingMedia(item);
				if (result)
				{
					database.DeleteItem(item);
					itemCount--;
				}
			}
		}

		bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (Object.Equals(storage, value))
				return false;

			storage = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
