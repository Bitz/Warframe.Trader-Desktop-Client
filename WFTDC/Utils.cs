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
                const int Size = 4096;
                byte[] buffer = new byte[Size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, Size);
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
    }
}
