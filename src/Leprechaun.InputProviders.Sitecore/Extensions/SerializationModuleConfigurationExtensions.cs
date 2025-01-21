using System;
using Sitecore.DevEx.Serialization.Client.Configuration;

namespace Leprechaun.InputProviders.Sitecore.Extensions
{
	public static List<string> GetLeprechaunModuleName(this SerializationModuleConfiguration configuration)
	{
		if(configuration == null)
			throw new NullReferenceException("SerializationModuleConfiguration is null");
	 
		List<string> configNames = new List<string>(); 
		if (!configuration.Extensions.ContainsKey("leprechaun"))
			return new List<string> { configuration.Namespace };
		var token =	configuration.Extensions["leprechaun"].SelectToken("configuration"); 
		if (token.Type == JTokenType.Array)
		{
			foreach (JToken item in token)
			{
				configNames.Add(item.Value<string>("@name"));
			}
		}
		else
		{
			configNames.Add(token.Value<string>("@name"));
		}			
 
		return configNames ?? new List<string> { configuration.Namespace };
	}
}
