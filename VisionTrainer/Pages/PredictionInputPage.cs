using System;
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
		RelativeLayout layout;

		CameraPreview cameraPreview;
		Image heroImage;
		PredictionInputViewModel predictionInputModel;
		Label titleLabel;
		Label messageLabel;
		Button captureButton;
		AnimationView animationView;
		ToolbarItem browseMedaToolbarItem;
		ToolbarItem toggleCameraToolbarItem;
		bool cameraIsAvailable;

		public PredictionInputPage()
		{
			BindingContext = predictionInputModel = new PredictionInputViewModel(Navigation);
			predictionInputModel.UpdatePageState += OnPageStateChanged;

			Title = ApplicationResource.PagePredictionInputTitle;
			layout = new RelativeLayout();

			// Camera Layout
			cameraIsAvailable = CrossMedia.Current.IsCameraAvailable;

			if (cameraIsAvailable)
			{
				// Camera Preview
				cameraPreview = new CameraPreview();
				cameraPreview.CameraOption = Settings.CameraOption;
				cameraPreview.CaptureBytesCallback = new Action<byte[]>(async (byte[] obj) => await ProcessCameraPhoto(obj));
				cameraPreview.IsVisible = false;

				layout.Children.Add(cameraPreview,
					Constraint.Constant(0),
					Constraint.Constant(0),
					Constraint.RelativeToParent((parent) => { return parent.Width; }),
					Constraint.RelativeToParent((parent) => { return parent.Height; }));

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

				layout.Children.Add(captureButton,
					Constraint.RelativeToParent((parent) => { return (parent.Width * .5) - (buttonSize * .5); }),
					Constraint.RelativeToParent((parent) => { return (parent.Height * .9) - (buttonSize * .5); }));

				toggleCameraToolbarItem = new ToolbarItem(ApplicationResource.PageCaptureToolbarToggleCamera, null, () => ToggleCamera()) { Icon = "toggle.png" };
			}



			heroImage = new Image();
			heroImage.HeightRequest = heroImage.WidthRequest = 200;
			heroImage.SetBinding(Image.SourceProperty, new Binding("HeroImage"));
			heroImage.IsVisible = false;
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

			animationView = new AnimationView();
			animationView.HeightRequest = animationView.WidthRequest = 200;
			animationView.Animation = "pulse.json";
			animationView.IsVisible = false;
			animationView.AutoPlay = true;
			layout.Children.Add(animationView,
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
			messageLabel.SetBinding(Label.TextProperty, new Binding("MessageLabel"));
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
			titleLabel.SetBinding(Label.TextProperty, new Binding("TitleLabel"));
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

			browseMedaToolbarItem = new ToolbarItem() { Icon = "folder.png" };
			browseMedaToolbarItem.SetBinding(ToolbarItem.CommandProperty, new Binding("BrowseMediaCommand"));

			Content = layout;
		}

		private void OnPageStateChanged(object sender, PageStateEventArgs e)
		{
			if (e.State == PredictionPageState.NoModelAvailable)
			{
				heroImage.IsVisible = true;
				titleLabel.IsVisible = true;
				messageLabel.IsVisible = true;
				animationView.IsVisible = false;

				if (cameraIsAvailable)
				{
					cameraPreview.IsVisible = false;
					captureButton.IsVisible = false;

					if (ToolbarItems.Contains(browseMedaToolbarItem))
						ToolbarItems.Remove(browseMedaToolbarItem);

					if (ToolbarItems.Contains(toggleCameraToolbarItem))
						ToolbarItems.Remove(toggleCameraToolbarItem);
				}
			}

			else if (e.State == PredictionPageState.CameraReady)
			{
				if (animationView.IsPlaying)
					animationView.Pause();

				if (!ToolbarItems.Contains(browseMedaToolbarItem))
					ToolbarItems.Add(browseMedaToolbarItem);
				browseMedaToolbarItem.IsEnabled = true;

				if (!ToolbarItems.Contains(toggleCameraToolbarItem))
					ToolbarItems.Add(toggleCameraToolbarItem);
				toggleCameraToolbarItem.IsEnabled = true;

				heroImage.IsVisible = false;
				titleLabel.IsVisible = false;
				messageLabel.IsVisible = false;
				animationView.IsVisible = false;
				cameraPreview.IsVisible = true;
				captureButton.IsVisible = true;

				StartCamera();
			}

			else if (e.State == PredictionPageState.NoCameraReady)
			{
				if (animationView.IsPlaying)
					animationView.Pause();

				if (!ToolbarItems.Contains(browseMedaToolbarItem))
				{
					ToolbarItems.Add(browseMedaToolbarItem);
					browseMedaToolbarItem.IsEnabled = true;
				}

				heroImage.IsVisible = true;
				titleLabel.IsVisible = true;
				messageLabel.IsVisible = true;
				animationView.IsVisible = false;
				if (cameraIsAvailable)
				{
					cameraPreview.IsVisible = false;
					captureButton.IsVisible = false;
				}
			}

			else if (e.State == PredictionPageState.Uploading)
			{
				if (browseMedaToolbarItem != null) browseMedaToolbarItem.IsEnabled = false;
				if (toggleCameraToolbarItem != null) toggleCameraToolbarItem.IsEnabled = false;

				heroImage.IsVisible = true;
				titleLabel.IsVisible = true;
				messageLabel.IsVisible = true;
				animationView.IsVisible = true;

				if (cameraIsAvailable)
				{
					cameraPreview.IsVisible = false;
					captureButton.IsVisible = false;
					StopCamera();
				}

				animationView.Play();
				animationView.Loop = true;
			}

			else if (e.State == PredictionPageState.None)
			{
				heroImage.IsVisible = false;
				titleLabel.IsVisible = false;
				messageLabel.IsVisible = false;
				animationView.IsVisible = false;

				if (cameraIsAvailable)
				{
					cameraPreview.IsVisible = false;
					captureButton.IsVisible = false;
					StopCamera();
				}
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
			animationView.Pause();

			messageLabel.IsVisible = false;
			animationView.IsVisible = false;
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
