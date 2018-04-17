using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WFTDC.Windows.Models
{
    public class ConfigPage 
    {
        private ObservableCollection<GridItem> _contentList;

        public ObservableCollection<GridItem> ContentList
        {
            get { return _contentList; }
            set { _contentList = value; }
        }

        public ConfigPage()
        {
            _contentList = new ObservableCollection<GridItem>();
        }
    }

    public class GridItem : INotifyPropertyChanged
    {
        private OrderType _type;
        private string _orderText;
        private string _orderForeground;
        private string _orderBackground;
        private bool _enabled;
        private string _image;
        private string _name;
        private string _rank;
        private string _quantity;
        private int _price;
        private C.Item _configItem;


        public C.Item Configitem
        {
            get { return _configItem; }
            set { _configItem = value; OnPropertyChanged(); }
        }

        public OrderType Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged(); }
        }

        public string OrderText
        {
            get { return _orderText; }
            set { _orderText = value; OnPropertyChanged(); }
        }

        public string OrderForeground
        {
            get { return _orderForeground; }
            set { _orderForeground = value; OnPropertyChanged(); }
        }

        public string OrderBackground
        {
            get { return _orderBackground; }
            set { _orderBackground = value; OnPropertyChanged(); }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; OnPropertyChanged(); }
        }

        public string Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        public string Rank
        {
            get { return _rank; }
            set { _rank = value; OnPropertyChanged(); }
        }

        public string Quantity
        {
            get { return _quantity; }
            set { _quantity = value; OnPropertyChanged(); }
        }

        public int Price
        {
            get { return _price; }
            set { _price = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
