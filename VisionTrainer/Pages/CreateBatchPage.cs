using System;
using DLToolkit.Forms.Controls;
using FFImageLoading.Forms;
using VisionTrainer.Resources;
using VisionTrainer.ViewModels;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class CreateBatchPage : ContentPage
	{
		public CreateBatchPage()
		{
			Title = ApplicationResource.PageCreateBatchTitle;
			BindingContext = new CreateBatchViewModel();

			var browsePhotosItem = new ToolbarItem()
			{
				Text = ApplicationResource.PageCaptureToolbarBrowsePhotos,
				Icon = "folder.png"
			};
			browsePhotosItem.SetBinding(MenuItem.CommandProperty, new Binding("SelectImagesCommand"));
			this.ToolbarItems.Add(browsePhotosItem);

			var flowList = new FlowListView()
			{
				FlowColumnCount = 3,
				SeparatorVisibility = SeparatorVisibility.None,
				HasUnevenRows = false,
				RowHeight = 100
			};

			var dataTemplate = new DataTemplate(() =>
			{
				var cachedImage = new CachedImage()
				{
					BackgroundColor = Color.Gray,
					DownsampleToViewSize = true,
					HeightRequest = 100,
					Aspect = Aspect.AspectFill,
					HorizontalOptions = LayoutOptions.FillAndExpand
				};
				cachedImage.SetBinding(CachedImage.SourceProperty, new Binding("PreviewPath"));

				return new ViewCell { View = cachedImage };
			});

			flowList.SetBinding(FlowListView.FlowItemsSourceProperty, new Binding("Media"));
			flowList.FlowColumnTemplate = dataTemplate;

			var layout = new StackLayout();
			layout.HorizontalOptions = LayoutOptions.CenterAndExpand;
			layout.VerticalOptions = LayoutOptions.CenterAndExpand;
			layout.Children.Add(flowList);

			Content = layout;
		}
	}
}
