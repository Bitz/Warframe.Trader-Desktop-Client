using System.Collections.Generic;

namespace WFTDC.Payloads.Items
{
    using J = Newtonsoft.Json.JsonPropertyAttribute;

    public class ItemPayload
    {
        [J("payload")] public MostRecentPayload Payload { get; set; }
    }

    public class MostRecentPayload
    {
        [J("item")] public Item Item { get; set; }
    }

    public class Item
    {
        [J("item_name")] public string ItemName { get; set; }
        [J("url_name")] public string UrlName { get; set; }
        [J("_id")] public string Id { get; set; }
        [J("items_in_set")] public List<ItemsInSet> ItemsInSet { get; set; }
    }

    public class ItemsInSet
    {
        [J("tags")] public List<string> Tags { get; set; }
        [J("mod_max_rank")] public long? ModMaxRank { get; set; }
        [J("rarity")] public string Rarity { get; set; }
        [J("thumb")] public string Thumb { get; set; }
        [J("icon_format")] public string IconFormat { get; set; }
        [J("url_name")] public string UrlName { get; set; }
        [J("fr")] public En Fr { get; set; }
        [J("icon")] public string Icon { get; set; }
        [J("sub_icon")] public string SubIcon { get; set; }
        [J("ko")] public En Ko { get; set; }
        [J("_id")] public string Id { get; set; }
        [J("trading_tax")] public long TradingTax { get; set; }
        [J("en")] public En En { get; set; }
    }

    public class En
    {
        [J("drop")] public List<Drop> Drop { get; set; }
        [J("description")] public string Description { get; set; }
        [J("wiki_link")] public string WikiLink { get; set; }
        [J("item_name")] public string ItemName { get; set; }
    }

    public class Drop
    {
        [J("name")] public string Name { get; set; }
        [J("link")] public object Link { get; set; }
    }
}
