
namespace MercyFork.Data.Models
{
    public interface IMercyForkApi
    {
        Task<SearchRepoResult> SearchRepos(RepoSearchCriteria search);
    }
}
