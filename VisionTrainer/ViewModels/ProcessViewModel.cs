using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using VisionTrainer.Pages;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public class ProcessViewModel : INotifyPropertyChanged
	{
		public ICommand NewBatchCommand { get; set; }
		public INavigation Navigation { get; set; }
		public event PropertyChangedEventHandler PropertyChanged;

		public ProcessViewModel(INavigation navigation)
		{
			this.Navigation = navigation;

			NewBatchCommand = new Command(async (obj) =>
			{
				await Navigation.PushAsync(new CreateBatchPage());
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
