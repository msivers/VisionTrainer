using System;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using VisionTrainer.Resources;

namespace VisionTrainer.Utils
{
	public static class PermissionsCheck
	{
		public static async Task<bool> CameraAsync()
		{
			var retVal = false;
			try
			{
				var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
				if (status != PermissionStatus.Granted)
				{
					if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
						await App.Current.MainPage.DisplayAlert(
							ApplicationResource.CameraPermissionPromptTitle,
							ApplicationResource.CameraPermissionPromptMessage,
							ApplicationResource.OK);

					var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
					if (results.ContainsKey(Permission.Camera))
						status = results[Permission.Camera];
				}

				if (status == PermissionStatus.Granted)
				{
					retVal = true;
				}

				else if (status != PermissionStatus.Unknown)
				{
					await App.Current.MainPage.DisplayAlert(
						ApplicationResource.CameraPermissionDeniedTitle,
						ApplicationResource.CameraPermissionDeniedMessage,
						ApplicationResource.OK);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				await App.Current.MainPage.DisplayAlert(
					ApplicationResource.GeneralErrorTitle,
					ApplicationResource.GeneralErrorSummary,
					ApplicationResource.OK);
			}

			return retVal;
		}

		public static async Task<bool> PhotosAsync()
		{
			var retVal = false;
			try
			{
				var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
				if (status != PermissionStatus.Granted)
				{
					if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
					{
						await App.Current.MainPage.DisplayAlert(
							ApplicationResource.PhotosPermissionPromptTitle,
							ApplicationResource.PhotosPermissionPromptMessage,
							ApplicationResource.OK);
					}

					var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
					status = results[Permission.Storage];
				}

				if (status == PermissionStatus.Granted)
				{
					retVal = true;
				}

				else if (status != PermissionStatus.Unknown)
				{
					await App.Current.MainPage.DisplayAlert(
						ApplicationResource.PhotosPermissionDeniedTitle,
						ApplicationResource.PhotosPermissionDeniedMessage,
						ApplicationResource.OK);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				await App.Current.MainPage.DisplayAlert(
					ApplicationResource.GeneralErrorTitle,
					ApplicationResource.GeneralErrorSummary,
					ApplicationResource.OK);
			}

			return retVal;
		}
	}
}

