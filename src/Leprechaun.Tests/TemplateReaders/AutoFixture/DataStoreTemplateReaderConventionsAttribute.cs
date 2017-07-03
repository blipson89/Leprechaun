using System.Reflection;
using NSubstitute;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit2;
using Rainbow.Model;
using Sitecore;

namespace Leprechaun.Tests.TemplateReaders.AutoFixture
{
	public class DataStoreTemplateReaderConventionsAttribute : AutoDataAttribute
	{
		public DataStoreTemplateReaderConventionsAttribute() : base(
			new Fixture().Customize(new AutoConfiguredNSubstituteCustomization()))
		{
			Fixture.Customizations.Add(new TemplateItemCustomization());
		}
		public class TemplateItemCustomization : ISpecimenBuilder
		{
			public object Create(object request, ISpecimenContext context)
			{
				var pi = request as ParameterInfo;
				if (pi != null && pi.Name == "templateItem")
				{
					var itemData = context.Resolve(typeof(IItemData)) as IItemData;
					itemData.TemplateId.Returns(TemplateIDs.Template.Guid);
					return itemData;
				}

				return new NoSpecimen();
			}
		}
	}
}