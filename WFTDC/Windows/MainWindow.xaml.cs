using System.IO;
using System.Windows;
using System.Windows.Controls;
using WFTDC.Windows.Models;

namespace WFTDC.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ConfigPage _config = new ConfigPage();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _config;
            MaxHeight = Utils.GetWorkableScreenHeight() * 1.1;
            ReloadData();
        }

        private void ReloadData()
        {
            _config.ContentList.Clear();
            for (var index = 0; index < Global.Configuration.Items.Count; index++)
            {
                _config.ContentList.Add(LoadFromConfiguration(Global.Configuration.Items[index], index));
            }
        }

        private GridItem LoadFromConfiguration(C.Item i, int index)
        {
            var g = new GridItem
            {
                Name = i.Name,
                Price = i.Price,
                Type = i.Type,
                Enabled = i.Enabled,
                Index = index,
                Image = Path.Combine(Functions.PathToTemp(), Path.GetFileName(FNA.GetFilePair(i.UrlName).FileName))
            };


            if (i.QuantityMin == 0 && i.QuantityMax != 999)
            {
                g.Quantity = $"≤ {i.QuantityMax}";
            }
            else if (i.QuantityMin != 0 && i.QuantityMax == 999)
            {
                g.Quantity = $"≥ {i.QuantityMax}";
            }
            else if (i.QuantityMin == 0 && i.QuantityMax == 999)
            {
                g.Quantity = "ANY";
            }
            else
            {
                g.Quantity = $"{i.QuantityMin} - {i.QuantityMax}";
            }

            if (i.ModRankMin != null && i.ModRankMax != null)
            {
                g.Rank = i.ModRankMax == i.ModRankMin ? $"{i.ModRankMin}" : $"{i.ModRankMin} - {i.ModRankMax}";
            }
            else if (i.ModRankMin != null && i.ModRankMax == null)
            {
                g.Rank = $"{i.ModRankMin} +";
            }
            else if (i.ModRankMin == null && i.ModRankMax != null)
            {
                g.Rank = $"- {i.ModRankMax}";
            }
            else
            {
                g.Rank = "N/A";
            }


            switch (i.Type)
            {
                case OrderType.Buy:
                    g.OrderText = "WTB";
                    g.OrderForeground = Constants.WtbForeground;
                    g.OrderBackground = Constants.WtbBackground;
                    break;
                case OrderType.Sell:
                    g.OrderText = "WTS";
                    g.OrderForeground = Constants.WtsForeground;
                    g.OrderBackground = Constants.WtsBackground;
                    break;
            }
            return g;
        }




        private void LvItemsList_OnSelected(object sender, SelectionChangedEventArgs e)
        {
            if (lvItemsList.SelectedIndex != -1)
            {
                lvItemsList.SelectedIndex = -1;
            }
        }

        private void Edit_OnClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem) sender;
            var gridView = (GridItem) item.DataContext;
            gridView.Name = "AAAAAAAAAAA";
        }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            var gridView = (GridItem)item.DataContext;
            //TODO Actual removal from config.
            _config.ContentList.Remove(gridView);
        }

        private void AddWatcher_Click(object sender, RoutedEventArgs e)
        {
            AddItemWindow window = new AddItemWindow();
            window.ShowDialog();
            window.Activate();
            window.Focus();
            window.Topmost = true;
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            ReloadData();
        }
    }
}
