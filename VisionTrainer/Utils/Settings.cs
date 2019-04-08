using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace VisionTrainer
{
	public static class Settings
	{
		static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}

		const string cameraOption = "cameraOption";
		private static readonly int CameraOptionDefault = (int)CameraOptions.Rear;

		public static CameraOptions CameraOption
		{
			get { return (CameraOptions)AppSettings.GetValueOrDefault(cameraOption, CameraOptionDefault); }
			set { AppSettings.AddOrUpdateValue(cameraOption, (int)value); }
		}

		const string monitorInvertal = "monitorInterval";
		private static readonly int MonitorIntervalDefault = 5;

		public static int MonitorInterval
		{
			get { return AppSettings.GetValueOrDefault(monitorInvertal, MonitorIntervalDefault); }
			set { AppSettings.AddOrUpdateValue(monitorInvertal, (int)value); }
		}

		const string historyInterval = "historyInterval";
		private static readonly int HistoryIntervalDefault = 5;

		public static int HistoryInterval
		{
			get { return AppSettings.GetValueOrDefault(historyInterval, HistoryIntervalDefault); }
			set { AppSettings.AddOrUpdateValue(historyInterval, (int)value); }
		}

		const string cameraLocation = "cameraLocation";
		private static readonly string CameraLocationDefault = "None";

		public static string CameraLocation
		{
			get { return AppSettings.GetValueOrDefault(cameraLocation, CameraLocationDefault); }
			set { AppSettings.AddOrUpdateValue(cameraLocation, (string)value); }
		}
	}
}

