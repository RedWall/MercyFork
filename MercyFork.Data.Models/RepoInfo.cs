namespace MercyFork.Data.Models
{
    public record RepoInfo(string Url, string HtmlUrl, string CloneUrl, string GitUrl, long Id, string Name, string FullName, string Description, string Homepage, int ForksCount, int StargazersCount, int OpenIssuesCount, DateTimeOffset? PushedAt, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, RepoLicense License, int SubscribersCount)
    {

    }
}
