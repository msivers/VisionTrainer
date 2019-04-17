using System;
using System.IO;
using LiteDB;
using MonkeyCache.FileStore;
using VisionTrainer.Constants;
using VisionTrainer.Interfaces;
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

		public App()
		{
			InitializeComponent();

			Barrel.ApplicationId = "VisionTrainer";
			//var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TrainerSQLite.db3");
			//ServiceContainer.Register<IDatabase>(new TrainerDatabase(dbPath));

			var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Trainer.db");
			ServiceContainer.Register<IDatabase>(new TrainerDb(dbPath));
			ServiceContainer.Register<IUploadManager>(new UploadManager());

			ServiceContainer.Resolve<IMultiMediaPickerService>().DirectoryName = ProjectConfig.ImagesDirectory;

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
