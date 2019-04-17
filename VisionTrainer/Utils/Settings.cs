using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using VisionTrainer.Constants;

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

		const string userIdKey = "userid";
		private static readonly string UserIdKeyDefault = "unknown";

		public static string UserId
		{
			get { return AppSettings.GetValueOrDefault(userIdKey, UserIdKeyDefault); }
			set { AppSettings.AddOrUpdateValue(userIdKey, (string)value); }
		}

		const string endpointKey = "endpoint";
		private static readonly string EndpointKeyDefault = ProjectConfig.BaseUrl;

		public static string Endpoint
		{
			get { return AppSettings.GetValueOrDefault(endpointKey, EndpointKeyDefault); }
			set { AppSettings.AddOrUpdateValue(endpointKey, (string)value); }
		}

		/*
				#region Training
				const string trainingKey = "trainingkey";
				private static readonly string TrainingKeyDefault = "";

				public static string TrainingKey
				{
					get { return AppSettings.GetValueOrDefault(trainingKey, TrainingKeyDefault); }
					set { AppSettings.AddOrUpdateValue(trainingKey, (string)value); }
				}

				const string trainingEndpoint = "trainingendpoint";
				private static readonly string TrainingEndpointDefault = "";

				public static string TrainingEndpoint
				{
					get { return AppSettings.GetValueOrDefault(trainingEndpoint, TrainingEndpointDefault); }
					set { AppSettings.AddOrUpdateValue(trainingEndpoint, (string)value); }
				}

				const string trainingResourceId = "trainingresourceid";
				private static readonly string TrainingResourceIdDefault = "";

				public static string TrainingResourceId
				{
					get { return AppSettings.GetValueOrDefault(trainingResourceId, TrainingResourceIdDefault); }
					set { AppSettings.AddOrUpdateValue(trainingResourceId, (string)value); }
				}
				#endregion

				#region Prediction
				const string predictionKey = "predictionkey";
				private static readonly string PredictionKeyDefault = "";

				public static string PredictionKey
				{
					get { return AppSettings.GetValueOrDefault(predictionKey, PredictionKeyDefault); }
					set { AppSettings.AddOrUpdateValue(predictionKey, (string)value); }
				}

				const string predictionEndpoint = "predictionendpoint";
				private static readonly string PredictionEndpointDefault = "";

				public static string PredictionEndpoint
				{
					get { return AppSettings.GetValueOrDefault(predictionEndpoint, PredictionEndpointDefault); }
					set { AppSettings.AddOrUpdateValue(predictionEndpoint, (string)value); }
				}

				const string predictionResourceId = "predictionresourceid";
				private static readonly string PredictionResourceIdDefault = "";

				public static string PredictionResourceId
				{
					get { return AppSettings.GetValueOrDefault(predictionResourceId, PredictionResourceIdDefault); }
					set { AppSettings.AddOrUpdateValue(predictionResourceId, (string)value); }
				}
				#endregion
		*/
	}
}

