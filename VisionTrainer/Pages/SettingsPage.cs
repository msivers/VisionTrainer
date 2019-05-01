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
			cameraSwitch.SetBinding(SwitchCell.OnProperty, new Binding("DefaultCameraRear", BindingMode.TwoWay));

			var clearDb = new TextCell() { Text = "Delete All Local Entries" };
			clearDb.SetBinding(TextCell.CommandProperty, new Binding("ClearDatabaseCommand"));

			var userIdCell = new EntryCell() { Label = "User Id" };
			userIdCell.SetBinding(EntryCell.TextProperty, new Binding("UserId", BindingMode.TwoWay));

			var endpointCell = new EntryCell() { Label = "Endpoint" };
			endpointCell.SetBinding(EntryCell.TextProperty, new Binding("Endpoint", BindingMode.TwoWay));

			var apiKeyCell = new EntryCell() { Label = "Api Key" };
			apiKeyCell.SetBinding(EntryCell.TextProperty, new Binding("ApiKey", BindingMode.TwoWay));

			var modelNameCell = new EntryCell() { Label = "Published Model Name" };
			modelNameCell.SetBinding(EntryCell.TextProperty, new Binding("PublishedModelName", BindingMode.TwoWay));

			Title = "Settings";
			Content = new TableView
			{
				Root = new TableRoot{
					new TableSection("Configuration")
					{
						userIdCell,
						endpointCell,
						apiKeyCell,
						modelNameCell
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


