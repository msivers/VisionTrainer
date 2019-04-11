﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using VisionTrainer.Models;
using VisionTrainer.Pages;
using VisionTrainer.Services;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public class ProcessViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public INavigation Navigation { get; set; }

		public ICommand NewBatchCommand { get; set; }
		public ICommand UploadFilesCommand { get; set; }
		public ICommand RefreshMediaEntriesCommand { get; set; }

		IDatabase database;

		ObservableCollection<MediaFile> media;
		public ObservableCollection<MediaFile> Media
		{
			get { return media; }
			set { SetProperty(ref media, value); }
		}

		public ProcessViewModel(INavigation navigation)
		{
			this.Navigation = navigation;
			database = ServiceContainer.Resolve<IDatabase>();

			NewBatchCommand = new Command(async (obj) =>
			{
				await Navigation.PushModalAsync(new CreateBatchPage());
			});

			UploadFilesCommand = new Command(async (obj) =>
			{
				// TODO Upload the files!
			});

			RefreshMediaEntriesCommand = new Command(async (obj) =>
			{
				var entries = await database.GetItemsNotDoneAsync();
				Media = new ObservableCollection<MediaFile>(entries);
			});
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
