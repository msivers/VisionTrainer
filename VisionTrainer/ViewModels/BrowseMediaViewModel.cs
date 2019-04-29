using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using VisionTrainer.Common.Models;
using VisionTrainer.Interfaces;
using VisionTrainer.Models;
using VisionTrainer.Services;
using VisionTrainer.Utils;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public class BrowseMediaViewModel : BaseViewModel
	{
		IMultiMediaPickerService _multiMediaPickerService;
		IDatabase database;
		INavigation Navigation { get; set; }
		bool popupDisplaying;

		ObservableCollection<MediaDetails> media;
		public ObservableCollection<MediaDetails> Media
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

		public BrowseMediaViewModel(INavigation navigation)
		{
			this.Navigation = navigation;

			_multiMediaPickerService = ServiceContainer.Resolve<IMultiMediaPickerService>();
			database = ServiceContainer.Resolve<IDatabase>();
			Media = new ObservableCollection<MediaDetails>();

			Tags = new string[]
			{
				"Object 1",
				"Object 2",
				"Object 3",
				"Object 4"
			};

			SelectImagesCommand = new Command(async (obj) =>
			{
				popupDisplaying = true;
				Media = new ObservableCollection<MediaDetails>();
				await _multiMediaPickerService.PickPhotosAsync();
				popupDisplaying = false;
			});

			SelectVideosCommand = new Command(async (obj) =>
			{
				popupDisplaying = true;
				Media = new ObservableCollection<MediaDetails>();
				await _multiMediaPickerService.PickVideosAsync();
				popupDisplaying = false;
			});

			RemoveImageCommand = new Command<MediaDetails>((obj) =>
			{
				FileHelper.DeleteLocalFiles(obj);
				Media.Remove(obj);
			});

			CompleteCommand = new Command(async (obj) =>
			{
				if (popupDisplaying)
					return;

				foreach (var item in Media)
				{
					if (!string.IsNullOrEmpty(SelectedTag))
						item.Tags = new Common.Models.TagArea[] { new Common.Models.TagArea() { Id = SelectedTag } }; // TEMP

					item.Location = new GeoLocation(10, 10);
					database.SaveItem(item);
				}
			});

			_multiMediaPickerService.OnMediaPicked += (s, a) =>
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					Media.Add(a);
				});
			};
		}
	}
}
