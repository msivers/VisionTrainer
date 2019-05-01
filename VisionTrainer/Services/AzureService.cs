using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Newtonsoft.Json;
using VisionTrainer.Common.Constants;
using VisionTrainer.Common.Messages;
using VisionTrainer.Common.Models;
using VisionTrainer.Constants;
using VisionTrainer.Models;
using VisionTrainer.Resources;
using VisionTrainer.Utils;

namespace VisionTrainer.Services
{
	public static class AzureService
	{
		static int remoteModelCachedResponse = -1;
		public static async Task<bool> RemoteModelAvailable(bool useCachedResponse = true)
		{
			if (useCachedResponse && remoteModelCachedResponse >= 0)
				return (remoteModelCachedResponse == 1);

			var result = await MakeGetRequest<PredictionModelResponse>(ProjectConfig.RemoteModelAvailableUrl);

			if (result == null || result.HasError)
			{
				remoteModelCachedResponse = 0;
				return false;
			}

			remoteModelCachedResponse = result.IsAvailable ? 1 : 0;
			return (remoteModelCachedResponse == 1);
		}

		public static async Task<ImagePrediction> UploadPredictionMedia(string filePath)
		{
			var data = new MediaPredictionData()
			{
				TargetModelName = Settings.PublishedModelName,
				MediaType = FileHelper.GetFileType(filePath),
				SubmissionId = Settings.UserId
			};

			byte[] image = File.ReadAllBytes(filePath);
			var stringContent = new StringContent(JsonConvert.SerializeObject(data));
			var content = new Dictionary<string, HttpContent>();

			using (var memoryStream = new MemoryStream(image))
			{
				using (var stream = new StreamContent(memoryStream))
				{
					content.Add(HttpContentIds.Image, stream);
					content.Add(HttpContentIds.Data, stringContent);

					var response = await PostMultipartRequest<PredictionMediaResponse>(ProjectConfig.SubmitPredictionMediaUrl, content);
					if (response?.StatusCode == (int)HttpStatusCode.OK && response?.Data != null)
						return response.Data;
					else
						await App.Current.MainPage.DisplayAlert(
							ApplicationResource.GeneralErrorTitle,
							"(UploadPredictionMedia) " + response.Message,
							ApplicationResource.OK);
				}
			}

			return null;
		}

		public static async Task<bool> UploadTrainingMedia(MediaDetails file)
		{
			var data = new MediaTrainingData()
			{
				Location = file.Location,
				MediaType = file.Type,
				Tags = file.Tags,
				SubmissionId = Settings.UserId
			};

			byte[] image = File.ReadAllBytes(file.FullPath);
			var stringContent = new StringContent(JsonConvert.SerializeObject(data));
			var content = new Dictionary<string, HttpContent>();

			using (var memoryStream = new MemoryStream(image))
			{
				using (var stream = new StreamContent(memoryStream))
				{
					content.Add(HttpContentIds.Image, stream);
					content.Add(HttpContentIds.Data, stringContent);

					var response = await PostMultipartRequest<BaseResponse>(ProjectConfig.SubmitTrainingMediaUrl, content);
					var result = (response.StatusCode == (int)HttpStatusCode.OK);
					if (!result) await App.Current.MainPage.DisplayAlert(
							ApplicationResource.GeneralErrorTitle,
							"(UploadTrainingMedia) " + response.Message,
							ApplicationResource.OK);

					return result;
				}
			}
		}

		/// <summary>
		/// Makes multipart http requests
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="url"></param>
		/// <param name="content"></param>
		/// <param name="converters"></param>
		/// <returns></returns>
		static async Task<T> PostMultipartRequest<T>(string url, Dictionary<string, HttpContent> content, JsonConverter[] converters = null) where T : BaseResponse
		{
			try
			{
				using (var httpClient = GetConfiguredClient())
				{
					using (var postContent = new MultipartFormDataContent("----MyBoundary"))
					{
						foreach (KeyValuePair<string, HttpContent> entry in content)
						{
							postContent.Add(entry.Value, entry.Key);
						}

						var httpResponse = await httpClient.PostAsync(url, postContent);
						// TODO Handle situations where 401's etc... ie not a 200 response are returned properly with an error
						//throw new NotImplementedException();
						if (httpResponse.Content != null)
						{
							var result = await httpResponse.Content.ReadAsStringAsync();
							var parsed = JsonConvert.DeserializeObject<T>(result, converters);

							return parsed;
						}
					}
				}
			}
			catch (Exception ex)
			{
				var response = Activator.CreateInstance<T>();
				response.ErrorCode = (int)HttpStatusCode.ExpectationFailed;
				response.Message = ex.Message;
				return response;
			}

			return default(T);
		}

		/// <summary>
		/// Makes post http requests.
		/// </summary>
		/// <returns>The request.</returns>
		/// <param name="url">URL.</param>
		/// <param name="httpContent">Http content.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		static async Task<T> MakePostRequest<T>(string url, HttpContent httpContent, JsonConverter[] converters = null) where T : BaseResponse
		{
			try
			{
				using (var httpClient = GetConfiguredClient())
				{
					var httpResponse = await httpClient.PostAsync(url, httpContent);
					if (httpResponse.Content != null)
					{
						var content = await httpResponse.Content.ReadAsStringAsync();
						var result = JsonConvert.DeserializeObject<T>(content, converters);

						return result;
					}
				}
			}
			catch (Exception ex)
			{
				var response = Activator.CreateInstance<T>();
				response.ErrorCode = (int)HttpStatusCode.ExpectationFailed;
				response.Message = ex.Message;
				return response;
			}

			return default(T);
		}

		/// <summary>
		/// Makes get http requests.
		/// </summary>
		/// <returns>The request.</returns>
		/// <param name="url">URL.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		static async Task<T> MakeGetRequest<T>(string url, JsonConverter[] converters = null) where T : BaseResponse
		{
			try
			{
				using (var httpClient = GetConfiguredClient())
				{
					var httpResponse = await httpClient.GetAsync(url);
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
				var response = Activator.CreateInstance<T>();
				response.ErrorCode = (int)HttpStatusCode.ExpectationFailed;
				response.Message = ex.Message;
				return response;
			}

			return default(T);
		}

		/// <summary>
		/// Returns a configured instance of an HttpClient
		/// </summary>
		/// <returns></returns>
		static HttpClient GetConfiguredClient()
		{
			var client = new HttpClient();

			if (!string.IsNullOrEmpty(Settings.ApiKey))
				client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Settings.ApiKey);

			return client;
		}
	}
}
