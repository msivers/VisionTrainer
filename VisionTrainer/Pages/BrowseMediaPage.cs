using System.Linq;
using FFImageLoading.Forms;
using VisionTrainer.Models;
using VisionTrainer.Resources;
using VisionTrainer.ViewModels;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class BrowseMediaPage : ContentPage
	{
		public BrowseMediaPage()
		{
			Title = ApplicationResource.PageBrowseMediaTitle;
			BindingContext = new BrowseMediaViewModel(Navigation);

			var browseToolbarItem = new ToolbarItem() { Icon = "folder.png" };
			browseToolbarItem.SetBinding(ToolbarItem.CommandProperty, new Binding("SelectImagesCommand"));
			this.ToolbarItems.Add(browseToolbarItem);


			/*
			// Top navigation
			var topGrid = new Grid
			{
				BackgroundColor = AppColors.HeaderColor,
				ColumnSpacing = 1,
				RowSpacing = 1
			};

			topGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(64, GridUnitType.Absolute) });
			topGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
			topGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			topGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });


			// Tab Label
			var tagLabel = new Label();
			tagLabel.Text = ApplicationResource.PageCreateBatchTagPrompt;
			tagLabel.HorizontalOptions = LayoutOptions.Start;

			// Tag Selection
			var tagPicker = new Picker();
			tagPicker.SetBinding(Picker.ItemsSourceProperty, new Binding("Tags"));
			tagPicker.SetBinding(Picker.SelectedItemProperty, new Binding("SelectedTag"));
			tagPicker.HorizontalOptions = LayoutOptions.StartAndExpand;
			*/

			// Collection View
			// https://docs.microsoft.com/en-gb/xamarin/xamarin-forms/user-interface/collectionview/selection
			var collectionView = new CollectionView { SelectionMode = SelectionMode.Single };
			collectionView.SelectionChanged += async (object sender, SelectionChangedEventArgs e) =>
			{
				string action = await DisplayActionSheet(ApplicationResource.PageBrowseMediaRemovePhotoPrompt, ApplicationResource.Cancel, ApplicationResource.PageBrowseMediaRemoveConfirm);

				if (action == ApplicationResource.PageBrowseMediaRemoveConfirm)
				{
					var targetMedia = (MediaDetails)e.CurrentSelection.FirstOrDefault();
					var binding = (BindingContext as BrowseMediaViewModel);
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

				var image = new CachedImage { Aspect = Aspect.AspectFill, HeightRequest = 100, WidthRequest = 100 };
				image.SetBinding(CachedImage.SourceProperty, "FullPreviewPath");

				grid.Children.Add(image);

				return grid;
			});

			Content = new StackLayout
			{
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.StartAndExpand,
				Children =
				{
					collectionView
				}
			};
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			BrowseMediaViewModel viewModel = (BrowseMediaViewModel)BindingContext;
			if (viewModel.CompleteCommand.CanExecute(null))
				viewModel.CompleteCommand.Execute(null);

			base.OnDisappearing();
		}
	}
}
