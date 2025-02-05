﻿using Relativity.Testing.Framework.Api.Services;
using Relativity.Testing.Framework.Api.Validators;
using Relativity.Testing.Framework.Versioning;

namespace Relativity.Testing.Framework.Api.Strategies
{
	[VersionRange(">=12.1")]
	internal class ImagingProfileDeleteStrategyV1 : IImagingProfileDeleteStrategy
	{
		private readonly IRestService _restService;
		private readonly IWorkspaceIdValidator _workspaceIdValidator;
		private readonly IArtifactIdValidator _artifactIdValidator;

		public ImagingProfileDeleteStrategyV1(IRestService restService, IWorkspaceIdValidator workspaceIdValidator, IArtifactIdValidator artifactIdValidator)
		{
			_restService = restService;
			_workspaceIdValidator = workspaceIdValidator;
			_artifactIdValidator = artifactIdValidator;
		}

		public void Delete(int workspaceId, int imagingProfileId)
		{
			ValidateInput(workspaceId, imagingProfileId);

			var url = BuildUrl(workspaceId, imagingProfileId);

			_restService.Delete(url);
		}

		private void ValidateInput(int workspaceId, int imagingProfileId)
		{
			_workspaceIdValidator.Validate(workspaceId);
			_artifactIdValidator.Validate(imagingProfileId, "ImagingProfile");
		}

		private string BuildUrl(int workspaceId, int imagingProfileId)
		{
			return $"relativity-imaging/v1/workspaces/{workspaceId}/imaging-profiles/{imagingProfileId}";
		}
	}
}
