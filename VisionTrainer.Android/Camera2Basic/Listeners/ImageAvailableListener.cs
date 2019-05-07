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
			reader.Close();
		}
	}
}