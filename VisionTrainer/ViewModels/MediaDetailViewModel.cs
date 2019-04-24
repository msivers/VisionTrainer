using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using VisionTrainer.Models;
using VisionTrainer.Resources;
using VisionTrainer.Services;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public class MediaDetailViewModel : INotifyPropertyChanged
	{
		public INavigation Navigation { get; set; }
		public event PropertyChangedEventHandler PropertyChanged;
		public ICommand DeleteMediaCommand { get; set; }
		IDatabase database;
		MediaFile media;

		public string MediaFilePath
		{
			get { return media.FullPath; }
			//set { SetProperty(ref selectedTag, value); }
		}

		public MediaDetailViewModel(INavigation navigation, MediaFile media)
		{
			Navigation = navigation;
			database = ServiceContainer.Resolve<IDatabase>();
			this.media = media;

			DeleteMediaCommand = new Command(async (obj) =>
			{
				database.DeleteItem(media);
				await navigation.PopAsync();
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
