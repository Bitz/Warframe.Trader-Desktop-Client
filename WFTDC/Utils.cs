using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

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

        public static int GetTaskBarHeight()
        {
            int height = Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height;
            return height;
        }
    }
}
