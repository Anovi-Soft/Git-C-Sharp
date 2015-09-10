using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGitHub.Version
{
    class VersionFarm
    {
        public static IVersion Parse(string version) => new BaseVersion().Parse(version);
    }
}
