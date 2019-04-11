using System;
using VisionTrainer.Constants;
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

			// Upload Button
			var uploadButton = new Button()
			{
				Text = "Upload",
				TextColor = Color.White,
				HeightRequest = 40
			};
			uploadButton.SetBinding(Button.CommandProperty, new Binding("UploadFilesCommand"));
			topGrid.Children.Add(uploadButton, 0, 0);

			// Add Button
			var addButton = new Button()
			{
				Text = "Add",
				TextColor = Color.White,
				HeightRequest = 40,
				BorderColor = Color.White
			};
			addButton.SetBinding(Button.CommandProperty, new Binding("NewBatchCommand"));
			topGrid.Children.Add(addButton, 1, 0);

			// TODO Upload button
			// TODO Create Button


			// TODO List view of all items
			// TODO Delete list items
			// TODO remove items once uploaded

			// TableView
			var itemsListView = new ListView { Margin = new Thickness(0, 20, 0, 0) };
			itemsListView.SetBinding(ListView.ItemsSourceProperty, new Binding("Media"));
			itemsListView.ItemTemplate = new DataTemplate(() =>
			{
				Grid grid = new Grid() { Margin = 5, BackgroundColor = Color.Gray };
				grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

				Image image = new Image { Aspect = Aspect.AspectFill };
				image.SetBinding(Image.SourceProperty, "PreviewPath");
				grid.Children.Add(image, 0, 0);

				Label idLabel = new Label();
				idLabel.SetBinding(Label.TextProperty, "Tag");
				grid.Children.Add(idLabel, 1, 0);

				return new ViewCell() { View = grid };
			});

			var layout = new StackLayout()
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
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
			(BindingContext as ProcessViewModel).RefreshMediaEntriesCommand.Execute(null);
		}
	}
}
