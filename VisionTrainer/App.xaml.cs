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
			AppCenter.Start("ios=661fec4a-f4dc-4202-8cc6-d9783867fb81;" +
				  "android=afb0687b-7bb4-4d8f-8619-46bf1b303bc5;",
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
