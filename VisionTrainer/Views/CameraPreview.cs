﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using VisionTrainer.Constants;
using Xamarin.Forms;

namespace VisionTrainer
{
	public class CameraPreview : View
	{
		public event EventHandler<EventArgs> CameraReady;

		public static readonly BindableProperty CameraProperty = BindableProperty.Create(
			propertyName: PropertyIds.CameraOption,
			returnType: typeof(CameraOptions),
			declaringType: typeof(CameraPreview),
			defaultValue: CameraOptions.Rear,
			defaultBindingMode: BindingMode.OneWay,
			propertyChanged: CameraPropertyChanged
		);

		public CameraOptions CameraOption
		{
			get { return (CameraOptions)GetValue(CameraProperty); }
			set { SetValue(CameraProperty, value); }
		}

		private static void CameraPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var control = (CameraPreview)bindable;
			control.CameraOption = (CameraOptions)newValue;
		}

		public static readonly BindableProperty AutoVisibleProperty = BindableProperty.Create(
			propertyName: "AutoVisible",
			returnType: typeof(bool),
			declaringType: typeof(CameraPreview),
			defaultValue: false);

		public bool AutoVisible
		{
			get { return (bool)GetValue(AutoVisibleProperty); }
			set { SetValue(AutoVisibleProperty, value); }
		}

		// File Path callback
		public static readonly BindableProperty CaptureBytesCallbackProperty = BindableProperty.Create(
			propertyName: "CaptureBytesCallback",
			returnType: typeof(Action<byte[]>),
			declaringType: typeof(CameraPreview),
			defaultValue: null);

		public Action<byte[]> CaptureBytesCallback
		{
			get { return (Action<byte[]>)GetValue(CaptureBytesCallbackProperty); }
			set { SetValue(CaptureBytesCallbackProperty, value); }
		}

		public static readonly BindableProperty CaptureCommandProperty = BindableProperty.Create<CameraPreview, ICommand>(p => p.Capture, null);
		public ICommand Capture
		{
			get { return (ICommand)GetValue(CaptureCommandProperty); }
			set
			{
				SetValue(CaptureCommandProperty, value);
				CheckIsReady();
			}
		}

		public static readonly BindableProperty StartCommandProperty = BindableProperty.Create<CameraPreview, ICommand>(p => p.StartCamera, null);
		public ICommand StartCamera
		{
			get { return (ICommand)GetValue(StartCommandProperty); }
			set
			{
				SetValue(StartCommandProperty, value);
				CheckIsReady();
			}
		}

		public static readonly BindableProperty StopCommandProperty = BindableProperty.Create<CameraPreview, ICommand>(p => p.StopCamera, null);
		public ICommand StopCamera
		{
			get { return (ICommand)GetValue(StopCommandProperty); }
			set
			{
				SetValue(StopCommandProperty, value);
				CheckIsReady();
			}
		}

		void CheckIsReady()
		{
			if (StartCommandProperty != null && CaptureCommandProperty != null && StopCommandProperty != null)
			{
				CameraReady?.Invoke(this, EventArgs.Empty);
			}
		}
	}
}
