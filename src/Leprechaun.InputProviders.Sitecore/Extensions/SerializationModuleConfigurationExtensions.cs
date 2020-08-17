using System;
using Sitecore.DevEx.Serialization.Client.Configuration;

namespace Leprechaun.InputProviders.Sitecore.Extensions
{
	public static class SerializationModuleConfigurationExtensions
	{
		public static string GetLeprechaunModuleName(this SerializationModuleConfiguration configuration)
		{
			if(configuration == null)
				throw new NullReferenceException("SerializationModuleConfiguration is null");

			if (!configuration.Extensions.ContainsKey("leprechaun"))
				return configuration.Namespace;

			var configName = configuration.Extensions["leprechaun"].SelectToken("configuration").Value<string>("@name");

			return string.IsNullOrEmpty(configName) ? configuration.Namespace : configName;
		}
	}
}
