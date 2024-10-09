using System.Text.Json;

using MercyFork.Data.Models;

using Microsoft.AspNetCore.WebUtilities;

namespace MercyFork.Web
{
    public class MercyForkApiClient(HttpClient httpClient)
: IMercyForkApiClient
    {
        static readonly JsonSerializerOptions serializerOptions = new() { IncludeFields = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase , PropertyNameCaseInsensitive = true };

        public async Task<SearchRepoResult> GetRepos(RepoSearchCriteria searchCriteria)
        {
            var baseUri = new Uri("/repos", UriKind.Relative);

            Dictionary<string, string?> queryParams = [];

            queryParams.Add("searchText", searchCriteria.SearchText);
            queryParams.Add("archived", searchCriteria.Archived?.ToString());

            if (searchCriteria.Stars is not null)
            {
                queryParams.Add("stars", searchCriteria.Stars.ToQueryStringValue());
            }

            if (searchCriteria.Forks is not null) {
                queryParams.Add("forks", searchCriteria.Forks.ToQueryStringValue());
            }

            if (searchCriteria.Followers is not null)
            {
                queryParams.Add("followers", searchCriteria.Followers.ToQueryStringValue());
            }

            queryParams.Add("sortField", searchCriteria.SortField);
            queryParams.Add("sortDirection", searchCriteria.SortDirection);
            queryParams.Add("page", searchCriteria.Page.ToString());
            queryParams.Add("pageSize", searchCriteria.PageSize.ToString());

            string uri = QueryHelpers.AddQueryString(baseUri.ToString(), queryParams);

            var result = await httpClient.GetAsync(uri.ToString());


            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();

                if (content is not null)
                {
                    var searchResult = JsonSerializer.Deserialize<SearchRepoResult>(content, serializerOptions);

                    if (searchResult is not null)
                        return searchResult;
                    else
                        return new SearchRepoResult(0, false, []);
                }
            }

            return new SearchRepoResult(0, false, []);

        }
    }
}
