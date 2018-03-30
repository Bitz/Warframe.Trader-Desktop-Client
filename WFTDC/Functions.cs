namespace WFTDC
{
    using System.IO;
    using Newtonsoft.Json;
    using WFMSocketizer;

    public class Functions
    {
        public class Config
        {
            public static Configuration Load()
            {
                string configBody = File.ReadAllText(PathToConfig());
                Configuration config = JsonConvert.DeserializeObject<Configuration>(configBody);
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
            }

            private static string PathToConfig()
            {
                string location = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string absoluteCurrentDirectory = Path.GetDirectoryName(location);
                return Path.Combine(absoluteCurrentDirectory, "config.json");
            }
        }
    }
}
