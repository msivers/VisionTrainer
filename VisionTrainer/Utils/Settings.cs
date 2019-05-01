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

		const string publishedModelNameKey = "publishedmodelname";
		private static readonly string PublishedModelNameKeyDefault = string.Empty;

		public static string PublishedModelName
		{
			get { return AppSettings.GetValueOrDefault(publishedModelNameKey, PublishedModelNameKeyDefault); }
			set { AppSettings.AddOrUpdateValue(publishedModelNameKey, (string)value); }
		}

		const string apiKeyId = "apikey";
		private static readonly string ApiKeyDefault = "";

		public static string ApiKey
		{
			get { return AppSettings.GetValueOrDefault(apiKeyId, ApiKeyDefault); }
			set { AppSettings.AddOrUpdateValue(apiKeyId, (string)value); }
		}
	}
}

