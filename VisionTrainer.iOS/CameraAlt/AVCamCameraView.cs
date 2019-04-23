using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using Photos;
using UIKit;
using VisionTrainer;

namespace AVCam
{
	// https://devblogs.microsoft.com/xamarin/custom-camera-display-avfoundation/
	public class AVCamCameraView : UIView, IAVCaptureFileOutputRecordingDelegate
	{
		AVCamSetupResult setupResult; // TODO Depreciate
		DispatchQueue sessionQueue;
		AVCaptureDeviceInput videoDeviceInput;
		AVCaptureDeviceDiscoverySession videoDeviceDiscoverySession;
		AVCamLivePhotoMode livePhotoMode;
		AVCamDepthDataDeliveryMode depthDataDeliveryMode;
		AVCapturePhotoOutput photoOutput;
		Dictionary<long, AVCamPhotoCaptureDelegate> inProgressPhotoCaptureDelegates;
		AVCaptureMovieFileOutput movieFileOutput;
		nint backgroundRecordingId;
		int inProgressLivePhotoCapturesCount;
		bool sessionRunning;


		AVCaptureSession session;
		AVCaptureVideoPreviewLayer VideoPreviewLayer;
		CameraOptions cameraOptions;
		public bool IsPreviewing { get; set; }

		public AVCamCameraView()
		{
			#region UIVIew adds
			session = new AVCaptureSession();
			VideoPreviewLayer = new AVCaptureVideoPreviewLayer(session)
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
			session.AddInput(input);
			Layer.AddSublayer(VideoPreviewLayer);
			//session.StartRunning();
			IsPreviewing = true;

			#endregion

			// Create a device discovery session.
			videoDeviceDiscoverySession = AVCaptureDeviceDiscoverySession.Create(
				new AVCaptureDeviceType[] { AVCaptureDeviceType.BuiltInWideAngleCamera, AVCaptureDeviceType.BuiltInDualCamera },
				AVMediaType.Video,
				AVCaptureDevicePosition.Unspecified
			);


			// Communicate with the session and other session objects on this queue.
			sessionQueue = new DispatchQueue("session queue", false);

			setupResult = AVCamSetupResult.Success;
		}

		#region new methods
		public void Initalize()
		{
			sessionQueue.DispatchAsync(() =>
			{
				ConfigureSession();
			});
		}

		public void StartPreviewing()
		{
			//sessionQueue.DispatchAsync(() =>
			//{
			//	AddObservers();
			//	session.StartRunning();
			//	sessionRunning = session.Running;
			//});
		}

		public void StopPreviewing()
		{
			//sessionQueue.DispatchAsync(() =>
			//{
			//	if (setupResult == AVCamSetupResult.Success)
			//	{
			//		session.StopRunning();
			//		RemoveObservers();
			//	}
			//});
		}

		public void CaptureImage()
		{

		}

		public void CaptureVideoStart()
		{

		}

		public void CaptureVideoStop()
		{

		}

		public override void Draw(CGRect rect)
		{
			base.Draw(rect);
			VideoPreviewLayer.Frame = rect;
		}
		#endregion

		public bool ShouldAutorotate()
		{
			// Disable autorotation of the interface when recording is in progress.
			return movieFileOutput?.Recording ?? false;
		}

		public UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
		{
			return UIInterfaceOrientationMask.All;
		}

