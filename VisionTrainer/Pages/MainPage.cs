using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using VisionTrainer.Resources;
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
			var backgroundColor = Color.FromRgb(73, 113, 175);

			var captureNavigationPage = new NavigationPage(new CapturePage());
			captureNavigationPage.Title = ApplicationResource.NavigationCaptureTitle;
			//captureNavigationPage.Icon = "person.png";
			captureNavigationPage.BarTextColor = textColor;
			captureNavigationPage.BarBackgroundColor = backgroundColor;
			Children.Add(captureNavigationPage);

			var processNavigationPage = new NavigationPage(new ProcessPage());
			processNavigationPage.Title = ApplicationResource.NavigationProcessTitle;
			//processNavigationPage.Icon = "person.png";
			processNavigationPage.BarTextColor = textColor;
			processNavigationPage.BarBackgroundColor = backgroundColor;
			Children.Add(processNavigationPage);

			var settingsNavigationPage = new NavigationPage(new SettingsPage());
			settingsNavigationPage.Title = ApplicationResource.NavigationSettingsTitle;
			//settingsNavigationPage.Icon = "settings.png";
			settingsNavigationPage.BarTextColor = textColor;
			settingsNavigationPage.BarBackgroundColor = backgroundColor;
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

