using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WFTDC.Items;

namespace WFTDC
{
    using System.IO;
    using Newtonsoft.Json;

    public class Functions
    {
        public static string PathToTemp()
        {
            var path = Path.Combine(Constants.ApplicationPath, @".\Temp");
            Directory.CreateDirectory(path);
            return Path.GetFullPath(path);
        }

        public class Config
        {
            public static C.Configuration Load()
            {
                FileInfo configFile = new FileInfo(PathToConfig());
                if (!configFile.Exists || configFile.Length == 0)
                {
                    var defaultConfig = new DefaultConfig().Config;
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.Indented
                    };
                    string defaultConfigText = JsonConvert.SerializeObject(defaultConfig, settings);
                    File.WriteAllText(PathToConfig(), defaultConfigText);
                }
                string configBody = File.ReadAllText(PathToConfig());
                C.Configuration config = JsonConvert.DeserializeObject<C.Configuration>(configBody);
                return config;
            }

            public static void Save()
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                };
                string configBody = JsonConvert.SerializeObject(Global.Configuration, settings);
                File.WriteAllText(PathToConfig(), configBody);
                if (Global.ItemWebSocket != null && Global.ItemWebSocket.IsAlive)
                {
                    Global.ItemWebSocket.SendWatchList();
                }
            }

            private static string PathToConfig()
            {
                return Path.Combine(Constants.ApplicationPath, "config.json");
            }
        }

        public class Data
        {
            private static string PathToItemDb()
            {
                var path = Path.Combine(PathToTemp(), "items.db");
                return Path.GetFullPath(path);
            }

            public static void UpdateItemDatabase()
            {
                string itemDB;
                using (WebClient c = new WebClient())
                {
                    itemDB = c.DownloadString("https://api.warframe.market/v1/items");
                }
                File.WriteAllText(PathToItemDb(), itemDB);
            }

            public static List<En> GetItemsDatabase()
            {
                FileInfo f = new FileInfo(PathToItemDb());
                if (!f.Exists)
                {
                    UpdateItemDatabase();
                }
                string fileContents = File.ReadAllText(PathToItemDb());
                if (String.IsNullOrEmpty(fileContents))
                {
                    UpdateItemDatabase();
                }
                fileContents = File.ReadAllText(PathToItemDb());
                var itemsfromDb = JsonConvert.DeserializeObject<List>(fileContents);
                return itemsfromDb.Payload.Items.En.OrderBy(x => x.Name).ToList();
            }
        }
    }
}
