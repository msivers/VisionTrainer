using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using VisionTrainer.Interfaces;
using VisionTrainer.Models;
using VisionTrainer.Services;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public class CreateBatchViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		IMultiMediaPickerService _multiMediaPickerService;
		IDatabase database;
		INavigation Navigation { get; set; }

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

		string selectedTag;
		public string SelectedTag
		{
			get { return selectedTag; }
			set { SetProperty(ref selectedTag, value); }
		}

		public ICommand SelectImagesCommand { get; set; }
		public ICommand SelectVideosCommand { get; set; }
		public ICommand TagSelectedCommand { get; set; }
		public ICommand RemoveImageCommand { get; set; }
		public ICommand CompleteCommand { get; set; }

		public CreateBatchViewModel(INavigation navigation)
		{
			this.Navigation = navigation;

			_multiMediaPickerService = ServiceContainer.Resolve<IMultiMediaPickerService>();
			database = ServiceContainer.Resolve<IDatabase>();
			Media = new ObservableCollection<MediaFile>();

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

			RemoveImageCommand = new Command<MediaFile>((obj) =>
			{
				Media.Remove(obj);
			});

			CompleteCommand = new Command(async (obj) =>
			{
				foreach (var item in Media)
				{
					//if (!string.IsNullOrEmpty(SelectedTag))
					//item.Tags = new Common.Models.TagArea[] { new Common.Models.TagArea() { Id = SelectedTag } }; // TEMP
					await database.SaveItemAsync(item);
				}

				await Navigation.PopModalAsync();
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
