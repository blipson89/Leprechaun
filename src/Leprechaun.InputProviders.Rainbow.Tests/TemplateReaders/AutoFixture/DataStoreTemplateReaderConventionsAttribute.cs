using System.Reflection;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using Leprechaun.Adapters;
using Leprechaun.TemplateReaders;
using NSubstitute;

namespace Leprechaun.InputProviders.Rainbow.Tests.TemplateReaders.AutoFixture
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
					var itemData = context.Resolve(typeof(IItemDataAdapter)) as IItemDataAdapter;
					itemData.TemplateId.Returns(BaseTemplateReader.TemplateTemplateId);
					return itemData;
				}

				return new NoSpecimen();
			}
		}
	}
}