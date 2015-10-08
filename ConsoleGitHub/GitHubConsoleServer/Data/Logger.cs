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
            if (!File.Exists(allLogsPath)) File.Create(allLogsPath);
        }

        public void Log(string projectName, string log)
        {
            var info = log.Split('\n').Select(a => $"[{Time()}][{ip}] - {log}");
            var projectsLogsPath = Path.Combine(clientPath, projectName, fileName);
            var allLogsPath = Path.Combine(clientPath, fileName);

            if (!File.Exists(projectsLogsPath)) File.Create(projectsLogsPath);
            FileHelper.UnSetReadOnlyHidden(projectsLogsPath);
            if (!File.Exists(allLogsPath)) File.Create(allLogsPath);
            FileHelper.UnSetReadOnlyHidden(allLogsPath);

            File.AppendAllLines(projectsLogsPath, info);
            info = info.Select(a => $"[{projectName}]{a}");
            File.AppendAllLines(allLogsPath, info);

            foreach(var line in info)
                Console.WriteLine(line);
        }

        public string Info(string projectName = null)
        {
            var path = projectName == null
                ? Path.Combine(clientPath, fileName)
                : Path.Combine(clientPath, projectName, fileName);
            return File.ReadAllText(path);
        }

        public static string Time() => $"{DateTime.Now :dd.MM.yy HH:mm:ss}";
    }
}
