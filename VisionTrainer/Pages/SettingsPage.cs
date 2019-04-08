using System;
using VisionTrainer.ViewModels;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class SettingsPage : ContentPage
	{
		public SettingsPage()
		{
			BindingContext = new SettingsViewModel();

			var cameraSwitch = new SwitchCell { Text = "Rear Camera" };
			cameraSwitch.SetBinding(SwitchCell.OnProperty, new Binding("DefaultCameraRear"));

			var cameraLocation = new EntryCell { Label = "Camera Location", Keyboard = Keyboard.Default };
			cameraLocation.SetBinding(EntryCell.TextProperty, new Binding("CameraLocation", BindingMode.TwoWay));

			var timerEntry = new EntryCell { Label = "Camera Refresh", Keyboard = Keyboard.Numeric };
			timerEntry.SetBinding(EntryCell.TextProperty, new Binding("TimerInterval", BindingMode.TwoWay));

			var historyEntry = new EntryCell { Label = "History Refresh", Keyboard = Keyboard.Numeric };
			timerEntry.SetBinding(EntryCell.TextProperty, new Binding("HistoryInterval", BindingMode.TwoWay));

			Title = "Settings";
			Content = new TableView
			{
				Root = new TableRoot{
					new TableSection("Camera") {
						cameraLocation,
						cameraSwitch
					},

					new TableSection("Monitor Interval"){
						timerEntry
					},

					new TableSection("History Interval"){
						timerEntry
					}
				}
			};
		}
	}
}


