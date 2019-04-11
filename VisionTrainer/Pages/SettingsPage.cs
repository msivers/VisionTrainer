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
			//cameraSwitch.SetBinding(SwitchCell.OnProperty, new Binding("DefaultCameraRear"));


			var clearDb = new TextCell() { Text = "Delete All Entries" };
			clearDb.SetBinding(TextCell.CommandProperty, new Binding("ClearDatabaseCommand"));
			// TODO set a prompt first

			Title = "Settings";
			Content = new TableView
			{
				Root = new TableRoot{
					new TableSection("Camera") {
						cameraSwitch
					},
					new TableSection("Database") {
						clearDb
					}
				}
			};
		}
	}
}


