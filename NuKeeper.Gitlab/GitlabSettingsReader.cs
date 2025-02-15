using System;
using System.Linq;
using System.Threading.Tasks;
using NuKeeper.Abstractions;
using NuKeeper.Abstractions.CollaborationPlatform;
using NuKeeper.Abstractions.Configuration;
using NuKeeper.Abstractions.Formats;

namespace NuKeeper.Gitlab
{
    public class GitlabSettingsReader : ISettingsReader
    {
        private readonly IEnvironmentVariablesProvider _environmentVariablesProvider;
        private const string GitLabTokenEnvironmentVariableName = "NuKeeper_gitlab_token";
        private const string UrlPattern = "https://gitlab.com/{username}/{projectname}.git";

        public GitlabSettingsReader(IEnvironmentVariablesProvider environmentVariablesProvider)
        {
            _environmentVariablesProvider = environmentVariablesProvider;
        }

        public Platform Platform => Platform.GitLab;

        public Task<bool> CanRead(Uri repositoryUri)
        {
            if (repositoryUri == null)
                return Task.FromResult(false);

            return Task.FromResult(repositoryUri.Host.Contains("gitlab", StringComparison.OrdinalIgnoreCase));
        }

        public void UpdateCollaborationPlatformSettings(CollaborationPlatformSettings settings)
        {
            var envToken = _environmentVariablesProvider.GetEnvironmentVariable(GitLabTokenEnvironmentVariableName);

            settings.Token = Concat.FirstValue(envToken, settings.Token);
        }

        public Task<RepositorySettings> RepositorySettings(Uri repositoryUri, string targetBranch = null)
        {
            if (repositoryUri == null)
            {
                throw new NuKeeperException(
                    $"The provided uri was is not in the correct format. Provided null and format should be {UrlPattern}");
            }

            // Assumption - url should look like https://gitlab.com/{username}/{projectname}.git";
            var path = repositoryUri.AbsolutePath;
            var pathParts = path.Split(new []
            {
                '/'
            }, 3)

                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            if (pathParts.Count < 2)
            {
                throw new NuKeeperException(
                    $"The provided uri was is not in the correct format. Provided {repositoryUri} and format should be {UrlPattern}");
            }

            var repoOwner = pathParts[0];
            var repoName = pathParts[1].Replace(".git", string.Empty);

            var uriBuilder = new UriBuilder(repositoryUri) { Path = "/api/v4/" };

            return Task.FromResult(new RepositorySettings
            {
                ApiUri = uriBuilder.Uri,
                RepositoryUri = repositoryUri,
                RepositoryName = repoName,
                RepositoryOwner = repoOwner,
                RemoteInfo = targetBranch == null
                    ? null
                    : new RemoteInfo { BranchName = targetBranch }
            });
        }
    }
}
