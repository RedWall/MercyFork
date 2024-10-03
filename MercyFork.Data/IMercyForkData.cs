
using MercyFork.Data.Models;

namespace MercyFork.Data
{
    public interface IMercyForkData
    {
        Task<SearchRepoResult> SearchRepos(RepoSearchCriteria search);
    }
}
