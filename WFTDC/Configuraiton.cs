namespace WFMSocketizer
{
    using System.Collections.Generic;

    using WFTDC;

    using J = Newtonsoft.Json.JsonPropertyAttribute;

    public class Configuration
    {
        [J("User")]
        public User User { get; set; }

        [J("Items")]
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        [J("Name")]
        public string Name { get; set; }

        [J("QuantityMin")]
        public int QuantityMin { get; set; }

        [J("QuantityMax")]
        public int QuantityMax { get; set; }

        [J("ModRankMin")]
        public int? ModRankMin { get; set; }

        [J("ModRankMax")]
        public int? ModRankMax { get; set; }

        [J("Type")]
        public OrderType Type { get; set; }

        [J("Price")]
        public int Price { get; set; }

        [J("UserStates")]
        public List<Status> UserStates { get; set; }
    }

    public class User
    {
        [J("Id")]
        public string Id { get; set; }

        [J("Account")]
        public Account Account { get; set; }

        [J("Platform")]
        public Platform Platform { get; set; }

        [J("Region")]
        public Region Region { get; set; }
    }

    public class Account
    {
        public enum GetCookieFromEnum
        {
            ManualEntry,
            Chrome,
            InternetExplorer
        }

        [J("Enabled")]
        public bool Enabled { get; set; }

        [J("SetStatus")]
        public string SetStatus { get; set; }

        [J("Cookie")]
        public string Cookie { get; set; }

        [J("GetCookieFrom")]
        public GetCookieFromEnum GetCookieFrom { get; set; }
    }
}