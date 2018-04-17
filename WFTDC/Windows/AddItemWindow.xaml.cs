using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MaterialDesignThemes.Wpf;
using WFTDC.Items;
using WFTDC.Windows.Models;

namespace WFTDC.Windows
{
    /// <summary>
    /// Interaction logic for AddItemWindow.xaml
    /// </summary>
    public partial class AddItemWindow
    {
        readonly AddItem _data = new AddItem();
        readonly List<En> _listOfItems;
        FNA.FilePair _selectedItem;
        private readonly C.Item _importedConfig;

        public AddItemWindow()
        {
            _listOfItems = Constants.ItemDatabase;
            DataContext = _data;
            InitializeComponent();
            CB_BuySellSelector.SelectedIndex = 0;
            CB_QuantityType.SelectedIndex = 0;

            PopulateWithDefaults();
        }

        public AddItemWindow(GridItem item)
        {
            _listOfItems = Constants.ItemDatabase;
            DataContext = _data;
            InitializeComponent();
            CB_BuySellSelector.SelectedIndex = 0;
            CB_QuantityType.SelectedIndex = 0;

            PopulateWithDefaults();

            _importedConfig = item.Configitem;

            LoadFromConfig(item);
        }

        private void LoadFromConfig(GridItem item)
        {
            //Buy/Sell
            CB_BuySellSelector.SelectedIndex = item.Type == OrderType.Buy ? 1 : 0;

            //Item name and rank selector visibility
            En itemName = _listOfItems.FirstOrDefault(c => c.UrlName == item.Configitem.UrlName);
            ItemTextbox.SelectedItem = itemName;

            //Ranks
            if (item.Configitem.ModRankMin.HasValue && item.Configitem.ModRankMax.HasValue)
            {
                Text_RankMin.Text = item.Configitem.ModRankMin.Value.ToString();
                Text_RankMax.Text = item.Configitem.ModRankMax.Value.ToString();
            }

            //Price
            Text_Price.Text = item.Configitem.Price.ToString();

            //Quantity Type
            if (item.Configitem.QuantityMin == 0 && item.Configitem.QuantityMax == 999)
            {
                CB_QuantityType.SelectedIndex = 0; //ANY
            } 
            else if (item.Configitem.QuantityMin != 0 && item.Configitem.QuantityMax == 999)
            {
                CB_QuantityType.SelectedIndex = 1; //At least
                Quantity_Selector_Single.Text = item.Configitem.QuantityMin.ToString();
            }
            else if (item.Configitem.QuantityMin == 0 && item.Configitem.QuantityMax != 999)
            {
                CB_QuantityType.SelectedIndex = 2; //At most
                Quantity_Selector_Single.Text = item.Configitem.QuantityMax.ToString();
            }
            else
            {
                CB_QuantityType.SelectedIndex = 3; //Between\
                Text_QuantityMin.Text = item.Configitem.QuantityMin.ToString();
                Text_QuantityMax.Text = item.Configitem.QuantityMax.ToString();
            }
        }

        private void PopulateWithDefaults()
        {
            foreach (C.Item configurationItem in Global.Configuration.Items)
            {
                var item = _listOfItems.FirstOrDefault(c => c.UrlName == configurationItem.UrlName);
                if (item != null)
                {
                    _data.ItemList.Add(item);
                }
            }
        }

