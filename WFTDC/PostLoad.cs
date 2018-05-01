namespace WFTDC
{
    using J = Newtonsoft.Json.JsonPropertyAttribute;

    public class PostLoad
    {
        [J("I")]
        public ItemBody Item { get; set; }

        [J("Q")]
        public long Quantity { get; set; }

        [J("M")]
        public long? ModRank { get; set; }

        [J("P")]
        public long Platinum { get; set; }

        [J("T")]
        public Payloads.OrderType Type { get; set; }

        [J("U")]
        public UserAccount User { get; set; }

        public class UserAccount
        {
            [J("I")]
            public string ID { get; set; }

            [J("N")]
            public string Name { get; set; }

            [J("S")]
            public Payloads.Status Status { get; set; }

            [J("P")]
            public Payloads.Platform Platform { get; set; }

            [J("R")]
            public Payloads.Region Region { get; set; }
        }

        public class ItemBody
        {
            [J("N")]
            public string Name { get; set; }

            [J("U")]
            public string UrlName { get; set; }
        }
    }
}
