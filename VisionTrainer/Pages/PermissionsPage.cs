using System.Threading.Tasks;
using Lottie.Forms;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using VisionTrainer.Resources;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class PermissionsPage : ContentPage
	{
		Button cameraButton;
		Button photosButton;

		bool hasCameraPermission;
		bool hasPhotoPermission;

		public PermissionsPage()
		{
			var animation = new AnimationView()
			{
				Loop = true,
				AutoPlay = true,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Animation = "LoadingAnimation.json"
			};

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
				BackgroundColor = Color.DarkBlue,
				TextColor = Color.White,
				FontAttributes = FontAttributes.Bold,
				WidthRequest = 180,
				HeightRequest = 40,
				CornerRadius = 10,
				Margin = new Thickness(0, 20, 0, 5)
			};
			cameraButton.Clicked += async (sender, e) => await CheckCameraPermissions();

			photosButton = new Button()
			{
				Text = ApplicationResource.PagePermissionsPhotosButton,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = Color.DarkBlue,
				TextColor = Color.White,
				FontAttributes = FontAttributes.Bold,
				WidthRequest = 180,
				HeightRequest = 40,
				CornerRadius = 10
			};
			photosButton.Clicked += async (sender, e) => await CheckPhotosPermissions();

			var centerLayout = new StackLayout();
			centerLayout.HorizontalOptions = LayoutOptions.CenterAndExpand;
			centerLayout.VerticalOptions = LayoutOptions.CenterAndExpand;
			centerLayout.Children.Add(titleLabel);
			centerLayout.Children.Add(messageLabel);
			centerLayout.Children.Add(cameraButton);
			centerLayout.Children.Add(photosButton);

			Content = centerLayout;
		}

		async Task CheckCameraPermissions()
		{
			var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
			if (status != PermissionStatus.Granted)
			{
				if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
					await DisplayAlert(ApplicationResource.CameraPermissionPromptTitle, ApplicationResource.CameraPermissionPromptMessage, ApplicationResource.OK);

				var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
				if (results.ContainsKey(Permission.Camera))
					status = results[Permission.Camera];
			}

			if (status == PermissionStatus.Granted)
			{
				cameraButton.IsEnabled = false;
				cameraButton.Text = ApplicationResource.CameraPermissionGranted;
				cameraButton.BackgroundColor = Color.LightGray;
				hasCameraPermission = true;
				await CheckComplete();
			}

			else if (status != PermissionStatus.Unknown)
				await DisplayAlert(ApplicationResource.CameraPermissionDeniedTitle, ApplicationResource.CameraPermissionDeniedMessage, ApplicationResource.OK);
		}

		async Task CheckPhotosPermissions()
		{
			var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);
			if (status != PermissionStatus.Granted)
			{
				if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Photos))
					await DisplayAlert(ApplicationResource.PhotosPermissionPromptTitle, ApplicationResource.PhotosPermissionPromptMessage, ApplicationResource.OK);

				var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Photos);
				if (results.ContainsKey(Permission.Photos))
					status = results[Permission.Photos];
			}

			if (status == PermissionStatus.Granted)
			{
				photosButton.IsEnabled = false;
				photosButton.Text = ApplicationResource.PhotosPermissionGranted;
				photosButton.BackgroundColor = Color.LightGray;
				hasPhotoPermission = true;
				await CheckComplete();
			}

			else if (status != PermissionStatus.Unknown)
				await DisplayAlert(ApplicationResource.PhotosPermissionDeniedTitle, ApplicationResource.PhotosPermissionDeniedMessage, ApplicationResource.OK);
		}

		async Task CheckComplete()
		{
			if (hasCameraPermission && hasPhotoPermission)
				await Navigation.PopModalAsync();
		}

	}
}
