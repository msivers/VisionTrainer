﻿using System;
using System.Threading.Tasks;
using Lottie.Forms;
using Plugin.Media;
using VisionTrainer.Resources;
using VisionTrainer.ViewModels;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class PredictionInputPage : ContentPage
	{
		AbsoluteLayout mainLayout;


		CameraPreview cameraPreview;
		PredictionInputViewModel predictionInputModel;
		Label messageLabel;
		Button captureButton;
		AnimationView uploadingAnimationView;
		ToolbarItem browseMedaToolbarItem;
		ToolbarItem toggleCameraToolbarItem;
		bool cameraIsAvailable;

		public PredictionInputPage()
		{
			BindingContext = predictionInputModel = new PredictionInputViewModel(Navigation);
			predictionInputModel.UpdatePageState += OnPageStateChanged;

			Title = ApplicationResource.PagePredictionInputTitle;
			mainLayout = new AbsoluteLayout();

			// Camera Layout
			cameraIsAvailable = CrossMedia.Current.IsCameraAvailable;

			if (cameraIsAvailable)
			{
				// Camera Preview
				cameraPreview = new CameraPreview();
				cameraPreview.CameraOption = Settings.CameraOption;
				cameraPreview.CaptureBytesCallback = new Action<byte[]>(async (byte[] obj) => await ProcessCameraPhoto(obj));
				cameraPreview.CameraReady += (s, e) => StartCamera();
				cameraPreview.IsVisible = false;

				AbsoluteLayout.SetLayoutBounds(cameraPreview, new Rectangle(1, 1, 1, 1));
				AbsoluteLayout.SetLayoutFlags(cameraPreview, AbsoluteLayoutFlags.All);

				// Capture Button
				var buttonSize = 60;

				captureButton = new Button();
				captureButton.Clicked += CaptureButton_Clicked;
				captureButton.BackgroundColor = Color.Black.MultiplyAlpha(.5);
				captureButton.WidthRequest = buttonSize;
				captureButton.HeightRequest = buttonSize;
				captureButton.CornerRadius = buttonSize / 2;
				captureButton.BorderWidth = 4;
				captureButton.BorderColor = Color.White;
				captureButton.HorizontalOptions = LayoutOptions.Center;
				captureButton.IsVisible = false;

				AbsoluteLayout.SetLayoutBounds(captureButton, new Rectangle(.5, .9, buttonSize, buttonSize));
				AbsoluteLayout.SetLayoutFlags(captureButton, AbsoluteLayoutFlags.PositionProportional);

				mainLayout.Children.Add(cameraPreview);
				mainLayout.Children.Add(captureButton);

				toggleCameraToolbarItem = new ToolbarItem(ApplicationResource.PageCaptureToolbarToggleCamera, null, () => ToggleCamera()) { Icon = "toggle.png" };
			}

			// Uploading Layout
			uploadingAnimationView = new AnimationView();
			uploadingAnimationView.Animation = "spinner.json";
			uploadingAnimationView.IsVisible = false;

			AbsoluteLayout.SetLayoutBounds(uploadingAnimationView, new Rectangle(.5, .5, 100, 100));
			AbsoluteLayout.SetLayoutFlags(uploadingAnimationView, AbsoluteLayoutFlags.PositionProportional);
			mainLayout.Children.Add(uploadingAnimationView);

			// Message Layout
			messageLabel = new Label() { HorizontalOptions = LayoutOptions.CenterAndExpand };
			messageLabel.SetBinding(Label.TextProperty, new Binding("MessageLabel"));
			messageLabel.IsVisible = false;

			AbsoluteLayout.SetLayoutBounds(messageLabel, new Rectangle(.5, .5, -1, -1));
			AbsoluteLayout.SetLayoutFlags(messageLabel, AbsoluteLayoutFlags.PositionProportional);
			mainLayout.Children.Add(messageLabel);

			browseMedaToolbarItem = new ToolbarItem() { Icon = "folder.png" };
			browseMedaToolbarItem.SetBinding(ToolbarItem.CommandProperty, new Binding("BrowseMediaCommand"));

			Content = mainLayout;
		}

		private void OnPageStateChanged(object sender, PageStateEventArgs e)
		{
			if (e.State == PredictionPageState.NoModelAvailable)
			{
				Console.WriteLine("No Model Available");

				messageLabel.IsVisible = true;
				uploadingAnimationView.IsVisible = false;
				if (cameraIsAvailable)
				{
					cameraPreview.IsVisible = false;
					captureButton.IsVisible = false;
				}
			}

			else if (e.State == PredictionPageState.CameraReady)
			{
				Console.WriteLine("Camera Ready");

				if (uploadingAnimationView.IsPlaying)
					uploadingAnimationView.Pause();

				if (!ToolbarItems.Contains(browseMedaToolbarItem))
					ToolbarItems.Add(browseMedaToolbarItem);
				browseMedaToolbarItem.IsEnabled = true;

				if (!ToolbarItems.Contains(toggleCameraToolbarItem))
					ToolbarItems.Add(toggleCameraToolbarItem);
				toggleCameraToolbarItem.IsEnabled = true;

				messageLabel.IsVisible = false;
				uploadingAnimationView.IsVisible = false;
				cameraPreview.IsVisible = true;
				captureButton.IsVisible = true;

				StartCamera();
			}

			else if (e.State == PredictionPageState.NoCameraReady)
			{
				Console.WriteLine("No Camera Ready");

				if (uploadingAnimationView.IsPlaying)
					uploadingAnimationView.Pause();

				if (!ToolbarItems.Contains(browseMedaToolbarItem))
				{
					ToolbarItems.Add(browseMedaToolbarItem);
					browseMedaToolbarItem.IsEnabled = true;
				}

				messageLabel.IsVisible = true;
				uploadingAnimationView.IsVisible = false;
				if (cameraIsAvailable)
				{
					cameraPreview.IsVisible = false;
					captureButton.IsVisible = false;
				}
			}

			else if (e.State == PredictionPageState.Uploading)
			{
				Console.WriteLine("Uploading");

				if (browseMedaToolbarItem != null) browseMedaToolbarItem.IsEnabled = false;
				if (toggleCameraToolbarItem != null) toggleCameraToolbarItem.IsEnabled = false;

				messageLabel.IsVisible = false;
				uploadingAnimationView.IsVisible = true;
				if (cameraIsAvailable)
				{
					cameraPreview.IsVisible = false;
					captureButton.IsVisible = false;
					StopCamera();
				}

				uploadingAnimationView.Play();
				uploadingAnimationView.Loop = true;
			}
		}

		async Task ProcessCameraPhoto(byte[] imageBytes)
		{
			await predictionInputModel.SaveBytes(imageBytes);
		}

		void CaptureButton_Clicked(object sender, EventArgs e)
		{
			if (cameraPreview != null && cameraPreview.Capture != null)
				cameraPreview.Capture.Execute(null);
		}

		void ToggleCamera()
		{
			if (cameraPreview != null && cameraPreview.Capture != null)
				cameraPreview.CameraOption = (cameraPreview.CameraOption == CameraOptions.Rear) ? CameraOptions.Front : CameraOptions.Rear;
		}

		void StopCamera()
		{
			if (CrossMedia.Current.IsCameraAvailable && cameraPreview != null && cameraPreview.StopCamera != null)
				cameraPreview.StopCamera.Execute(null);
		}

		void StartCamera()
		{
			if (CrossMedia.Current.IsCameraAvailable && cameraPreview != null && cameraPreview.StartCamera != null)
			{
				cameraPreview.CameraOption = Settings.CameraOption;
				cameraPreview.StartCamera.Execute(null);
			}
		}

		protected override void OnAppearing()
		{
			if (predictionInputModel.RefreshViewCommand.CanExecute(null))
				predictionInputModel.RefreshViewCommand.Execute(null);

			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			uploadingAnimationView.Pause();

			messageLabel.IsVisible = false;
			uploadingAnimationView.IsVisible = false;
			if (cameraIsAvailable)
			{
				StopCamera();
				cameraPreview.IsVisible = false;
				captureButton.IsVisible = false;
			}

			base.OnDisappearing();
		}
	}
}