		public void ViewWillTransitionToSize(CoreGraphics.CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
		{
			var deviceOrientation = UIDevice.CurrentDevice.Orientation;

			if (deviceOrientation.IsPortrait() || deviceOrientation.IsLandscape())
			{
				VideoPreviewLayer.Connection.VideoOrientation = (AVCaptureVideoOrientation)deviceOrientation;
			}
		}

		// Call this on the session queue.
		void ConfigureSession()
		{
			if (setupResult != AVCamSetupResult.Success)
			{
				return;
			}

			NSError error = null;

			session.BeginConfiguration();

			/*
				We do not create an AVCaptureMovieFileOutput when setting up the session because the
				AVCaptureMovieFileOutput does not support movie recording with AVCaptureSessionPresetPhoto.
			*/
			session.SessionPreset = AVCaptureSession.PresetPhoto;

			// Add video input.

			// Choose the back dual camera if available, otherwise default to a wide angle camera.
			var videoDevice = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInDualCamera, AVMediaType.Video, AVCaptureDevicePosition.Back);
			if (videoDevice == null)
			{
				// If the back dual camera is not available, default to the back wide angle camera.
				videoDevice = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInWideAngleCamera, AVMediaType.Video, AVCaptureDevicePosition.Back);

				// In some cases where users break their phones, the back wide angle camera is not available. In this case, we should default to the front wide angle camera.
				if (videoDevice == null)
				{
					videoDevice = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInWideAngleCamera, AVMediaType.Video, AVCaptureDevicePosition.Front);
				}
			}
			var lVideoDeviceInput = AVCaptureDeviceInput.FromDevice(videoDevice, out error);
			if (lVideoDeviceInput == null)
			{
				Console.WriteLine($"Could not create video device input: {error}");
				setupResult = AVCamSetupResult.SessionConfigurationFailed;
				session.CommitConfiguration();
				return;
			}
			if (session.CanAddInput(lVideoDeviceInput))
			{
				session.AddInput(lVideoDeviceInput);
				videoDeviceInput = lVideoDeviceInput;

				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					/*
						Why are we dispatching this to the main queue?
						Because AVCaptureVideoPreviewLayer is the backing layer for AVCamPreviewView and UIView
						can only be manipulated on the main thread.
						Note: As an exception to the above rule, it is not necessary to serialize video orientation changes
						on the AVCaptureVideoPreviewLayer’s connection with other session manipulation.

						Use the status bar orientation as the initial video orientation. Subsequent orientation changes are
						handled by -[AVCamCameraViewController viewWillTransitionToSize:withTransitionCoordinator:].
					*/
					var statusBarOrientation = UIApplication.SharedApplication.StatusBarOrientation;
					var initialVideoOrientation = AVCaptureVideoOrientation.Portrait;
					if (statusBarOrientation != UIInterfaceOrientation.Unknown)
					{
						initialVideoOrientation = (AVCaptureVideoOrientation)statusBarOrientation;
					}

					VideoPreviewLayer.Connection.VideoOrientation = initialVideoOrientation;
				});
			}
			else
			{
				Console.WriteLine(@"Could not add video device input to the session");
				setupResult = AVCamSetupResult.SessionConfigurationFailed;
				session.CommitConfiguration();
				return;
			}

			// Add audio input.
			var audioDevice = AVCaptureDevice.GetDefaultDevice(AVMediaType.Audio);
			var audioDeviceInput = AVCaptureDeviceInput.FromDevice(audioDevice, out error);
			if (audioDeviceInput == null)
			{
				Console.WriteLine($"Could not create audio device input: {error}");
			}
			if (session.CanAddInput(audioDeviceInput))
			{
				session.AddInput(audioDeviceInput);
			}
			else
			{
				Console.WriteLine(@"Could not add audio device input to the session");
			}

			// Add photo output.
			var lPhotoOutput = new AVCapturePhotoOutput();
			if (session.CanAddOutput(lPhotoOutput))
			{
				session.AddOutput(lPhotoOutput);
				photoOutput = lPhotoOutput;

				photoOutput.IsHighResolutionCaptureEnabled = true;
				photoOutput.IsLivePhotoCaptureEnabled = photoOutput.IsLivePhotoCaptureSupported;
				//photoOutput.IsDepthDataDeliveryEnabled(photoOutput.IsDepthDataDeliverySupported());

				livePhotoMode = photoOutput.IsLivePhotoCaptureSupported ? AVCamLivePhotoMode.On : AVCamLivePhotoMode.Off;
				//depthDataDeliveryMode = photoOutput.IsDepthDataDeliverySupported() ? AVCamDepthDataDeliveryMode.On : AVCamDepthDataDeliveryMode.Off;

				inProgressPhotoCaptureDelegates = new Dictionary<long, AVCamPhotoCaptureDelegate>();
				inProgressLivePhotoCapturesCount = 0;
			}
			else
			{
				Console.WriteLine(@"Could not add photo output to the session");
				setupResult = AVCamSetupResult.SessionConfigurationFailed;
				session.CommitConfiguration();
				return;
			}

			backgroundRecordingId = UIApplication.BackgroundTaskInvalid;

			session.CommitConfiguration();
		}

		void ToggleCaptureMode(UISegmentedControl captureModeControl)
		{
			if (captureModeControl.SelectedSegment == (int)AVCamCaptureMode.Photo)
			{
				//RecordButton.Enabled = false;

				sessionQueue.DispatchAsync(() =>
				{
					/*
						Remove the AVCaptureMovieFileOutput from the session because movie recording is
						not supported with AVCaptureSessionPresetPhoto. Additionally, Live Photo
						capture is not supported when an AVCaptureMovieFileOutput is connected to the session.
					*/
					session.BeginConfiguration();
					session.RemoveOutput(movieFileOutput);
					session.SessionPreset = AVCaptureSession.PresetPhoto;
					session.CommitConfiguration();

					movieFileOutput = null;

					if (photoOutput.IsLivePhotoCaptureSupported)
					{
						photoOutput.IsLivePhotoCaptureEnabled = true;


						//DispatchQueue.MainQueue.DispatchAsync(() =>
						//{
						//	LivePhotoModeButton.Enabled = true;
						//	LivePhotoModeButton.Hidden = false;
						//});
					}

					//if (photoOutput.IsDepthDataDeliverySupported())
					//{
					//	photoOutput.IsDepthDataDeliveryEnabled(true);


					//	DispatchQueue.MainQueue.DispatchAsync(() =>
					//	{
					//		DepthDataDeliveryButton.Hidden = false;
					//		DepthDataDeliveryButton.Enabled = true;
					//	});
					//}
				});
			}
			else if (captureModeControl.SelectedSegment == (int)AVCamCaptureMode.Movie)
			{
				//LivePhotoModeButton.Hidden = true;
				//DepthDataDeliveryButton.Hidden = true;

				sessionQueue.DispatchAsync(() =>
				{
					var lMovieFileOutput = new AVCaptureMovieFileOutput();

					if (session.CanAddOutput(lMovieFileOutput))
					{
						session.BeginConfiguration();
						session.AddOutput(lMovieFileOutput);
						session.SessionPreset = AVCaptureSession.PresetHigh;
						var connection = lMovieFileOutput.ConnectionFromMediaType(AVMediaType.Video);
						if (connection.SupportsVideoStabilization)
						{
							connection.PreferredVideoStabilizationMode = AVCaptureVideoStabilizationMode.Auto;
						}
						session.CommitConfiguration();

						movieFileOutput = lMovieFileOutput;

						//DispatchQueue.MainQueue.DispatchAsync(() =>
						//{
						//	RecordButton.Enabled = true;
						//});
					}
				});
			}
		}

		void ChangeCamera(NSObject sender)
		{
			//CameraButton.Enabled = false;
			//RecordButton.Enabled = false;
			//PhotoButton.Enabled = false;
			//LivePhotoModeButton.Enabled = false;
			//CaptureModeControl.Enabled = false;

			sessionQueue.DispatchAsync(() =>
			{
				var currentVideoDevice = videoDeviceInput.Device;
				var currentPosition = currentVideoDevice.Position;

				AVCaptureDevicePosition preferredPosition = AVCaptureDevicePosition.Unspecified;
				AVCaptureDeviceType preferredDeviceType = AVCaptureDeviceType.BuiltInDualCamera;

				switch (currentPosition)
				{
					//case AVCaptureDevicePosition.Unspecified:
					//preferredPosition = AVCaptureDevicePosition.Back;
					//preferredDeviceType = AVCaptureDeviceType.BuiltInDualCamera;
					//break;
					case AVCaptureDevicePosition.Unspecified:
					case AVCaptureDevicePosition.Front:
						preferredPosition = AVCaptureDevicePosition.Back;
						preferredDeviceType = AVCaptureDeviceType.BuiltInDualCamera;
						break;
					case AVCaptureDevicePosition.Back:
						preferredPosition = AVCaptureDevicePosition.Front;
						preferredDeviceType = AVCaptureDeviceType.BuiltInWideAngleCamera;
						break;
				}

				var devices = videoDeviceDiscoverySession.Devices;
				AVCaptureDevice newVideoDevice = null;

				// First, look for a device with both the preferred position and device type.
				foreach (var device in devices)
				{
					if (device.Position == preferredPosition && device.DeviceType.GetConstant() == preferredDeviceType.GetConstant())
					{
						newVideoDevice = device;
						break;
					}
				}

				// Otherwise, look for a device with only the preferred position.
				if (newVideoDevice == null)
				{
					foreach (var device in devices)
					{
						if (device.Position == preferredPosition)
						{
							newVideoDevice = device;
							break;
						}
					}
				}

				if (newVideoDevice != null)
				{
					var lVideoDeviceInput = AVCaptureDeviceInput.FromDevice(newVideoDevice);

					session.BeginConfiguration();

					// Remove the existing device input first, since using the front and back camera simultaneously is not supported.
					session.RemoveInput(videoDeviceInput);

					if (session.CanAddInput(lVideoDeviceInput))
					{
						if (subjectAreaDidChangeObserver != null)
							subjectAreaDidChangeObserver.Dispose();

						subjectAreaDidChangeObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVCaptureDevice.SubjectAreaDidChangeNotification, SubjectAreaDidChange, newVideoDevice);

						session.AddInput(lVideoDeviceInput);
						videoDeviceInput = lVideoDeviceInput;
					}
					else
					{
						session.AddInput(videoDeviceInput);
					}

					if (movieFileOutput != null)
					{
						var movieFileOutputConnection = movieFileOutput.ConnectionFromMediaType(AVMediaType.Video);
						if (movieFileOutputConnection.SupportsVideoStabilization)
						{
							movieFileOutputConnection.PreferredVideoStabilizationMode = AVCaptureVideoStabilizationMode.Auto;
						}
					}

					/*
						Set Live Photo capture and depth data delivery if it is supported. When changing cameras, the
						`livePhotoCaptureEnabled` and `depthDataDeliveryEnabled` properties of the AVCapturePhotoOutput gets set to NO when
						a video device is disconnected from the session. After the new video device is
						added to the session, re-enable Live Photo capture and depth data delivery if they are supported.
					 */
					photoOutput.IsLivePhotoCaptureEnabled = photoOutput.IsLivePhotoCaptureSupported;
					//photoOutput.IsDepthDataDeliveryEnabled(photoOutput.IsDepthDataDeliverySupported());

					session.CommitConfiguration();
				}


				//DispatchQueue.MainQueue.DispatchAsync(() =>
				//{
				//	CameraButton.Enabled = true;
				//	RecordButton.Enabled = CaptureModeControl.SelectedSegment == (int)AVCamCaptureMode.Movie;
				//	PhotoButton.Enabled = true;
				//	LivePhotoModeButton.Enabled = true;
				//	CaptureModeControl.Enabled = true;
				//	DepthDataDeliveryButton.Enabled = photoOutput.IsDepthDataDeliveryEnabled();
				//	DepthDataDeliveryButton.Hidden = !photoOutput.IsDepthDataDeliverySupported();
				//});
			});
		}

		void FocusAndExposeTap(UIGestureRecognizer gestureRecognizer)
		{
			var devicePoint = VideoPreviewLayer.CaptureDevicePointOfInterestForPoint(gestureRecognizer.LocationInView(gestureRecognizer.View));
			FocusWithMode(AVCaptureFocusMode.AutoFocus, AVCaptureExposureMode.AutoExpose, devicePoint, true);
		}

		void FocusWithMode(AVCaptureFocusMode focusMode, AVCaptureExposureMode exposureMode, CGPoint point, bool monitorSubjectAreaChange)
		{
			sessionQueue.DispatchAsync(() =>
			{
				var device = videoDeviceInput.Device;
				NSError error = null;
				if (device.LockForConfiguration(out error))
				{
					/*
						Setting (focus/exposure)PointOfInterest alone does not initiate a (focus/exposure) operation.
						Call set(Focus/Exposure)Mode() to apply the new point of interest.
					*/
					if (device.FocusPointOfInterestSupported && device.IsFocusModeSupported(focusMode))
					{
						device.FocusPointOfInterest = point;
						device.FocusMode = focusMode;
					}

					if (device.ExposurePointOfInterestSupported && device.IsExposureModeSupported(exposureMode))
					{
						device.ExposurePointOfInterest = point;
						device.ExposureMode = exposureMode;
					}

					device.SubjectAreaChangeMonitoringEnabled = monitorSubjectAreaChange;
					device.UnlockForConfiguration();
				}
				else
				{
					Console.WriteLine($"Could not lock device for configuration: {error}");
				}
			});
		}

		void CapturePhoto(NSObject sender)
		{
			/*
				Retrieve the video preview layer's video orientation on the main queue before
				entering the session queue. We do this to ensure UI elements are accessed on
				the main thread and session configuration is done on the session queue.
			*/
			var videoPreviewLayerVideoOrientation = VideoPreviewLayer.Connection.VideoOrientation;

			sessionQueue.DispatchAsync(() =>
			{

				// Update the photo output's connection to match the video orientation of the video preview layer.
				var photoOutputConnection = photoOutput.ConnectionFromMediaType(AVMediaType.Video);
				photoOutputConnection.VideoOrientation = videoPreviewLayerVideoOrientation;

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

				if (videoDeviceInput.Device.FlashAvailable)
				{
					photoSettings.FlashMode = AVCaptureFlashMode.Auto;
				}
				photoSettings.IsHighResolutionPhotoEnabled = true;
				if (photoSettings.AvailablePreviewPhotoPixelFormatTypes.Count() > 0)
				{
					photoSettings.PreviewPhotoFormat = new NSDictionary<NSString, NSObject>(CoreVideo.CVPixelBuffer.PixelFormatTypeKey, photoSettings.AvailablePreviewPhotoPixelFormatTypes.First());
				}
				if (livePhotoMode == AVCamLivePhotoMode.On && photoOutput.IsLivePhotoCaptureSupported)
				{
					// Live Photo capture is not supported in movie mode.
					var livePhotoMovieFileName = Guid.NewGuid().ToString();
					var livePhotoMovieFilePath = NSFileManager.DefaultManager.GetTemporaryDirectory().Append($"{livePhotoMovieFileName}.mov", false);
					photoSettings.LivePhotoMovieFileUrl = livePhotoMovieFilePath;
				}

				if (depthDataDeliveryMode == AVCamDepthDataDeliveryMode.On && photoOutput.IsDepthDataDeliverySupported())
				{
					photoSettings.IsDepthDataDeliveryEnabled(true);
				}
				else
				{
					photoSettings.IsDepthDataDeliveryEnabled(false);
				}

				// Use a separate object for the photo capture delegate to isolate each capture life cycle.
				var photoCaptureDelegate = new AVCamPhotoCaptureDelegate(photoSettings, () =>
				{
					DispatchQueue.MainQueue.DispatchAsync(() =>
					{
						VideoPreviewLayer.Opacity = 0.0f;
						UIView.Animate(0.25, () =>
						{
							VideoPreviewLayer.Opacity = 1.0f;
						});
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
						//DispatchQueue.MainQueue.DispatchAsync(() =>
						//{
						//	if (lInProgressLivePhotoCapturesCount > 0)
						//	{
						//		CapturingLivePhotoLabel.Hidden = false;
						//	}
						//	else if (lInProgressLivePhotoCapturesCount == 0)
						//	{
						//		CapturingLivePhotoLabel.Hidden = true;
						//	}
						//	else
						//	{
						//		Console.WriteLine(@"Error: In progress live photo capture count is less than 0");
						//	}
						//});
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
		}

		void ToggleLivePhotoMode(UIButton livePhotoModeButton)
		{
			sessionQueue.DispatchAsync(() =>
			{
				livePhotoMode = (livePhotoMode == AVCamLivePhotoMode.On) ? AVCamLivePhotoMode.Off : AVCamLivePhotoMode.On;
				var lLivePhotoMode = livePhotoMode;

				//DispatchQueue.MainQueue.DispatchAsync(() =>
				//{
				//	if (lLivePhotoMode == AVCamLivePhotoMode.On)
				//	{
				//		LivePhotoModeButton.SetTitle(NSBundle.MainBundle.LocalizedString(@"Live Photo Mode: On", @"Live photo mode button on title"), UIControlState.Normal);
				//	}
				//	else
				//	{
				//		LivePhotoModeButton.SetTitle(NSBundle.MainBundle.LocalizedString(@"Live Photo Mode: Off", @"Live photo mode button off title"), UIControlState.Normal);
				//	}
				//});
			});
		}

		void ToggleDepthDataDeliveryMode(UIButton depthDataDeliveryButton)
		{
			sessionQueue.DispatchAsync(() =>
			{
				depthDataDeliveryMode = (depthDataDeliveryMode == AVCamDepthDataDeliveryMode.On) ? AVCamDepthDataDeliveryMode.Off : AVCamDepthDataDeliveryMode.On;
				var lDepthDataDeliveryMode = depthDataDeliveryMode;

				//DispatchQueue.MainQueue.DispatchAsync(() =>
				//{
				//	if (lDepthDataDeliveryMode == AVCamDepthDataDeliveryMode.On)
				//	{

				//		DepthDataDeliveryButton.SetTitle(NSBundle.MainBundle.LocalizedString(@"Depth Data Delivery: On", @"Depth Data mode button on title"), UIControlState.Normal);
				//	}
				//	else
				//	{
				//		DepthDataDeliveryButton.SetTitle(NSBundle.MainBundle.LocalizedString(@"Depth Data Delivery: Off", @"Depth Data mode button off title"), UIControlState.Normal);

				//	}
				//});
			});
		}

		void ToggleMovieRecording(NSObject sender)
		{
			/*
				Disable the Camera button until recording finishes, and disable
				the Record button until recording starts or finishes.
				
				See the AVCaptureFileOutputRecordingDelegate methods.
			 */
			//CameraButton.Enabled = false;
			//RecordButton.Enabled = false;
			//CaptureModeControl.Enabled = false;

			/*
				Retrieve the video preview layer's video orientation on the main queue
				before entering the session queue. We do this to ensure UI elements are
				accessed on the main thread and session configuration is done on the session queue.
			*/
			var videoPreviewLayerVideoOrientation = VideoPreviewLayer.Connection.VideoOrientation;

			sessionQueue.DispatchAsync(() =>
			{
				if (!movieFileOutput.Recording)
				{
					if (UIDevice.CurrentDevice.IsMultitaskingSupported)
					{
						/*
							Setup background task.
							This is needed because the -[captureOutput:didFinishRecordingToOutputFileAtURL:fromConnections:error:]
							callback is not received until AVCam returns to the foreground unless you request background execution time.
							This also ensures that there will be time to write the file to the photo library when AVCam is backgrounded.
							To conclude this background execution, -[endBackgroundTask:] is called in
							-[captureOutput:didFinishRecordingToOutputFileAtURL:fromConnections:error:] after the recorded file has been saved.
						*/
						backgroundRecordingId = UIApplication.SharedApplication.BeginBackgroundTask(null);
					}

					// Update the orientation on the movie file output video connection before starting recording.
					var movieFileOutputConnection = movieFileOutput.ConnectionFromMediaType(AVMediaType.Video);
					movieFileOutputConnection.VideoOrientation = videoPreviewLayerVideoOrientation;

					// Use HEVC codec if supported
					if (movieFileOutput.AvailableVideoCodecTypes.Where(codec => codec == AVVideo2.CodecHEVC).Any())
					{
						movieFileOutput.SetOutputSettings(new NSDictionary(AVVideo.CodecKey, AVVideo2.CodecHEVC), movieFileOutputConnection);
					}

					// Start recording to a temporary file.
					var outputFileName = Guid.NewGuid().ToString();
					var livePhotoMovieFilePath = NSFileManager.DefaultManager.GetTemporaryDirectory().Append($"{outputFileName}.mov", false);
					movieFileOutput.StartRecordingToOutputFile(livePhotoMovieFilePath, this);
				}
				else
				{
					movieFileOutput.StopRecording();
				}
			});
		}

		public void FinishedRecording(AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections, NSError error)
		{
			/*
				Note that currentBackgroundRecordingID is used to end the background task
				associated with this recording. This allows a new recording to be started,
				associated with a new UIBackgroundTaskIdentifier, once the movie file output's
				`recording` property is back to NO — which happens sometime after this method
				returns.
				
				Note: Since we use a unique file path for each recording, a new recording will
				not overwrite a recording currently being saved.
			*/
			var currentBackgroundRecordingId = backgroundRecordingId;
			backgroundRecordingId = UIApplication.BackgroundTaskInvalid;

			Action cleanup = () =>
			{
				if (NSFileManager.DefaultManager.FileExists(outputFileUrl.Path))
				{
					NSError tError;
					NSFileManager.DefaultManager.Remove(outputFileUrl.Path, out tError);
				}

				if (currentBackgroundRecordingId != UIApplication.BackgroundTaskInvalid)
				{
					UIApplication.SharedApplication.EndBackgroundTask(currentBackgroundRecordingId);
				}
			};

			var success = true;

			if (error != null)
			{
				Console.WriteLine($"Movie file finishing error: {error}");
				var tmpObj = error.UserInfo[AVErrorKeys.RecordingSuccessfullyFinished];
				if (tmpObj is NSNumber) success = ((NSNumber)tmpObj).BoolValue;
				else if (tmpObj is NSString) success = ((NSString)tmpObj).BoolValue();
			}
			if (success)
			{
				// Check authorization status.
				PHPhotoLibrary.RequestAuthorization((PHAuthorizationStatus status) =>
				{
					if (status == PHAuthorizationStatus.Authorized)
					{
						// Save the movie file to the photo library and cleanup.
						PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
						{
							var options = new PHAssetResourceCreationOptions();
							options.ShouldMoveFile = true;
							var creationRequest = PHAssetCreationRequest.CreationRequestForAsset();
							creationRequest.AddResource(PHAssetResourceType.Video, outputFileUrl, options);
						}, (cbSuccess, cbError) =>
						{
							if (!cbSuccess)
							{
								Console.WriteLine($"Could not save movie to photo library: {cbError}");
							}
							cleanup();
						});
					}
					else
					{
						cleanup();
					}
				});
			}
			else
			{
				cleanup();
			}

			// Enable the Camera and Record buttons to let the user switch camera and start another recording.
			//DispatchQueue.MainQueue.DispatchAsync(() =>
			//{
			//	// Only enable the ability to change camera if the device has more than one camera.
			//	CameraButton.Enabled = (videoDeviceDiscoverySession.UniqueDevicePositionsCount() > 1);
			//	RecordButton.Enabled = true;
			//	CaptureModeControl.Enabled = true;
			//	RecordButton.SetTitle(NSBundle.MainBundle.LocalizedString(@"Record", @"Recording button record title"), UIControlState.Normal);
			//});
		}

		List<IDisposable> observerList = new List<IDisposable>();
		IDisposable subjectAreaDidChangeObserver = null;

		private void AddObservers()
		{
			if (subjectAreaDidChangeObserver != null)
				subjectAreaDidChangeObserver.Dispose();
			subjectAreaDidChangeObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVCaptureDevice.SubjectAreaDidChangeNotification, SubjectAreaDidChange, videoDeviceInput.Device);
			NSNotificationCenter.DefaultCenter.AddObserver(AVCaptureSession.RuntimeErrorNotification, SessionRuntimeError, session);

			/*
				A session can only run when the app is full screen. It will be interrupted
				in a multi-app layout, introduced in iOS 9, see also the documentation of
				AVCaptureSessionInterruptionReason. Add observers to handle these session
				interruptions and show a preview is paused message. See the documentation
				of AVCaptureSessionWasInterruptedNotification for other interruption reasons.
			*/
			NSNotificationCenter.DefaultCenter.AddObserver(AVCaptureSession.WasInterruptedNotification, SessionWasInterrupted, session);
			NSNotificationCenter.DefaultCenter.AddObserver(AVCaptureSession.InterruptionEndedNotification, SessionInterruptionEnded, session);
		}

		private void RemoveObservers()
		{
			if (subjectAreaDidChangeObserver != null)
				subjectAreaDidChangeObserver.Dispose();
			subjectAreaDidChangeObserver = null;

			observerList.ForEach(i => i.Dispose());
			observerList.Clear();
		}

		void SubjectAreaDidChange(NSNotification notification)
		{
			var devicePoint = new CGPoint(0.5, 0.5);
			this.FocusWithMode(AVCaptureFocusMode.ContinuousAutoFocus, AVCaptureExposureMode.ContinuousAutoExposure, devicePoint, false);
		}

		void SessionRuntimeError(NSNotification notification)
		{
			NSError error = notification.UserInfo[AVCaptureSession.ErrorKey] as NSError;
			if (error == null)
			{
				return;
			}
			Console.WriteLine($"Capture session runtime error: {error}");

			/*
				Automatically try to restart the session running if media services were
				reset and the last start running succeeded. Otherwise, enable the user
				to try to resume the session running.
			*/
			if (error.Code == (int)AVError.MediaServicesWereReset)
			{
				sessionQueue.DispatchAsync(() =>
				{
					if (sessionRunning)
					{
						session.StartRunning();
						sessionRunning = session.Running;
					}
				});
			}
		}

		void SessionWasInterrupted(NSNotification notification)
		{
			/*
				In some scenarios we want to enable the user to resume the session running.
				For example, if music playback is initiated via control center while
				using AVCam, then the user can let AVCam resume
				the session running, which will stop music playback. Note that stopping
				music playback in control center will not automatically resume the session
				running. Also note that it is not always possible to resume, see -[resumeInterruptedSession:].
			*/

			var reason = (AVCaptureSessionInterruptionReason)(notification.UserInfo[AVCaptureSession.InterruptionReasonKey] as NSNumber).Int32Value;
			Console.WriteLine($"Capture session was interrupted with reason {(int)reason}");

			if (reason == AVCaptureSessionInterruptionReason.AudioDeviceInUseByAnotherClient ||
				reason == AVCaptureSessionInterruptionReason.VideoDeviceInUseByAnotherClient)
			{
				// Do something
			}
			else if (reason == AVCaptureSessionInterruptionReason.VideoDeviceNotAvailableWithMultipleForegroundApps)
			{
				// Do something
			}
		}

		void SessionInterruptionEnded(NSNotification notification)
		{
			Console.WriteLine(@"Capture session interruption ended");
		}
	}
}





enum AVCamSetupResult
{
	Success,
	CameraNotAuthorized,
	SessionConfigurationFailed
};

enum AVCamCaptureMode
{
	Photo = 0,
	Movie = 1
};

enum AVCamLivePhotoMode
{
	On,
	Off
};

enum AVCamDepthDataDeliveryMode
{
	On,
	Off
};

static class AVCaptureDeviceDiscoverySessionUtilities
{
	public static int UniqueDevicePositionsCount(this AVCaptureDeviceDiscoverySession session)
	{
		var uniqueDevicePositions = new List<AVCaptureDevicePosition>();

		foreach (AVCaptureDevice device in session.Devices)
		{
			if (!uniqueDevicePositions.Contains(device.Position))
			{
				uniqueDevicePositions.Add(device.Position);
			}
		}

		return uniqueDevicePositions.Count;
	}
}