using Newtonsoft.Json;

using System.Collections.Generic;
using System.Net.Http;

namespace HttpComparer
{
	[JsonConverter(typeof(ScenarioParametersSetConverter))]
	public abstract class ScenarioParametersSet
	{
		public Dictionary<string, string> PathParameters { get; set; } = new Dictionary<string, string>();
		public Dictionary<string, string> QueryParameters { get; set; } = new Dictionary<string, string>();
		public string Method { get; set; }
		public abstract HttpRequestMessage CreateRequestMessage(string scheme, string host, string pathTemplate);
	}
}
