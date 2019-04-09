using System;
using VisionTrainer.Resources;
using VisionTrainer.ViewModels;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class ProcessPage : ContentPage
	{
		public ProcessPage()
		{
			Title = ApplicationResource.PageProcessTitle;
			BindingContext = new ProcessViewModel(this.Navigation);

			var createNewItem = new ToolbarItem()
			{
				Text = ApplicationResource.PageCaptureToolbarBrowsePhotos,
				Icon = "folder.png"
			};
			createNewItem.SetBinding(MenuItem.CommandProperty, new Binding("NewBatchCommand"));
			this.ToolbarItems.Add(createNewItem);

			var layout = new StackLayout();
			layout.HorizontalOptions = LayoutOptions.CenterAndExpand;
			layout.VerticalOptions = LayoutOptions.CenterAndExpand;
			layout.Children.Add(new Label() { Text = "List of Batches" });

			Content = layout;
		}
	}
}
