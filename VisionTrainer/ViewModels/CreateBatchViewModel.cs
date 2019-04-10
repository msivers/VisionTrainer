using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using VisionTrainer.Interfaces;
using VisionTrainer.Models;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public class CreateBatchViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		IMultiMediaPickerService _multiMediaPickerService;

		ObservableCollection<MediaFile> media;
		public ObservableCollection<MediaFile> Media
		{
			get { return media; }
			set { SetProperty(ref media, value); }
		}

		string[] tags;
		public string[] Tags
		{
			get { return tags; }
			set { SetProperty(ref tags, value); }
		}

		public ICommand SelectImagesCommand { get; set; }
		public ICommand SelectVideosCommand { get; set; }

		public CreateBatchViewModel()
		{
			_multiMediaPickerService = ServiceContainer.Resolve<IMultiMediaPickerService>();

			Tags = new string[]
			{
				"Object 1",
				"Object 2",
				"Object 3",
				"Object 4"
			};

			SelectImagesCommand = new Command(async (obj) =>
			{
				Media = new ObservableCollection<MediaFile>();
				await _multiMediaPickerService.PickPhotosAsync();
			});

			SelectVideosCommand = new Command(async (obj) =>
			{
				Media = new ObservableCollection<MediaFile>();
				await _multiMediaPickerService.PickVideosAsync();
			});

			_multiMediaPickerService.OnMediaPicked += (s, a) =>
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					Media.Add(a);
				});
			};
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
