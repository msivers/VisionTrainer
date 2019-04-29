using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Occur.Functions.Utils
{
	public static class MultipartPostUtils
	{
		public static async Task<Dictionary<string, FormItem>> Parse(Collection<HttpContent> contents)
		{
			var formItems = new Dictionary<string, FormItem>();

			// Scan the Multiple Parts 
			foreach (HttpContent contentPart in contents)
			{
				var formItem = new FormItem();
				var contentDisposition = contentPart.Headers.ContentDisposition;
				formItem.Name = contentDisposition.Name.Trim('"');
				formItem.Data = await contentPart.ReadAsByteArrayAsync();
				formItem.FileName = String.IsNullOrEmpty(contentDisposition.FileName) ? "" : contentDisposition.FileName.Trim('"');
				formItem.MediaType = contentPart.Headers.ContentType == null ? "" : String.IsNullOrEmpty(contentPart.Headers.ContentType.MediaType) ? "" : contentPart.Headers.ContentType.MediaType;
				formItems.Add(formItem.Name, formItem);
			}

			return formItems;
		}
	}

	public class FormItem
	{
		public string Name { get; set; }
		public byte[] Data { get; set; }
		public string FileName { get; set; }
		public string MediaType { get; set; }
		public string Value { get { return Encoding.Default.GetString(Data); } }
		public bool IsAFileUpload { get { return !String.IsNullOrEmpty(FileName); } }
	}
}
