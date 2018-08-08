using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using Leprechaun.Model;

namespace Leprechaun.Tests.Test.SpecimenBuilders.StandardArchitectorValidator
{
	public class TemplateMetadataSpec : ITemplateMetadataSpec
	{
		protected TemplateCodeGenerationMetadata Metadata;
		protected List<TemplateCodeGenerationMetadata> Metadatas;
		private bool _init;

		protected virtual void InnerInit(IFixture fixture)
		{
			Metadatas = fixture.CreateMany<TemplateCodeGenerationMetadata>(5).ToList();
			Metadatas[0].BaseTemplates.Add(Metadatas[1]);
			Metadatas[0].BaseTemplates.Add(Metadatas[2]);
			Metadatas[0].BaseTemplates.Add(Metadatas[3]);
			Metadatas[0].BaseTemplates.Add(Metadatas[4]);
			Metadata = Metadatas[0];
			_init = true;
		}
		public void Init(IFixture fixture)
		{
			InnerInit(fixture);
		}

		public object Create(object request, ISpecimenContext context)
		{
			if(!_init) throw new InvalidOperationException("Init not called");

			if (!(request is ParameterInfo pi)) return new NoSpecimen();

			if (pi.Name == "template")
				return Metadata;
			if (pi.Name == "allTemplatesIndex")
				return Metadatas.ToDictionary(m => m.Id);

			return new NoSpecimen();
		}
	}
}