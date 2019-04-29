using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Occur.Functions.Utils;
using VisionTrainer.Common.Constants;
using VisionTrainer.Common.Messages;
using VisionTrainer.Common.Models;
using VisionTrainer.Functions.Services;

namespace VisionTrainer.Functions
{
	public static class VisionTrainer
	{

		[FunctionName("LocalModelAvailable")]
		public static async Task<IActionResult> LocalModelAvailable(
		[HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestMessage req, ILogger log)
		{
			var response = new PredictionModelResponse()
			{
				StatusCode = (int)HttpStatusCode.OK,
				IsAvailable = false
			};

			return new OkObjectResult(response);
		}

		[FunctionName("FetchNewLocalModel")]
		public static async Task<IActionResult> FetchNewLocalModel(
		[HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestMessage req, ILogger log)
		{
			var response = new PredictionModelResponse()
			{
				StatusCode = (int)HttpStatusCode.OK,
				IsAvailable = false
			};

			return new OkObjectResult(response);
		}

		[FunctionName("RemoteModelAvailable")]
		public static IActionResult RemoteModelAvailable(
		[HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestMessage req, ILogger log)
		{
			var publishedModelName = Environment.GetEnvironmentVariable("CustomVisionPredictionPublishedName");
			var response = new PredictionModelResponse()
			{
				StatusCode = (int)HttpStatusCode.OK,
				IsAvailable = !string.IsNullOrEmpty(publishedModelName),
				Name = publishedModelName
			};

			return new OkObjectResult(response);
		}

		[FunctionName("SubmitPredictionMedia")]
		public static async Task<IActionResult> SubmitPredictionMedia(
			[HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req, ILogger log)
		{
			var response = new PredictionMediaResponse();

			try
			{
				var provider = new MultipartMemoryStreamProvider();
				await req.Content.ReadAsMultipartAsync(provider);

				var contents = await MultipartPostUtils.Parse(provider.Contents);

				// Grab the Image
				var fileContent = contents.GetValueOrDefault(HttpContentIds.Image);
				if (fileContent == null)
					throw new ArgumentException("'file' content was not present in the request");

				// Grab the Data
				var dataContent = contents.GetValueOrDefault(HttpContentIds.Data);
				if (dataContent == null)
					throw new ArgumentException("'data' content was not present in the request");

				// Deserialize the JSON
				var mediaData = JsonConvert.DeserializeObject<MediaPredictionData>(dataContent.Value);

				// Submit for training
				response.Data = await CustomVisionService.UploadPredictionImage(fileContent.Data, mediaData.TargetModelName);
				response.StatusCode = (int)HttpStatusCode.OK;
				return new OkObjectResult(response);
			}
			catch (Exception ex)
			{
				response.Message = ex.Message;
				response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return new BadRequestObjectResult(response);
			}
		}

		[FunctionName("SubmitTrainingMedia")]
		public static async Task<IActionResult> SubmitTrainingMedia(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, ILogger log)
		{
			var response = new BaseResponse();

			try
			{
				var provider = new MultipartMemoryStreamProvider();
				await req.Content.ReadAsMultipartAsync(provider);

				// Grab the file
				var file = provider.Contents[0];
				var fileData = await file.ReadAsByteArrayAsync();

				// Grab the Data
				var data = provider.Contents[1];
				var stringData = await data.ReadAsStringAsync();

				// Deserialize the JSON
				var mediaEntry = JsonConvert.DeserializeObject<MediaTrainingData>(stringData);
				mediaEntry.FileName = Guid.NewGuid().ToString() + ".jpg";
				mediaEntry.SubmissionDate = DateTime.Now;

				// Store the image
				var uri = await FileStorageService.Instance.StoreImage(fileData, "visiontrainer", mediaEntry.FileName);

				// Save into the DB
				var dbService = new DBService();
				var token = new System.Threading.CancellationToken();
				var result = await dbService.WriteAsync(mediaEntry, token);

				// Submit for training
				await CustomVisionService.UploadTrainingImage(fileData);

				response.StatusCode = (int)HttpStatusCode.OK;
				return new OkObjectResult(response);
			}
			catch (Exception ex)
			{
				response.Message = ex.Message;
				response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return new BadRequestObjectResult(response);
			}
		}
	}
}
