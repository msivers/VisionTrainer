using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.HttpTransferTasks;
using VisionTrainer.Common.Messages;
using VisionTrainer.Common.Models;
using VisionTrainer.Constants;
using VisionTrainer.Models;

namespace VisionTrainer.Services
{
	public static class AzureService
	{
		public static async Task<IHttpTask> UploadTrainingMedia(MediaFile file)
		{

			var data = new MediaData()
			{
				Location = file.Location,
				MediaType = file.Type,
				Tags = file.Tags
			};

			var dataQueryString = await QueryString.Encode(data);
			var url = ProjectConfig.SubmitTrainingImageUrl + "?" + dataQueryString;

			return CrossHttpTransfers.Current.Upload(url, file.FullPath);
		}

		/*
			public static async Task<AudienceResponse> IdentifyAudience(string location, byte[] image)
			{
				var post = new AudienceRequest();
				post.Image = Convert.ToBase64String(image);
				post.Location = location;

				try
				{
					var json = JsonConvert.SerializeObject(post);
					var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

					return await MakeRequest<AudienceResponse>(ProjectConfig.IdentifyAudienceUrl, httpContent);
				}
				catch (Exception ex)
				{
					return new AudienceResponse()
					{
						Message = ex.Message,
						ErrorCode = 1
					};
				}
			}
			*/

		/// <summary>
		/// Makes the http requests.
		/// </summary>
		/// <returns>The request.</returns>
		/// <param name="url">URL.</param>
		/// <param name="httpContent">Http content.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		static async Task<T> MakeRequest<T>(string url, HttpContent httpContent, JsonConverter[] converters = null) where T : BaseResponse
		{
			try
			{
				using (var httpClient = new HttpClient())
				{
					var httpResponse = await httpClient.PostAsync(url, httpContent);
					if (httpResponse.Content != null)
					{
						var content = await httpResponse.Content.ReadAsStringAsync();
						var identifyResult = JsonConvert.DeserializeObject<T>(content, converters);

						return identifyResult;
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return default(T);
		}
	}
}
