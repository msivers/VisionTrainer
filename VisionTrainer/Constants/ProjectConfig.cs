﻿using System;
namespace VisionTrainer.Constants
{
	public static class ProjectConfig
	{
		public static string BaseUrl = "https://msdemo.azure-api.net/visiontrainer/";
		//public static string BaseUrl = "http://localhost:7071/api/";

		public static string SubmitTrainingMediaUrl { get { return Settings.Endpoint + "SubmitTrainingMedia"; } }
		public static string SubmitPredictionMediaUrl { get { return Settings.Endpoint + "SubmitPredictionMedia"; } }
		public static string RemoteModelAvailableUrl { get { return Settings.Endpoint + "RemoteModelAvailable"; } }

		public static int JpegCompression = 90;

	}
}