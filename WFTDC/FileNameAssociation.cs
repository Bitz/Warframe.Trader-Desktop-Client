using System.Net;
using Newtonsoft.Json;
using WFTDC.IP;

namespace WFTDC
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class FNA
    {
        private static string PathToItemDb()
        {
            var path = Path.Combine(Functions.PathToTemp(), "fna.db");
            return Path.GetFullPath(path);
        }

        public class Model
        {
            public List<FilePair> Pairs { get; set; } = new List<FilePair>();
        }

        public class FilePair
        {
            public string UrlName { get; set; }
            public string FileName { get; set; }
            public bool ItemHasRanks { get; set; }
        }

        public static Model LoadDB()
        {
            string pathToItemDb = PathToItemDb();
                FileInfo fileInfo = new FileInfo(pathToItemDb);

                if (!fileInfo.Exists)
                {
                    using (File.Create(pathToItemDb))
                    {
                        //Just create the file 
                    }
                }
                string readLines = File.ReadAllText(pathToItemDb);
            var model = JsonConvert.DeserializeObject<Model>(readLines) ?? new Model();
            return model;
        }

        private static void SaveDB()
        {
            string pathToItemDb = PathToItemDb();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
            string configBody = JsonConvert.SerializeObject(Constants.FillNameDatabase, settings);

            File.WriteAllText(pathToItemDb, configBody);
        }

        public static FilePair GetFilePair(string UrlName)
        {
            FilePair filePair;
            string fileName = String.Empty;
            if (Constants.FillNameDatabase.Pairs.All(x => x.UrlName != UrlName))
            {
                string url = $"https://api.warframe.market/v1/items/{UrlName}";
                string iteminfo;
                using (WebClient c = new WebClient())
                {
                    iteminfo = c.DownloadString(url);
                }
                var thisItem = JsonConvert.DeserializeObject<ItemPayload>(iteminfo)
                    .Payload.Item.ItemsInSet.FirstOrDefault(x => x.UrlName == UrlName);

                if (!String.IsNullOrEmpty(thisItem.SubIcon))
                {
                    fileName = thisItem.SubIcon;
                }
                else if (!String.IsNullOrEmpty(thisItem.Icon))
                {
                    fileName = thisItem.Icon;
                }

                var hasRanks = thisItem.ModMaxRank.HasValue;
                filePair = new FilePair
                {
                    FileName = fileName,
                    UrlName = UrlName,
                    ItemHasRanks = hasRanks
                };
                Constants.FillNameDatabase.Pairs.Add(filePair);
                SaveDB();
            }
            else
            {
                filePair = Constants.FillNameDatabase.Pairs.FirstOrDefault(x => x.UrlName == UrlName);
            }
            GetImageIfNotCached(filePair.FileName);
            return filePair;
        }

        private static void GetImageIfNotCached(string imageFileName)
        {
            string filename = Path.GetFileName(imageFileName);
            string IconPathWhenDownloaded = Path.Combine(Functions.PathToTemp(), filename);
            FileInfo file = new FileInfo(IconPathWhenDownloaded);
            //Fallback for when the user deletes the file for some reason. We will need to get it from the online url again.
            if (!file.Exists)
            {
                string OnlineParentDir = "https://warframe.market/static/assets/";
                string url = $"{OnlineParentDir}{imageFileName}";
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(url, IconPathWhenDownloaded);
                }
            }
        }
    }
}