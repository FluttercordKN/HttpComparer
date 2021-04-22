using System;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace HttpComparer
{
	public class ScenarioGetParametersSet : ScenarioParametersSet
	{
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
			return new HttpRequestMessage(HttpMethod.Get, builder.Uri);
		}
	}
}
