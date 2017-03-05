using System.Web;
using System.IO;
using Newtonsoft.Json;

namespace NineWebService
{
	public class JsonPostHandler : IHttpHandler
	{
		private const string _DefaultPage = 
			@"<html>
				<head>
					<title>Error 400: Bad Request</title>
				</head>
				<body>Error 400: Bad Request - This webpage is intended to respond to requests using the POST method only.</body>
			</html>";

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "application/json";
			if (context.Request.HttpMethod == null || context.Request.HttpMethod.ToUpper() != "POST")
			{
				context.Response.StatusCode = 400;
				context.Response.Write(_DefaultPage);
				return;
			}

			if (context.Request.InputStream == null)
			{
				context.Response.StatusCode = 400;
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
				context.Response.StatusCode = 400;
				context.Response.Write(responseString);
				return;
			}
			context.Response.Write(responseString);
		}
	}
}