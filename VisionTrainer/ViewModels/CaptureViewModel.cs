using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VisionTrainer.Common.Enums;
using VisionTrainer.Constants;
using VisionTrainer.Models;
using VisionTrainer.Services;
using VisionTrainer.Utils;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public class CaptureViewModel : INotifyPropertyChanged
	{
		public INavigation Navigation { get; set; }
		public event PropertyChangedEventHandler PropertyChanged;
		IDatabase database;

		public CaptureViewModel(INavigation navigation)
		{
			this.Navigation = navigation;
			database = ServiceContainer.Resolve<IDatabase>();
		}

		public string SaveBytes(byte[] bytes)
		{
			var fileName = Guid.NewGuid() + ".jpg";
			var media = new MediaFile()
			{
				Path = fileName,
				PreviewPath = fileName,
				Type = MediaFileType.Image
			};
			File.WriteAllBytes(media.FullPath, bytes);
			database.SaveItem(media);

			return fileName;
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
