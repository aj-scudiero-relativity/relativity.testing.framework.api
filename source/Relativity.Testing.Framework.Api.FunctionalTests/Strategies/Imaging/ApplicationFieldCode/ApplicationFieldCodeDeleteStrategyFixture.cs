﻿using System.Net.Http;
using FluentAssertions;
using NUnit.Framework;
using Relativity.Testing.Framework.Api.Strategies;
using Relativity.Testing.Framework.Models;
using Relativity.Testing.Framework.Versioning;

namespace Relativity.Testing.Framework.Api.FunctionalTests.Strategies
{
	[VersionRange(">=12.1")]
	[TestOf(typeof(IApplicationFieldCodeDeleteStrategy))]
	internal class ApplicationFieldCodeDeleteStrategyFixture : ApiServiceTestFixture<IApplicationFieldCodeDeleteStrategy>
	{
		private IApplicationFieldCodeCreateStrategy _applicationFieldCodeCreateStrategy;

		protected override void OnSetUpFixture()
		{
			base.OnSetUpFixture();

			_applicationFieldCodeCreateStrategy = Facade.Resolve<IApplicationFieldCodeCreateStrategy>();
		}

		[Test]
		public void Delete_NotExistingApplicationFieldCode_ShouldThrowNotFoundException()
		{
			var exception = Assert.Throws<HttpRequestException>(() => Sut.Delete(DefaultWorkspace.ArtifactID, int.MaxValue));

			exception.Message.Should().StartWith("StatusCode: 404, ReasonPhrase: 'Not Found'");
		}

		[Test]
		public void Delete_ExistingApplicationFieldCode_ShouldBeSuccessful()
		{
			var applicationFieldCode = PrepareTestData();
			applicationFieldCode = _applicationFieldCodeCreateStrategy.Create(DefaultWorkspace.ArtifactID, applicationFieldCode);

			Assert.DoesNotThrow(() => Sut.Delete(DefaultWorkspace.ArtifactID, applicationFieldCode.ArtifactID));
		}

		private ApplicationFieldCode PrepareTestData()
		{
			return new ApplicationFieldCode
			{
				Application = ApplicationType.MicrosoftExcel,
				FieldCode = "Author",
				Option = ApplicationFieldCodeOption.DocumentDefault
			};
		}
	}
}
