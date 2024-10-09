
namespace MercyFork.Data.Models
{
    public interface IMercyForkApiClient
    {
        Task<SearchRepoResult> GetRepos(RepoSearchCriteria searchCriteria);
    }
}
