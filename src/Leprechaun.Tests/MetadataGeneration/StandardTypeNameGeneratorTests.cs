using FluentAssertions;
using Leprechaun.MetadataGeneration;
using Xunit;

namespace Leprechaun.Tests.MetadataGeneration
{
	public class StandardTypeNameGeneratorTests
	{
		[Theory,
			InlineData("/Foo", "/Foo/Bar", "Bar"),
			InlineData("/Foo/Bar", "/Foo/Bar/Baz/Quux", "Baz.Quux"),
			InlineData("/Foo/Bar", "/Foo/Bar/Baz/_Quux", "Baz.Quux"),
			InlineData("/Foo/Bar", "/Foo/Bar/Baz/_Quux", "Baz._Quux", true),
			InlineData("/Foo/Bar", "/Foo/Bar/Baz/9Foo9/Quux", "Baz._9Foo9.Quux"),
			InlineData("/Foo", "/Foo/Name Transform.Test", "NameTransform.Test"),
			InlineData("/Foo", "/Foo/Name Transform", "NameTransform")]
		public void GetFullTypeName_ShouldPerformAsExpected(string rootNamespace, string fullPath, string expected, bool keepLeadingUnderscores = false)
		{
			var sut = new StandardTypeNameGenerator(rootNamespace, keepLeadingUnderscores);
			sut.GetFullTypeName(fullPath).Should().Be(expected);
		}

		[Theory, 
			InlineData("Foo", "Foo"),
			InlineData("_Foo", "Foo"),
			InlineData("_Foo", "_Foo", true),
			InlineData("9Foo9", "_9Foo9"), // identifier cannot begin with number
			InlineData("Field Name", "FieldName"),
			InlineData("field_name", "FieldName"),
			InlineData("field_name_id", "FieldNameId"),
			InlineData("Field.Name", "Field.Name")]
		public void ConvertToIdentifier_ShouldPerformAsExpected(string input, string expected, bool keepLeadingUnderscores = false)
		{
			var sut = new StandardTypeNameGenerator(string.Empty, keepLeadingUnderscores);

			sut.ConvertToIdentifier(input).Should().Be(expected);
		}
	}
}
