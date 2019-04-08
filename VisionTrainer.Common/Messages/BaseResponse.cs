using Newtonsoft.Json;

namespace VisionTrainer.Common.Messages
{
	public class BaseResponse
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("requestId")]
		public string RequestId { get; set; }

		[JsonProperty("statusCode")]
		public int StatusCode { get; set; }

		[JsonProperty("errorCode")]
		public int ErrorCode { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }

		public bool HasError
		{
			get { return (ErrorCode != 0) || (StatusCode != 0 && StatusCode != 200); }
		}
	}
}
