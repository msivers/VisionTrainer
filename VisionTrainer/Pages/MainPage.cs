﻿using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using VisionTrainer.Constants;
using VisionTrainer.Resources;
using VisionTrainer.Services;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class MainPage : TabbedPage
	{
		IStatefulContent currentStatefulPage;

		public MainPage()
		{
			CheckPermissions();
		}

		async Task CheckPermissions()
		{
			Settings.ActivePublishedModel = await AzureService.RemoteModelAvailable();

			var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
			if (status != PermissionStatus.Granted)
			{
				Application.Current.ModalPopping += HandleModalPopping;
				await Navigation.PushModalAsync(new PermissionsPage(), false);
			}

			else
			{
				CreateContent();
			}
		}

		private void HandleModalPopping(object sender, ModalPoppingEventArgs e)
		{
			Application.Current.ModalPopping -= HandleModalPopping;
			CreateContent();
		}

		void CreateContent()
		{
			var textColor = Color.White;

			var testNavigationPage = new NavigationPage(new PredictionInputPage());
			testNavigationPage.Title = ApplicationResource.NavigationTestTitle;
			testNavigationPage.Icon = "capture.png";
			testNavigationPage.BarTextColor = textColor;
			testNavigationPage.BarBackgroundColor = AppColors.HeaderColor;
			Children.Add(testNavigationPage);

			var trainNavigationPage = new NavigationPage(new TrainPage());
			trainNavigationPage.Title = ApplicationResource.NavigationTrainTitle;
			trainNavigationPage.Icon = "pictures.png";
			trainNavigationPage.BarTextColor = textColor;
			trainNavigationPage.BarBackgroundColor = AppColors.HeaderColor;
			Children.Add(trainNavigationPage);

			var settingsNavigationPage = new NavigationPage(new SettingsPage());
			settingsNavigationPage.Title = ApplicationResource.NavigationSettingsTitle;
			settingsNavigationPage.Icon = "settings.png";
			settingsNavigationPage.BarTextColor = textColor;
			settingsNavigationPage.BarBackgroundColor = AppColors.HeaderColor;
			Children.Add(settingsNavigationPage);
		}


		protected override void OnCurrentPageChanged()
		{
			base.OnCurrentPageChanged();

			var navPage = (NavigationPage)CurrentPage;
			if (currentStatefulPage != null && currentStatefulPage != navPage.CurrentPage)
			{
				currentStatefulPage.DidDisappear();
				currentStatefulPage = null;
			}

			if (navPage.CurrentPage is IStatefulContent)
			{
				var stateful = (IStatefulContent)navPage.CurrentPage;
				stateful.DidAppear();
				currentStatefulPage = stateful;
			}
		}
	}
}

