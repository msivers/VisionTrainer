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

			var clearDb = new TextCell() { Text = "Delete All Local Entries" };
			clearDb.SetBinding(TextCell.CommandProperty, new Binding("ClearDatabaseCommand"));
			// TODO set a prompt first

			var userIdCell = new EntryCell() { Label = "User Id" };
			userIdCell.SetBinding(EntryCell.TextProperty, new Binding("UserId"));

			var endpointCell = new EntryCell() { Label = "Endpoint" };
			endpointCell.SetBinding(EntryCell.TextProperty, new Binding("Endpoint"));

			Title = "Settings";
			Content = new TableView
			{
				Root = new TableRoot{
					new TableSection("Configuration")
					{
					userIdCell,
endpointCell
					},
					new TableSection("Camera") {
						cameraSwitch
					},
					new TableSection("Database") {
						clearDb
					},
				}
			};
		}
	}
}


