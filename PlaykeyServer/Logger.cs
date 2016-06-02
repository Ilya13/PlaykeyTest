using System.Configuration;
using System.IO;
using System.Text;

namespace PlaykeyServer
{
    internal class Logger
    {
        private const string DefaultLogPath = "Playkey.log";
        private static readonly string LogPath;

        static Logger()
        {
            LogPath = ConfigurationManager.AppSettings.Get("LogPath");
            if (string.IsNullOrEmpty(LogPath))
            {
                LogPath = DefaultLogPath;
            }
            if (!File.Exists(LogPath))
            {
                var fs = File.Create(LogPath);
                fs.Close();
            }
        }

        public static void Log(string message)
        {
            var sb = ReadAndInsert(message);
            File.WriteAllText(LogPath, sb.ToString());
        }

        public static string GetLog()
        {
            return File.ReadAllText(LogPath);
        }

        private static StringBuilder ReadAndInsert(string message)
        {
            var sb = new StringBuilder();

            using (var sr = new StreamReader(LogPath))
            {
                string line;
                while ((line = sr.ReadLine()) != null && string.CompareOrdinal(message, line) > 0)
                {
                    sb.AppendLine(line);
                }
                sb.AppendLine(message);
                if (line != null)
                {
                    sb.AppendLine(line);
                    sb.Append(sr.ReadToEnd());
                }
            }

            return sb;
        }
    }
}
