using System;

namespace GitHub.Network
{
    [Flags]
    public enum CommandType: short
    {
        None,
        Hello,
        Add,
        Update,
        Commit,
        Revert,
        Log,
        Login,
        Registration
    }
}