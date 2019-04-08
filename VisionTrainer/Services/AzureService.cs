using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VisionTrainer.Common.Messages;

namespace VisionTrainer.Services
{
	public static class AzureService
	{
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
