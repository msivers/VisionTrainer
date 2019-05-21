using Lottie.Forms;
using VisionTrainer.Constants;
using VisionTrainer.Resources;
using VisionTrainer.ViewModels;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class UploadingPage : BaseContentPage
	{
		AnimationView animationView;
		RelativeLayout uploadingLayout;

		public UploadingPage()
		{
			Title = ApplicationResource.PageUploadingTitle;
			BindingContext = new UploadingViewModel(Navigation);

			animationView = new AnimationView();
			animationView.SetBinding(AnimationView.IsPlayingProperty, new Binding("ShowAnimation"));
			animationView.SetBinding(AnimationView.IsVisibleProperty, new Binding("ShowAnimation"));
			animationView.SetBinding(AnimationView.AnimationProperty, new Binding("Animation"));
			animationView.SetBinding(AnimationView.LoopProperty, new Binding("ShouldLoopAnimation"));
			animationView.AutoPlay = true;
			animationView.HeightRequest = animationView.WidthRequest = 200;
			animationView.Play();

			var heroImage = new Image();
			heroImage.HeightRequest = heroImage.WidthRequest = 200;
			heroImage.SetBinding(Image.SourceProperty, new Binding("HeroImage"));
			heroImage.SizeChanged += HeroImage_SizeChanged;

			var titleLabel = new Label()
			{
				WidthRequest = 300,
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.Black,
				HorizontalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Center
			};
			titleLabel.SetBinding(Label.TextProperty, new Binding("StatusTitle"));

			var messageLabel = new Label()
			{
				WidthRequest = 300,
				TextColor = Color.SlateGray,
				HorizontalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Center
			};
			messageLabel.SetBinding(Label.TextProperty, new Binding("StatusMessage"));

			var startButton = new Button()
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				FontAttributes = FontAttributes.Bold,
				WidthRequest = 180,
				HeightRequest = 40,
				CornerRadius = 20
			};
			startButton.SetBinding(Button.TextProperty, new Binding("UploadButtonText"));
			startButton.SetBinding(Button.TextColorProperty, new Binding("UploadButtonTextColor"));
			startButton.SetBinding(Button.BackgroundColorProperty, new Binding("UploadButtonColor"));
			startButton.SetBinding(Button.CommandProperty, new Binding("StartUploadCommand"));
			startButton.SetBinding(Button.IsEnabledProperty, new Binding("UploadButtonEnabled"));
			startButton.SetBinding(Button.IsVisibleProperty, new Binding("UploadEnabled"));

			AbsoluteLayout.SetLayoutBounds(startButton, new Rectangle(.5, 1, 180, 40));
			AbsoluteLayout.SetLayoutFlags(startButton, AbsoluteLayoutFlags.PositionProportional);

			uploadingLayout = new RelativeLayout();
			uploadingLayout.Margin = new Thickness(40);

			uploadingLayout.Children.Add(animationView,
				Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width * .5) - (heroImage.Width / 2);
				}),
				Constraint.RelativeToParent((parent) =>
				{
					return parent.Height * .3 - (heroImage.Height / 2);
				})
			);
			uploadingLayout.Children.Add(heroImage,
				Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width * .5) - (heroImage.Width / 2);
				}),
				Constraint.RelativeToParent((parent) =>
				{
					return parent.Height * .3 - (heroImage.Height / 2);
				})
			);

			uploadingLayout.Children.Add(startButton,
				Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width * .5) - (startButton.Width / 2);
				}),
				Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height * .9) - startButton.Height;
				})
			);
			uploadingLayout.Children.Add(messageLabel,
				Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width * .5) - (messageLabel.Width / 2);
				}),
				Constraint.RelativeToView(startButton, (parent, sibling) =>
				{
					return startButton.Y - messageLabel.Height - 40;
				})
			);
			uploadingLayout.Children.Add(titleLabel,
				Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width * .5) - (titleLabel.Width / 2);
				}),
				Constraint.RelativeToView(messageLabel, (parent, sibling) =>
				{
					return messageLabel.Y - titleLabel.Height - 10;
				})
			);

			Content = uploadingLayout;
		}

		private void HeroImage_SizeChanged(object sender, System.EventArgs e)
		{
			uploadingLayout.ForceLayout();
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
