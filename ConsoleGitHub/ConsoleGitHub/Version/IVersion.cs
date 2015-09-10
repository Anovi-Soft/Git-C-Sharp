﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGitHub.Version
{
    public interface IVersion
    {
        IVersion AddVersion(int i=0);
        IVersion Zero();
        IVersion Parse(string version);

    }
}
