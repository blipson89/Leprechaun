using System;
using System.IO;
using System.Reflection;
using System.Xml;
using FluentAssertions;
using Leprechaun.Filters;
using Leprechaun.Model;
using Xunit;

namespace Leprechaun.Tests.Filters
{
	public class StandardTemplateFilterTests
	{
		[Fact]
		public void ctor_ThrowsArgumentNullException_WhenNodeIsNull()
		{
			Assert.Throws<ArgumentNullException>(() => new StandardTemplateFilter(null, null));
		}

		//
		// PATH INCLUSION/EXCLUSION
		//

		[Theory]
		// BASIC test config
		[InlineData("/sitecore/layout/Simulators/iPad", true)]
		[InlineData("/sitecore/layout/Simulators/Android Phone", false)]
		[InlineData("/sitecore/layout/Simulators/iPhone", false)]
		[InlineData("/sitecore/layout/Simulators/iPhone Apps", true)] // path starts with excluded iPhone path but is not equal
		[InlineData("/sitecore/layout/Simulators/iPhone Apps/1.0", false)]
		// EXPLICIT NO-CHILDREN test config
		[InlineData("/nochildren", true)]
		[InlineData("/nochildren/ignoredchild", false)]
		[InlineData("/nochildren/ignored/stillignored", false)]
		// IMPLICIT NO-CHILDREN test config
		[InlineData("/implicit-nochildren", true)]
		[InlineData("/implicit-nochildren/ignoredchild", false)]
		[InlineData("/implicit-nochildren/ignored/stillignored", false)]
		[InlineData("/implicit-nochildrenwithextrachars", false)]
		// SOME-CHILDREN test config
		[InlineData("/somechildren", true)]
		[InlineData("/somechildren/ignoredchild", false)]
		[InlineData("/somechildren/tests", true)]
		[InlineData("/somechildren/tests/testschild", true)]
		[InlineData("/somechildren/testswithextrachars", false)]
		[InlineData("/somechildren/fests", true)]
		// CHILDREN-OF-CHILDREN test config
		[InlineData("/CoC", true)]
		[InlineData("/CoC/stuff", true)]
		[InlineData("/CoC/stuff/child", false)]
		[InlineData("/CoC/stuffwithextrachars", true)]
		[InlineData("/CoC/morestuff", true)]
		[InlineData("/CoC/morestuff/child/exclusion", false)]
		[InlineData("/CoC/yetmorestuff", true)]
		[InlineData("/CoC/yetmorestuff/gorilla", false)]
		[InlineData("/CoC/yetmorestuff/monkey", true)]
		// WILDCARD CHILDREN-OF-CHILREN test config
		[InlineData("/Wild", true)]
		[InlineData("/Wild/Wild Woozles", true)]
		[InlineData("/Wild/MikesBeers", true)]
		[InlineData("/Wild/MikesBeers/Unopened", false)]
		// WILDCARD CHILDREN-OF-CHILREN subitem test config
		[InlineData("/ChildWild", true)]
		[InlineData("/ChildWild/Wild Woozles", true)]
		[InlineData("/ChildWild/Mike", true)]
		[InlineData("/ChildWild/Mike/Fridge", true)]
		[InlineData("/ChildWild/Mike/Fridge/Beers", false)]
		// LITERAL WILDCARDS (root/child)
		[InlineData("/LiteralWild", false)]
		[InlineData("/LiteralWild/*", true)]
		[InlineData("/LiteralWild/*/Foo", false)]
		[InlineData("/LiteralWild/*/*", true)]
		// PATH PREFIX
		[InlineData("/somechildrenofmine", true)]
		[InlineData("/somechildrenofmine/somegrandchild", true)]
		public void Includes_MatchesExpectedPathResult(string testPath, bool expectedResult)
		{
			var predicate = CreateTestPredicate(CreateTestConfiguration());

			var item = CreateTestTemplate(testPath);

			// all test cases should handle mismatching path casing so we use uppercase paths too as a check on that
			var mismatchedCasePathItem = CreateTestTemplate(testPath.ToUpperInvariant());

			predicate.Includes(item).Should().Be(expectedResult);
			predicate.Includes(mismatchedCasePathItem).Should().Be(expectedResult);
		}

		//
		// DATABASE INCLUSION/EXCLUSION
		//

		[Fact]
		// Deps: BASIC and DB TEST test configs
		public void GetRootItems_ReturnsExpectedRootValues()
		{
			var predicate = new StandardTemplateFilter(CreateTestConfiguration(), null);

			var roots = predicate.GetRootPaths();

			roots.Length.Should().Be(10);
			roots[0].DatabaseName.Should().Be("master");
			roots[0].Path.Should().Be("/sitecore/layout/Simulators");
		}

		private StandardTemplateFilter CreateTestPredicate(XmlNode configNode)
		{
			return new StandardTemplateFilter(configNode, null);
		}

		private XmlNode CreateTestConfiguration()
		{
			var assembly = Assembly.GetExecutingAssembly();
			string text;
			// ReSharper disable AssignNullToNotNullAttribute
			using (var textStreamReader = new StreamReader(assembly.GetManifestResourceStream("Leprechaun.Tests.Filters.TestConfiguration.xml")))
			// ReSharper restore AssignNullToNotNullAttribute
			{
				text = textStreamReader.ReadToEnd();
			}

			var doc = new XmlDocument();
			doc.LoadXml(text);

			return doc.DocumentElement;
		}

		private TemplateInfo CreateTestTemplate(string path)
		{
			return new TemplateInfo { Path = path };
		}
	}
}
