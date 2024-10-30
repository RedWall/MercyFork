using MercyFork.Data.Models;

using Octokit;

namespace MercyFork.Data
{
    public static class MercyForkOcktoKitExtensions
    {
        //extension method to convert RepoSearchCriteria to SearchRepositoriesRequest
        public static SearchRepositoriesRequest ToSearchRepositoriesRequest(this RepoSearchCriteria searchCriteria)
        {
            if (string.IsNullOrWhiteSpace(searchCriteria.SortDirection))
                searchCriteria.SortDirection = "Descending";

            return new SearchRepositoriesRequest(searchCriteria.SearchText)
            {
                Archived = searchCriteria.Archived,
                Stars = searchCriteria.Stars?.ToRange(),
                Forks = searchCriteria.Forks?.ToRange(),
                //Followers = searchCriteria.Followers?.ToRange(),
                SortField = searchCriteria.SortField is not null ? Enum.Parse<RepoSearchSort>(searchCriteria.SortField) : null,
                Order = Enum.Parse<SortDirection>(searchCriteria.SortDirection ?? "desc"),
                Page = searchCriteria.Page,
                PerPage = searchCriteria.PageSize
            };
        }

        public static Octokit.Range ToRange(this RepoSearchRange range)
        {
            return range.SearchQualifier switch
            {
                RepoSearchRange.SearchQualifierOperator.Exactly => new Octokit.Range(range.Size),
                RepoSearchRange.SearchQualifierOperator.Between => new Octokit.Range(range.Min, range.Max),
                RepoSearchRange.SearchQualifierOperator.GreaterThan => Octokit.Range.GreaterThan(range.Size),
                RepoSearchRange.SearchQualifierOperator.LessThan => Octokit.Range.LessThan(range.Size),
                RepoSearchRange.SearchQualifierOperator.LessThanOrEqualTo => Octokit.Range.LessThanOrEquals(range.Size),
                RepoSearchRange.SearchQualifierOperator.GreaterThanOrEqualTo => Octokit.Range.GreaterThanOrEquals(range.Size),
                _ => new Octokit.Range(range.Size)
            };
        }

        public static SearchRepoResult ToSearchRepoResult(this SearchRepositoryResult result)
        {
            return new SearchRepoResult()
            {
                TotalCount = result.TotalCount,
                IncompleteResults = result.IncompleteResults,
                Items = (result.Items ?? []).Select(i => i.ToRepoInfo()).ToList()
            };
        }

        public static RepoInfo ToRepoInfo(this Repository repository)
        {
            return new RepoInfo(
                repository.Url,
                repository.HtmlUrl,
                repository.CloneUrl,
                repository.GitUrl,
                repository.Id,
                repository.Name,
                repository.FullName,
                repository.Description,
                repository.Homepage,
                repository.ForksCount,
                repository.StargazersCount,
                repository.OpenIssuesCount,
                repository.PushedAt,
                repository.CreatedAt,
                repository.UpdatedAt,
                repository.License?.ToRepoLicense() ?? new RepoLicense("", ""),
                repository.SubscribersCount
            );
        }

        public static RepoLicense ToRepoLicense(this LicenseMetadata license)
        {
            return new RepoLicense(license.Name, license.Url);
        }
    }
}
