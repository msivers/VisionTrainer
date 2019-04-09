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

		#region Training
		public string TrainingKey
		{
			get { return Settings.TrainingKey; }
			set
			{
				if (Settings.TrainingKey == value)
					return;

				Settings.TrainingKey = value;
				OnPropertyChanged("TrainingKey");
			}
		}

		public string TrainingEndpoint
		{
			get { return Settings.TrainingEndpoint; }
			set
			{
				if (Settings.TrainingEndpoint == value)
					return;

				Settings.TrainingEndpoint = value;
				OnPropertyChanged("TrainingEndpoint");
			}
		}

		public string TrainingResourceId
		{
			get { return Settings.TrainingResourceId; }
			set
			{
				if (Settings.TrainingResourceId == value)
					return;

				Settings.TrainingResourceId = value;
				OnPropertyChanged("TrainingResourceId");
			}
		}
		#endregion

		#region Prediction
		public string PredictionKey
		{
			get { return Settings.PredictionKey; }
			set
			{
				if (Settings.PredictionKey == value)
					return;

				Settings.PredictionKey = value;
				OnPropertyChanged("PredictionKey");
			}
		}

		public string PredictionEndpoint
		{
			get { return Settings.PredictionEndpoint; }
			set
			{
				if (Settings.PredictionEndpoint == value)
					return;

				Settings.TrainingEndpoint = value;
				OnPropertyChanged("PredictionEndpoint");
			}
		}

		public string PredictionResourceId
		{
			get { return Settings.PredictionResourceId; }
			set
			{
				if (Settings.PredictionResourceId == value)
					return;

				Settings.TrainingResourceId = value;
				OnPropertyChanged("PredictionResourceId");
			}
		}
		#endregion

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

