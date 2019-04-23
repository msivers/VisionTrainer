using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VisionTrainer.Common.Messages;
using VisionTrainer.Common.Models;
using VisionTrainer.Functions.Services;

namespace VisionTrainer.Functions
{
	public static class VisionTrainer
	{
		[FunctionName("UploadImageTest")]
		public static async Task<IActionResult> UploadImage(
			[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req, ILogger log)
		{
			log.LogInformation("C# HTTP trigger function processed a request.");

			var imageData = await req.Content.ReadAsByteArrayAsync();

			try
			{
				// Store the image
				var uri = await FileStorageService.Instance.StoreImage(imageData, "uploads", "test.jpg");

				// Submit for training
				await CustomVisionService.UploadImage(imageData);
			}
			catch (Exception ex)
			{
				return new BadRequestObjectResult("Something went wrong:" + ex.Message);
			}


			return new OkObjectResult("Complete");
		}

		[FunctionName("SubmitTrainingMedia")]
		public static async Task<IActionResult> Run(
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
				var mediaEntry = JsonConvert.DeserializeObject<MediaEntry>(stringData);
				mediaEntry.FileName = Guid.NewGuid().ToString() + ".jpg";
				mediaEntry.SubmissionDate = DateTime.Now;

				// Store the image
				var uri = await FileStorageService.Instance.StoreImage(fileData, "visiontrainer", mediaEntry.FileName);

				// Save into the DB
				var dbService = new DBService();
				var token = new System.Threading.CancellationToken();
				var result = await dbService.WriteAsync(mediaEntry, token);

				// Submit for training
				await CustomVisionService.UploadImage(fileData);

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
