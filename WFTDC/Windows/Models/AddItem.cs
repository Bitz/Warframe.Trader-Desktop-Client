using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using WFTDC.Items;

namespace WFTDC.Windows.Models
{
    public class AddItem
    {
        public ObservableCollection<En> ItemList { get; set; }
        public string TestText { get; set; }

        public AddItem()
        {
            ItemList = new ObservableCollection<En>();
        }
    }
}
