using System;
using System.IO;
using Plugin.Media;
using VisionTrainer.Resources;
using VisionTrainer.ViewModels;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class CapturePage : ContentPage, IStatefulContent
	{
		CameraPreview cameraPreview;
		ActivityIndicator activityIndicator;
		CaptureViewModel captureModel;
		Label cameraMissingLabel;

		// Language
		string messageGeneralErrorTitle = ApplicationResource.GeneralErrorTitle;
		string messageGeneralErrorSummary = ApplicationResource.GeneralErrorSummary;
		string messageOK = ApplicationResource.OK;
		string messageCameraNotSupported = ApplicationResource.CameraNotSupported;
		string messagePhotosNotSupported = ApplicationResource.PhotosNotSupported;
		string messageUploading = ApplicationResource.PageCaptureImageUploading;
		string messageCameraPermissionsMissing = ApplicationResource.CameraPermissionMissing;
		string messagePhotosPermissionsMissing = ApplicationResource.PhotosPermissionMissing;

		public CapturePage()
		{
			BindingContext = captureModel = new CaptureViewModel(Navigation);
			Title = ApplicationResource.PageCaptureTitle;

			// Activity Indicator
			activityIndicator = new ActivityIndicator();
			activityIndicator.IsRunning = false;
			activityIndicator.IsVisible = false;

			this.ToolbarItems.Add(
				new ToolbarItem(ApplicationResource.PageCaptureToolbarBrowsePhotos, null, () => PickPhoto()) { Icon = "folder.png" }
			);

			if (CrossMedia.Current.IsCameraAvailable)
			{
				BackgroundColor = Color.Black;

				// Camera Preview
				cameraPreview = new CameraPreview();
				cameraPreview.Filename = "capture";
				cameraPreview.CameraOption = Settings.CameraOption;
				cameraPreview.CapturePathCallback = new Action<string>(ProcessCameraPhoto);
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

				activityIndicator.Color = Color.White;
				AbsoluteLayout.SetLayoutBounds(activityIndicator, new Rectangle(.5, .5, buttonSize, buttonSize));
				AbsoluteLayout.SetLayoutFlags(activityIndicator, AbsoluteLayoutFlags.PositionProportional);

				var layout = new AbsoluteLayout();
				layout.Children.Add(cameraPreview);
				layout.Children.Add(captureButton);
				layout.Children.Add(activityIndicator);

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

				activityIndicator.Color = Color.Black;
				activityIndicator.HorizontalOptions = LayoutOptions.CenterAndExpand;

				var centerLayout = new StackLayout();
				centerLayout.HorizontalOptions = LayoutOptions.CenterAndExpand;
				centerLayout.VerticalOptions = LayoutOptions.CenterAndExpand;
				centerLayout.Children.Add(cameraMissingLabel);
				centerLayout.Children.Add(activityIndicator);

				Content = centerLayout;
			}
		}

		void ShowActivityIndicator(bool value)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				activityIndicator.IsVisible = value;
				activityIndicator.IsRunning = value;
				if (cameraPreview != null)
				{
					cameraPreview.Opacity = value ? .6 : 1;
				}

				if (cameraMissingLabel != null) cameraMissingLabel.Text = value ? messageUploading : messageCameraNotSupported;
			});
		}

		async void ProcessCameraPhoto(string path)
		{
			byte[] imageBytes = File.ReadAllBytes(path);

			ShowActivityIndicator(true);
			var response = await captureModel.Submit(imageBytes);
			ShowActivityIndicator(false);

			Device.BeginInvokeOnMainThread(async () =>
			{
				if (response.Result)
					await captureModel.Success();
				else
				{
					var message = string.IsNullOrEmpty(response.Message) ? messageGeneralErrorSummary : response.Message;
					await DisplayAlert(messageGeneralErrorTitle, message, messageOK);
				}
			});
		}

		async void PickPhoto()
		{
			if (!CrossMedia.Current.IsPickPhotoSupported)
			{
				await DisplayAlert(messagePhotosNotSupported, messagePhotosPermissionsMissing, messageOK);
				return;
			}
			var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
			{
				PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
			});

			if (file == null)
				return;

			byte[] imageBytes = null;

			using (var memoryStream = new MemoryStream())
			{
				file.GetStream().CopyTo(memoryStream);
				file.Dispose();
				imageBytes = memoryStream.ToArray();
			}

			ShowActivityIndicator(true);
			var response = await captureModel.Submit(imageBytes);
			ShowActivityIndicator(false);

			if (response.Result)
				await captureModel.Success();
			else
			{
				var message = string.IsNullOrEmpty(response.Message) ? messageGeneralErrorSummary : response.Message;
				await DisplayAlert(messageGeneralErrorTitle, message, messageOK);
			}
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
		}
	}
}