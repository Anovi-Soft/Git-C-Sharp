using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubConsoleServer.Workers
{
    interface IWorker
    {
        void Run(Object arg = null);
    }
}
