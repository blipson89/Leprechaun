using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace Leprechaun.Console.Tests
{
	public class ProgramTests
	{
		[Fact]
		public void EnsureAbsoluteConfigPath_WhenPathIsBlank_ReturnLeprechaunConfig()
		{
			string test = Program.EnsureAbsoluteConfigPath(string.Empty);
			string expected = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Leprechaun.config");

			test.Should().BeEquivalentTo(expected);
		}

		[Theory]
		[InlineData(".\\path\\Leprechaun.config", "[exe]\\path\\Leprechaun.config")]
		[InlineData(".\\path\\..\\Leprechaun.config", "[exe]\\Leprechaun.config")]
		[InlineData("path\\Leprechaun.config", "[exe]\\path\\Leprechaun.config")]
		public void EnsureAbsoluteConfigPath_WhenPathIsRelative_ResolveAbsolutePath(string path, string expected)
		{
			string test = Program.EnsureAbsoluteConfigPath(path);

			test.Should().BeEquivalentTo(expected.Replace("[exe]",AppDomain.CurrentDomain.BaseDirectory));
		}

		[Theory]
		[InlineData("C:\\path\\to\\Leprechaun.config")]
		public void EnsureAbsoluteConfigPath_WhenPathIsAbsolute_ReturnIt(string path)
		{
			string test = Program.EnsureAbsoluteConfigPath(path);

			test.Should().BeEquivalentTo(path);
		}
	}
}
