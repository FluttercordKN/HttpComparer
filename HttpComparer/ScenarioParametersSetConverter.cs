using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;

namespace HttpComparer
{

	public class ScenarioParametersSetConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			var typeInfo = typeof(ScenarioParametersSet);
			return typeInfo.IsAssignableFrom(objectType) && typeInfo.IsAbstract;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			else
			{
				JObject obj = JObject.Load(reader);
				if (obj.TryGetValue(nameof(ScenarioParametersSet.Method), out var etype))
				{
					switch (etype.ToString())
					{
						case "GET":
							var parametersGet = new ScenarioGetParametersSet();
							serializer.Populate(obj.CreateReader(), parametersGet);
							return parametersGet;
						case "POST":
							var parametersPost = new ScenarioPostParametersSet();
							serializer.Populate(obj.CreateReader(), parametersPost);
							return parametersPost;
					}
				}
			}
			return null;
		}

		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
