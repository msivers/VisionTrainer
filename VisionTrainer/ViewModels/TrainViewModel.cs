using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using VisionTrainer.Models;
using VisionTrainer.Pages;
using VisionTrainer.Services;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public class TrainViewModel : BaseViewModel
	{
		public INavigation Navigation { get; set; }

		public ICommand AddMediaCommand { get; set; }
		public ICommand UploadFilesCommand { get; set; }
		public ICommand RefreshMediaEntriesCommand { get; set; }
		public ICommand MediaSelectedCommand { get; set; }
		public ICommand CaptureImagesCommand { get; set; }

		IDatabase database;

		ObservableCollection<MediaDetails> media;
		public ObservableCollection<MediaDetails> Media
		{
			get { return media; }
			set { SetProperty(ref media, value); }
		}

		public ObservableCollection<MediaDetails> SelectedItem
		{
			//get { return media; }
			set
			{
				Console.WriteLine(value);
			}
		}

		public TrainViewModel(INavigation navigation)
		{
			this.Navigation = navigation;
			database = ServiceContainer.Resolve<IDatabase>();

			AddMediaCommand = new Command(async (obj) =>
			{
				await Navigation.PushAsync(new BrowseMediaPage());
			});

			UploadFilesCommand = new Command(async (obj) =>
			{
				await Navigation.PushAsync(new UploadingPage());
			});

			CaptureImagesCommand = new Command(async (obj) =>
			{
				await Navigation.PushAsync(new CapturePage());
			});

			RefreshMediaEntriesCommand = new Command((obj) =>
			{
				var entries = database.GetItemsNotDone().OrderByDescending(x => x.Date);
				Media = new ObservableCollection<MediaDetails>(entries);
			});

			MediaSelectedCommand = new Command<MediaDetails>(async (obj) =>
			{
				await Navigation.PushAsync(new MediaDetailPage(obj));
			});
		}
	}
}
