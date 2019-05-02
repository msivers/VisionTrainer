using System;
using System.IO;
using VisionTrainer.Pages;
using VisionTrainer.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
			ServiceContainer.Register<IUploadManager>(new UploadManager());

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
