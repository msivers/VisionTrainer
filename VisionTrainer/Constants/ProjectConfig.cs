using System;
namespace VisionTrainer.Constants
{
	public static class ProjectConfig
	{
		public static string BaseUrl = "https://visiontrainer.azurewebsites.net/api/";
		//public static string BaseUrl = "http://localhost:7071/api/";

		public static string SubmitTrainingMediaUrl { get { return Settings.Endpoint + "SubmitTrainingMedia"; } }
		public static string SubmitPredictionMediaUrl { get { return Settings.Endpoint + "SubmitPredictionMedia"; } }
		public static string RemoteModelAvailableUrl { get { return Settings.Endpoint + "RemoteModelAvailable"; } }
	}
}