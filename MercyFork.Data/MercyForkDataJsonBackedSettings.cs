using System.Data;
using System.Text.Json;

using MercyFork.Data.Models;

using Microsoft.Extensions.Options;

using Octokit;
using Octokit.Internal;


namespace MercyFork.Data
{

    public class MercyForkDataJsonBackedSettings
    {
        public const string Section = "ApiJsonBackedSettings";
        public string ReposFolder { get; set; } = string.Empty;
    }

    public class MercyForkApiJsonBacked : IMercyForkData
    {
        private readonly MercyForkDataJsonBackedSettings _settings;

        public MercyForkApiJsonBacked(IOptions<MercyForkDataJsonBackedSettings> settings)
        {
            _settings = settings.Value;

            if (string.IsNullOrWhiteSpace(_settings.ReposFolder))
            {
                _settings.ReposFolder = "repos";
            }

            ManageDataPaths();
        }

        public async Task<SearchRepoResult> SearchRepos(RepoSearchCriteria search)
        {
            var data = await LoadAsync<Repository>(_settings.ReposFolder);

            var results = data.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search.SearchText))
                results = results.Where(r =>
                    r.Name.Contains(search.SearchText, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrWhiteSpace(r.Description) && 
                      r.Description.Contains(search.SearchText, StringComparison.OrdinalIgnoreCase))
                    );

            if (search.Archived is bool isArchived)
                results = results.Where(r => r.Archived == isArchived);

            if (search.Stars is not null)
            {
                results = search.Stars.SearchQualifier switch
                {
                    RepoSearchRange.SearchQualifierOperator.GreaterThan => results.Where(r => r.StargazersCount > search.Stars.Size),
                    RepoSearchRange.SearchQualifierOperator.LessThan => results.Where(r => r.StargazersCount < search.Stars.Size),
                    RepoSearchRange.SearchQualifierOperator.GreaterThanOrEqualTo => results.Where(r => r.StargazersCount >= search.Stars.Size),
                    RepoSearchRange.SearchQualifierOperator.LessThanOrEqualTo => results.Where(r => r.StargazersCount <= search.Stars.Size),
                    RepoSearchRange.SearchQualifierOperator.Exactly => results.Where(r => r.StargazersCount == search.Stars.Size),
                    RepoSearchRange.SearchQualifierOperator.Between => results.Where(r => r.StargazersCount >= search.Stars.Min && r.StargazersCount <= search.Stars.Max),
                    _ => results
                };
            }

            if (search.Forks is not null)
            {
                results = search.Forks.SearchQualifier switch
                {
                    RepoSearchRange.SearchQualifierOperator.GreaterThan => results.Where(r => r.ForksCount > search.Forks.Size),
                    RepoSearchRange.SearchQualifierOperator.LessThan => results.Where(r => r.ForksCount < search.Forks.Size),
                    RepoSearchRange.SearchQualifierOperator.GreaterThanOrEqualTo => results.Where(r => r.ForksCount >= search.Forks.Size),
                    RepoSearchRange.SearchQualifierOperator.LessThanOrEqualTo => results.Where(r => r.ForksCount <= search.Forks.Size),
                    RepoSearchRange.SearchQualifierOperator.Exactly => results.Where(r => r.ForksCount == search.Forks.Size),
                    RepoSearchRange.SearchQualifierOperator.Between => results.Where(r => r.ForksCount >= search.Forks.Min && r.ForksCount <= search.Forks.Max),
                    _ => results
                };
            }

            //if (search.Followers is not null)
            //{
            //    results = search.Followers.SearchQualifier switch
            //    {
            //        RepoSearchRange.SearchQualifierOperator.GreaterThan => results.Where(r => r.SubscribersCount > search.Followers.Size),
            //        RepoSearchRange.SearchQualifierOperator.LessThan => results.Where(r => r.SubscribersCount < search.Followers.Size),
            //        RepoSearchRange.SearchQualifierOperator.GreaterThanOrEqualTo => results.Where(r => r.SubscribersCount >= search.Followers.Size),
            //        RepoSearchRange.SearchQualifierOperator.LessThanOrEqualTo => results.Where(r => r.SubscribersCount <= search.Followers.Size),
            //        RepoSearchRange.SearchQualifierOperator.Exactly => results.Where(r => r.SubscribersCount == search.Followers.Size),
            //        RepoSearchRange.SearchQualifierOperator.Between => results.Where(r => r.SubscribersCount >= search.Followers.Min && r.ForksCount <= search.Followers.Max),
            //        _ => results
            //    };
            //}

            if (!string.IsNullOrWhiteSpace(search.SortField) && Enum.TryParse<RepoSearchSort>(search.SortField, out var sortVal) && Enum.TryParse<SortDirection>(search.SortDirection, out var sortDir))
            {
                results = sortVal switch
                {
                    RepoSearchSort.Stars => sortDir == SortDirection.Ascending ? results.OrderBy(r => r.StargazersCount) : results.OrderByDescending(r => r.StargazersCount),
                    RepoSearchSort.Forks => sortDir == SortDirection.Ascending ? results.OrderBy(r => r.ForksCount) : results.OrderByDescending(r => r.ForksCount),
                    RepoSearchSort.Updated => sortDir == SortDirection.Ascending ? results.OrderBy(r => r.UpdatedAt) : results.OrderByDescending(r => r.UpdatedAt),
                    _ => results
                };
            }

            var totalResults = results.Count();

            results = results.Skip((search.Page - 1) * search.PageSize).Take(search.PageSize);

            return new(totalResults, false, results.Select(r => r.ToRepoInfo()).ToList());
        }

        private void ManageDataPaths()
        {
            if (!Directory.Exists(_settings.ReposFolder))
            {
                Directory.CreateDirectory(_settings.ReposFolder);
            }
        }

        private static async Task<List<T>> LoadAsync<T>(string path)
        {
            var results = new List<T>();

            DirectoryInfo di = new(path);

            //use the GitHub serializer to populate the GH objects from the JSON files
            var simpleJson = new SimpleJsonSerializer();

            foreach (FileInfo file in di.EnumerateFiles("*.json"))
            {
                var json = await File.ReadAllTextAsync(file.FullName);

                if (string.IsNullOrWhiteSpace(json))
                {
                    continue;
                }

                var obj = simpleJson.Deserialize<T>(json);

                if (obj is null)
                {
                    continue;
                }

                results.Add(obj);
            }

            return results;
        }

        private static Task Save<T>(string folder, string fileName, T item)
        {
            var filePath = Path.Combine(folder, $"{fileName}.json");

            return File.WriteAllTextAsync(filePath, JsonSerializer.Serialize<T>(item));
        }

        private static Task Delete<T>(string folder, string fileName)
        {
            var filePath = Path.Combine(folder, $"{fileName}.json");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }
    }
}
