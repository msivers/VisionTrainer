﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VisionTrainer.Common.Messages;
using VisionTrainer.Common.Models;
using VisionTrainer.Constants;
using VisionTrainer.Models;

namespace VisionTrainer.Services
{
	public static class AzureService
	{
		public static async Task<bool> UploadTrainingMedia(MediaFile file)
		{
			var data = new MediaData()
			{
				Location = file.Location,
				MediaType = file.Type,
				Tags = file.Tags,
				UserId = Settings.UserId
			};

			var stringContent = new StringContent(JsonConvert.SerializeObject(data));
			var url = ProjectConfig.SubmitTrainingImageUrl;

			try
			{
				byte[] image = File.ReadAllBytes(file.FullPath);
				Uri webService = new Uri(url);

				using (var client = new HttpClient())
				{
					using (var content = new MultipartFormDataContent("----MyBoundary"))
					{
						using (var memoryStream = new MemoryStream(image))
						{
							using (var stream = new StreamContent(memoryStream))
							{
								content.Add(stream, "file");
								content.Add(stringContent, "data");

								using (var message = await client.PostAsync(webService, content))
								{
									if (message.ReasonPhrase.ToLower() == "ok")
									{
										//content.Dispose();
										return true;
									}
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}

			return false;
		}

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
