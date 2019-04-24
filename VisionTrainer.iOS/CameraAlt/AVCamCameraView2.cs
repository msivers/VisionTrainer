using System;
using System.Linq;
using System.Threading.Tasks;
using AVCam;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using UIKit;
using VisionTrainer;
using VisionTrainer.iOS.CameraAlt;

namespace Temp
{
	public class UICameraPreviewAlt : UIView
	{
		AVCaptureVideoPreviewLayer previewLayer;
		AVCaptureSession captureSession;
		AVCapturePhotoOutput photoOutput;
		AVCapturePhotoSettings photoSettings;
		VisionTrainerPhotoCaptureDelegate photoCaptureDelegate;
		CameraOptions cameraOptions;

		public event EventHandler<EventArgs> Tapped;
		public event EventHandler<EventArgs> Captured; // TODO

		public bool IsPreviewing { get; set; }

		public UICameraPreviewAlt(CameraOptions options)
		{
			cameraOptions = options;
			IsPreviewing = false;
			Initialize();
		}

		public override void Draw(CGRect rect)
		{
			base.Draw(rect);
			previewLayer.Frame = rect;
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			OnTapped();
		}

		protected virtual void OnTapped()
		{
			Console.WriteLine("Tapped");
			Tapped?.Invoke(this, new EventArgs());
		}

		void Initialize()
		{
			// Create the catpure session
			captureSession = new AVCaptureSession();
			previewLayer = new AVCaptureVideoPreviewLayer(captureSession)
			{
				Frame = Bounds,
				VideoGravity = AVLayerVideoGravity.ResizeAspectFill
			};

			captureSession.BeginConfiguration();
			SetupVideoInput();
			SetupPhotoCapture();
			captureSession.CommitConfiguration();

			Layer.AddSublayer(previewLayer);
		}

		void SetupVideoInput()
		{
			// Video Input
			var videoDevices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
			var cameraPosition = (cameraOptions == CameraOptions.Front) ? AVCaptureDevicePosition.Front : AVCaptureDevicePosition.Back;
			var device = videoDevices.FirstOrDefault(d => d.Position == cameraPosition);
			if (device == null)
				return;

			ConfigureCameraForDevice(device);

			NSError error;
			var input = new AVCaptureDeviceInput(device, out error);
			captureSession.AddInput(input);
		}


		void SetupPhotoCapture()
		{
			captureSession.SessionPreset = AVCaptureSession.PresetPhoto;

			// Add photo output.
			photoOutput = new AVCapturePhotoOutput();
			photoOutput.IsHighResolutionCaptureEnabled = true;
			captureSession.AddOutput(photoOutput);
			captureSession.CommitConfiguration();

			// Create Photo Settings
			photoSettings = AVCapturePhotoSettings.FromFormat(new NSDictionary<NSString, NSObject>(AVVideo.CodecKey, AVVideo.CodecJPEG));
			photoSettings.IsHighResolutionPhotoEnabled = true;
			photoSettings.IsDepthDataDeliveryEnabled(false);

			if (photoSettings.AvailablePreviewPhotoPixelFormatTypes.Count() > 0)
				photoSettings.PreviewPhotoFormat = new NSDictionary<NSString, NSObject>(CoreVideo.CVPixelBuffer.PixelFormatTypeKey, photoSettings.AvailablePreviewPhotoPixelFormatTypes.First());

			// Use a separate object for the photo capture delegate to isolate each capture life cycle.
			photoCaptureDelegate = new VisionTrainerPhotoCaptureDelegate(photoSettings, CompletionHandler);
		}

		#region NewMethods
		void ConfigureCameraForDevice(AVCaptureDevice device)
		{
			var error = new NSError();
			if (device.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
			{
				device.LockForConfiguration(out error);
				device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
				device.UnlockForConfiguration();
			}
			else if (device.IsExposureModeSupported(AVCaptureExposureMode.ContinuousAutoExposure))
			{
				device.LockForConfiguration(out error);
				device.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
				device.UnlockForConfiguration();
			}
			else if (device.IsWhiteBalanceModeSupported(AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance))
			{
				device.LockForConfiguration(out error);
				device.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
				device.UnlockForConfiguration();
			}
		}
		#endregion

		public void StartPreviewing()
		{
			Console.WriteLine("StartPreviewing");
			captureSession.StartRunning();
			IsPreviewing = true;
		}

		public void StopPreviewing()
		{
			Console.WriteLine("StopPreviewing");
			captureSession.StopRunning();
			IsPreviewing = false;
		}

		public void UpdateCameraOption(CameraOptions option)
		{
			Console.WriteLine("UpdateCameraOption");
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}


		public Task<byte[]> Capture()
		{
			Console.WriteLine("Capture");

			photoOutput.CapturePhoto(photoSettings, photoCaptureDelegate);

			return Task.FromResult(new byte[0]);
		}

		void CompletionHandler(byte[] bytes)
		{
			Console.WriteLine(bytes.Length);
		}
	}
}
