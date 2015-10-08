using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubConsoleServer.Auth
{
    interface IAuth
    {
        string Login();
        string Registration();
    }
}
