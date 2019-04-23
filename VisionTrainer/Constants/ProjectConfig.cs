using System;
namespace VisionTrainer.Constants
{
	public static class ProjectConfig
	{
		public static string ImagesDirectory = "CachedMedia";

		//public static string BaseUrl = "https://visiontrainer.azurewebsites.net/api/";
		public static string BaseUrl = "http://localhost:7071/api/";

		public static string SubmitTrainingMediaUrl { get { return Settings.Endpoint + "SubmitTrainingMedia"; } }
	}
}