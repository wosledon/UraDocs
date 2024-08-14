namespace UraDocs.Shared;

public class UraRepository
{
    public enum RepoType
    {
        GitHub,
        GitLab,
        Gitee
    }

    public RepoType Type { get; set; } = RepoType.GitHub;
    public string RepositoryUrl { get; set; } = string.Empty;
}
