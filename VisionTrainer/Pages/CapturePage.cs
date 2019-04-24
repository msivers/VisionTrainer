using System;
using System.IO;
using FFImageLoading.Forms;
using Plugin.Media;
using VisionTrainer.Resources;
using VisionTrainer.ViewModels;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class CapturePage : ContentPage, IStatefulContent
	{
		CameraPreview cameraPreview;
		CaptureViewModel captureModel;
		Label cameraMissingLabel;
		CachedImage cachedCapture;

		// Language
		string messageOK = ApplicationResource.OK;
		string messageCameraNotSupported = ApplicationResource.CameraNotSupported;
		string messageCameraPermissionsMissing = ApplicationResource.CameraPermissionMissing;

		public CapturePage()
		{
			BindingContext = captureModel = new CaptureViewModel(Navigation);
			Title = ApplicationResource.PageCaptureTitle;

			if (CrossMedia.Current.IsCameraAvailable)
			{
				BackgroundColor = Color.Black;

				// Camera Preview
				cameraPreview = new CameraPreview();
				cameraPreview.Filename = "capture";
				cameraPreview.CameraOption = Settings.CameraOption;
				cameraPreview.CaptureBytesCallback = new Action<byte[]>(ProcessCameraPhoto);
				cameraPreview.CameraReady += (s, e) => StartCamera();

				AbsoluteLayout.SetLayoutBounds(cameraPreview, new Rectangle(1, 1, 1, 1));
				AbsoluteLayout.SetLayoutFlags(cameraPreview, AbsoluteLayoutFlags.All);

				// Last Capture
				cachedCapture = new CachedImage();
				cachedCapture.Aspect = Aspect.AspectFill;
				cachedCapture.BackgroundColor = Color.White;
				AbsoluteLayout.SetLayoutBounds(cachedCapture, new Rectangle(5, 5, 80, 80));
				AbsoluteLayout.SetLayoutFlags(cachedCapture, AbsoluteLayoutFlags.None);

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

				var layout = new AbsoluteLayout();
				layout.Children.Add(cameraPreview);
				layout.Children.Add(cachedCapture);
				layout.Children.Add(captureButton);

				Content = layout;

				this.ToolbarItems.Add(
					new ToolbarItem(ApplicationResource.PageCaptureToolbarToggleCamera, null, () => ToggleCamera()) { Icon = "toggle.png" }
				);
			}

			else
			{
				cameraMissingLabel = new Label()
				{
					Text = messageCameraNotSupported,
					HorizontalOptions = LayoutOptions.CenterAndExpand
				};

				var centerLayout = new StackLayout();
				centerLayout.HorizontalOptions = LayoutOptions.CenterAndExpand;
				centerLayout.VerticalOptions = LayoutOptions.CenterAndExpand;
				centerLayout.Children.Add(cameraMissingLabel);

				Content = centerLayout;
			}
		}

		void ProcessCameraPhoto(byte[] imageBytes)
		{
			var filename = captureModel.SaveBytes(imageBytes);

			cachedCapture.Source = ImageSource.FromStream(() =>
			{
				return new MemoryStream(imageBytes);
			});
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

		public void DidAppear()
		{
			StartCamera();
		}

		public void DidDisappear()
		{
			StopCamera();
			if (cachedCapture != null)
				cachedCapture.Source = null;
		}
	}
}