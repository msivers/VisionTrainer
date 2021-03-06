﻿using FFImageLoading.Forms;
using VisionTrainer.Constants;
using VisionTrainer.Resources;
using VisionTrainer.ViewModels;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class TrainPage : ContentPage
	{
		public TrainPage()
		{
			Title = ApplicationResource.PageTrainTitle;
			BindingContext = new TrainViewModel(this.Navigation);

			// Top navigation
			var topGrid = new Grid
			{
				BackgroundColor = AppColors.HeaderColor,
				ColumnSpacing = 1,
				RowSpacing = 1
			};

			topGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
			topGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			topGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			topGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

			// Upload Button
			var uploadButton = new ImageButton()
			{
				Source = "upload_alt.png",
				Margin = new Thickness(5),
				BackgroundColor = Color.Transparent
			};
			uploadButton.SetBinding(Button.CommandProperty, new Binding("UploadFilesCommand"));
			topGrid.Children.Add(uploadButton, 0, 0);

			// Capture Button
			var captureButton = new ImageButton()
			{
				Source = "capture_alt.png",
				Margin = new Thickness(5),
				BackgroundColor = Color.Transparent
			};
			captureButton.SetBinding(Button.CommandProperty, new Binding("CaptureImagesCommand"));
			topGrid.Children.Add(captureButton, 1, 0);

			// Add Button
			var addButton = new ImageButton()
			{
				Source = "folder_alt.png",
				Margin = new Thickness(5),
				BackgroundColor = Color.Transparent
			};
			addButton.SetBinding(Button.CommandProperty, new Binding("AddMediaCommand"));
			topGrid.Children.Add(addButton, 2, 0);

			// TableView
			var itemsListView = new ListView { Margin = new Thickness(6, 0, 6, 0), SeparatorVisibility = SeparatorVisibility.None, RowHeight = 64 };
			itemsListView.SetBinding(ListView.ItemsSourceProperty, new Binding("Media"));
			itemsListView.ItemSelected += (sender, e) => (BindingContext as TrainViewModel).MediaSelectedCommand.Execute(e.SelectedItem);
			itemsListView.ItemTemplate = new DataTemplate(() =>
			{
				Grid grid = new Grid();
				grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60, GridUnitType.Absolute) });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Absolute) });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

				CachedImage image = new CachedImage { Aspect = Aspect.AspectFill, WidthRequest = 60, HeightRequest = 60 };
				image.SetBinding(CachedImage.SourceProperty, "FullPreviewPath");
				grid.Children.Add(image, 0, 0);

				var vertLayout = new StackLayout() { Orientation = StackOrientation.Vertical };

				Label idLabel = new Label() { VerticalOptions = LayoutOptions.Center };
				idLabel.TextColor = Color.SlateGray;
				idLabel.FontSize = 12;
				idLabel.SetBinding(Label.TextProperty, "Type");
				vertLayout.Children.Add(idLabel);

				Label dateLabel = new Label() { VerticalOptions = LayoutOptions.Center };
				dateLabel.TextColor = Color.Black;
				dateLabel.FontSize = 10;
				dateLabel.SetBinding(Label.TextProperty, "Date");
				vertLayout.Children.Add(dateLabel);

				grid.Children.Add(vertLayout, 2, 0);

				return new ViewCell() { View = grid };
			});

			var layout = new StackLayout()
			{
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.StartAndExpand,
				Children =
				{
					topGrid,
					itemsListView
				}
			};

			Content = layout;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			(BindingContext as TrainViewModel).RefreshMediaEntriesCommand.Execute(null);
		}
	}
}
