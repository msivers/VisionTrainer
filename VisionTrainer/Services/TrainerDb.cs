using System;
using System.Collections.Generic;
using LiteDB;
using VisionTrainer.Common.Models;
using VisionTrainer.Models;
using VisionTrainer.Utils;

namespace VisionTrainer.Services
{
	public interface IDatabase
	{
		IEnumerable<MediaDetails> GetItems();
		IEnumerable<MediaDetails> GetItemsNotDone();
		MediaDetails GetItem(int id);
		int SaveItem(MediaDetails item);
		bool DeleteItem(MediaDetails item);
		bool DeleteAllItems();
	}

	public class TrainerDb : IDatabase
	{
		const string dbName = @"TrainerDatabase.db";
		LiteDatabase db;
		LiteCollection<MediaDetails> mediaCollection;
		//LiteCollection<GeoLocation> locationCollection;
		//LiteCollection<TagArea> tagCollection;

		public TrainerDb(string path)
		{
			db = new LiteDatabase(path);
			mediaCollection = db.GetCollection<MediaDetails>("media");
			//locationCollection = db.GetCollection<GeoLocation>("locations");
			//tagCollection = db.GetCollection<TagArea>("tags");

			BsonMapper.Global.RegisterType<TagArea>
			(
				serialize: (tag) => tag.ToString(),
				deserialize: (bson) => TagArea.Parse(bson.AsString)
			);

			BsonMapper.Global.RegisterType<GeoLocation>
			(
				serialize: (loc) => loc.ToString(),
				deserialize: (bson) => GeoLocation.Parse(bson.AsString)
			);

			BsonMapper.Global.Entity<MediaDetails>()
				.Id(x => x.Id)
				.Ignore(x => x.FullPath)
				.Ignore(x => x.FullPreviewPath);
		}

		public bool DeleteAllItems()
		{
			// todo remove all stored media
			var collection = GetItems();
			foreach (var item in collection)
				FileHelper.DeleteLocalFiles(item);

			return db.DropCollection("media");
		}

		public bool DeleteItem(MediaDetails item)
		{
			FileHelper.DeleteLocalFiles(item);

			var result = mediaCollection.Delete(item.Id);
			return result;
		}

		public MediaDetails GetItem(int id)
		{
			return mediaCollection.FindById(id);
		}

		public IEnumerable<MediaDetails> GetItems()
		{
			return mediaCollection.FindAll();
		}

		public IEnumerable<MediaDetails> GetItemsNotDone()
		{
			return mediaCollection.Find(x => !x.Complete);
		}

		public int SaveItem(MediaDetails item)
		{
			return mediaCollection.Insert(item);
		}
	}
}
