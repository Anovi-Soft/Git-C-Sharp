using System;

namespace ConsoleGitHub.Network
{
    [Flags]
    public enum CommandType: short
    {
        None,
        Version,
        Commit
    }
}