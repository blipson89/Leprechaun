using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Leprechaun.Adapters;
using Leprechaun.InputProviders.Rainbow.TemplateReaders;
using Leprechaun.InputProviders.Rainbow.Tests.TemplateReaders.AutoFixture;
using Leprechaun.Model;
using Rainbow.Model;
using Rainbow.Storage;
using Xunit;

namespace Leprechaun.InputProviders.Rainbow.Tests.TemplateReaders
{
	public class DataStoreTemplateReaderTests
	{
		#region ParseTemplate

		[Theory, DataStoreTemplateReaderConventions]
		public void ParseTemplate_WhenTemplateItemIsNull_ThrowException(TestableDataSourceTemplateReader sut)
		{
			Xunit.Assert.Throws<ArgumentException>(() => sut.Public_ParseTemplate(null));
		}

		[Theory, DataStoreTemplateReaderConventions]
		public void ParseTemplate_WhenTemplateIsNotTemplate_ThrowException(TestableDataSourceTemplateReader sut, IItemDataAdapter invalidTemplateItem)
		{
			Xunit.Assert.Throws<ArgumentException>(() => sut.Public_ParseTemplate(invalidTemplateItem));
		}

		[Theory, DataStoreTemplateReaderConventions]
		public void ParseTemplate_WhenTemplateIsATemplate_ReturnTemplate(TestableDataSourceTemplateReader sut, IItemDataAdapter templateItem)
		{
			sut.Public_ParseTemplate(templateItem).Id.Should().Be(templateItem.Id);
		}

		#endregion

		#region GetAllFields
		[Theory, DataStoreTemplateReaderConventions]
		public void GetAllFields_Includes_AllFields(TestableDataSourceTemplateReader sut, IItemDataAdapter itemData)
		{
			var allFields = sut.Public_GetAllFields(itemData);
			itemData.SharedFields.Select(x => x.FieldId).All(allFields.Keys.Contains).Should().BeTrue();
			itemData.UnversionedFields.SelectMany(x => x.Fields.Select(y => y.FieldId)).All(allFields.Keys.Contains).Should().BeTrue();
			itemData.Versions.SelectMany(x => x.Fields.Select(y => y.FieldId)).All(allFields.Keys.Contains).Should().BeTrue();
		}


		#endregion
	}

	public class TestableDataSourceTemplateReader : DataStoreTemplateReader
	{
		public TestableDataSourceTemplateReader(IDataStore dataStore) : base(null, dataStore)
		{
		}

		public TemplateInfo Public_ParseTemplate(IItemDataAdapter templateItem)
		{
			return ParseTemplate(templateItem);
		}

		public IDictionary<Guid, string> Public_GetAllFields(IItemDataAdapter item)
		{
			return GetAllFields(item);
		}
	}
}
