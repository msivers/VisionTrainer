using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using VisionTrainer.Common.Models;

public class DBService
{
	internal DocumentClient Client { get; private set; }
	DocumentCollection audienceCollection;
	bool initialized;
	string databaseName;
	string collectionName;

	public DBService()
	{

	}

	public async Task CheckInitialized()
	{
		if (initialized)
			return;

		var endpoint = Environment.GetEnvironmentVariable("DocumentEndpointUri");
		var key = Environment.GetEnvironmentVariable("DocumentAuthKey");
		databaseName = Environment.GetEnvironmentVariable("DocumentDatabaseName");
		collectionName = Environment.GetEnvironmentVariable("DocumentCollectionName");

		if (String.IsNullOrEmpty(endpoint))
		{
			throw new MissingFieldException("A DocumentEndpointUri string has not been defined in the system environment variables. " +
				"Add an environment variable named 'DocumentEndpointUri' with your " +
				"connection string as a value.");
		}
		if (String.IsNullOrEmpty(key))
		{
			throw new MissingFieldException(
				"A DocumentAuthKey string has not been defined in the system environment variables. " +
				"Add an environment variable named 'DocumentAuthKey' with your" +
				"Auth Key string as a value.");
		}
		if (String.IsNullOrEmpty(databaseName))
		{
			throw new MissingFieldException(
				"A DatabaseId string has not been defined in the system environment variables. " +
				"Add an environment variable named 'DatabaseId' with your" +
				"Database Id string as a value.");
		}

		if (String.IsNullOrEmpty(collectionName))
		{
			throw new MissingFieldException(
				"A CollectionId string has not been defined in the system environment variables. " +
				"Add an environment variable named 'CollectionId' with your" +
				"Collection Id string as a value.");
		}

		Client = new DocumentClient(new Uri(endpoint), key);

		// Create the Database if required
		await Client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseName });

		// Get a reference to the Document Collection
		var response = await Client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(databaseName), new DocumentCollection { Id = collectionName });
		audienceCollection = response.Resource;

		initialized = true;
	}

	/// <summary>
	/// Deletes storage items from storage.
	/// </summary>
	/// <returns>The async.</returns>
	/// <param name="locations">Locations.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	public async Task<bool> DeleteAsync(MediaEntry entry, CancellationToken cancellationToken)
	{
		await CheckInitialized();

		if (entry == null)
			throw new ArgumentNullException(nameof(entry));

		try
		{
			var result = await Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, entry.Id));
			return true;
		}
		catch (DocumentClientException de)
		{
			Console.WriteLine(de.Message);
			throw de;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			throw ex;
		}
	}

	/// <summary>
	/// Reads storage items from storage.
	/// </summary>
	/// <returns>The async.</returns>
	/// <param name="locations">Locations.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	public async Task<List<MediaEntry>> ReadAsync(string[] locations, CancellationToken cancellationToken, int maxItemCount = -1)
	{
		await CheckInitialized();

		if (locations == null)
			throw new ArgumentNullException(nameof(locations));

		var items = new List<MediaEntry>();
		try
		{
			FeedOptions queryOptions = (maxItemCount > 0) ? new FeedOptions { MaxItemCount = maxItemCount } : null;
			items = Client.CreateDocumentQuery<MediaEntry>(audienceCollection.SelfLink, queryOptions)
					//.Where(x => locations.Contains(x.Location))
					.ToList();
		}
		catch (DocumentClientException de)
		{
			Console.WriteLine(de.Message);
			throw de;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			throw ex;
		}

		return items;
	}

	/// <summary>
	/// Writes storage items to storage.
	/// </summary>
	/// <returns>The async.</returns>
	/// <param name="entry">Changes.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	public async Task<MediaEntry> WriteAsync(MediaEntry entry, CancellationToken cancellationToken)
	{
		await CheckInitialized();

		if (entry == null)
			throw new ArgumentNullException(nameof(entry));

		try
		{
			var result = await Client.CreateDocumentAsync(audienceCollection.SelfLink, entry);
			entry.Id = result.Resource.Id;
			return entry;
		}
		catch (DocumentClientException de)
		{
			Console.WriteLine(de.Message);
			throw de;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			throw ex;
		}
	}

}