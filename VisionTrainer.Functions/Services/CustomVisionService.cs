using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;

namespace VisionTrainer.Functions.Services
{
	// WIP from https://github.com/Azure-Samples/cognitive-services-dotnet-sdk-samples/blob/master/CustomVision/ImageClassification/Program.cs
	public static class CustomVisionService
	{
		private static CustomVisionTrainingClient _trainingClient;
		public static CustomVisionTrainingClient TrainingClient
		{
			get
			{
				_trainingClient = _trainingClient ?? new CustomVisionTrainingClient()
				{
					ApiKey = Environment.GetEnvironmentVariable("CustomVisionTrainingKey"),
					Endpoint = Environment.GetEnvironmentVariable("CustomVisionTrainingEndpoint")

				};

				return _trainingClient;
			}
		}

		public static async Task UploadImage(byte[] bytes)
		{
			var projectId = Environment.GetEnvironmentVariable("CustomVisionProjectId");

			var projectGuid = Guid.Parse(projectId);
			using (var stream = new MemoryStream(bytes))
			{
				var summary = await TrainingClient.CreateImagesFromDataAsync(projectGuid, stream, null);
			}
		}
	}
}
