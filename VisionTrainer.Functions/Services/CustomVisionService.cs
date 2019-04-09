using System;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;

namespace VisionTrainer.Functions.Services
{
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

		public static void Test()
		{

		}
	}
}
