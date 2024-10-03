namespace MercyFork.Data.Models
{

    public record SearchRepoResult
    {
        public SearchRepoResult()
        {
        }

        public SearchRepoResult(int totalCount, bool incompleteResults, List<RepoInfo> items)
        {
            TotalCount = totalCount;
            IncompleteResults = incompleteResults;
            Items = items;
        }

        /// <summary>
        /// Total number of matching items.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// True if the query timed out and it's possible that the results are incomplete.
        /// </summary>
        public bool IncompleteResults { get; set; }

        /// <summary>
        /// The found items. Up to 100 per page.
        /// </summary>
        public IReadOnlyList<RepoInfo> Items { get; set; } = [];

    }
}
