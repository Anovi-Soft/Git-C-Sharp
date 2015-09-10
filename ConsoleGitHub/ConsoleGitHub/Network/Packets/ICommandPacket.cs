using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGitHub.Network.Packets
{
    public interface ICommandPacket
    {
        CommandType Command { get;}
        string[] Args { get;}
        byte[] Bytes { get; }
    }
}
