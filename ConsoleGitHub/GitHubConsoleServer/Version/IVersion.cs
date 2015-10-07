namespace ConsoleGitHub.Data.Version
{
    public interface IVersion
    {
        IVersion AddVersion(int i=0);
        IVersion Zero();
        IVersion Parse(string version);

    }
}
