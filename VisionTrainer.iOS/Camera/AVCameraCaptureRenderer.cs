using System;
using System.ComponentModel;
using AVCameraCapture;
using VisionTrainer;
using VisionTrainer.Constants;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CameraPreview), typeof(AVCameraCaptureRenderer))]
namespace AVCameraCapture
{
	public class AVCameraCaptureRenderer : ViewRenderer<CameraPreview, AVCameraCaptureView>
	{
		CameraPreview element;
		AVCameraCaptureView uiCameraPreview;
		Action<byte[]> capturePathCallbackAction;

		protected override void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
		{
			base.OnElementChanged(e);


			if (Control == null)
			{
				uiCameraPreview = new AVCameraCaptureView(e.NewElement.CameraOption);
				uiCameraPreview.ImageCaptured += UiCameraPreview_ImageCaptured;
				SetNativeControl(uiCameraPreview);
			}
			if (e.OldElement != null)
			{
				// Unsubscribe
				uiCameraPreview.ImageCaptured -= UiCameraPreview_ImageCaptured;
				capturePathCallbackAction = null;
				element.Capture = null;
				element.StartCamera = null;
				element.StopCamera = null;
			}
			if (e.NewElement != null)
			{
				// Subscribe
				element = e.NewElement;
				capturePathCallbackAction = element.CaptureBytesCallback;
				element.Capture = new Command(() => uiCameraPreview.Capture());
				element.StartCamera = new Command(() => uiCameraPreview.StartPreviewing());
				element.StopCamera = new Command(() => uiCameraPreview.StopPreviewing());
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			Console.WriteLine(e.PropertyName);

			if (e.PropertyName == PropertyIds.CameraOption)
			{
				var view = (CameraPreview)sender;
				uiCameraPreview.UpdateCameraOption(view.CameraOption);
			}

			else if (e.PropertyName == PropertyIds.Width)
			{
				uiCameraPreview.SetNeedsDisplay();
			}
		}

		void UiCameraPreview_ImageCaptured(object sender, ImageCapturedEventArgs e)
		{
			capturePathCallbackAction(e.Data);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Control.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
