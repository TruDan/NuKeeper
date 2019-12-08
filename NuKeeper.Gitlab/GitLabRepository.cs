using System;
using System.Collections.Generic;
using System.Text;
using NuKeeper.Abstractions.CollaborationModels;
using NuKeeper.Gitlab.Model;
using User = NuKeeper.Abstractions.CollaborationModels.User;

namespace NuKeeper.Gitlab
{
    public class GitLabRepository : Repository
    {
        public GitLabRepository(Project project, Group group)
            : base(
                   project.Name,
                   project.Archived,
                   //new UserPermissions(GitLabHelpers.AccessLevel.IsOwner(project.Permissions.ProjectAccess.AccessLevel) || GitLabHelpers.AccessLevel.IsOwner(project.Permissions.GroupAccess.AccessLevel), true, true), 
                   new UserPermissions(true, true, true), 
                   project.HttpUrlToRepo,
                   new User(group.FullPath, group.Name, null), false, null)
        {

        }
    }
}
