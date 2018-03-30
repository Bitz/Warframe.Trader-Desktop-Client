namespace WFTDC
{
    using WFMSocketizer;

    public class Global
    {
        public static Configuration Configuration
        {
            get
            {
                return _Configuration ?? (_Configuration = Functions.Config.Load());
            }

            set
            {
                _Configuration = value;
            }
        }

        private static Configuration _Configuration { get; set; }
    }
}
