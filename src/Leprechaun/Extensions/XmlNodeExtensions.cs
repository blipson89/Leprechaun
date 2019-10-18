using System;
using System.Xml;

namespace Leprechaun.Extensions
{
	public static class XmlNodeExtensions
	{
		public static string GetExpectedAttribute(this XmlNode node, string attributeName)
		{
			if(node == null)
				throw new ArgumentNullException(nameof(node));

			// ReSharper disable once PossibleNullReferenceException
			XmlAttribute attribute = node.Attributes[attributeName];

			if (attribute == null) throw new InvalidOperationException($"Missing expected '{attributeName}' attribute on '{node.Name}' node while processing: {node.OuterXml}");

			return attribute.Value;
		}
	}
}
