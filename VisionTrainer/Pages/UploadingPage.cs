using Lottie.Forms;
using VisionTrainer.Constants;
using VisionTrainer.Resources;
using VisionTrainer.ViewModels;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class UploadingPage : ContentPage
	{
		AnimationView animationView;

		public UploadingPage()
		{
			Title = ApplicationResource.PageUploadingTitle;
			BindingContext = new UploadingViewModel(Navigation);

			animationView = new AnimationView();
			animationView.Loop = true;
			animationView.Play();
			//animationView.SetBinding(AnimationView.IsPlayingProperty, new Binding("ShowAnimation"));
			//animationView.SetBinding(AnimationView.IsVisibleProperty, new Binding("ShowAnimation"));
			animationView.SetBinding(AnimationView.AnimationProperty, new Binding("Animation"));

			AbsoluteLayout.SetLayoutBounds(animationView, new Rectangle(.5, .4, 100, 100));
			AbsoluteLayout.SetLayoutFlags(animationView, AbsoluteLayoutFlags.PositionProportional);

			var messageLabel = new Label()
			{
				WidthRequest = 300,
				TextColor = Color.SlateGray,
				HorizontalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Center
			};
			messageLabel.SetBinding(Label.TextProperty, new Binding("RemainingItems"));
			AbsoluteLayout.SetLayoutBounds(messageLabel, new Rectangle(.5, 0, 400, 40));
			AbsoluteLayout.SetLayoutFlags(messageLabel, AbsoluteLayoutFlags.PositionProportional);

			var startButton = new Button()
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = AppColors.HeaderColor,
				TextColor = Color.White,
				FontAttributes = FontAttributes.Bold,
				WidthRequest = 180,
				HeightRequest = 40,
				CornerRadius = 10
			};
			startButton.SetBinding(Button.TextProperty, new Binding("UploadButtonText"));
			startButton.SetBinding(Button.CommandProperty, new Binding("StartUploadCommand"));
			startButton.SetBinding(Button.IsEnabledProperty, new Binding("UploadButtonEnabled"));
			AbsoluteLayout.SetLayoutBounds(startButton, new Rectangle(.5, 1, 180, 40));
			AbsoluteLayout.SetLayoutFlags(startButton, AbsoluteLayoutFlags.PositionProportional);

			var uploadingLayout = new AbsoluteLayout();
			uploadingLayout.Margin = new Thickness(40);
			uploadingLayout.Children.Add(messageLabel);
			uploadingLayout.Children.Add(animationView);
			uploadingLayout.Children.Add(startButton);

			Content = uploadingLayout;
		}

		protected override void OnDisappearing()
		{
			UploadingViewModel viewModel = (UploadingViewModel)BindingContext;
			if (viewModel.StopUploadCommand.CanExecute(null))
				viewModel.StopUploadCommand.Execute(null);

			base.OnDisappearing();
		}
	}
}
