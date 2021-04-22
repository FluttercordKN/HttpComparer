using Newtonsoft.Json;

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace HttpComparer
{
	public class ScenarioPostParametersSet : ScenarioParametersSet
	{
		public dynamic Body { get; set; }
		public override HttpRequestMessage CreateRequestMessage(string scheme, string host, string pathTemplate)
		{
			var path = pathTemplate;
			foreach (var parameterPair in PathParameters)
				path = path.Replace($"{{{parameterPair.Key}}}", parameterPair.Value);
			var builder = new UriBuilder(scheme, host)
			{
				Path = path,
				Query = string.Join("&", QueryParameters.Select(p => $"{HttpUtility.UrlEncode(p.Key)}={HttpUtility.UrlEncode(p.Value)}"))
			};
			var result = new HttpRequestMessage(HttpMethod.Post, builder.Uri);
			var content = new StringContent(JsonConvert.SerializeObject(Body));
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			result.Content = content;
			return result;
		}
	}
}
