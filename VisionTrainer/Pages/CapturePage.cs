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
		CachedImage cachedCapture;
		Label titleLabel;
		Label messageLabel;
		Image heroImage;
		RelativeLayout layout;

		public CapturePage()
		{
			BindingContext = captureModel = new CaptureViewModel(Navigation);
			Title = ApplicationResource.PageCaptureTitle;
			layout = new RelativeLayout();

			if (CrossMedia.Current.IsCameraAvailable)
			{
				BackgroundColor = Color.Black;

				// Camera Preview
				cameraPreview = new CameraPreview();
				cameraPreview.CameraOption = Settings.CameraOption;
				cameraPreview.CaptureBytesCallback = new Action<byte[]>(ProcessCameraPhoto);
				cameraPreview.CameraReady += (s, e) => StartCamera();

				layout.Children.Add(cameraPreview,
					Constraint.Constant(0),
					Constraint.Constant(0),
					Constraint.RelativeToParent((parent) => { return parent.Width; }),
					Constraint.RelativeToParent((parent) => { return parent.Height; }));

				// Last Capture
				cachedCapture = new CachedImage();
				cachedCapture.Aspect = Aspect.AspectFill;
				cachedCapture.BackgroundColor = Color.White;

				layout.Children.Add(cachedCapture,
					Constraint.Constant(5),
					Constraint.Constant(5),
					Constraint.Constant(80),
					Constraint.Constant(80));

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

				layout.Children.Add(captureButton,
					Constraint.RelativeToParent((parent) => { return (parent.Width * .5) - (buttonSize * .5); }),
					Constraint.RelativeToParent((parent) => { return (parent.Height * .9) - (buttonSize * .5); }));

				this.ToolbarItems.Add(
					new ToolbarItem(ApplicationResource.PageCaptureToolbarToggleCamera, null, () => ToggleCamera()) { Icon = "toggle.png" }
				);
			}

			else
			{
				heroImage = new Image();
				heroImage.HeightRequest = heroImage.WidthRequest = 200;
				heroImage.Source = ImageSource.FromFile("CameraMissing");
				heroImage.SizeChanged += (s, e) =>
				{
					layout.ForceLayout();
				};


				layout.Children.Add(heroImage,
					Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width * .5) - (heroImage.Width / 2);
					}),
					Constraint.RelativeToParent((parent) =>
					{
						return parent.Height * .3 - (heroImage.Height / 2);
					})
				);

				messageLabel = new Label()
				{
					WidthRequest = 300,
					TextColor = Color.SlateGray,
					HorizontalOptions = LayoutOptions.Center,
					HorizontalTextAlignment = TextAlignment.Center
				};
				messageLabel.Text = ApplicationResource.CameraNotSupportedMessage;

				layout.Children.Add(messageLabel,
				Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width * .5) - (messageLabel.Width / 2);
				}),
					Constraint.RelativeToParent((parent) =>
					{
						return (parent.Height * .8) - (messageLabel.Height);
					})
				);

				titleLabel = new Label()
				{
					WidthRequest = 300,
					HeightRequest = 20,
					FontAttributes = FontAttributes.Bold,
					TextColor = Color.Black,
					HorizontalOptions = LayoutOptions.Center,
					HorizontalTextAlignment = TextAlignment.Center
				};
				titleLabel.Text = ApplicationResource.CameraNotSupportedTitle;
				layout.Children.Add(titleLabel,
				Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width * .5) - (titleLabel.Width / 2);
					}),
					Constraint.RelativeToView(messageLabel, (parent, sibling) =>
					{
						return messageLabel.Y - titleLabel.Height - 10;
					})
				);
			}

			Content = layout;
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
				DisplayAlert(ApplicationResource.CameraNotSupportedTitle, ApplicationResource.CameraPermissionMissing, ApplicationResource.OK);
				return;
			}
			Console.WriteLine("Clicked");
			if (cameraPreview != null && cameraPreview.Capture != null)
				cameraPreview.Capture.Execute(null);
		}

		void ToggleCamera()
		{
			if (!CrossMedia.Current.IsCameraAvailable)
			{
				DisplayAlert(ApplicationResource.CameraNotSupportedTitle, ApplicationResource.CameraPermissionMissing, ApplicationResource.OK);
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