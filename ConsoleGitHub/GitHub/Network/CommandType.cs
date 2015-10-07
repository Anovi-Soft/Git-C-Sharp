using System;

namespace GitHub.Network
{
    [Flags]
    public enum CommandType: short
    {
        None,
        Add,
        Update,
        Commit,
        Revert,
        Log
    }
}