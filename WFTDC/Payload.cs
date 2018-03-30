namespace WFTDC
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using J = Newtonsoft.Json.JsonPropertyAttribute;

    public class MostRecent
    {
        public static MostRecent FromJson(string json) => JsonConvert.DeserializeObject<MostRecent>(
            json,
            Converter.Settings);

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

    public partial class Item
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

    public partial class User
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

    public enum OrderType
    {
        Buy,

        Sell
    };

    public enum Platform
    {
        Pc
    };

    public enum Region
    {
        En,

        Fr,

        Ru,

        Ko
    };

    public enum Status
    {
        Ingame,

        Offline,

        Online
    };

    static class OrderTypeExtensions
    {
        public static OrderType? ValueForString(string str)
        {
            switch (str)
            {
                case "buy": return OrderType.Buy;
                case "sell": return OrderType.Sell;
                default: return null;
            }
        }

        public static OrderType ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            var str = serializer.Deserialize<string>(reader);
            var maybeValue = ValueForString(str);
            if (maybeValue.HasValue) return maybeValue.Value;
            throw new Exception("Unknown enum case " + str);
        }

        public static void WriteJson(this OrderType value, JsonWriter writer, JsonSerializer serializer)
        {
            switch (value)
            {
                case OrderType.Buy:
                    serializer.Serialize(writer, "buy");
                    break;
                case OrderType.Sell:
                    serializer.Serialize(writer, "sell");
                    break;
            }
        }
    }

    static class PlatformExtensions
    {
        public static Platform? ValueForString(string str)
        {
            switch (str)
            {
                case "pc": return Platform.Pc;
                default: return null;
            }
        }

        public static Platform ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            var str = serializer.Deserialize<string>(reader);
            var maybeValue = ValueForString(str);
            if (maybeValue.HasValue) return maybeValue.Value;
            throw new Exception("Unknown enum case " + str);
        }

        public static void WriteJson(this Platform value, JsonWriter writer, JsonSerializer serializer)
        {
            switch (value)
            {
                case Platform.Pc:
                    serializer.Serialize(writer, "pc");
                    break;
            }
        }
    }

    static class RegionExtensions
    {
        public static Region? ValueForString(string str)
        {
            switch (str)
            {
                case "en": return Region.En;
                case "fr": return Region.Fr;
                case "ru": return Region.Ru;
                case "ko": return Region.Ko;
                default: return null;
            }
        }

        public static Region ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            var str = serializer.Deserialize<string>(reader);
            var maybeValue = ValueForString(str);
            if (maybeValue.HasValue) return maybeValue.Value;
            throw new Exception("Unknown enum case " + str);
        }

        public static void WriteJson(this Region value, JsonWriter writer, JsonSerializer serializer)
        {
            switch (value)
            {
                case Region.En:
                    serializer.Serialize(writer, "en");
                    break;
                case Region.Fr:
                    serializer.Serialize(writer, "fr");
                    break;
                case Region.Ru:
                    serializer.Serialize(writer, "ru");
                    break;
            }
        }
    }

    static class StatusExtensions
    {
        public static Status? ValueForString(string str)
        {
            switch (str)
            {
                case "ingame": return Status.Ingame;
                case "offline": return Status.Offline;
                case "online": return Status.Online;
                default: return null;
            }
        }

        public static Status ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            var str = serializer.Deserialize<string>(reader);
            var maybeValue = ValueForString(str);
            if (maybeValue.HasValue) return maybeValue.Value;
            throw new Exception("Unknown enum case " + str);
        }

        public static void WriteJson(this Status value, JsonWriter writer, JsonSerializer serializer)
        {
            switch (value)
            {
                case Status.Ingame:
                    serializer.Serialize(writer, "ingame");
                    break;
                case Status.Offline:
                    serializer.Serialize(writer, "offline");
                    break;
                case Status.Online:
                    serializer.Serialize(writer, "online");
                    break;
            }
        }
    }

    public static class Serialize
    {
        public static string ToJson(this Payload self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal class Converter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(OrderType) || t == typeof(Platform)
                                                   || t == typeof(Region) || t == typeof(Status)
                                                   || t == typeof(OrderType?) || t == typeof(Platform?)
                                                   || t == typeof(Region?) || t == typeof(Status?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (t == typeof(OrderType)) return OrderTypeExtensions.ReadJson(reader, serializer);
            if (t == typeof(Platform)) return PlatformExtensions.ReadJson(reader, serializer);
            if (t == typeof(Region)) return RegionExtensions.ReadJson(reader, serializer);
            if (t == typeof(Status)) return StatusExtensions.ReadJson(reader, serializer);
            if (t == typeof(OrderType?))
            {
                if (reader.TokenType == JsonToken.Null) return null;
                return OrderTypeExtensions.ReadJson(reader, serializer);
            }
            if (t == typeof(Platform?))
            {
                if (reader.TokenType == JsonToken.Null) return null;
                return PlatformExtensions.ReadJson(reader, serializer);
            }
            if (t == typeof(Region?))
            {
                if (reader.TokenType == JsonToken.Null) return null;
                return RegionExtensions.ReadJson(reader, serializer);
            }
            if (t == typeof(Status?))
            {
                if (reader.TokenType == JsonToken.Null) return null;
                return StatusExtensions.ReadJson(reader, serializer);
            }
            throw new Exception("Unknown type");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var t = value.GetType();
            if (t == typeof(OrderType))
            {
                ((OrderType)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(Platform))
            {
                ((Platform)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(Region))
            {
                ((Region)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(Status))
            {
                ((Status)value).WriteJson(writer, serializer);
                return;
            }
            throw new Exception("Unknown type");
        }

        public static readonly JsonSerializerSettings Settings =
            new JsonSerializerSettings
                {
                    MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                    DateParseHandling = DateParseHandling.None,
                    Converters =
                        {
                            new Converter(),
                            new IsoDateTimeConverter
                                {
                                    DateTimeStyles = DateTimeStyles
                                        .AssumeUniversal
                                }
                        },
                };
    }
}
