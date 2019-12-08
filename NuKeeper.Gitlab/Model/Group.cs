using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using NuKeeper.Abstractions;

namespace NuKeeper.Gitlab.Model
{
    public class Group
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("visibility")]
        public string Visibility { get; set; }

        [JsonProperty("project_creation_level")]
        public string ProjectCreationLevel { get; set; }

        [JsonProperty("subgroup_creation_level")]
        public string SubgroupCreationLevel { get; set; }

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("web_url")]
        public string WebUrl { get; set; }

        [JsonProperty("request_access_enabled")]
        public bool RequestAccessEnabled { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("full_path")]
        public string FullPath { get; set; }

        public List<Project> Projects { get; set; }
    }
}
