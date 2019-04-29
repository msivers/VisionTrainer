using System;
namespace VisionTrainer.Constants
{
	public static class ProjectConfig
	{
		public static string BaseUrl = "https://visiontrainer.azurewebsites.net/api/";
		//public static string BaseUrl = "http://localhost:7071/api/";
		public static string UrlKey = "?code=SoBaRf91I68utGUGnOLxlVB8NHf6rzBaajMHSFWfgg7oT0tyHZelEg==";

		public static string SubmitTrainingMediaUrl { get { return Settings.Endpoint + "SubmitTrainingMedia" + UrlKey; } }
		public static string SubmitPredictionMediaUrl { get { return Settings.Endpoint + "SubmitPredictionMedia" + UrlKey; } }
		public static string RemoteModelAvailableUrl { get { return Settings.Endpoint + "RemoteModelAvailable" + UrlKey; } }
	}
}