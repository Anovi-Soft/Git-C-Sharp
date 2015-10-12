using System;

namespace GitHub.Network
{
    [Flags]
    public enum CommandType: short
    {
        None=0,
        Hello=1,
        GoodBy=2,
        Add=4,
        Update=8,
        Commit=16,
        Revert=32,
        Log=64,
        Clone = 128,
        Login=256,
        Registration = 512,
        Auth = Login | Registration | GoodBy,
        WorkerCommands = Add | Clone | Update | Commit | Revert | Log | GoodBy
    }
}