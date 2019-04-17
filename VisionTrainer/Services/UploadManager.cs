using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace VisionTrainer.Services
{
	public interface IUploadManager
	{
		void Start();
		void Stop();
	}

	public class UploadManager : IUploadManager
	{
		IDatabase database;

		public UploadManager()
		{
			database = ServiceContainer.Resolve<IDatabase>();
		}

		public async void Start()
		{
			// TODO open an uploads page instead
			var entries = database.GetItemsNotDone();

			foreach (var item in entries)
			{
				var result = await AzureService.UploadTrainingMedia(item);
				if (result)
				{
					database.DeleteItem(item);
				}
			}
		}

		public void Stop()
		{
			throw new NotImplementedException();
		}
	}
}
