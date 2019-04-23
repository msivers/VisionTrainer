using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AVCam;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using Photos;
using UIKit;
using VisionTrainer;

namespace Temp
{
	public class UICameraPreviewAlt : UIView, IAVCaptureFileOutputRecordingDelegate
	{
		AVCaptureVideoPreviewLayer previewLayer;
		CameraOptions cameraOptions;

		public event EventHandler<EventArgs> Tapped;

		public AVCaptureSession CaptureSession { get; private set; }

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
			CaptureSession = new AVCaptureSession();
			previewLayer = new AVCaptureVideoPreviewLayer(CaptureSession)
			{
				Frame = Bounds,
				VideoGravity = AVLayerVideoGravity.ResizeAspectFill
			};

			var videoDevices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
			var cameraPosition = (cameraOptions == CameraOptions.Front) ? AVCaptureDevicePosition.Front : AVCaptureDevicePosition.Back;
			var device = videoDevices.FirstOrDefault(d => d.Position == cameraPosition);

			if (device == null)
			{
				return;
			}

			NSError error;
			var input = new AVCaptureDeviceInput(device, out error);
			CaptureSession.AddInput(input);
			Layer.AddSublayer(previewLayer);

			// New Adds
			//var dictionary = new NSMutableDictionary();
			//dictionary[AVVideo.CodecKey] = new NSNumber((int)AVVideoCodec.JPEG);
			//stillImageOutput = new AVCaptureStillImageOutput()
			//{
			//	OutputSettings = new NSDictionary()
			//};

			//captureSession.AddOutput(stillImageOutput);
		}

		public void FinishedRecording(AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections, NSError error)
		{
			throw new NotImplementedException();
		}

		public void StartPreviewing()
		{
			Console.WriteLine("StartPreviewing");
			CaptureSession.StartRunning();
			IsPreviewing = true;
		}

		public void StopPreviewing()
		{
			Console.WriteLine("StopPreviewing");
			CaptureSession.StopRunning();
			IsPreviewing = false;
		}

		public Task<byte[]> Capture()
		{
			Console.WriteLine("Capture");
			return Task.FromResult(new byte[0]);
		}

		public void UpdateCameraOption(CameraOptions option)
		{
			Console.WriteLine("UpdateCameraOption");
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

	}
}
