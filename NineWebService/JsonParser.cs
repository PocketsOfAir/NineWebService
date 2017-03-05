using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NineWebService
{
	public static class JsonParser
	{
		public static JsonSerializerSettings _Settings = new JsonSerializerSettings();

		public static bool TryParseRequest(string input, out string output)
		{
			JObject parsedInput;
			try
			{
				parsedInput = JObject.Parse(input);
			}
			catch (JsonReaderException)
			{
				ErrorStructure error = new ErrorStructure(ErrorStructure.ErrorTypes.FailedParseError);
				JsonParser._Settings.Formatting = Formatting.Indented;
				output = JsonConvert.SerializeObject(error);
				return false;
			}

			IList<JToken> entries = parsedInput["payload"].Children().ToList();
			ResponseContainer responses = new ResponseContainer();
			foreach (JToken entry in entries)
			{
				PayloadStructure p = JsonConvert.DeserializeObject<PayloadStructure>(entry.ToString());

				if (p.drm && p.episodeCount > 0)
					responses.response.Add(new ResponseStructure(p));
			}

			_Settings.Formatting = Formatting.Indented;
			output = JsonConvert.SerializeObject(responses, _Settings);

			return true;
		}
	}
}