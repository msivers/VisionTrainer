using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using VisionTrainer.Pages;
using MonkeyCache.FileStore;
using VisionTrainer.Models;
using VisionTrainer.Services;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace VisionTrainer
{
	public partial class App : Application
	{
		public static int ScreenWidth;
		public static int ScreenHeight;
		public static int CameraRatio;

		public App()
		{
			InitializeComponent();

			Barrel.ApplicationId = "VisionTrainer";
			//ServiceContainer.Register<IHistoryService>(new HistoryService());

			MainPage = new MainPage();
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
