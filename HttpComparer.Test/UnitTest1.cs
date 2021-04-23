using HttpComparer.Console;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HttpComparer.Test
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			var creds = new CredentialsSet
			{
				Base = new HostAccess
				{
					ClientId = "",
					ClientSecret = "",
					Host = "beta.knoema.org",
					Scheme = Uri.UriSchemeHttp
				},
				Side = new HostAccess
				{
					ClientId = "",
					ClientSecret = "",
					Host = "knoema.org",
					Scheme = Uri.UriSchemeHttp
				}
			};
			var scenarios = new[]
			{
				new EndpointScenario
				{
					Template = "api/1.0/meta/dataset/{datasetId}",
					Parameters = new []
					{
						new ScenarioGetParametersSet
						{
							Method = "GET",
							PathParameters = new Dictionary<string, string>
							{
								{ "datasetId", "CDIACTACHIINDUSAA" }
							}
						},
						new ScenarioGetParametersSet
						{
							Method = "GET",
							PathParameters = new Dictionary<string, string>
							{
								{ "datasetId", "MEITY_EAIPF2019" }
							}
						}
					}
				},
				new EndpointScenario
				{
					Template = "api/1.0/meta/dataset/{datasetId}/dimension/{dimensionId}",
					Parameters = new[]
					{
						new ScenarioGetParametersSet
						{
							Method = "GET",
							PathParameters = new Dictionary<string, string>
							{
								{ "datasetId", "CDIACTACHIINDUSAA" },
								{ "dimensionId", "Location" }
							}
						},
						new ScenarioGetParametersSet
						{
							Method = "GET",
							PathParameters = new Dictionary<string, string>
							{
								{ "datasetId", "CDIACTACHIINDUSAA" },
								{ "dimensionId", "Variable" }
							}
						},
						new ScenarioGetParametersSet
						{
							Method = "GET",
							PathParameters = new Dictionary<string, string>
							{
								{ "datasetId", "MEITY_EAIPF2019" },
								{ "dimensionId", "item" }
							}
						}
					}
				},
				new EndpointScenario
				{
					Template = "api/1.0/data/get",
					Parameters = new[]
					{
						new ScenarioPostParametersSet
						{
							Method = "POST",
							Body = new List<object>
							{
								new
								{
									DatasetId = "CDIACTACHIINDUSAA",
									TimeseriesKey = 1000000
								},
								new
								{
									DatasetId = "CDIACTACHIINDUSAA",
									TimeseriesKey = 1000020
								}
							}
						}
					}
				},
				new EndpointScenario
				{
					Template = "api/1.0/frontend/tags",
					Parameters = new[]
					{
						new ScenarioGetParametersSet
						{
							Method = "GET",
							QueryParameters = new Dictionary<string, string>
							{
								{ "tag", "energy" }
							}
						}
					}
				}
			};

			File.WriteAllText("creds.json", JsonConvert.SerializeObject(creds));
			File.WriteAllText("scenarios.json", JsonConvert.SerializeObject(scenarios));

			var instance = new Comparer
			{
				BaseHost = creds.Base,
				SideHost = creds.Side,
				Scenarios = scenarios
			};

			var result = instance.Execute().ToListAsync().GetAwaiter().GetResult();

			System.Console.WriteLine(string.Join(Environment.NewLine, result));
		}
	}
}
