using JsonDiffPatch;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Collections.Generic;

namespace HttpComparer
{
	public static class JCompareHelper
	{
		private static readonly JToken _emptyToken = JObject.Parse("{}");

		public static IReadOnlyCollection<PatchOperation> Diff(JToken from, JToken to)
		{
			var patchDoc = new JsonDiffer().Diff(from, to, true);
			return JsonConvert.DeserializeObject<IReadOnlyCollection<PatchOperation>>(patchDoc.ToString());
		}

		public static IEnumerable<IReadOnlyCollection<PatchOperation>> Diff(IList<JToken> from, IList<JToken> to)
		{
			var length = from.Count > to.Count ? from.Count : to.Count;
			for (var i = 0; i < length; i++)
			{
				var fromToken = i < from.Count ? from[i] : _emptyToken;
				var toToken = i < to.Count ? to[i] : _emptyToken;
				yield return Diff(fromToken, toToken);
			}
		}
	}
}
