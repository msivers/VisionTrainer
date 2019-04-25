using System;
using System.Threading.Tasks;
using Plugin.Media;
using VisionTrainer.Resources;
using VisionTrainer.ViewModels;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class PredictionInputPage : ContentPage
	{
		CameraPreview cameraPreview;
		PredictionInputViewModel predictionInputModel;
		Label messageLabel;

		// Language
		string messageOK = ApplicationResource.OK;
		string messageCameraNotSupported = ApplicationResource.CameraNotSupported;
		string messageCameraPermissionsMissing = ApplicationResource.CameraPermissionMissing;

		public PredictionInputPage()
		{
			BindingContext = predictionInputModel = new PredictionInputViewModel(Navigation);
			Title = ApplicationResource.PageCaptureTitle;

			var layout = new AbsoluteLayout();

			// TODO move this switch into the viewmodel to determine whats visible?
			if (CrossMedia.Current.IsCameraAvailable)
			{
				BackgroundColor = Color.Black;

				// Camera Preview
				cameraPreview = new CameraPreview();
				cameraPreview.CameraOption = Settings.CameraOption;
				cameraPreview.CaptureBytesCallback = new Action<byte[]>(async (byte[] obj) => await ProcessCameraPhoto(obj));
				cameraPreview.CameraReady += (s, e) => StartCamera();

				AbsoluteLayout.SetLayoutBounds(cameraPreview, new Rectangle(1, 1, 1, 1));
				AbsoluteLayout.SetLayoutFlags(cameraPreview, AbsoluteLayoutFlags.All);

				// Capture Button
				var buttonSize = 60;
				var captureButton = new Button();
				captureButton.Clicked += CaptureButton_Clicked;
				captureButton.BackgroundColor = Color.White;
				captureButton.WidthRequest = buttonSize;
				captureButton.HeightRequest = buttonSize;
				captureButton.CornerRadius = buttonSize / 2;
				captureButton.BorderWidth = 1;
				captureButton.BorderColor = Color.Black;
				captureButton.HorizontalOptions = LayoutOptions.Center;

				AbsoluteLayout.SetLayoutBounds(captureButton, new Rectangle(.5, .9, buttonSize, buttonSize));
				AbsoluteLayout.SetLayoutFlags(captureButton, AbsoluteLayoutFlags.PositionProportional);

				layout.Children.Add(cameraPreview);
				layout.Children.Add(captureButton);

				Content = layout;

				this.ToolbarItems.Add(
					new ToolbarItem(ApplicationResource.PageCaptureToolbarToggleCamera, null, () => ToggleCamera()) { Icon = "toggle.png" }
				);

				//this.ToolbarItems.Add(
				//	new ToolbarItem(ApplicationResource.PageCaptureToolbarBrowsePhotos, null, () => ToggleCamera()) { Icon = "toggle.png" }
				//);
			}

			else
			{
				messageLabel = new Label()
				{
					Text = messageCameraNotSupported,
					HorizontalOptions = LayoutOptions.CenterAndExpand
				};

				AbsoluteLayout.SetLayoutBounds(messageLabel, new Rectangle(.5, .5, -1, -1));
				AbsoluteLayout.SetLayoutFlags(messageLabel, AbsoluteLayoutFlags.PositionProportional);
				layout.Children.Add(messageLabel);
			}

			Content = layout;
		}

		async Task ProcessCameraPhoto(byte[] imageBytes)
		{
			cameraPreview.Opacity = .5;
			StopCamera();

			await predictionInputModel.SaveBytes(imageBytes);
		}

		void CaptureButton_Clicked(object sender, EventArgs e)
		{
			if (!CrossMedia.Current.IsCameraAvailable)
			{
				DisplayAlert(messageCameraNotSupported, messageCameraPermissionsMissing, messageOK);
				return;
			}

			if (cameraPreview != null && cameraPreview.Capture != null)
				cameraPreview.Capture.Execute(null);
		}

		void ToggleCamera()
		{
			if (!CrossMedia.Current.IsCameraAvailable)
			{
				DisplayAlert(messageCameraNotSupported, messageCameraPermissionsMissing, messageOK);
				return;
			}

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

		public void DidAppear()
		{
			StartCamera();
		}

		public void DidDisappear()
		{
			StopCamera();
		}
	}
}
