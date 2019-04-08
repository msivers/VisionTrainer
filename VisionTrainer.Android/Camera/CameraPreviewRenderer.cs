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
		Action<string> capturePathCallbackAction;
		string captureFilename;

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
				capturePathCallbackAction = null;
				element.Capture = null;
				element.StartCamera = null;
				element.StopCamera = null;
				captureFilename = "temp";
			}
			if (e.NewElement != null)
			{
				// Subscribe
				element = e.NewElement;
				capturePathCallbackAction = element.CapturePathCallback;
				element.Capture = new Command(() => CaptureToFile());
				element.StartCamera = new Command(() => cameraPreview.StartPreviewing());
				element.StopCamera = new Command(() => cameraPreview.StopPreviewing());
				captureFilename = element.Filename;
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
			if (capturePathCallbackAction == null)
				return;

			cameraPreview.Capture(captureFilename);
		}

		void ImageCaptured(object sender, ImageCaptureEventArgs e)
		{
			capturePathCallbackAction(e.Filepath);
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
