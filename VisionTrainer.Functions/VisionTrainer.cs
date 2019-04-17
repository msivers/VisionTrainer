using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using System.Net;
using Microsoft.Azure.WebJobs.Host;
using VisionTrainer.Common.Models;
using VisionTrainer.Common.Messages;

namespace VisionTrainer.Functions
{
	public static class VisionTrainer
	{
		[FunctionName("SubmitTrainingImage")]
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
				var uri = await FileStorageService.Instance.StoreImage(fileData, "visiontraineruploads", mediaEntry.FileName);

				// Save into the DB
				var dbService = new DBService();
				var token = new System.Threading.CancellationToken();
				var result = await dbService.WriteAsync(mediaEntry, token);

				// TODO Submit for training?

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
