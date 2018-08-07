using System;
using FluentAssertions;
using Leprechaun.Model;
using Leprechaun.TemplateReaders;
using Leprechaun.Tests.TemplateReaders.AutoFixture;
using Rainbow.Model;
using Rainbow.Storage;
using Xunit;

namespace Leprechaun.Tests.TemplateReaders
{
	public class DataStoreTemplateReaderTests
	{
		#region ParseTemplate

		[Theory, DataStoreTemplateReaderConventions]
		public void ParseTemplate_WhenTemplateItemIsNull_ThrowException(TestableDataSourceTemplateReader sut)
		{
			Assert.Throws<ArgumentException>(() => sut.Public_ParseTemplate(null));
		}

		[Theory, DataStoreTemplateReaderConventions]
		public void ParseTemplate_WhenTemplateIsNotTemplate_ThrowException(TestableDataSourceTemplateReader sut, IItemData invalidTemplateItem)
		{
			Assert.Throws<ArgumentException>(() => sut.Public_ParseTemplate(invalidTemplateItem));
		}

		[Theory, DataStoreTemplateReaderConventions]
		public void ParseTemplate_WhenTemplateIsATemplate_ReturnTemplate(TestableDataSourceTemplateReader sut, IItemData templateItem)
		{
			sut.Public_ParseTemplate(templateItem).Id.Should().Be(templateItem.Id);
		}

		#endregion
	}

	public class TestableDataSourceTemplateReader : DataStoreTemplateReader
	{
		public TestableDataSourceTemplateReader(IDataStore dataStore) : base(dataStore)
		{
		}

		public TemplateInfo Public_ParseTemplate(IItemData templateItem)
		{
			return ParseTemplate(templateItem);
		}
	}
}
