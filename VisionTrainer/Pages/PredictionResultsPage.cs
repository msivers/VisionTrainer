using System;
using SkiaSharp.Views.Forms;
using VisionTrainer.ViewModels;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class PredictionResultsPage : ContentPage
	{
		SKCanvasView canvasView;

		public PredictionResultsPage()
		{
			Title = "Results here";
			BindingContext = new PredictionResultsViewModel(Navigation);
		}
	}
}
