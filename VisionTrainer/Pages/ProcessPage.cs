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
			BindingContext = new ProcessViewModel();

			var browsePhotosItem = new ToolbarItem()
			{
				Text = ApplicationResource.PageCaptureToolbarBrowsePhotos,
				Icon = "folder.png"
			};
			browsePhotosItem.SetBinding(MenuItem.CommandProperty, new Binding("SelectImagesCommand"));
			this.ToolbarItems.Add(browsePhotosItem);

			var layout = new StackLayout();
			layout.HorizontalOptions = LayoutOptions.CenterAndExpand;
			layout.VerticalOptions = LayoutOptions.CenterAndExpand;
			layout.Children.Add(new Label() { Text = "Hello World" });

			Content = layout;
		}
	}
}
