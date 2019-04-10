using DLToolkit.Forms.Controls;
using MonkeyCache.FileStore;
using VisionTrainer.Pages;
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
			FlowListView.Init();

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
