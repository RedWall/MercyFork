using System.ComponentModel;
using System.Xml.Linq;

using MercyFork.Data.Models;

using Octokit;

namespace MercyFork.Data
{
    public class MercyForkData : IMercyForkData
    {
        private readonly GitHubClient _client;

        public MercyForkData(GitHubClient client)
        {
            _client = client;
        }

        public async Task<SearchRepoResult> SearchRepos(RepoSearchCriteria searchCriteria)
        {
            var result = await _client.Search.SearchRepo(searchCriteria.ToSearchRepositoriesRequest());

            return result.ToSearchRepoResult();
        }
    }
}
