using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WFTDC.Items;
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
            MaxHeight = Utils.GetWorkableScreenHeight() * 1.1;
            DataContext = _config;
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
                g.Quantity = $"≥ {i.QuantityMin}";
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

            g.Configitem = i;
            return g;
        }




        private void LvItemsList_OnSelected(object sender, SelectionChangedEventArgs e)
        {
            if (LvItemsList.SelectedIndex != -1)
            {
                LvItemsList.SelectedIndex = -1;
            }
        }

        private void Edit_OnClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem) sender;
            var gridView = (GridItem) item.DataContext;
            IsEnabled = false;
            window = new AddItemWindow(gridView);
            window.Closing += (o, args) =>
            {
                ReloadData();
                IsEnabled = true;
            };
            window.Owner = this;
            window.ShowDialog();
            window.Activate();
            window.Focus();
        }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            var gridView = (GridItem)item.DataContext;
            _config.ContentList.Remove(gridView);
            Global.Configuration.Items.Remove(gridView.Configitem);
            Functions.Config.Save();
        }
        
        private void AddWatcher_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            var window = new AddItemWindow {Owner = this};
            window.Closing += (o, args) =>
            {
                ReloadData();
                IsEnabled = true;
            };
            window.Owner = this;
            window.ShowDialog();
            window.Activate();
            window.Focus();
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Coming Soon", "TODO", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            //Dispose of ui related things being stored in memory.
            System.GC.Collect();
        }
    }
}
