namespace WFTDC
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class Cookie
    {
        public static bool GetCookieFromInternetExplorer(string strHost, string strField, ref string value)
        {
            value = string.Empty;
            bool found = false;

            try
            {
                var strPath = Environment.GetFolderPath(Environment.SpecialFolder.Cookies);
                string[] cookieFiles = Directory.GetFiles(strPath, "*.txt", SearchOption.AllDirectories);

                foreach (string path in cookieFiles)
                {
                    List<string> strCookie = File.ReadAllLines(path).ToList();

                    if (strCookie.Any(x => x == $"{strHost}/"))
                    {
                        int nameIndex = strCookie.FindIndex(x => x == strField);
                        if (nameIndex != -1 && strCookie.Count > nameIndex)
                        {
                            value = strCookie[nameIndex + 1];
                            found = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception) 
            {
                value = string.Empty;
                found = false;
            }

            return found;
        }

        public static bool GetCookieFromChrome(string strHost, string strField, ref string value)
        {
            value = string.Empty;
            bool found = false;

            // Check to see if Chrome Installed
            var strPath = GetChromeCookiePath();
            if (string.Empty == strPath)
            {
                return false;
            }

            try
            {
                var strDb = "Data Source=" + strPath + ";pooling=false";

                using (SQLiteConnection conn = new SQLiteConnection(strDb))
                {
                    using (SQLiteCommand cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@strHost", $"%{strHost}%"));
                        cmd.Parameters.Add(new SQLiteParameter("@strField", $"%{strField}%"));

                        cmd.CommandText = "SELECT encrypted_value FROM cookies WHERE host_key LIKE @strHost AND name LIKE @strField;";
                        conn.Open();
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var blob = (byte[])reader[0];
                                var decodedData = System.Security.Cryptography.ProtectedData.Unprotect(
                                    blob,
                                    null,
                                    System.Security.Cryptography.DataProtectionScope.CurrentUser);
                                value = Encoding.ASCII.GetString(decodedData);
                                found = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                value = string.Empty;
                found = false;
            }

            return found;
        }

        private static string GetChromeCookiePath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string s = Path.Combine(appData, @"Google\Chrome\User Data\");
            DirectoryInfo directoryInWhichToSearch = new DirectoryInfo(s);

            FileInfo[] cookieFiles = directoryInWhichToSearch.GetFiles("Cookies", SearchOption.AllDirectories);

            if (cookieFiles.Length > 0)
            {
                return cookieFiles.OrderByDescending(x => x.LastAccessTime)
                    .FirstOrDefault()
                    .FullName; // x => !x.FullName.Contains("Storage")
            }

            return string.Empty;
        }
    }
}
