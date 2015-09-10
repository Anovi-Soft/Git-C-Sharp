using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGitHub.Version
{
    public class BaseVersion : IVersion
    {
        private int _lvl;

        public IVersion Zero() => new BaseVersion {_lvl = 0};

        public IVersion AddVersion(int i = 0) => new BaseVersion{_lvl = _lvl+1};

        public IVersion Parse(string version)
        {
            if (version.StartsWith("v"))
                    return new BaseVersion
                    {
                        _lvl = Convert.ToInt32(version.Substring(1))
                    };
            throw new FormatException($"This string \"{version}\" can`t parse");
        }

        public override string ToString() => "v" + _lvl;
        
    }
}
