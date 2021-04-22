
using JsonDiffPatch;


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace HttpComparer
{
	public class Comparer
	{
		public HostAccess BaseHost { get; set; }
		public HostAccess SideHost { get; set; }
		public IReadOnlyCollection<EndpointScenario> Scenarios { get; set; }


		public async Task<string> Execute()
		{
			using var client = new HttpClient();
			var tasks = new List<Task<string>>();
			foreach (var scenario in Scenarios)
				foreach (var parametersSet in scenario.Parameters)
					tasks.Add(ProcessScenarioCase(client, BaseHost, SideHost, scenario.Template, parametersSet));
			var results = await Task.WhenAll(tasks);

			return string.Join(Environment.NewLine, results);
		}

		private static async Task<string> ProcessScenarioCase(HttpClient client, HostAccess baseAccess, HostAccess sideAccess, string urlTemplate, ScenarioParametersSet parametersSet)
		{
			var realPath = urlTemplate;
			foreach (var parameterPair in parametersSet.PathParameters)
				realPath = realPath.Replace($"{{{parameterPair.Key}}}", parameterPair.Value);
			if (parametersSet.QueryParameters.Any())
				realPath = $"{realPath}&{string.Join("?", parametersSet.QueryParameters.Select(p => $"{p.Key}={p.Value}"))}";
			var tasks = new[]
			{
				GetResponse(client, baseAccess, urlTemplate, parametersSet),
				GetResponse(client, sideAccess, urlTemplate, parametersSet)
			};
			var results = await Task.WhenAll(tasks);
			if (results[0].Item1 != HttpStatusCode.OK)
				return $"FAIL\t{realPath}\tBase status code: {results[0].Item1}";
			if (results[1].Item1 != HttpStatusCode.OK)
				return $"FAIL\t{realPath}\tSide status code: {results[1].Item1}";

			var baseContent = results[0].Item2;
			var sideContent = results[1].Item2;

			if (TryParseJson(baseContent, out var baseTokens) && TryParseJson(sideContent, out var sideTokens))
			{
				var fails = new List<string>();
				var operationsSections = JCompareHelper.Diff(baseTokens, sideTokens).ToArray();
				for (var i = 0; i < operationsSections.Length; i++)
				{
					var operationsSet = operationsSections[i];
					if (operationsSet.Any())
					{
						fails.Add($"FAIL\t{realPath}\tResponse item {i}");
						fails.AddRange(operationsSet.Select(operation => $"\tOperation: {operation.Op}; Path: {operation.Path}; Value: {operation.Value}"));
					}
				}
				if (fails.Any())
					return string.Join(Environment.NewLine, fails);
				else
					return $"OK\t{realPath}";
			}
			else
			{
				if (string.Equals(baseContent, sideContent))
					return $"OK\t{realPath}";
				else
					return $"FAIL\t{realPath}\tExtected: {baseContent}; Actual:{sideContent}";
			}
		}

		private static bool TryParseJson(string content, out IList<JToken> tokens)
		{
			try
			{
				var token = JObject.Parse(content);
				tokens = new [] { token };
				return true;
			}
			catch (JsonReaderException)
			{
				try
				{
					tokens = JArray.Parse(content);
					return true;
				}
				catch (JsonReaderException)
				{
					tokens = null;
					return false;
				}
			}
		}

		private static async Task<Tuple<HttpStatusCode, string>> GetResponse(HttpClient client, HostAccess hostAccess, string urlTemplate, ScenarioParametersSet parametersSet)
		{
			using var request = parametersSet.CreateRequestMessage(hostAccess.Scheme, hostAccess.Host, urlTemplate);
			if (!string.IsNullOrEmpty(hostAccess.ClientId) && !string.IsNullOrEmpty(hostAccess.ClientSecret))
				request.Headers.Authorization = new AuthenticationHeaderValue("Knoema", GetAuthorizationValue(hostAccess.ClientId, hostAccess.ClientSecret));
			using var response = await client.SendAsync(request);
			if (!response.IsSuccessStatusCode)
				return new Tuple<HttpStatusCode, string>(response.StatusCode, null);
			var responseContent = await response.Content.ReadAsStringAsync();
			return new Tuple<HttpStatusCode, string>(response.StatusCode, responseContent);
		}

		private static string GetAuthorizationValue(string clientId, string clientSecret)
		{
			var base64hash = Convert.ToBase64String(new HMACSHA1(Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("dd-MM-yy-HH", CultureInfo.InvariantCulture))).ComputeHash(Encoding.UTF8.GetBytes(clientSecret)));
			return $"{clientId}:{base64hash}:1.2";
		}
	}
}
