﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using VisionTrainer.Interfaces;
using VisionTrainer.Models;
using Xamarin.Forms;

namespace VisionTrainer.ViewModels
{
	public class CreateBatchViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		IMultiMediaPickerService _multiMediaPickerService;

		ObservableCollection<MediaFile> media;
		public ObservableCollection<MediaFile> Media
		{
			get { return media; }
			set { SetProperty(ref media, value); }
		}

		public ICommand SelectImagesCommand { get; set; }
		public ICommand SelectVideosCommand { get; set; }

		public CreateBatchViewModel()
		{
			_multiMediaPickerService = ServiceContainer.Resolve<IMultiMediaPickerService>();

			//Media = new ObservableCollection<MediaFile>();

			SelectImagesCommand = new Command(async (obj) =>
			{
				//var hasPermission = await CheckPermissionsAsync();
				//if (hasPermission)
				//{
				Media = new ObservableCollection<MediaFile>();
				await _multiMediaPickerService.PickPhotosAsync();
				//}
			});

			SelectVideosCommand = new Command(async (obj) =>
			{
				//var hasPermission = await CheckPermissionsAsync();
				//if (hasPermission)
				//{
				Media = new ObservableCollection<MediaFile>();
				await _multiMediaPickerService.PickVideosAsync();
				//}
			});

			_multiMediaPickerService.OnMediaPicked += (s, a) =>
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					Media.Add(a);
				});
			};
		}

		/*
		async Task<bool> CheckPermissionsAsync()
		{
			var retVal = false;
			try
			{
				var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);
				if (status != PermissionStatus.Granted)
				{
					if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Plugin.Permissions.Abstractions.Permission.Storage))
					{
						await App.Current.MainPage.DisplayAlert("Alert", "Need Storage permission to access to your photos.", "Ok");
					}

					var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Plugin.Permissions.Abstractions.Permission.Storage });
					status = results[Plugin.Permissions.Abstractions.Permission.Storage];
				}

				if (status == PermissionStatus.Granted)
				{
					retVal = true;

				}
				else if (status != PermissionStatus.Unknown)
				{
					await App.Current.MainPage.DisplayAlert("Alert", "Permission Denied. Can not continue, try again.", "Ok");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				await App.Current.MainPage.DisplayAlert("Alert", "Error. Can not continue, try again.", "Ok");
			}

			return retVal;

		}
		*/

		bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (Object.Equals(storage, value))
				return false;

			storage = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
