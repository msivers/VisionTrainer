using System;
using System.Collections.Generic;
using LiteDB;
using VisionTrainer.Common.Models;
using VisionTrainer.Models;

namespace VisionTrainer.Services
{
	public interface IDatabase2
	{
		IEnumerable<MediaFile> GetItems();
		IEnumerable<MediaFile> GetItemsNotDone();
		MediaFile GetItem(int id);
		int SaveItem(MediaFile item);
		bool DeleteItem(MediaFile item);
		bool DeleteAllItems();
	}

	public class TrainerDb : IDatabase2
	{
		const string dbName = @"TrainerDatabase.db";
		LiteDatabase db;
		LiteCollection<MediaFile> mediaCollection;
		//LiteCollection<GeoLocation> locationCollection;
		//LiteCollection<TagArea> tagCollection;

		public TrainerDb(string path)
		{
			db = new LiteDatabase(path);
			mediaCollection = db.GetCollection<MediaFile>("media");
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
		}

		public bool DeleteAllItems()
		{
			return db.DropCollection("media");
		}

		public bool DeleteItem(MediaFile item)
		{
			return mediaCollection.Delete(item.Id);
		}

		public MediaFile GetItem(int id)
		{
			return mediaCollection.FindById(id);
		}

		public IEnumerable<MediaFile> GetItems()
		{
			return mediaCollection.FindAll();
		}

		public IEnumerable<MediaFile> GetItemsNotDone()
		{
			return mediaCollection.Find(x => !x.Complete);
		}

		public int SaveItem(MediaFile item)
		{
			return mediaCollection.Insert(item);
		}
	}
}
