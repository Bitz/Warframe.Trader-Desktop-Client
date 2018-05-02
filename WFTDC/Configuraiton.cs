namespace WFTDC
{
    using System.Collections.Generic;

    using J = Newtonsoft.Json.JsonPropertyAttribute;

    public class C
    {
        public class Configuration
        {
            [J("Application")]
            public Application Application { get; set; }

            [J("User")]
            public User User { get; set; }

            [J("Items")]
            public List<Item> Items { get; set; }
        }

        public class Item
        {
            [J("Name")]
            public string Name { get; set; }

            [J("UrlName")]
            public string UrlName { get; set; }

            [J("QuantityMin")]
            public int QuantityMin { get; set; }

            [J("QuantityMax")]
            public int QuantityMax { get; set; }

            [J("ModRankMin")]
            public int? ModRankMin { get; set; }

            [J("ModRankMax")]
            public int? ModRankMax { get; set; }

            [J("Type")]
            public Payloads.OrderType Type { get; set; }

            [J("Price")]
            public int Price { get; set; }

            [J("Enabled")]
            public bool Enabled { get; set; }

        }

        public class User
        {
            [J("Id")]
            public string Id { get; set; }

            [J("Account")]
            public Account Account { get; set; }

            [J("Platform")]
            public Payloads.Platform Platform { get; set; }

            [J("Region")]
            public Payloads.Region Region { get; set; }

            [J("UserStates")]
            public List<Payloads.Status> UserStates { get; set; }
        }

        public class Application
        {
            [J("StartWithWindows")]
            public bool StartWithWindows { get; set; }

            [J("Watcher")]
            public bool Watcher { get; set; }
        }

        public class Account
        {
            public enum GetCookieFromEnum
            {
                Chrome,
                Firefox,
                //InternetExplorer,
                ManualEntry
            }

            [J("GetMessages")]
            public bool GetMessages { get; set; }

            [J("Username")]
            public string Username { get; set; }

            [J("SetStatus")]
            public bool SetStatus { get; set; }

            [J("Cookie")]
            public string Cookie { get; set; }

            [J("GetCookieFrom")]
            public GetCookieFromEnum GetCookieFrom { get; set; }
        }
    }
}