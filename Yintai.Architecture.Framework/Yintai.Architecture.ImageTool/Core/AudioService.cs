using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Yintai.Architecture.ImageTool.Core
{
    internal class AudioService
    {
        private readonly string _compressExePath;

        private static AudioService _instance;
        private static readonly object SyncObject = new object();

        #region .ctor

        private AudioService()
            : this(Path.GetFullPath(ConfigurationManager.AppSettings["audiocompressionexepath"]))
        {
        }

        private AudioService(string exePath)
        {
            _compressExePath = exePath;
        }

        #endregion

        #region properties

        public static AudioService Current
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new AudioService();
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region methods

        private static void CallProcess(string exePath, string fileArgs)
        {
            var startInfo = new ProcessStartInfo
            {
                Arguments = fileArgs,
                FileName = exePath,
                UseShellExecute = false,
                CreateNoWindow = false,
                RedirectStandardOutput = true
            };

            using (var exeProcess = Process.Start(startInfo))//Process.Start(System.IO.Path.Combine(pathImageMagick,appImageMagick),fileArgs))
            {
                exeProcess.WaitForExit();
                exeProcess.Close();
            }
        }

        private static string CallProcessAndReturn(string exePath, string fileArgs)
        {
            var startInfo = new ProcessStartInfo
            {
                Arguments = fileArgs,
                FileName = exePath,
                UseShellExecute = false,
                CreateNoWindow = false,
                RedirectStandardOutput = true
            };

            string result;
            using (var exeProcess = Process.Start(startInfo))//Process.Start(System.IO.Path.Combine(pathImageMagick,appImageMagick),fileArgs))
            {
                using (StreamReader errorreader = exeProcess.StandardError)
                {
                    result = errorreader.ReadToEnd();
                }

                exeProcess.WaitForExit();
                exeProcess.Close();
            }

            return result;
        }

        private static string ConvertDuration(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.Substring(str.IndexOf("Duration: ", System.StringComparison.Ordinal) + ("Duration: ").Length, ("00:00:00").Length);
            }

            return String.Empty;
        }

        #endregion

        public void Compression(string originalFullName, string targetFullName)
        {
            var sbFileArgs = new StringBuilder()
               .AppendFormat(" -i {0} -acodec libmp3lame -ac 2 -ab 17k -ar 16000 -vol 200 {1}", originalFullName, targetFullName);
            var fileArgs = sbFileArgs.ToString();

            CallProcess(_compressExePath, fileArgs);
        }

        public TimeSpan GetDuration(string originalFullName)
        {
            var sbFileArgs = new StringBuilder()
   .AppendFormat(" -i {0}", originalFullName);
            var fileArgs = sbFileArgs.ToString();


            var result = ConvertDuration(CallProcessAndReturn(_compressExePath, fileArgs));

            if (String.IsNullOrEmpty(result))
            {
                return TimeSpan.Parse(result);
            }

            return new TimeSpan(0);
        }
    }
}
