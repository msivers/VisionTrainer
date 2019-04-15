using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using VisionTrainer.Common.Models;
using VisionTrainer.Models;

namespace VisionTrainer.Services
{
	public interface IDatabase
	{
		Task<List<MediaFile>> GetItemsAsync();
		Task<List<MediaFile>> GetItemsNotDoneAsync();
		Task<MediaFile> GetItemAsync(int id);
		Task<int> SaveItemAsync(MediaFile item);
		Task<int> DeleteItemAsync(MediaFile item);
		Task DeleteAllItemsAsync();
	}

	public class TrainerDatabase : IDatabase
	{
		readonly SQLiteAsyncConnection database;

		public TrainerDatabase(string dbPath)
		{
			database = new SQLiteAsyncConnection(dbPath);
			database.CreateTableAsync<MediaFile>().Wait();
			//database.CreateTableAsync<GeoLocation>().Wait();
			//database.CreateTableAsync<TagArea>().Wait();
		}

		public Task<List<MediaFile>> GetItemsAsync()
		{
			return database.Table<MediaFile>().ToListAsync();
		}

		public Task<List<MediaFile>> GetItemsNotDoneAsync()
		{
			return database.QueryAsync<MediaFile>("SELECT * FROM [MediaFile] WHERE [Complete] = 0");
		}

		public Task<MediaFile> GetItemAsync(int id)
		{
			return database.Table<MediaFile>().Where(i => i.Id == id).FirstOrDefaultAsync();
		}

		public Task<int> SaveItemAsync(MediaFile item)
		{
			if (item.Id != 0)
			{
				return database.UpdateAsync(item);
			}
			else
			{
				return database.InsertAsync(item);
			}
		}

		public Task<int> DeleteItemAsync(MediaFile item)
		{
			return database.DeleteAsync(item);
		}

		public Task DeleteAllItemsAsync()
		{
			return database.DeleteAllAsync<MediaFile>();
		}
	}
}

