using System;
using System.IO;
using VisionTrainer.Pages;
using VisionTrainer.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace VisionTrainer
{
	public partial class App : Application
	{
		public static int ScreenWidth;
		public static int ScreenHeight;
		public static int CameraRatio;
		public static Page CurrentTabPage;

		public App()
		{
			InitializeComponent();

			var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Trainer.db");
			ServiceContainer.Register<IDatabase>(new TrainerDb(dbPath));

			MainPage = new MainPage();
		}

		protected override void OnStart()
		{
			AppCenter.Start("ios=a3696b2c-7dab-4ff3-b00f-a6e6fe3d99e0;" +
				  "android=4501ec07-075d-48df-ae30-b6526d16361c;",
				  typeof(Analytics), typeof(Crashes));
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
