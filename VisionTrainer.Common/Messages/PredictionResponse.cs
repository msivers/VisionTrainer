using System;
using VisionTrainer.Common.Models;

namespace VisionTrainer.Common.Messages
{
	public class PredictionResponse : BaseResponse
	{
		public MediaRecognition Data { get; set; }
	}
}
