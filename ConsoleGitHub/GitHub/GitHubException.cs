using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GitHub
{
    public class GitHubException: Exception
    {
        public GitHubException(string message) : base(message)
        {
        }
        public GitHubException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
