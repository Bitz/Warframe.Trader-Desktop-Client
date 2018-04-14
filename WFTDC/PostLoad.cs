namespace WFTDC
{
    public class PostLoad
    {
        public ItemBody Item { get; set; }

        public long Quantity { get; set; }

        public long? ModRank { get; set; }

        public long Platinum { get; set; }

        public OrderType Type { get; set; }

        public UserAccount User { get; set; }

        public class UserAccount
        {
            public string ID { get; set; }

            public string Name { get; set; }

            public Status Status { get; set; }

            public Platform Platform { get; set; }

            public Region Region { get; set; }
        }

        public class ItemBody
        {
            public string Name { get; set; }

            public string UrlName { get; set; }
        }
    }
}
