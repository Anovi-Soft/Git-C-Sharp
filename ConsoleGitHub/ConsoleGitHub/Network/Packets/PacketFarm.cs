using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGitHub.Network.Packets
{
    class PacketFarm
    {
        public static ICommandPacket GetFromBytes(byte[] bytes)
        {
            return CPacket.FromBytes(bytes);
        }
        
    }
}
