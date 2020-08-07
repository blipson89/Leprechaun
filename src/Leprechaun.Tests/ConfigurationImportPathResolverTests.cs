using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Leprechaun.Configuration;
using Leprechaun.Logging;
using Xunit;

namespace Leprechaun.Tests
{
	public class ConfigurationImportPathResolverTests
	{
		[Fact]
		public void ResolveImportPaths_ShouldWorkWithAbsolutePath()
		{
			var path = Path.Combine(AbsoluteTestRoot, "Directory2", "Bar.config");

			var sut = CreateTestResolver();

			sut.ResolveImportPaths(path).Should().HaveCount(1).And.Subject.First().Should().Be(path);
		}

		[Fact]
		public void ResolveImportPaths_ShouldWorkWithRelativePath()
		{
			var path = Path.Combine(RelativeTestRoot, "Directory2", "Bar.config");

			var sut = CreateTestResolver();

			sut.ResolveImportPaths(path).Should().HaveCount(1).And.Subject.First().Should().Be(path);
		}

		[Fact]
		public void ResolveImportPaths_ShouldWorkWithDirectoryWildcard()
		{
			var path = Path.Combine(AbsoluteTestRoot, "*", "Bar.config");

			var sut = CreateTestResolver();

			sut.ResolveImportPaths(path).Should().HaveCount(1).And.Subject.First().Should().Be(AbsolutePathToBarConfig);
		}

		[Fact]
		public void ResolveImportPaths_ShouldWorkWithPartialDirectoryWildcard()
		{
			var path = Path.Combine(AbsoluteTestRoot, "Dir*", "Bar.config");

			var sut = CreateTestResolver();

			sut.ResolveImportPaths(path).Should().HaveCount(1).And.Subject.First().Should().Be(AbsolutePathToBarConfig);
		}

		[Fact]
		public void ResolveImportPaths_ShouldWorkWithMultipleWildcard()
		{
			var path = Path.Combine(AbsoluteTestRoot, "*", "*.config");

			var sut = CreateTestResolver();

			sut.ResolveImportPaths(path).Should().HaveCount(1).And.Subject.First().Should().Be(AbsolutePathToBarConfig);
		}

		[Fact]
		public void ResolveImportPaths_ShouldWorkWithMultipleDirectoryWildcard()
		{
			var path = Path.Combine(AbsoluteTestRoot, "*", "*", "*.config");

			var sut = CreateTestResolver();

			sut.ResolveImportPaths(path).Should().HaveCount(1).And.Subject.First().Should().Be(AbsolutePathToFooConfig);
		}

		[Fact]
		public void ResolveImportPaths_ShouldWorkWithRecursiveDirectoryWildcard()
		{
			var path = Path.Combine(AbsoluteTestRoot, "**", "*.config");

			var sut = CreateTestResolver();

			sut.ResolveImportPaths(path).Should().HaveCount(2).And.Contain(AbsolutePathToFooConfig, AbsolutePathToBarConfig);
		}

		private string AbsoluteTestRoot => Path.Combine(Environment.CurrentDirectory, "ConfigurationImportPathResolverTests");
		private string RelativeTestRoot => ".\\ConfigurationImportPathResolverTests";
		private string AbsolutePathToBarConfig => Path.Combine(AbsoluteTestRoot, "Directory2", "Bar.config");
		private string AbsolutePathToFooConfig => Path.Combine(AbsoluteTestRoot, "Directory1", "Directory1.1", "Foo.config");

		private ConfigurationImportPathResolver CreateTestResolver()
		{
			return new ConfigurationImportPathResolver(new NullLogger());
		}

		private class NullLogger : ILogger
		{
			public void Info(string message)
			{
				
			}

			public void Debug(string message)
			{
				
			}

			public void Warn(string message)
			{
				
			}

			public void Error(string message)
			{
				
			}

			public void Error(Exception exception)
			{
				
			}

			public void Error(string message, Exception exception)
			{
				
			}
		}
	}
}
