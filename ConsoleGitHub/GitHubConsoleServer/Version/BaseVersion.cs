﻿using System;
using GitHub;

namespace ConsoleGitHub.Data.Version
{
    public class BaseVersion : IVersion
    {
        private int _lvl;

        public IVersion Zero() => new BaseVersion {_lvl = 0};

        public IVersion AddVersion(int i = 1) => new BaseVersion{_lvl = _lvl+i};

        public IVersion Parse(string version)
        {
            if (version.StartsWith("v"))
                try
                {
                    return new BaseVersion
                    {
                        _lvl = Convert.ToInt32(version.Substring(1))
                    };
                }
                catch (Exception)
                {
                    // ignored
                }
            throw new GitHubException($"This string \"{version}\" can`t be parsed, use 'vX' where X is number");
        }

        public override string ToString() => "v" + _lvl;
        
    }
}
