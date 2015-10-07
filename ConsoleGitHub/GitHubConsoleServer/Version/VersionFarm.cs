namespace ConsoleGitHub.Data.Version
{
    class VersionFarm
    {
        public static IVersion Parse(string version) => new BaseVersion().Parse(version);
    }
}
