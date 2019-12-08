using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NuKeeper.Abstractions.CollaborationModels;
using NuKeeper.Abstractions.CollaborationPlatform;
using NuKeeper.Abstractions.Configuration;
using NuKeeper.Abstractions.Formats;
using NuKeeper.Abstractions.Logging;
using NuKeeper.Gitlab.Model;
using User = NuKeeper.Abstractions.CollaborationModels.User;

namespace NuKeeper.Gitlab
{
    public class GitlabPlatform : ICollaborationPlatform
    {
        private readonly INuKeeperLogger _logger;
        private GitlabRestClient _client;

        public GitlabPlatform(INuKeeperLogger logger)
        {
            _logger = logger;
        }

        public void Initialise(AuthSettings settings)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = settings.ApiBase
            };
            _client = new GitlabRestClient(httpClient, settings.Token, _logger);
        }

        public async Task<User> GetCurrentUser()
        {
            var user = await _client.GetCurrentUser();

            return new User(user.UserName, user.Name, user.Email);
        }

        public async Task OpenPullRequest(ForkData target, PullRequestRequest request, IEnumerable<string> labels)
        {
            var projectName = target.Owner;
            var repositoryName = target.Name;

            var mergeRequest = new MergeRequest
            {
                Title = request.Title,
                SourceBranch = request.Head,
                Description = request.Body,
                TargetBranch = request.BaseRef,
                Id = $"{projectName}/{repositoryName}",
                RemoveSourceBranch = request.DeleteBranchAfterMerge,
                Labels = labels.ToList()
            };

            await _client.OpenMergeRequest(projectName, repositoryName, mergeRequest);
        }

        public async Task<IReadOnlyList<Organization>> GetOrganizations()
        {
            var groups = await _client.GetAllGroups();
            _logger.Normal($"Read {groups.Count} groups");

            return groups.Select(grp => new Organization(grp.Path ?? grp.Name)).ToList();
        }

        public async Task<IReadOnlyList<Repository>> GetRepositoriesForOrganisation(string organisationName)
        {
            var group = await _client.GetGroup(organisationName);
            //var repos = await _client.GetProjectsForGroup(organisationName);

            _logger.Normal($"Read {group.Projects.Count()} repos for org '{organisationName}'");
            return group.Projects.Select(repo => new GitLabRepository(repo, group)).ToList();
        }

        public async Task<Repository> GetUserRepository(string userName, string repositoryName)
        {
            var project = await _client.GetProject(userName, repositoryName);

            return new Repository(project.Name, project.Archived,
                new UserPermissions(true, true, true),
                project.HttpUrlToRepo,
                null, false, null);
        }

        public Task<Repository> MakeUserFork(string owner, string repositoryName)
        {
            _logger.Error($"{ForkMode.PreferFork} has not yet been implemented for GitLab.");
            throw new NotImplementedException();
        }

        public async Task<bool> RepositoryBranchExists(string userName, string repositoryName, string branchName)
        {
            var result = await _client.CheckExistingBranch(userName, repositoryName, branchName);

            return result != null;
        }

        public Task<SearchCodeResult> Search(SearchCodeRequest search)
        {
            _logger.Error($"Search has not yet been implemented for GitLab.");
            throw new NotImplementedException();
        }
    }
}
