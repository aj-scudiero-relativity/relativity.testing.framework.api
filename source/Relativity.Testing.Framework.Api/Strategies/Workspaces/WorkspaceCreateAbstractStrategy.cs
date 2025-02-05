﻿using Relativity.Testing.Framework.Api.Services;
using Relativity.Testing.Framework.Models;
using Relativity.Testing.Framework.Strategies;

namespace Relativity.Testing.Framework.Api.Strategies
{
	internal abstract class WorkspaceCreateAbstractStrategy : CreateStrategy<Workspace>
	{
		private readonly IGetByIdStrategy<Workspace> _getWorkspaceByIdStrategy;
		private readonly IWorkspaceFillRequiredPropertiesStrategy _workspaceFillRequiredPropertiesStrategy;

		protected WorkspaceCreateAbstractStrategy(
			IGetByIdStrategy<Workspace> getWorkspaceByIdStrategy,
			IWorkspaceFillRequiredPropertiesStrategy workspaceFillRequiredPropertiesStrategy)
		{
			_getWorkspaceByIdStrategy = getWorkspaceByIdStrategy;
			_workspaceFillRequiredPropertiesStrategy = workspaceFillRequiredPropertiesStrategy;
		}

		protected override Workspace DoCreate(Workspace entity)
		{
			entity = _workspaceFillRequiredPropertiesStrategy.FillRequiredProperties(entity);

			object workspaceToCreate = BuildRequest(entity);

			int workspaceId = CreateWorkspace(workspaceToCreate);

			return _getWorkspaceByIdStrategy.Get(workspaceId)
				?? throw ObjectNotFoundException.CreateForNotFoundById<Workspace>(workspaceId);
		}

		protected abstract int CreateWorkspace(object workspaceToCreate);

		private object BuildRequest(Workspace entity)
		{
			return new
			{
				WorkspaceRequest = new
				{
					Name = entity.Name,
					Matter = new Securable<Artifact>(new Artifact(entity.Matter.ArtifactID)),
					DefaultCacheLocation = new Securable<Artifact>(entity.DefaultCacheLocation),
					DefaultFileRepository = new Securable<Artifact>(entity.DefaultFileRepository),
					DownloadHandlerUrl = entity.DownloadHandlerUrl,
					EnableDataGrid = false,
					entity.Keywords,
					entity.Notes,
					ResourcePool = new Securable<Artifact>(entity.ResourcePool),
					SqlFullTextLanguage = (int)entity.SqlFullTextLanguage,
					SqlServer = new Securable<Artifact>(entity.SqlServer),
					Status = new Artifact(675),
					Template = new Securable<Artifact>(new Artifact(entity.TemplateWorkspace.ArtifactID)),
					WorkspaceAdminGroup = entity.WorkspaceAdminGroup == null ? null : new Securable<Artifact>(entity.WorkspaceAdminGroup)
				}
			};
		}
	}
}
