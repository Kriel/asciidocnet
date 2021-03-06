﻿using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AsciiDocNet.Tests.Unit
{
	public class AuthorInfoTests
	{
		public class AsciiAuthorInfoTestCase
		{
			private readonly string DocumentTitle = "= Title";

			public AsciiAuthorInfoTestCase(string input, IList<AuthorInfo> expected)
			{
				Input = DocumentTitle + "\n" + input;
				Expected = expected;
			}

			public AsciiAuthorInfoTestCase(string input, AuthorInfo expected)
			{
				Input = DocumentTitle + "\n" + input;
				Expected = new List<AuthorInfo> { expected };
			}

			public IList<AuthorInfo> Expected { get; set; }

			public string Input { get; set; }
		}

		public static IEnumerable<object[]> TestCases
		{
			get
			{
				// single
				yield return new object[] { new AsciiAuthorInfoTestCase("firstName", new AuthorInfo { FirstName = "firstName" }) };
				yield return new object[] { new AsciiAuthorInfoTestCase("firstName <email>", new AuthorInfo { FirstName = "firstName", Email = "email" }) };
				yield return new object[] { new AsciiAuthorInfoTestCase("firstName lastName", new AuthorInfo { FirstName = "firstName", LastName = "lastName" }) };
				yield return
					new object[] { new AsciiAuthorInfoTestCase("firstName lastName <email>", new AuthorInfo { FirstName = "firstName", LastName = "lastName", Email = "email" }) };
				yield return
					new object[] { new AsciiAuthorInfoTestCase("firstName middleName lastName",
						new AuthorInfo { FirstName = "firstName", MiddleName = "middleName", LastName = "lastName" }) };
				yield return
					new object[] { new AsciiAuthorInfoTestCase("firstName middleName lastName <email>",
						new AuthorInfo { FirstName = "firstName", MiddleName = "middleName", LastName = "lastName", Email = "email" }) };

				// multiple
				yield return new object[] { new AsciiAuthorInfoTestCase(
					"firstName; firstName2",
					new List<AuthorInfo> { new AuthorInfo { FirstName = "firstName" }, new AuthorInfo { FirstName = "firstName2" } }) };

				yield return new object[] { new AsciiAuthorInfoTestCase(
					"firstName <email>; firstName2 <email2>",
					new List<AuthorInfo>
					{
						new AuthorInfo { FirstName = "firstName", Email = "email" },
						new AuthorInfo { FirstName = "firstName2", Email = "email2" }
					}) };

				yield return new object[] { new AsciiAuthorInfoTestCase("firstName lastName; firstName2 lastName2;",
					new List<AuthorInfo>
					{
						new AuthorInfo { FirstName = "firstName", LastName = "lastName" },
						new AuthorInfo { FirstName = "firstName2", LastName = "lastName2" }
					}) };

				yield return new object[] { new AsciiAuthorInfoTestCase(
					"firstName lastName <email>;firstName2 lastName2 <email2>",
					new List<AuthorInfo>
					{
						new AuthorInfo { FirstName = "firstName", LastName = "lastName", Email = "email" },
						new AuthorInfo { FirstName = "firstName2", LastName = "lastName2", Email = "email2" }
					}) };

				yield return new object[] { new AsciiAuthorInfoTestCase(
					"firstName middleName lastName;firstName2 middleName2 lastName2;",
					new List<AuthorInfo>
					{
						new AuthorInfo { FirstName = "firstName", MiddleName = "middleName", LastName = "lastName" },
						new AuthorInfo { FirstName = "firstName2", MiddleName = "middleName2", LastName = "lastName2" }
					}) };

				yield return new object[] { new AsciiAuthorInfoTestCase(
					"firstName middleName lastName <email>; firstName2 middleName2 lastName2 <email2>",
					new List<AuthorInfo>
					{
						new AuthorInfo { FirstName = "firstName", MiddleName = "middleName", LastName = "lastName", Email = "email" },
						new AuthorInfo { FirstName = "firstName2", MiddleName = "middleName2", LastName = "lastName2", Email = "email2" }
					}) };
			}
		}

		[Theory]
		[MemberData(nameof(TestCases))]
		public void ShouldParseAsciiAuthorInfos(AsciiAuthorInfoTestCase testCase)
		{
			var document = Document.Parse(testCase.Input);

			Assert.NotNull(document);
			Assert.NotEmpty(document.Authors);
			Assert.True(document.Authors.SequenceEqual(testCase.Expected));
		}
	}
}