        private void Search_ForItem_MouseUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ItemTextbox.Text))
            {
                int maxDisplay = 5;
                int currentDisplay = 0;
                foreach (var item in _listOfItems)
                {
                    if (item.Name.ToLowerInvariant().Contains(ItemTextbox.Text.ToLowerInvariant()))
                    {
                        if (_data.ItemList.All(w => w != item))
                        {
                            if (currentDisplay < maxDisplay)
                            {
                                _data.ItemList.Add(item);
                                currentDisplay++;
                            }
                        }
                    }
                    else
                    {
                        _data.ItemList.Remove(item);
                    }
                    
                }
            }
            else
            {
                _data.ItemList.Clear();
                PopulateWithDefaults();
            }
        }

        private void ItemTextbox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb != null)
            {
                var thisSelection = cb.SelectedItem as En;
                if (thisSelection != null)
                {
                    BackgroundWorker bw = new BackgroundWorker
                    {
                        WorkerReportsProgress = true
                    };

                    bw.DoWork += delegate
                    {
                        _selectedItem = FNA.GetFilePair(thisSelection.UrlName);
                    };
                    bw.RunWorkerAsync();

                    bw.RunWorkerCompleted += (o, args) =>
                    {
                        if (_selectedItem.ItemHasRanks)
                        {
                            ShowRankSelector();
                        }
                        else
                        {
                            HideRankSelector();
                        }
                        Image_Item.MouseUp -= ClickImage;
                        Image_Item.Source = new BitmapImage(new Uri(Path.Combine(Functions.PathToTemp(), Path.GetFileName(_selectedItem.FileName))));
                        Image_Item.MouseUp += ClickImage;
                    };
                }
            }
        }

        private void ClickImage(object sender, MouseButtonEventArgs e)
        {
             Process.Start($"https://warframe.market/items/{_selectedItem.UrlName}");
        }


        private void HideRankSelector()
        {
            Ranked_Selector.Visibility = Visibility.Collapsed;
            Ranked_Text.Visibility = Visibility.Collapsed;

        }

        private void ShowRankSelector()
        {
            Ranked_Text.Visibility = Visibility.Visible;
            Ranked_Selector.Visibility = Visibility.Visible;
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text)) e.CancelCommand();
            }
            else e.CancelCommand();
        }

        private void BuySellSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string thisSelection = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
            switch (thisSelection)
            {
                case "Buying":
                    Text_CostString.Text = "For more than or equal to...";
                    break;
                case "Selling":
                    Text_CostString.Text = "For less than or equal to...";
                    break;
            }
        }

        private void CB_QuantityType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string thisSelection = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
            Quantity_Selector_Between.Visibility = Visibility.Collapsed;
            Quantity_Selector_Single.Visibility = Visibility.Collapsed;
            
            switch (thisSelection)
            {
                case "Any":
                    break;
                case "At least":
                    Quantity_Selector_Single.Visibility = Visibility.Visible;
                    break;
                case "At most":
                    Quantity_Selector_Single.Visibility = Visibility.Visible;
                    break;
                case "Between":
                    Quantity_Selector_Between.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void TrySave_Click(object sender, RoutedEventArgs e)
        {
            C.Item itemBody = new C.Item
            {
                UrlName = _selectedItem.UrlName,
                Name = ItemTextbox.Text,
                Price = int.Parse(Text_Price.Text),
                Enabled = true
            };

            if (_selectedItem.ItemHasRanks)
            {
                itemBody.ModRankMin = int.Parse(Text_RankMin.Text);
                itemBody.ModRankMax = int.Parse(Text_RankMax.Text);
            }

            switch ((CB_QuantityType.SelectedItem as ComboBoxItem).Content.ToString())
            {
                case "Any":
                    itemBody.QuantityMin = 0;
                    itemBody.QuantityMax = 999;
                    break;
                case "At least":
                    itemBody.QuantityMin = int.Parse(Quantity_Selector_Single.Text);
                    itemBody.QuantityMax = 999;
                    break;
                case "At most":
                    itemBody.QuantityMin = 0;
                    itemBody.QuantityMax = int.Parse(Quantity_Selector_Single.Text);
                    break;
                case "Between":
                    itemBody.QuantityMin = int.Parse(Text_QuantityMin.Text);
                    itemBody.QuantityMax = int.Parse(Text_QuantityMax.Text);
                    break;
            }


            switch ((CB_BuySellSelector.SelectedItem as ComboBoxItem).Content.ToString())
            {
                case "Buying":
                    itemBody.Type = OrderType.Buy;
                    break;
                case "Selling":
                    itemBody.Type = OrderType.Sell;
                    break;
            }
            if (_importedConfig != null)
            {
                int index = Global.Configuration.Items.IndexOf(_importedConfig);
                if (index != -1)
                    Global.Configuration.Items[index] = itemBody;
            }
            else
            {
                Global.Configuration.Items.Add(itemBody);
            }
            Functions.Config.Save();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            StackPanel stackPanel = new StackPanel {Margin = new Thickness(4, 4, 4, 4)};
            TextBlock t = new TextBlock
            {
                Text = "AAAAAAAAAA",
                Padding = new Thickness(20, 20, 20, 20)
            };
            Button b = new Button
            {
                Command = DialogHost.CloseDialogCommand,
                Content = "Close"
            };
            stackPanel.Children.Add(t);
            stackPanel.Children.Add(b);
            await DialogHost.Show(stackPanel);
        }

    }
}