using System.Collections.Generic;

namespace HttpComparer
{
	public class EndpointScenario
	{
		public string Template { get; set; }
		public IReadOnlyCollection<ScenarioParametersSet> Parameters { get; set; }
	}
}
