using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TargetAudience.Functions.Services
{
	public class FileStorageService
	{
		private static FileStorageService _instance;
		public static FileStorageService Instance
		{
			get
			{
				_instance = _instance ?? new FileStorageService();
				return _instance;
			}
		}

		string storageConnection = Environment.GetEnvironmentVariable("FileStorageConnectionString");
		CloudStorageAccount storageAccount;
		CloudBlobClient blobClient;

		private FileStorageService()
		{
			// Check whether the connection string can be parsed.
			if (CloudStorageAccount.TryParse(storageConnection, out storageAccount))
			{
				blobClient = storageAccount.CreateCloudBlobClient();
			}
			else
			{
				// Otherwise, let the user know that they need to define the environment variable.
				Console.WriteLine(
					"A connection string has not been defined in the system environment variables. " +
					"Add an environment variable named 'storageconnectionstring' with your storage " +
					"connection string as a value.");
			}
		}

		public async Task<Uri> StoreImage(byte[] image, string containerName, string fileName)
		{
			var blockBlob = await GetBlockBlob(containerName, fileName);
			await blockBlob.UploadFromByteArrayAsync(image, 0, image.Length);

			return blockBlob.Uri;
		}

		public async Task<Uri> StoreImage(Stream image, string containerName, string fileName)
		{
			var blockBlob = await GetBlockBlob(containerName, fileName);
			await blockBlob.UploadFromStreamAsync(image, image.Length);

			return blockBlob.Uri;
		}

		async Task<CloudBlockBlob> GetBlockBlob(string containerName, string fileName)
		{
			CloudBlobContainer container = blobClient.GetContainerReference(containerName);

			if (await container.CreateIfNotExistsAsync())
			{
				var permissions = new BlobContainerPermissions
				{
					PublicAccess = BlobContainerPublicAccessType.Blob
				};
				await container.SetPermissionsAsync(permissions);
			}

			return container.GetBlockBlobReference(fileName);
		}

		public async Task RemoveFile(string containerName, string fileName)
		{
			CloudBlobContainer container = blobClient.GetContainerReference(containerName);

			CloudBlob cloudBlob = container.GetBlobReference(fileName);
			await cloudBlob.DeleteIfExistsAsync();
		}

		public async Task ResetContainer(string containerName)
		{
			BlobContinuationToken blobContinuationToken = null;
			CloudBlobContainer container = blobClient.GetContainerReference(containerName);

			var contents = await container.ListBlobsSegmentedAsync(blobContinuationToken);
			foreach (var blob in contents.Results)
			{
				CloudBlob cloudBlob = container.GetBlobReference(blob.Uri.ToString());
				await cloudBlob.DeleteIfExistsAsync();
			}
		}
	}
}
