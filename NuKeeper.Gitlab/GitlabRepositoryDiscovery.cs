using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuKeeper.Abstractions;
using NuKeeper.Abstractions.CollaborationModels;
using NuKeeper.Abstractions.CollaborationPlatform;
using NuKeeper.Abstractions.Configuration;
using NuKeeper.Abstractions.Logging;

namespace NuKeeper.Gitlab
{
    public class GitlabRepositoryDiscovery : IRepositoryDiscovery
    {
        private readonly INuKeeperLogger _logger;
        private readonly ICollaborationPlatform _collaborationPlatform;

        public GitlabRepositoryDiscovery(INuKeeperLogger logger, ICollaborationPlatform collaborationPlatform)
        {
            _logger = logger;
            _collaborationPlatform = collaborationPlatform;
        }

        public async Task<IEnumerable<RepositorySettings>> GetRepositories(SourceControlServerSettings settings)
        {
            switch (settings.Scope)
            {
                case ServerScope.Global:
                    _logger.Error($"{settings.Scope} not yet implemented");
                    throw new NotImplementedException();

                case ServerScope.Organisation:
                    return await FromGroup(settings.OrganisationName, settings);


                case ServerScope.Repository:
                    IEnumerable<RepositorySettings> repositorySettings = new[] { settings.Repository };
                    return repositorySettings;

                default:
                    _logger.Error($"Unknown Server Scope {settings.Scope}");
                    throw new NotImplementedException();
            }
        }

        private async Task<IReadOnlyCollection<RepositorySettings>> FromGroup(
            string groupName, SourceControlServerSettings settings)
        {
            var allOrgRepos = await _collaborationPlatform.GetRepositoriesForOrganisation(groupName);

            var usableRepos = allOrgRepos
                              .Where(r => MatchesIncludeExclude(r, settings))
                              .Where(RepoIsModifiable)
                              .ToList();

            if (allOrgRepos.Count > usableRepos.Count)
            {
                _logger.Detailed($"Can pull from {usableRepos.Count} repos out of {allOrgRepos.Count}");
            }

            return usableRepos
                   .Select(r => new RepositorySettings(r))
                   .ToList();
        }

        private static bool MatchesIncludeExclude(Repository repo, SourceControlServerSettings settings)
        {
            return RegexMatch.IncludeExclude(repo.Name, settings.IncludeRepos, settings.ExcludeRepos);
        }

        private static bool RepoIsModifiable(Repository repo)
        {
            return
                !repo.Archived &&
                repo.UserPermissions.Pull;
        }
    }
}
