using System;
using DLToolkit.Forms.Controls;
using VisionTrainer.Resources;
using VisionTrainer.ViewModels;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class CreateBatchPageAlt : ContentPage
	{
		public CreateBatchPageAlt()
		{
			Title = ApplicationResource.PageCreateBatchTitle;
			BindingContext = new CreateBatchViewModel();

			// Browse for images toolbar item
			var browsePhotosItem = new ToolbarItem()
			{
				Text = ApplicationResource.PageCaptureToolbarBrowsePhotos,
				Icon = "folder.png"
			};
			browsePhotosItem.SetBinding(MenuItem.CommandProperty, new Binding("SelectImagesCommand"));
			this.ToolbarItems.Add(browsePhotosItem);

			// Flowlist
			var flowList = new FlowListView()
			{
				FlowColumnCount = 3,
				SeparatorVisibility = SeparatorVisibility.None,
				HasUnevenRows = false,
				RowHeight = 100
			};

			// Template
			var dataTemplate = new DataTemplate(() =>
			{
				var basicView = new BoxView()
				{
					WidthRequest = 100,
					HeightRequest = 100,
					BackgroundColor = Color.Red,
					HorizontalOptions = LayoutOptions.FillAndExpand
				};

				//var cachedImage = new CachedImage()
				//{
				//	BackgroundColor = Color.Gray,
				//	DownsampleToViewSize = true,
				//	HeightRequest = 100,
				//	Aspect = Aspect.AspectFill,
				//	HorizontalOptions = LayoutOptions.FillAndExpand
				//};
				//cachedImage.SetBinding(CachedImage.SourceProperty, new Binding("PreviewPath"));

				return basicView;
			});

			flowList.SetBinding(FlowListView.FlowItemsSourceProperty, new Binding("Media"));
			flowList.ItemTemplate = dataTemplate;

			var buttonTest = new Button()
			{
				Text = "test",
				BackgroundColor = Color.AliceBlue,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 40
			};

			buttonTest.Clicked += (sender, e) =>
			{
				//collection.AddRange(new MediaFile[3]);
			};

			Content = new StackLayout
			{
				Margin = new Thickness(20),
				Children =
				{
					buttonTest,
					flowList
				}
			};
		}
	}
}

