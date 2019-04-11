using System;
using System.Linq;
using System.Threading.Tasks;
using VisionTrainer.Constants;
using VisionTrainer.Models;
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
			BindingContext = new CreateBatchViewModel(Navigation);

			// Browse for images toolbar item
			var browsePhotosItem = new ToolbarItem()
			{
				Text = ApplicationResource.PageCaptureToolbarBrowsePhotos,
				Icon = "folder.png"
			};
			browsePhotosItem.SetBinding(MenuItem.CommandProperty, new Binding("SelectImagesCommand"));
			this.ToolbarItems.Add(browsePhotosItem);

			// Top navigation
			var topGrid = new Grid
			{
				BackgroundColor = AppColors.HeaderColor,
				ColumnSpacing = 1,
				RowSpacing = 1
			};

			topGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24, GridUnitType.Absolute) }); // TODO Create pretty header
			topGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
			topGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			topGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

			// Done button
			var doneButton = new Button { Text = "Done", FontAttributes = FontAttributes.Bold, TextColor = Color.White };
			doneButton.SetBinding(Button.CommandProperty, new Binding("CompleteCommand"));
			topGrid.Children.Add(doneButton, 0, 1);

			// Add button
			var addPhotosButton = new Button { Text = "Select Photos", FontAttributes = FontAttributes.Bold, TextColor = Color.White };
			addPhotosButton.SetBinding(Button.CommandProperty, new Binding("SelectImagesCommand"));
			topGrid.Children.Add(addPhotosButton, 1, 1);

			// Tab Label
			var tagLabel = new Label();
			tagLabel.Text = ApplicationResource.PageCreateBatchTagPrompt;
			tagLabel.HorizontalOptions = LayoutOptions.Start;

			// Tag Selection
			var tagPicker = new Picker();
			tagPicker.SetBinding(Picker.ItemsSourceProperty, new Binding("Tags"));
			tagPicker.SetBinding(Picker.SelectedItemProperty, new Binding("SelectedTag"));
			tagPicker.HorizontalOptions = LayoutOptions.StartAndExpand;

			// Collection View
			// https://docs.microsoft.com/en-gb/xamarin/xamarin-forms/user-interface/collectionview/selection
			var collectionView = new CollectionView { SelectionMode = SelectionMode.Single };
			collectionView.SelectionChanged += async (object sender, SelectionChangedEventArgs e) =>
			{
				string action = await DisplayActionSheet(ApplicationResource.PageCreateBatchRemovePhotoPrompt, ApplicationResource.Cancel, ApplicationResource.PageCreateBatchRemoveConfirm);

				if (action == ApplicationResource.PageCreateBatchRemovePhotoPrompt)
				{
					var targetMedia = (MediaFile)e.CurrentSelection.FirstOrDefault();
					var binding = (BindingContext as CreateBatchViewModel);
					if (binding.RemoveImageCommand.CanExecute(targetMedia))
						binding.RemoveImageCommand.Execute(targetMedia);
				}
			};
			collectionView.ItemsLayout = new GridItemsLayout(4, ItemsLayoutOrientation.Vertical);
			collectionView.SetBinding(CollectionView.ItemsSourceProperty, "Media");

			collectionView.ItemTemplate = new DataTemplate(() =>
			{
				Grid grid = new Grid();
				grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

				Image image = new Image { Aspect = Aspect.AspectFill, HeightRequest = 100, WidthRequest = 100 };
				image.SetBinding(Image.SourceProperty, "PreviewPath");

				grid.Children.Add(image);

				return grid;
			});

			Content = new StackLayout
			{
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.StartAndExpand,
				Children =
				{
					topGrid,
					tagLabel,
					tagPicker,
					collectionView
				}
			};
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
		}
	}
}
