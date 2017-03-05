using System.Web;
using System.IO;
using Newtonsoft.Json;

namespace NineWebService
{
	public class JsonPostHandler : IHttpHandler
	{
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "application/json";
			if (context.Request.HttpMethod == null || context.Request.HttpMethod.ToUpper() != "POST")
			{
				context.Response.TrySkipIisCustomErrors = true;
				context.Response.StatusCode = 400;
				context.Response.StatusDescription = "HTTP_STATUS_BAD_REQUEST";
				ErrorStructure error = new ErrorStructure(ErrorStructure.ErrorTypes.NoContentError);
				JsonParser._Settings.Formatting = Formatting.Indented;
				context.Response.Write(JsonConvert.SerializeObject(error));
				return;
			}

			if (context.Request.InputStream == null)
			{
				context.Response.TrySkipIisCustomErrors = true;
				context.Response.StatusCode = 400;
				context.Response.StatusDescription = "HTTP_STATUS_BAD_REQUEST";
				ErrorStructure error = new ErrorStructure(ErrorStructure.ErrorTypes.NoContentError);
				JsonParser._Settings.Formatting = Formatting.Indented;
				context.Response.Write(JsonConvert.SerializeObject(error));
				return;
			}

			string requestString, responseString;
			using (StreamReader reader = new StreamReader(context.Request.InputStream))
			{
				requestString = reader.ReadToEnd();
			}

			if (!JsonParser.TryParseRequest(requestString, out responseString))
			{
				context.Response.TrySkipIisCustomErrors = true;
				context.Response.StatusCode = 400;
				context.Response.StatusDescription = "HTTP_STATUS_BAD_REQUEST";
				context.Response.Write(responseString);
				return;
			}
			context.Response.Write(responseString);
		}
	}
}