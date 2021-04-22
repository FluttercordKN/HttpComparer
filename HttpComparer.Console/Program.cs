using Newtonsoft.Json;

using System;
using System.IO;
using System.Collections.Generic;

namespace HttpComparer.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args == null || args.Length != 3)
				return;

			var credsPath = args[0];
			if (!File.Exists(credsPath))
				return;

			var scenariosPath = args[1];
			if (!File.Exists(scenariosPath))
				return;

			var outputPath = args[2];
			if (!Directory.Exists(outputPath))
				return;

			var creds = JsonConvert.DeserializeObject<CredentialsSet>(File.ReadAllText(credsPath));
			var scenarios = JsonConvert.DeserializeObject<IReadOnlyCollection<EndpointScenario>>(File.ReadAllText(scenariosPath));

			var comparer = new Comparer
			{ 
				BaseHost = creds.Base,
				SideHost = creds.Side,
				Scenarios = scenarios
			};

			var resultFile = Path.Combine(outputPath, $"{comparer.BaseHost.Host}-{comparer.SideHost.Host}-{DateTime.Now.ToString().Replace('/', '-').Replace(':', '-')}.txt");

			File.WriteAllText(resultFile, comparer.Execute().GetAwaiter().GetResult());
		}
	}
}
