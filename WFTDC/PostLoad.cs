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
        public OrderType Type { get; set; }

        [J("U")]
        public UserAccount User { get; set; }

        public class UserAccount
        {
            [J("I")]
            public string ID { get; set; }

            [J("N")]
            public string Name { get; set; }

            [J("S")]
            public Status Status { get; set; }

            [J("P")]
            public Platform Platform { get; set; }

            [J("R")]
            public Region Region { get; set; }
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
