using System;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class BaseContentPage : ContentPage
	{
		public BaseContentPage()
		{
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			var name = this.GetType().Name;

			Analytics.TrackEvent("Page: " + name);
		}
	}
}
