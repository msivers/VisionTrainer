using System;
using System.ComponentModel;
using Android.Content;
using VisionTrainer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(VisionTrainer.CameraPreview), typeof(CameraPreviewRenderer))]
namespace VisionTrainer.Droid
{
	public class CameraPreviewRenderer : ViewRenderer<VisionTrainer.CameraPreview, DroidCameraPreview>
	{
		DroidCameraPreview cameraPreview;
		CameraPreview element;
		Action<byte[]> captureBytesCallbackAction;

		public CameraPreviewRenderer(Context context) : base(context)
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
			{
				cameraPreview = new DroidCameraPreview(Context, e.NewElement.CameraOption);
				cameraPreview.ImageCaptured += ImageCaptured;
				SetNativeControl(cameraPreview);
			}
			if (e.OldElement != null)
			{
				// Unsubscribe
				captureBytesCallbackAction = null;
				element.Capture = null;
				element.StartCamera = null;
				element.StopCamera = null;
			}
			if (e.NewElement != null)
			{
				// Subscribe
				element = e.NewElement;
				captureBytesCallbackAction = element.CaptureBytesCallback;
				element.Capture = new Command(() => CaptureToFile());
				element.StartCamera = new Command(() => cameraPreview.StartPreviewing());
				element.StopCamera = new Command(() => cameraPreview.StopPreviewing());
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == "CameraOption")
			{
				var view = (CameraPreview)sender;
				cameraPreview.UpdateCameraOption(view.CameraOption);
			}
		}

		void CaptureToFile()
		{
			if (captureBytesCallbackAction == null)
				return;

			cameraPreview.Capture();
		}

		void ImageCaptured(object sender, ImageCaptureEventArgs e)
		{
			captureBytesCallbackAction(e.Bytes);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				cameraPreview.ImageCaptured -= ImageCaptured;
				Control.CaptureSession.Dispose();
				Control.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
