using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using VisionTrainer.Services;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public class SettingsViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public ICommand ClearDatabaseCommand { get; set; }
		IDatabase database;

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

		public string UserId
		{
			get { return Settings.UserId; }
			set
			{
				if (Settings.UserId == value)
					return;

				Settings.UserId = value;
				OnPropertyChanged("UserId");
			}
		}

		public string Endpoint
		{
			get { return Settings.Endpoint; }
			set
			{
				if (Settings.Endpoint == value)
					return;

				Settings.Endpoint = value;
				OnPropertyChanged("Endpoint");
			}
		}

		public string PublishedModelName
		{
			get { return Settings.PublishedModelName; }
			set
			{
				if (Settings.PublishedModelName == value)
					return;

				Console.WriteLine("wtf");
				Settings.PublishedModelName = value;
				OnPropertyChanged("PublishedModelName");
			}
		}
		/*
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
		*/

		public SettingsViewModel()
		{
			database = ServiceContainer.Resolve<IDatabase>();
			DefaultCameraRear = (Settings.CameraOption == CameraOptions.Rear);
			ClearDatabaseCommand = new Command((obj) =>
			{
				// Clear the DB
				database.DeleteAllItems();

				// Clean any remaining files
				var mediaDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				var list = Directory.EnumerateFiles(mediaDirectory, "*.*", SearchOption.AllDirectories)
				.Where(s => s.EndsWith(".jpg") || s.EndsWith(".mp4")).ToArray();

				if (list.Count() > 0)
				{
					for (int i = 0; i < list.Count(); i++)
					{
						File.Delete(list[i]);
					}
				}
			});
		}

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

