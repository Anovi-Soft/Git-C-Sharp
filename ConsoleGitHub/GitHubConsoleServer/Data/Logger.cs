using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitHub.Util;

namespace GitHubConsoleServer.Data
{
    class Logger
    {
        private readonly string ip;
        private readonly string clientPath;
        private readonly string fileName = "info.txt";

        public Logger(string path, string ip)
        {
            clientPath = path;
            this.ip = ip;
            var allLogsPath = Path.Combine(clientPath, fileName);
        }

        public void Log(string projectName, string log)
        {
            var info = log.Split('\n')
                .Select(a => $"[{Time()}][{ip}] - {log}")
                .ToList();
            var projectsLogsPath = Path.Combine(clientPath, projectName, fileName);
            var allLogsPath = Path.Combine(clientPath, fileName);

            using (var stream = new FileStream(projectsLogsPath, FileMode.OpenOrCreate))//File.Open(projectsLogsPath, FileMode.OpenOrCreate, FileAccess.Write))))
            using (var se = new StreamWriter(stream))
                info.ForEach(se.WriteLine);

            info = info.Select(a => $"[{projectName}]{a}").ToList();

            //using (var stream = File.Open(allLogsPath, FileMode.OpenOrCreate, FileAccess.Write))
            using (var stream = new FileStream(allLogsPath, FileMode.OpenOrCreate))
            using (var se = new StreamWriter(stream))
                info.ForEach(se.WriteLine);

            info.ForEach(Console.WriteLine);
        }

        public string Info(string projectName = null)
        {
            var path = projectName == null ?
                Path.Combine(clientPath, fileName) :
                Path.Combine(clientPath, projectName, fileName);
            return File.ReadAllText(path);
        }

        public static string Time() => $"{DateTime.Now :dd.MM.yy HH:mm:ss}";
    }
}
