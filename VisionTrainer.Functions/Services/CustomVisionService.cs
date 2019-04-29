using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
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

		private static CustomVisionPredictionClient _predictionClient;
		public static CustomVisionPredictionClient PredictionClient
		{
			get
			{
				_predictionClient = _predictionClient ?? new CustomVisionPredictionClient()
				{
					ApiKey = Environment.GetEnvironmentVariable("CustomVisionPredictionKey"),
					Endpoint = Environment.GetEnvironmentVariable("CustomVisionPredictionEndpoint")

				};

				return _predictionClient;
			}
		}

		public static async Task UploadTrainingImage(byte[] bytes)
		{
			var projectId = Environment.GetEnvironmentVariable("CustomVisionProjectId");

			var projectGuid = Guid.Parse(projectId);
			using (var stream = new MemoryStream(bytes))
			{
				var summary = await TrainingClient.CreateImagesFromDataAsync(projectGuid, stream, null);
			}
		}

		public static async Task<ImagePrediction> UploadPredictionImage(byte[] bytes, string modelName = null)
		{
			var projectId = Environment.GetEnvironmentVariable("CustomVisionProjectId");
			var publishedModelName = (!string.IsNullOrEmpty(modelName)) ? modelName : Environment.GetEnvironmentVariable("CustomVisionPredictionPublishedName");
			var projectGuid = Guid.Parse(projectId);

			// Make a prediction against the new project
			using (var stream = new MemoryStream(bytes))
			{
				var result = await PredictionClient.DetectImageAsync(projectGuid, publishedModelName, stream);
				return result;
			}
		}
	}
}
