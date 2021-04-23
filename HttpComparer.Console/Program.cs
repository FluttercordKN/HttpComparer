using Newtonsoft.Json;

using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace HttpComparer.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			MainAsync(args).GetAwaiter().GetResult();
		}

		static async Task MainAsync(string[] args)
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

			var creds = JsonConvert.DeserializeObject<CredentialsSet>(await File.ReadAllTextAsync(credsPath));
			var scenarios = JsonConvert.DeserializeObject<IReadOnlyCollection<EndpointScenario>>(await File.ReadAllTextAsync(scenariosPath));

			var comparer = new Comparer
			{
				BaseHost = creds.Base,
				SideHost = creds.Side,
				Scenarios = scenarios
			};

			var resultFile = Path.Combine(outputPath, $"{comparer.BaseHost.Host}-{comparer.SideHost.Host}-{DateTime.Now.ToString().Replace('/', '-').Replace(':', '-')}.txt");

			var total = scenarios.SelectMany(s => s.Parameters).Count();
			var done = 0;
			using var fileWriter = new StreamWriter(resultFile);
			await foreach (var scenarioResult in comparer.Execute())
			{
				await fileWriter.WriteLineAsync(scenarioResult);
				System.Console.WriteLine($"{DateTime.Now}\t{++done} of {total} API cases done");
			}
		}
	}
}
