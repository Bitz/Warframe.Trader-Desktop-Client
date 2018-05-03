using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace WFTDC
{
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    /// <summary>
    /// The utils.
    /// </summary>
    internal class Utils
    {
        public static class Retry
        {
            /// <summary>
            ///  Utility used to retry things that may be fixed by wating - for example, if the file is still being written to.  
            /// </summary>
            /// <param name="action"> The function to retry</param>
            /// <param name="timeBetweenRetry">Time to wait between retries. Defaults to 2 seconds.</param>
            /// <param name="retryCount">Number of times to retry. Defaults to 3 tries.</param>
            public static void Do(Action action, int timeBetweenRetry = 2, int retryCount = 3)
            {
                TimeSpan retryInterval = TimeSpan.FromSeconds(timeBetweenRetry);
                DoAndGetReturn<object>(() =>
                {
                    action();
                    return null;
                }, retryInterval.Seconds, retryCount);
            }


            public static T DoAndGetReturn<T>(Func<T> action, int timeBetweenRetry = 5, int retryCount = 3)
            {
                TimeSpan retryInterval = TimeSpan.FromSeconds(timeBetweenRetry);
                List<Exception> exceptions = new List<Exception>();
                for (int retry = 0; retry < retryCount; retry++)
                {
                    try
                    {
                        if (retry > 0)
                            Thread.Sleep(retryInterval);
                        return action();
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
                throw new AggregateException(exceptions);
            }
        }

        public static byte[] CompressData(string message)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            return Compress(bytes);
        }

        public static string DecompressData(byte[] message)
        {
            byte[] bytes = Decompress(message);
            return Encoding.ASCII.GetString(bytes);
        }

        private static byte[] Compress(byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }

                return memory.ToArray();
            }
        }

        private static byte[] Decompress(byte[] gzip)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }


        public enum TaskBarLocation { Top, Bottom, Left, Right }

        public static TaskBarLocation GetTaskBarLocation()
        {
            TaskBarLocation taskBarLocation = TaskBarLocation.Bottom;
            bool taskBarOnTopOrBottom = (Screen.PrimaryScreen.WorkingArea.Width == Screen.PrimaryScreen.Bounds.Width);
            if (taskBarOnTopOrBottom)
            {
                if (Screen.PrimaryScreen.WorkingArea.Top > 0) taskBarLocation = TaskBarLocation.Top;
            }
            else
            {
                taskBarLocation = Screen.PrimaryScreen.WorkingArea.Left > 0 ? TaskBarLocation.Left : TaskBarLocation.Right;
            }
            return taskBarLocation;
        }

        public static int GetWorkableScreenHeight()
        {
            return Screen.PrimaryScreen.WorkingArea.Height;
        }

        public static int GetTaskBarHeight()
        {
            int height = Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height;
            return height;
        }

        public static int LevenshteinDistance(string source, string target)
        {
            if (String.IsNullOrEmpty(source))
            {
                if (String.IsNullOrEmpty(target)) return 0;
                return target.Length;
            }
            if (String.IsNullOrEmpty(target)) return source.Length;

            if (source.Length > target.Length)
            {
                var temp = target;
                target = source;
                source = temp;
            }

            var m = target.Length;
            var n = source.Length;
            var distance = new int[2, m + 1];
            // Initialize the distance 'matrix'
            for (var j = 1; j <= m; j++) distance[0, j] = j;

            var currentRow = 0;
            for (var i = 1; i <= n; ++i)
            {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                var previousRow = currentRow ^ 1;
                for (var j = 1; j <= m; j++)
                {
                    var cost = target[j - 1] == source[i - 1] ? 0 : 1;
                    distance[currentRow, j] = Math.Min(Math.Min(
                            distance[previousRow, j] + 1,
                            distance[currentRow, j - 1] + 1),
                        distance[previousRow, j - 1] + cost);
                }
            }
            return distance[currentRow, m];
        }


        public static void StartWithWindows()
        {
            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rkApp.SetValue("WarframeMarketDesktopClient", executable);
        }


        public static void DoNotStartWithWindows()
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rkApp.DeleteValue("WarframeMarketDesktopClient", false);
        }

        public static bool IsStartUpEnabled()
        {
            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rkApp.GetValue("WarframeMarketDesktopClient") == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(rkApp.GetValue("WarframeMarketDesktopClient").ToString()))
            {
                return false;
            }
            if (rkApp.GetValue("WarframeMarketDesktopClient").ToString() != executable)
            {
                return false;
            }

            return true;
        }



        public static string SetWebsocketStatusString(Payloads.Status status)
        {
            string statusString = @"{{""type"": ""@WS/USER/SET_STATUS"", ""payload"": ""{0}""}}";
            switch (status)
            {
                case Payloads.Status.Ingame:
                    statusString = string.Format(statusString, "ingame");
                        break;
                case Payloads.Status.Online:
                    statusString = string.Format(statusString, "online");
                    break;
                case Payloads.Status.Offline:
                    statusString = string.Format(statusString, "offline");
                    break;
                case Payloads.Status.Invisible:
                    statusString = string.Format(statusString, "invisible");
                    break;
            }
            return statusString;
        }

        public static void Shutdown()
        {

        }
    }
}
