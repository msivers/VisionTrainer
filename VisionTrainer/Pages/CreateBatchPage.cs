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

			// Browse for images toolbar item
			var browsePhotosItem = new ToolbarItem()
			{
				Text = ApplicationResource.PageCaptureToolbarBrowsePhotos,
				Icon = "folder.png"
			};
			browsePhotosItem.SetBinding(MenuItem.CommandProperty, new Binding("SelectImagesCommand"));
			this.ToolbarItems.Add(browsePhotosItem);

			// Tag Selection
			var tagPicker = new Picker();
			tagPicker.Title = "Traiing Tag:";
			tagPicker.SetBinding(Picker.ItemsSourceProperty, new Binding("Tags"));
			tagPicker.HorizontalOptions = LayoutOptions.CenterAndExpand;


			// Collection View
			// https://docs.microsoft.com/en-gb/xamarin/xamarin-forms/user-interface/collectionview/selection
			var collectionView = new CollectionView();
			collectionView.ItemsLayout = new GridItemsLayout(4, ItemsLayoutOrientation.Vertical);
			collectionView.SetBinding(CollectionView.ItemsSourceProperty, "Media");

			collectionView.ItemTemplate = new DataTemplate(() =>
			{
				Grid grid = new Grid { Padding = 5 };
				grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

				Image image = new Image { Aspect = Aspect.AspectFill, HeightRequest = 100, WidthRequest = 100 };
				image.SetBinding(Image.SourceProperty, "PreviewPath");

				grid.Children.Add(image);

				return grid;
			});

			Content = new StackLayout
			{
				Margin = new Thickness(10),
				Children =
				{
					tagPicker,
					collectionView
				}
			};
		}
	}
}
