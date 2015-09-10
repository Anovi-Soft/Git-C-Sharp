using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGitHub.Network
{
    public class AdvancedSocket:Socket, IAdvancedSocket
    {
        public AdvancedSocket(SocketType socketType, ProtocolType protocolType) : base(socketType, protocolType)
        {
        }

        public AdvancedSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) : base(addressFamily, socketType, protocolType)
        {
        }

        public AdvancedSocket(SocketInformation socketInformation) : base(socketInformation)
        {
            base.Receive()
        }
        
    }
}
