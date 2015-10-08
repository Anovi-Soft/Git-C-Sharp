using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitHub.Packets;

namespace GitHubConsoleServer.Auth
{
    interface IAuth
    {
        string Login(ICommandPacket packet);
        string Registration(ICommandPacket packet);
    }
}
