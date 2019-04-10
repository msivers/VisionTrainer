using System;
using System.Collections.Generic;
using VisionTrainer.ViewModels;
using Xamarin.Forms;

namespace VisionTrainer.Pages
{
	public partial class TestPage : ContentPage
	{
		public TestPage()
		{
			InitializeComponent();

			BindingContext = new CreateBatchViewModel();
		}
	}
}
