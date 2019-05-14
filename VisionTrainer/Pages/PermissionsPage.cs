using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using VisionTrainer.Constants;
using VisionTrainer.Resources;
using VisionTrainer.Utils;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class PermissionsPage : BaseContentPage
	{
		Button cameraButton;
		Button photosButton;

		bool hasCameraPermission;
		bool hasPhotoPermission;

		public PermissionsPage()
		{
			var titleLabel = new Label()
			{
				Text = ApplicationResource.PagePermissionsTitle,
				FontSize = 30,
				TextColor = Color.DarkSlateGray,
				HorizontalOptions = LayoutOptions.CenterAndExpand
			};

			var messageLabel = new Label()
			{
				Text = ApplicationResource.PagePermissionsMessage,
				WidthRequest = 300,
				TextColor = Color.SlateGray,
				HorizontalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Center
			};

			cameraButton = new Button()
			{
				Text = ApplicationResource.PagePermissionsCameraButton,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = AppColors.HeaderColor,
				TextColor = Color.White,
				FontAttributes = FontAttributes.Bold,
				WidthRequest = 180,
				HeightRequest = 40,
				CornerRadius = 10,
				Margin = new Thickness(0, 20, 0, 5)
			};
			cameraButton.Clicked += async (sender, e) =>
			{

				var hasPermission = await PermissionsCheck.CameraAsync();
				if (hasPermission)
				{
					cameraButton.IsEnabled = false;
					cameraButton.Text = ApplicationResource.CameraPermissionGranted;
					cameraButton.BackgroundColor = Color.LightGray;
					hasCameraPermission = true;
					await CheckComplete();
				}
			};

			photosButton = new Button()
			{
				Text = ApplicationResource.PagePermissionsPhotosButton,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = AppColors.HeaderColor,
				TextColor = Color.White,
				FontAttributes = FontAttributes.Bold,
				WidthRequest = 180,
				HeightRequest = 40,
				CornerRadius = 10
			};

			photosButton.Clicked += async (sender, e) =>
			{

				var hasPermission = await PermissionsCheck.PhotosAsync();
				if (hasPermission)
				{
					photosButton.IsEnabled = false;
					photosButton.Text = ApplicationResource.PhotosPermissionGranted;
					photosButton.BackgroundColor = Color.LightGray;
					hasPhotoPermission = true;
					await CheckComplete();
				}
			};

			var centerLayout = new StackLayout();
			centerLayout.HorizontalOptions = LayoutOptions.CenterAndExpand;
			centerLayout.VerticalOptions = LayoutOptions.CenterAndExpand;
			centerLayout.Children.Add(titleLabel);
			centerLayout.Children.Add(messageLabel);
			centerLayout.Children.Add(cameraButton);
			centerLayout.Children.Add(photosButton);

			Content = centerLayout;
		}

		async Task CheckComplete()
		{
			if (hasCameraPermission && hasPhotoPermission)
				await Navigation.PopModalAsync();
		}
	}
}
