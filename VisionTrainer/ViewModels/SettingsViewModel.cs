using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public class SettingsViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		bool defaultCameraRear;
		public bool DefaultCameraRear
		{
			get { return defaultCameraRear; }
			set
			{
				if (defaultCameraRear == value)
					return;

				defaultCameraRear = value;
				Settings.CameraOption = value ? CameraOptions.Rear : CameraOptions.Front;

				OnPropertyChanged("DefaultCameraRear");
			}
		}

		int timerInterval;
		public int TimerInterval
		{
			get { return Settings.MonitorInterval; }
			set
			{
				if (Settings.MonitorInterval == value)
					return;

				Settings.MonitorInterval = value;
				OnPropertyChanged("TimerInterval");
			}
		}

		int historyInterval;
		public int HistoryInterval
		{
			get { return Settings.HistoryInterval; }
			set
			{
				if (Settings.HistoryInterval == value)
					return;

				Settings.HistoryInterval = value;
				OnPropertyChanged("HistoryInterval");
			}
		}

		string cameraLocation;
		public string CameraLocation
		{
			get { return Settings.CameraLocation; }
			set
			{
				if (Settings.CameraLocation == value)
					return;

				Settings.CameraLocation = value;
				OnPropertyChanged("CameraLocation");
			}
		}

		public SettingsViewModel()
		{
			DefaultCameraRear = (Settings.CameraOption == CameraOptions.Rear);
		}

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

