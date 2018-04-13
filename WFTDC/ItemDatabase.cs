namespace WFTDC.Items
{
    using System.Collections.Generic;

    using J = Newtonsoft.Json.JsonPropertyAttribute;

    public partial class List
    {
        [J("payload")] public Payload Payload { get; set; }
    }

    public partial class Payload
    {
        [J("items")] public Items Items { get; set; }
    }

    public partial class Items
    {
        [J("en")] public List<En> En { get; set; }
    }

    public partial class En
    {
        [J("id")] public string Id { get; set; }
        [J("url_name")] public string UrlName { get; set; }
        [J("item_name")] public string Name { get; set; }
    }
}
