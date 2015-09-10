using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGitHub.Network.Packets
{
    class PacketFarm
    {
        public static ICommandPacket Get(byte[] bytes)
        {
            return new CPacket(bytes);
        }
        
    }
}
