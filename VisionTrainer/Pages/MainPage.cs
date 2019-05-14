using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using VisionTrainer.Constants;
using VisionTrainer.Resources;
using VisionTrainer.Services;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace VisionTrainer.Pages
{
	public class MainPage : Xamarin.Forms.TabbedPage
	{
		IStatefulContent currentStatefulPage;

		public MainPage()
		{
			On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);

			Analytics.TrackEvent("Main Page");

			CreateContent();
		}

		void CreateDebugPrediction()
		{
			var media = new Models.MediaDetails()
			{
				Path = "IMG_0111.jpg"
			};

			var predictions = new List<PredictionModel>
				{
					new PredictionModel(tagName:"test", boundingBox:new BoundingBox(.25,.25, .5, .5), probability:.5),
					new PredictionModel(tagName:"test", boundingBox:new BoundingBox(.1,.1, .2, .2), probability:.8)

				};

			var imagePrediction = new ImagePrediction(predictions: predictions);

			var testNavigationPage = new NavigationPage(new PredictionResultsPage(media, imagePrediction));
			testNavigationPage.Title = ApplicationResource.NavigationTestTitle;
			testNavigationPage.Icon = "capture.png";
			testNavigationPage.BarTextColor = Color.White;
			testNavigationPage.BarBackgroundColor = AppColors.HeaderColor;
			Children.Add(testNavigationPage);
		}

		void CreateAnimationPage()
		{
			var testNavigationPage = new NavigationPage(new AnimationPage());
			testNavigationPage.Title = "Anim";
			testNavigationPage.BarTextColor = Color.White;
			testNavigationPage.BarBackgroundColor = AppColors.HeaderColor;
			Children.Add(testNavigationPage);
		}

		void CreateContent()
		{
			var textColor = Color.White;

			var predictionNavigationPage = new NavigationPage(new PredictionInputPage());
			predictionNavigationPage.Title = ApplicationResource.NavigationTestTitle;
			predictionNavigationPage.Icon = "capture.png";
			predictionNavigationPage.BarTextColor = textColor;
			predictionNavigationPage.BarBackgroundColor = AppColors.HeaderColor;
			Children.Add(predictionNavigationPage);

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

			// Check for permissions
			Task.Run(async () =>
			{
				Analytics.TrackEvent("Check Permissions");
				var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
				if (status != PermissionStatus.Granted)
				{
					await Navigation.PushModalAsync(new PermissionsPage(), false);
				}
			});
		}

		protected override void OnCurrentPageChanged()
		{
			base.OnCurrentPageChanged();

			var navPage = (NavigationPage)CurrentPage;
			//navPage.PopToRootAsync();

			var name = navPage.CurrentPage.GetType().Name;
			Analytics.TrackEvent("Page: " + name);

			App.CurrentTabPage = navPage.CurrentPage;
		}
	}
}

