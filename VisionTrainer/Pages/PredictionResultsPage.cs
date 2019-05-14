using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using VisionTrainer.Models;
using VisionTrainer.Resources;
using VisionTrainer.Utils;
using VisionTrainer.ViewModels;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class PredictionResultsPage : BaseContentPage
	{
		SKCanvasView canvasView;
		PredictionResultsViewModel viewModel;
		IList<PredictionModel> predictions;
		byte[] imageBytes;
		SKBitmap masterBitmap;
		Image photoView;

		public PredictionResultsPage(MediaDetails media, ImagePrediction prediction)
		{
			Title = ApplicationResource.PagePredictionResultsTitle;
			BindingContext = viewModel = new PredictionResultsViewModel(Navigation, media, prediction);
			viewModel.PredictionsChanged += OnPredictionsChanged;

			var screenSize = new Size(App.ScreenWidth, App.ScreenHeight);

			// Image View
			photoView = new Image();
			photoView.Source = ImageSource.FromFile(media.FullPath);
			AbsoluteLayout.SetLayoutBounds(photoView, new Rectangle(0, 0, 1, 1));
			AbsoluteLayout.SetLayoutFlags(photoView, AbsoluteLayoutFlags.All);

			// Canvas View
			canvasView = new SKCanvasView();
			canvasView.PaintSurface += OnCanvasViewPaintSurface;
			AbsoluteLayout.SetLayoutBounds(canvasView, new Rectangle(0, 0, 1, 1));
			AbsoluteLayout.SetLayoutFlags(canvasView, AbsoluteLayoutFlags.All);

			// Slider view
			Slider confidenceSlider = new Slider() { Minimum = 0, Maximum = 1 };
			confidenceSlider.SetBinding(Slider.ValueProperty, new Binding("Confidence"));
			AbsoluteLayout.SetLayoutBounds(confidenceSlider, new Rectangle(.5, 1, .8, .1));
			AbsoluteLayout.SetLayoutFlags(confidenceSlider, AbsoluteLayoutFlags.All);

			var layout = new AbsoluteLayout();
			layout.Margin = new Thickness(10);
			layout.Children.Add(photoView);
			layout.Children.Add(canvasView);
			layout.Children.Add(confidenceSlider);
			layout.SizeChanged += Layout_SizeChanged;
			Content = layout;

			var orientation = ImageUtils.GetImageOrientation(media.FullPath);

			imageBytes = File.ReadAllBytes(media.FullPath);
			masterBitmap = ImageUtils.HandleOrientation(SKBitmap.Decode(imageBytes), orientation);
		}

		private void OnPredictionsChanged(object sender, PredictionEventArgs e)
		{
			Console.WriteLine(e.Predictions.Count);

			predictions = e.Predictions;
			Device.BeginInvokeOnMainThread(() => canvasView.InvalidateSurface());
		}

		void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
		{
			SKImageInfo info = args.Info;
			SKSurface surface = args.Surface;
			SKCanvas canvas = surface.Canvas;

			canvas.Clear(SKColors.Transparent);

			using (var bitmap = masterBitmap.Copy())
			{
				float scale = Math.Min((float)info.Width / bitmap.Width,
									   (float)info.Height / bitmap.Height);
				float x = (info.Width - scale * bitmap.Width) / 2;
				float y = (info.Height - scale * bitmap.Height) / 2;
				SKRect destRect = new SKRect(x, y, x + scale * bitmap.Width,
												   y + scale * bitmap.Height);

				if (predictions != null)
				{
					// draw directly on the bitmap
					using (var annotationCanvas = new SKCanvas(bitmap))
					using (var textPaint = new SKPaint())
					using (var boxPaint = new SKPaint())
					{
						boxPaint.StrokeWidth = 3 / scale;
						textPaint.TextSize = 25 / scale;
						boxPaint.Style = SKPaintStyle.Stroke;


						foreach (var result in predictions)
						{
							// Draw the bounding box
							var rect = result.BoundingBox;
							var zone = SKRectI.Create(
								(int)(rect.Left * bitmap.Width),
								(int)(rect.Top * bitmap.Height),
								(int)(rect.Width * bitmap.Width),
								(int)(rect.Height * bitmap.Height));

							boxPaint.Color = SKColors.Red;
							annotationCanvas.DrawRect(zone, boxPaint);

							// Draw the textbox
							textPaint.Color = SKColors.Red;
							textPaint.FakeBoldText = true;
							//var textBox = SKRectI.Create(zone.Left, zone.Top - textPaint.TextSize, );

							var message = result.TagName + ":" + Math.Round(result.Probability, 2);
							annotationCanvas.DrawText(message, zone.Left + 10, zone.Top - textPaint.TextSize, textPaint);
						}
					}
				}

				// Resizing
				//var pictureFrame = canvasView.Bounds.ToSKRect();
				//var imageSize = new SKSize(bitmap.Width, bitmap.Height);
				//var dest = pictureFrame.AspectFill(imageSize);

				// draw the image
				//var paint = new SKPaint
				//{
				//	FilterQuality = SKFilterQuality.High // high quality scaling
				//};

				// draw the modified bitmap to the screen
				canvas.DrawBitmap(bitmap, destRect);
			}
		}

		void Layout_SizeChanged(object sender, EventArgs e)
		{
			try
			{
				var layout = (AbsoluteLayout)sender;
				var canvasBounds = canvasView.Bounds;

				canvasView.Scale = layout.Scale;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
