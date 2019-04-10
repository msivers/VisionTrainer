using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DLToolkit.Forms.Controls;
using FFImageLoading.Forms;
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
			BindingContext = new CreateBatchViewModel();

			// Browse for images toolbar item
			var browsePhotosItem = new ToolbarItem()
			{
				Text = ApplicationResource.PageCaptureToolbarBrowsePhotos,
				Icon = "folder.png"
			};
			browsePhotosItem.SetBinding(MenuItem.CommandProperty, new Binding("SelectImagesCommand"));
			this.ToolbarItems.Add(browsePhotosItem);

			// List view
			ListView lstView = new ListView();
			lstView.SeparatorVisibility = SeparatorVisibility.None;
			lstView.RowHeight = 100;
			//lstView.IsPullToRefreshEnabled = true;
			//lstView.Refreshing += OnRefresh;
			//lstView.ItemSelected += OnSelection;
			//lstView.ItemTapped += OnTap;
			//lstView.ItemsSource = items;
			lstView.SetBinding(ListView.ItemsSourceProperty, new Binding("Media"));
			//lstView.SelectionMode = ListViewSelectionMode.None;
			lstView.ItemTemplate = new DataTemplate(typeof(CachedImageCell));

			Content = new StackLayout
			{
				Margin = new Thickness(10),
				Children =
				{
					new Label { Text = "ListView Interactivity", FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center },
					lstView
				}
			};
		}
	}

	public class CachedImageCell : ViewCell
	{
		public CachedImageCell()
		{
			var cachedImage = new CachedImage()
			{
				BackgroundColor = Color.Gray,
				DownsampleToViewSize = true,
				HeightRequest = 100,
				WidthRequest = 100,
				Aspect = Aspect.AspectFill,
				HorizontalOptions = LayoutOptions.Start,
				IsEnabled = false
			};
			cachedImage.SetBinding(CachedImage.SourceProperty, new Binding("PreviewPath"));

			var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
			deleteAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
			deleteAction.Clicked += OnDelete;
			this.ContextActions.Add(deleteAction);

			StackLayout layout = new StackLayout();
			layout.Padding = new Thickness(15, 0);
			layout.Children.Add(cachedImage);
			View = layout;
		}

		void OnDelete(object sender, EventArgs e)
		{
			//var item = (MenuItem)sender;
			//interactiveListViewCode.items.Remove(item.CommandParameter.ToString());
		}
	}

	public class textViewCell : ViewCell
	{
		public textViewCell()
		{
			StackLayout layout = new StackLayout();
			layout.Padding = new Thickness(15, 0);
			Label label = new Label();

			label.SetBinding(Label.TextProperty, "Path");
			layout.Children.Add(label);

			var moreAction = new MenuItem { Text = "More" };
			moreAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
			moreAction.Clicked += OnMore;

			var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
			deleteAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
			deleteAction.Clicked += OnDelete;

			this.ContextActions.Add(moreAction);
			this.ContextActions.Add(deleteAction);
			View = layout;
		}

		void OnMore(object sender, EventArgs e)
		{
			var item = (MenuItem)sender;
			//Do something here... e.g. Navigation.pushAsync(new specialPage(item.commandParameter));
			//page.DisplayAlert("More Context Action", item.CommandParameter + " more context action", 	"OK");
		}

		void OnDelete(object sender, EventArgs e)
		{
			//var item = (MenuItem)sender;
			//interactiveListViewCode.items.Remove(item.CommandParameter.ToString());
		}
	}
}
