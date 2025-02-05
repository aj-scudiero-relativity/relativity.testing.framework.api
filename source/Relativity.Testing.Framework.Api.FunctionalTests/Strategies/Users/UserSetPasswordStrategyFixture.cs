﻿using NUnit.Framework;
using Relativity.Testing.Framework.Api.Services;
using Relativity.Testing.Framework.Api.Strategies;
using Relativity.Testing.Framework.Models;

namespace Relativity.Testing.Framework.Api.FunctionalTests.Strategies
{
	[TestOf(typeof(IUserSetPasswordStrategy))]
	internal class UserSetPasswordStrategyFixture : ApiServiceTestFixture<IUserSetPasswordStrategy>
	{
		[Test]
		public void SetPassword()
		{
			User existingEntity = null;

			Arrange(x => x.Create(new User()).Pick(out existingEntity));

			string newPassword = Randomizer.GetPassword();

			Sut.SetPassword(existingEntity.ArtifactID, newPassword);

			var httpService = new HttpService(
				$"{RelativityInstanceConfiguration.ServerBindingType}://{RelativityInstanceConfiguration.RestServicesHostAddress}",
				existingEntity.EmailAddress,
				newPassword);

			Assert.That(() => httpService.Get<string>("/relativity.rest/api/Relativity.Services.InstanceDetails.IInstanceDetailsModule/InstanceDetailsService/GetKeplerStatusAsync"), Throws.Nothing);
		}

		[Test]
		public void SetPassword_ToSameValue_DoesNotThrow()
		{
			User existingEntity = null;
			const string password = "Test1234!";

			Arrange(x => x.Create(new User { Password = password }).Pick(out existingEntity));

			Assert.That(() => Sut.SetPassword(existingEntity.ArtifactID, password), Throws.Nothing);

			var httpService = new HttpService(
				$"{RelativityInstanceConfiguration.ServerBindingType}://{RelativityInstanceConfiguration.RestServicesHostAddress}",
				existingEntity.EmailAddress,
				password);

			Assert.That(() => httpService.Get<string>("/relativity.rest/api/Relativity.Services.InstanceDetails.IInstanceDetailsModule/InstanceDetailsService/GetKeplerStatusAsync"), Throws.Nothing);
		}
	}
}
