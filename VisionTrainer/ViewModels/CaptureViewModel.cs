using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MonkeyCache.FileStore;
using VisionTrainer.Models;
using VisionTrainer.Services;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public class CaptureViewModel : INotifyPropertyChanged
	{
		public INavigation Navigation { get; set; }
		public event PropertyChangedEventHandler PropertyChanged;

		byte[] imageData;
		public byte[] ImageData
		{
			get { return imageData; }
			protected set { SetProperty(ref imageData, value); }
		}

		bool isBusy;
		public bool IsBusy
		{
			get { return isBusy; }
			set { SetProperty(ref isBusy, value); }
		}

		public CaptureViewModel(INavigation navigation)
		{
			this.Navigation = navigation;
		}

		public async Task<UIResponse> Submit(byte[] bytes)
		{
			if (IsBusy)
				return new UIResponse();

			IsBusy = true;
			ImageData = bytes;

			var result = new UIResponse();
			//AudienceResponse response = await AzureService.CaptureAudience(location, imageData);
			//if (!response.HasError)
			//{
			//	//Saves to the cache with a timespan for expiration
			//	var capture = new AudienceData() { ImageData = imageData, Location = location, Audience = response.Audience };
			//	Barrel.Current.Add(StorageIds.CurrentCapture, capture, TimeSpan.FromMinutes(5));

			//	result.Result = true;
			//}
			//else
			//{
			//	result.Message = response.Message;
			//}

			IsBusy = false;
			return result;
		}

		public async Task Success()
		{
			//await Navigation.PushAsync(new CaptureResultsPage());
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
