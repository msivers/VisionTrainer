using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public delegate void PredictionEventHandler(object sender, PredictionEventArgs e);

	public class PredictionEventArgs : EventArgs
	{
		public double Value { get; private set; }
		public IList<PredictionModel> Predictions { get; private set; }

		public PredictionEventArgs(double value, IList<PredictionModel> predictions)
		{
			Value = value;
			Predictions = predictions;
		}
	}

	public class PredictionResultsViewModel : BaseViewModel
	{
		ImagePrediction prediction;
		Models.MediaDetails media;
		INavigation Navigation;
		int lastPredictionCount = 0;

		public event PredictionEventHandler PredictionsChanged;

		double confidenceValue;
		public double Confidence
		{
			get { return confidenceValue; }
			set
			{
				var isDifferent = SetProperty(ref confidenceValue, value);
				if (isDifferent)
				{
					List<PredictionModel> predictions = prediction.Predictions.Where(x => x.Probability > value).ToList();
					if (predictions.Count() == lastPredictionCount)
						return;
					lastPredictionCount = predictions.Count();

					var predictEvent = new PredictionEventArgs(value, predictions);
					PredictionsChanged?.Invoke(this, predictEvent);
				}
			}
		}

		public PredictionResultsViewModel(INavigation navigation, Models.MediaDetails media, ImagePrediction prediction)
		{
			Navigation = navigation;
			this.media = media;
			this.prediction = prediction;
		}
	}
}
