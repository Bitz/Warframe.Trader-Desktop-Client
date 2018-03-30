namespace WFTDC
{
    public class PostLoad
    {
        public Item Item { get; set; }

        public long Quantity { get; set; }

        public long? ModRank { get; set; }

        public long Platinum { get; set; }

        public OrderType Type { get; set; }

        public UserAccount User { get; set; }

        public class UserAccount
        {
            public string Name { get; set; }

            public Status Status { get; set; }

            public Platform Platform { get; set; }

            public Region Region { get; set; }
        }
    }
}
