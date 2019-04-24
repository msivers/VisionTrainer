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
	public class UICameraPreviewAlt : UIView
	{
		AVCaptureVideoPreviewLayer previewLayer;
		AVCaptureSession captureSession;
		AVCaptureDeviceInput captureDeviceInput;
		AVCaptureStillImageOutput stillImageOutput;
		AVCapturePhotoOutput photoOutput;
		CameraOptions cameraOptions;

		//new
		Dictionary<long, AVCamPhotoCaptureDelegate> inProgressPhotoCaptureDelegates;
		int inProgressLivePhotoCapturesCount;
		DispatchQueue sessionQueue;


		public event EventHandler<EventArgs> Tapped;
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
			// Communicate with the session and other session objects on this queue.
			sessionQueue = new DispatchQueue("session queue", false);

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
			// OLD
			//var captureDevice = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);
			//ConfigureCameraForDevice(captureDevice);
			//captureDeviceInput = AVCaptureDeviceInput.FromDevice(captureDevice);
			//captureSession.AddInput(captureDeviceInput);

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
			// OLD
			//var dictionary = new NSMutableDictionary();
			//dictionary[AVVideo.CodecKey] = new NSNumber((int)AVVideoCodec.JPEG);
			//stillImageOutput = new AVCaptureStillImageOutput() { OutputSettings = new NSDictionary() };
			//captureSession.AddOutput(stillImageOutput);

			captureSession.SessionPreset = AVCaptureSession.PresetPhoto;

			// Add photo output.
			photoOutput = new AVCapturePhotoOutput();
			photoOutput.IsHighResolutionCaptureEnabled = true;
			if (captureSession.CanAddOutput(photoOutput))
			{
				captureSession.AddOutput(photoOutput);
				inProgressPhotoCaptureDelegates = new Dictionary<long, AVCamPhotoCaptureDelegate>();
				inProgressLivePhotoCapturesCount = 0;
			}
			else
			{
				Console.WriteLine(@"Could not add photo output to the session");
				//setupResult = AVCamSetupResult.SessionConfigurationFailed;
				captureSession.CommitConfiguration();
				return;
			}

			captureSession.CommitConfiguration();
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

		public Task<byte[]> Capture()
		{
			Console.WriteLine("Capture");
			//return Task.FromResult(new byte[0]);

			sessionQueue.DispatchAsync(() =>
						{

							// Update the photo output's connection to match the video orientation of the video preview layer.
							var photoOutputConnection = photoOutput.ConnectionFromMediaType(AVMediaType.Video);
							//photoOutputConnection.VideoOrientation = videoPreviewLayerVideoOrientation;

							AVCapturePhotoSettings photoSettings;
							// Capture HEIF photo when supported, with flash set to auto and high resolution photo enabled.

							if (photoOutput.AvailablePhotoCodecTypes.Where(codec => codec == AVVideo2.CodecHEVC).Any())
							{
								photoSettings = AVCapturePhotoSettings.FromFormat(new NSDictionary<NSString, NSObject>(AVVideo.CodecKey, AVVideo2.CodecHEVC));
							}
							else
							{
								photoSettings = AVCapturePhotoSettings.Create();
							}

							//if (videoDeviceInput.Device.FlashAvailable)
							//{
							//	photoSettings.FlashMode = AVCaptureFlashMode.Auto;
							//}
							photoSettings.IsHighResolutionPhotoEnabled = true;
							if (photoSettings.AvailablePreviewPhotoPixelFormatTypes.Count() > 0)
							{
								photoSettings.PreviewPhotoFormat = new NSDictionary<NSString, NSObject>(CoreVideo.CVPixelBuffer.PixelFormatTypeKey, photoSettings.AvailablePreviewPhotoPixelFormatTypes.First());
							}

							photoSettings.IsDepthDataDeliveryEnabled(false);

							// Use a separate object for the photo capture delegate to isolate each capture life cycle.
							var photoCaptureDelegate = new AVCamPhotoCaptureDelegate(photoSettings, () =>
										{
											DispatchQueue.MainQueue.DispatchAsync(() =>
											{
												//PreviewView.VideoPreviewLayer.Opacity = 0.0f;
												//UIView.Animate(0.25, () =>
												//{
												//	PreviewView.VideoPreviewLayer.Opacity = 1.0f;
												//});
											});
										}, (bool capturing) =>
										{
											/*
												Because Live Photo captures can overlap, we need to keep track of the
												number of in progress Live Photo captures to ensure that the
												Live Photo label stays visible during these captures.
											*/
											sessionQueue.DispatchAsync(() =>
											{
												if (capturing)
												{
													inProgressLivePhotoCapturesCount++;
												}
												else
												{
													inProgressLivePhotoCapturesCount--;
												}

												var lInProgressLivePhotoCapturesCount = inProgressLivePhotoCapturesCount;
												DispatchQueue.MainQueue.DispatchAsync(() =>
												{
													//if (lInProgressLivePhotoCapturesCount > 0)
													//{
													//	CapturingLivePhotoLabel.Hidden = false;
													//}
													//else if (lInProgressLivePhotoCapturesCount == 0)
													//{
													//	CapturingLivePhotoLabel.Hidden = true;
													//}
													//else
													//{
													//	Console.WriteLine(@"Error: In progress live photo capture count is less than 0");
													//}
												});
											});
										}, (AVCamPhotoCaptureDelegate lPhotoCaptureDelegate) =>
										{
											// When the capture is complete, remove a reference to the photo capture delegate so it can be deallocated.
											sessionQueue.DispatchAsync(() =>
														{
															inProgressPhotoCaptureDelegates[lPhotoCaptureDelegate.RequestedPhotoSettings.UniqueID] = null;
														});
										});

							/*
								The Photo Output keeps a weak reference to the photo capture delegate so
								we store it in an array to maintain a strong reference to this object
								until the capture is completed.
							*/
							inProgressPhotoCaptureDelegates[photoCaptureDelegate.RequestedPhotoSettings.UniqueID] = photoCaptureDelegate;
							photoOutput.CapturePhoto(photoSettings, photoCaptureDelegate);
						});

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
