using System;
namespace VisionTrainer.Constants
{
	public static class ProjectConfig
	{
		public static string ImagesDirectory = "CachedMedia";

		//static string BaseUrl = "https://targetaudience.azurewebsites.net/api/";
		public static string BaseUrl = "http://localhost:7071/api/";

		public static string SubmitTrainingImageUrl = Settings.Endpoint + "SubmitTrainingImage";
		//public static string CaptureAudienceUrl = BaseUrl + "CaptureAudience?code=20gdLydUKR0JbwTpyUSk9EGeEC9JFUqj8v4Z5TA/MscvR39fTnnykw==";
	}
}