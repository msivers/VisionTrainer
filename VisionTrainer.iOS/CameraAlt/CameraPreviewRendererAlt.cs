using System;
using System.ComponentModel;
using System.Threading.Tasks;
using AVCam;
using Temp;
using VisionTrainer;
using VisionTrainer.Constants;
using VisionTrainer.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CameraPreview), typeof(CameraPreviewRendererAlt))]
namespace VisionTrainer.iOS
{
	public class CameraPreviewRendererAlt : ViewRenderer<CameraPreview, UICameraPreviewAlt>
	{
		CameraPreview element;
		UICameraPreviewAlt uiCameraPreview;
		Action<byte[]> capturePathCallbackAction;

		protected override void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
			{
				uiCameraPreview = new UICameraPreviewAlt(CameraOptions.Rear);
				//uiCameraPreview.Initalize();
				SetNativeControl(uiCameraPreview);
			}
			if (e.OldElement != null)
			{
				// Unsubscribe
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
				element.Capture = new Command(async () => await CaptureToFile());
				element.StartCamera = new Command(() => uiCameraPreview.StartPreviewing());
				element.StopCamera = new Command(() => uiCameraPreview.StopPreviewing());
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == PropertyIds.CameraOption)
			{
				var view = (CameraPreview)sender;
				uiCameraPreview.UpdateCameraOption(view.CameraOption);
			}
		}

		async Task CaptureToFile()
		{
			if (capturePathCallbackAction == null)
				return;

			var result = await uiCameraPreview.Capture();
			capturePathCallbackAction(result);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				//Control.CaptureSession.Dispose();
				Control.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
