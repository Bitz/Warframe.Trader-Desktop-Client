namespace WFTDC.Payloads
{
    using System;
    using System.Collections.Generic;
    using J = Newtonsoft.Json.JsonPropertyAttribute;

    public class MostRecent
    {
        [J("payload")]
        public Payload Payload { get; set; }
    }

    public class Payload
    {
        [J("buy_orders")]
        public List<Order> BuyOrders { get; set; }

        [J("sell_orders")]
        public List<Order> SellOrders { get; set; }
    }

    public class Order
    {
        [J("platform")]
        public Platform Platform { get; set; }

        [J("quantity")]
        public long Quantity { get; set; }

        [J("last_update")]
        public DateTimeOffset LastUpdate { get; set; }

        [J("user")]
        public User User { get; set; }

        [J("platinum")]
        public long Platinum { get; set; }

        [J("order_type")]
        public OrderType OrderType { get; set; }

        [J("visible")]
        public bool Visible { get; set; }

        [J("item")]
        public Item Item { get; set; }

        [J("region")]
        public Region Region { get; set; }

        [J("id")]
        public string Id { get; set; }

        [J("creation_date")]
        public DateTimeOffset CreationDate { get; set; }

        [J("mod_rank")]
        public long? ModRank { get; set; }
    }

    public class Item
    {
        [J("id")]
        public string Id { get; set; }

        //[J("fr")] public Name Fr { get; set; }
        [J("en")]
        public Name Name { get; set; }

        //[J("ko")] public Name Ko { get; set; }
        //[J("ru")] public Name Ru { get; set; }
        [J("sub_icon")]
        public string SubIcon { get; set; }

        [J("url_name")]
        public string UrlName { get; set; }

        [J("icon")]
        public string Icon { get; set; }

        [J("tags")]
        public List<string> Tags { get; set; }

        [J("thumb")]
        public string Thumb { get; set; }

        [J("mod_max_rank")]
        public long? ModMaxRank { get; set; }
    }

    public class Name
    {
        [J("item_name")]
        public string ItemName { get; set; }
    }

    public class User
    {
        [J("reputation")]
        public long Reputation { get; set; }

        [J("avatar")]
        public string Avatar { get; set; }

        [J("ingame_name")]
        public string IngameName { get; set; }

        [J("status")]
        public Status Status { get; set; }

        [J("region")]
        public Region Region { get; set; }

        [J("id")]
        public string Id { get; set; }
    }


}
