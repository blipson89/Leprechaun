using System.Reflection;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using Leprechaun.TemplateReaders;
using NSubstitute;
using Rainbow.Model;

namespace Leprechaun.Tests.TemplateReaders.AutoFixture
{
	public class DataStoreTemplateReaderConventionsAttribute : AutoDataAttribute
	{
		private static IFixture MakeFixture()
		{
			var fixture = new Fixture();
			fixture.Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });
			fixture.Customizations.Add(new TemplateItemCustomization());
			return fixture;
		}
		public DataStoreTemplateReaderConventionsAttribute() : base(MakeFixture)
		{
		}
		public class TemplateItemCustomization : ISpecimenBuilder
		{
			public object Create(object request, ISpecimenContext context)
			{
				var pi = request as ParameterInfo;
				if (pi != null && pi.Name == "templateItem")
				{
					var itemData = context.Resolve(typeof(IItemData)) as IItemData;
					itemData.TemplateId.Returns(DataStoreTemplateReader.TemplateTemplateId);
					return itemData;
				}

				return new NoSpecimen();
			}
		}
	}
}