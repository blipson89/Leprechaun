using System.Reflection;
using System.Xml;
using AutoFixture.Kernel;

namespace Leprechaun.Tests.Test.SpecimenBuilders
{
	public class XmlNodeBuilder : ISpecimenBuilder
	{
		public object Create(object request, ISpecimenContext context)
		{
			if (!(request is ParameterInfo pi) || pi.ParameterType != typeof(XmlNode)) return new NoSpecimen();

			var xmlDocument = new XmlDocument();
			xmlDocument.Load(@"Leprechaun.config");
			return xmlDocument;
		}
	}
}