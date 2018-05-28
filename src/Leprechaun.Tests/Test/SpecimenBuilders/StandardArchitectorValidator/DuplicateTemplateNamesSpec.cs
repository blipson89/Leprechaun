using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using Leprechaun.Model;

namespace Leprechaun.Tests.Test.SpecimenBuilders.StandardArchitectorValidator
{
	public class DuplicateTemplateNamesSpec : ITemplateMetadataSpec
	{
		protected List<TemplateCodeGenerationMetadata> Metadatas;
		private bool _init;
		public void Init(IFixture fixture)
		{
			Metadatas = fixture.CreateMany<TemplateCodeGenerationMetadata>(5).ToList();
			Metadatas.Add(Metadatas[0]);

			_init = true;
		}

		public object Create(object request, ISpecimenContext context)
		{
			if (!_init) throw new InvalidOperationException("Init not called");

			if (!(request is ParameterInfo pi)) return new NoSpecimen();

			if (pi.Name == "allTemplates")
				return Metadatas;

			return new NoSpecimen();
		}
	}
}