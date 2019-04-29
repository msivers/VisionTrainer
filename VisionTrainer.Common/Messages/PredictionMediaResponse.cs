using System;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using VisionTrainer.Common.Models;

namespace VisionTrainer.Common.Messages
{
	public class PredictionMediaResponse : BaseResponse
	{
		public ImagePrediction Data { get; set; }
	}
}
