using System;
using VisionTrainer.Constants;
using VisionTrainer.Resources;
using VisionTrainer.ViewModels;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class UploadingPage : ContentPage
	{
		public UploadingPage()
		{
			BindingContext = new UploadingViewModel(Navigation);

			var titleLabel = new Label()
			{
				Text = "Uploading", // ApplicationResource.PagePermissionsTitle,
				FontSize = 30,
				TextColor = Color.DarkSlateGray,
				HorizontalOptions = LayoutOptions.CenterAndExpand
			};

			var messageLabel = new Label()
			{
				WidthRequest = 300,
				TextColor = Color.SlateGray,
				HorizontalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Center
			};
			messageLabel.SetBinding(Label.TextProperty, new Binding("RemainingItems"));

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
			startButton.SetBinding(Button.TextProperty, new Binding("ButtonText"));
			startButton.SetBinding(Button.CommandProperty, new Binding("StartUploadCommand"));


			var centerLayout = new StackLayout();
			centerLayout.HorizontalOptions = LayoutOptions.CenterAndExpand;
			centerLayout.VerticalOptions = LayoutOptions.CenterAndExpand;
			centerLayout.Children.Add(titleLabel);
			centerLayout.Children.Add(messageLabel);
			centerLayout.Children.Add(startButton);


			Content = centerLayout;
		}
	}
}
