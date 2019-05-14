using System;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using VisionTrainer.Models;
using VisionTrainer.Resources;
using VisionTrainer.ViewModels;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public class MediaDetailPage : BaseContentPage
	{
		public MediaDetailPage(MediaDetails media)
		{
			Title = ApplicationResource.PageMediaDetailTitle;
			BindingContext = new MediaDetailViewModel(this.Navigation, media);

			var image = new CachedImage { Aspect = Aspect.AspectFill };
			image.SetBinding(CachedImage.SourceProperty, "MediaFilePath");

			var deleteToolbarItem = new ToolbarItem() { Text = "Delete" };
			deleteToolbarItem.Command = new Command(async () => await PromptDeleteMedia());

			//deleteToolbarItem.SetBinding(ToolbarItem.CommandProperty, new Binding("DeleteMediaCommand"));

			ToolbarItems.Add(deleteToolbarItem);

			var layout = new StackLayout
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Children =
				{
					image
				}
			};

			Content = layout;
		}

		async Task PromptDeleteMedia()
		{
			string action = await DisplayActionSheet(ApplicationResource.PageBrowseMediaRemovePhotoPrompt, ApplicationResource.Cancel, ApplicationResource.PageBrowseMediaRemoveConfirm);

			if (action == ApplicationResource.PageBrowseMediaRemoveConfirm)
			{
				var binding = (BindingContext as MediaDetailViewModel);
				if (binding.DeleteMediaCommand.CanExecute(null))
					binding.DeleteMediaCommand.Execute(null);
			}
		}
	}
}
