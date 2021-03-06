﻿using System;
using System.Linq;
using AVCam;
using AVFoundation;
using CoreGraphics;
using Foundation;
using UIKit;
using VisionTrainer;

namespace AVCameraCapture
{
	public delegate void ImageCapturedEventHandler(object sender, ImageCapturedEventArgs e);

	public class ImageCapturedEventArgs : EventArgs
	{
		public byte[] Data { get; private set; }

		public ImageCapturedEventArgs(byte[] data)
		{
			Data = data;
		}
	}

	public class AVCameraCaptureView : UIView
	{
		AVCaptureVideoPreviewLayer previewLayer;
		AVCaptureSession captureSession;
		AVCapturePhotoOutput photoOutput;
		AVCapturePhotoSettings photoSettings;
		AVCaptureDeviceInput videoDeviceInput;
		AVCameraCaptureDelegate photoCaptureDelegate;
		CameraOptions cameraOptions;

		public event EventHandler<EventArgs> Tapped;
		public event ImageCapturedEventHandler ImageCaptured;

		public bool IsPreviewing { get; private set; }

		public AVCameraCaptureView(CameraOptions options)
		{
			cameraOptions = options;
			IsPreviewing = false;
			Initialize();
		}

		public override void Draw(CGRect rect)
		{
			base.Draw(rect);
			previewLayer.Frame = rect;

			var deviceOrientation = UIDevice.CurrentDevice.Orientation;

			// Update orientation
			if (deviceOrientation.IsPortrait() || deviceOrientation.IsLandscape())
			{
				previewLayer.Connection.VideoOrientation = (AVCaptureVideoOrientation)deviceOrientation;

				var photoOutputConnection = photoOutput.ConnectionFromMediaType(AVMediaType.Video);
				photoOutputConnection.VideoOrientation = previewLayer.Connection.VideoOrientation; ;
			}
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);

			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null)
			{
				OnTapped(touch);
			}
		}

		protected virtual void OnTapped(UITouch touch)
		{
			var devicePoint = previewLayer.CaptureDevicePointOfInterestForPoint(touch.LocationInView(this));
			//FocusWithMode(AVCaptureFocusMode.AutoFocus, AVCaptureExposureMode.AutoExpose, devicePoint, true);

			Console.WriteLine("Tapped {0}", devicePoint);
			Tapped?.Invoke(this, new EventArgs());
		}

		void Initialize()
		{
			// Create the capture session
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

			var videoDevices = AVCaptureDeviceDiscoverySession.Create(
							new AVCaptureDeviceType[] { AVCaptureDeviceType.BuiltInWideAngleCamera, AVCaptureDeviceType.BuiltInDualCamera },
							AVMediaType.Video,
							AVCaptureDevicePosition.Unspecified
						);

			var cameraPosition = (cameraOptions == CameraOptions.Front) ? AVCaptureDevicePosition.Front : AVCaptureDevicePosition.Back;
			var device = videoDevices.Devices.FirstOrDefault(d => d.Position == cameraPosition);
			if (device == null)
				return;

			ConfigureCameraForDevice(device);

			NSError error;
			videoDeviceInput = new AVCaptureDeviceInput(device, out error);
			captureSession.AddInput(videoDeviceInput);
		}


		void SetupPhotoCapture()
		{
			captureSession.SessionPreset = AVCaptureSession.PresetPhoto;

			// Add photo output.
			photoOutput = new AVCapturePhotoOutput();
			photoOutput.IsHighResolutionCaptureEnabled = true;
			captureSession.AddOutput(photoOutput);
			captureSession.CommitConfiguration();
		}

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
			var devices = AVCaptureDeviceDiscoverySession.Create(
										new AVCaptureDeviceType[] { AVCaptureDeviceType.BuiltInWideAngleCamera, AVCaptureDeviceType.BuiltInDualCamera },
										AVMediaType.Video,
										AVCaptureDevicePosition.Unspecified
									);

			var cameraPosition = (option == CameraOptions.Front) ? AVCaptureDevicePosition.Front : AVCaptureDevicePosition.Back;
			var device = devices.Devices.FirstOrDefault(d => d.Position == cameraPosition);

			if (device != null)
			{
				var lVideoDeviceInput = AVCaptureDeviceInput.FromDevice(device);

				captureSession.BeginConfiguration();

				// Remove the existing device input first, since using the front and back camera simultaneously is not supported.
				captureSession.RemoveInput(videoDeviceInput);

				if (captureSession.CanAddInput(lVideoDeviceInput))
				{
					captureSession.AddInput(lVideoDeviceInput);
					videoDeviceInput = lVideoDeviceInput;
				}
				else
				{
					captureSession.AddInput(videoDeviceInput);
				}

				captureSession.CommitConfiguration();
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		public void Capture()
		{
			Console.WriteLine("Capture");

			// Create Photo Settings
			photoSettings = AVCapturePhotoSettings.FromFormat(new NSDictionary<NSString, NSObject>(AVVideo.CodecKey, AVVideo.CodecJPEG));
			photoSettings.IsHighResolutionPhotoEnabled = true;
			photoSettings.IsDepthDataDeliveryEnabled(false);

			if (photoSettings.AvailablePreviewPhotoPixelFormatTypes.Count() > 0)
				photoSettings.PreviewPhotoFormat = new NSDictionary<NSString, NSObject>(CoreVideo.CVPixelBuffer.PixelFormatTypeKey, photoSettings.AvailablePreviewPhotoPixelFormatTypes.First());

			// Use a separate object for the photo capture delegate to isolate each capture life cycle.
			photoCaptureDelegate = new AVCameraCaptureDelegate(photoSettings, CompletionHandler);

			photoOutput.CapturePhoto(photoSettings, photoCaptureDelegate);
		}

		void CompletionHandler(byte[] bytes)
		{
			var eventData = new ImageCapturedEventArgs(bytes);
			ImageCaptured?.Invoke(this, eventData);
		}
	}
}
