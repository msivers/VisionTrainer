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

namespace VisionTrainer.Functions
{
	public static class VisionTrainer
	{
		[FunctionName("SubmitTrainingImage")]
		public static async Task<IActionResult> SubmitTrainingImage(
			[HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req, ILogger log)
		{
			log.LogInformation("C# HTTP trigger function processed a request.");

			var test = await req.Content.ReadAsByteArrayAsync();

			// TODO pull information out of the query params


			return new OkObjectResult($"Hello");
		}

		[FunctionName("TEMP")]
		public static async Task<HttpResponseMessage> Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, ILogger log)
		{
			var test = await req.Content.ReadAsStreamAsync();

			//req.Content.
			if (test != null)
			{

			}
			//application/octet-stream

			//var provider = new MultipartMemoryStreamProvider();
			//var test = req.Content;
			//await req.Content.ReadAsMultipartAsync(provider);
			//var file = provider.Contents.First();
			//var fileInfo = file.Headers.ContentDisposition;
			//var fileData = await file.ReadAsByteArrayAsync();

			//var newImage = new Image()
			//{
			//	FileName = fileInfo.FileName,
			//	Size = fileData.LongLength,
			//	Status = ImageStatus.Processing
			//};

			//var imageName = await DataHelper.CreateImageRecord(newImage);
			//if (!(await StorageHelper.SaveToBlobStorage(imageName, fileData)))
			//return new HttpResponseMessage(HttpStatusCode.InternalServerError);

			return new HttpResponseMessage(HttpStatusCode.Created)
			{
				Content = new StringContent("Temp")
			};
		}
	}
}
