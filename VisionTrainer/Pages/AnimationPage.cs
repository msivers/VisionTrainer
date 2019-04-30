using System;
using Lottie.Forms;
using Xamarin.Forms;
using XamLottie;

namespace VisionTrainer.Pages
{
	public class AnimationPage : ContentPage
	{
		public AnimationPage()
		{
			var animationView = new AnimationView();
			animationView.Animation = "spinner.json"; //https://lottiefiles.com/271-loader
			animationView.AutoPlay = true;
			animationView.WidthRequest = animationView.HeightRequest = 100;
			animationView.Loop = true;

			var layout = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Children =
				{
					animationView
				}
			};

			Content = layout;
		}
	}
}
