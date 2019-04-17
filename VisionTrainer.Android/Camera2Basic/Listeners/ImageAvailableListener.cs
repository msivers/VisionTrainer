using Android.Media;
using Java.IO;
using Java.Lang;
using Java.Nio;

namespace Camera2Basic.Listeners
{
	public class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
	{
		public ImageAvailableListener(ICameraPreview preview)
		{
			if (preview == null)
				throw new System.ArgumentNullException("ICameraPreview");

			owner = preview;
		}

		private readonly ICameraPreview owner;

		public void OnImageAvailable(ImageReader reader)
		{
			var image = reader.AcquireNextImage();
			ByteBuffer buffer = image.GetPlanes()[0].Buffer;
			byte[] bytes = new byte[buffer.Remaining()];
			buffer.Get(bytes);

			owner.CaptureByteArray(bytes);
		}

		/*
				// Saves a JPEG {@link Image} into the specified {@link File}.
				private class ImageSaver : Java.Lang.Object, IRunnable
				{
					// The JPEG image
					private Image mImage;

					// The file we save the image into.
					private File mFile;

					public ImageSaver(Image image, File file)
					{
						if (image == null)
							throw new System.ArgumentNullException("image");
						if (file == null)
							throw new System.ArgumentNullException("file");

						mImage = image;
						mFile = file;
					}

					public void Run()
					{
						ByteBuffer buffer = mImage.GetPlanes()[0].Buffer;
						byte[] bytes = new byte[buffer.Remaining()];
						buffer.Get(bytes);
						using (var output = new FileOutputStream(mFile))
						{
							try
							{
								output.Write(bytes);
							}
							catch (IOException e)
							{
								e.PrintStackTrace();
							}
							finally
							{
								mImage.Close();
							}
						}
					}
				}
		*/
	}
}