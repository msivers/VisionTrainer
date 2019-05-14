using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.Permissions;
using VisionTrainer.Interfaces;
using VisionTrainer.Droid.Services;
using Lottie.Forms;
using Android.Content;

namespace VisionTrainer.Droid
{
	[Activity(Label = "VisionTrainer", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			App.ScreenWidth = (int)Resources.DisplayMetrics.WidthPixels; // real pixels
			App.ScreenHeight = (int)Resources.DisplayMetrics.HeightPixels; // real pixels

			this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

			base.OnCreate(savedInstanceState);

			// Initialize platform specific services
			FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
			ServiceContainer.Register<IMultiMediaPickerService>(MultiMediaPickerService.SharedInstance);

			// Load forms app
			global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
			global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
			Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);

			LoadApplication(new App());
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
			PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			MultiMediaPickerService.SharedInstance.OnActivityResult(requestCode, resultCode, data);
		}
	}
}