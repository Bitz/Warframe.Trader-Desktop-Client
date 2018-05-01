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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MaterialDesignThemes.Wpf;
using WFTDC.Items;
using WFTDC.Windows.Models;
using WFTDC.Payloads;

namespace WFTDC.Windows
{
    /// <summary>
    /// Interaction logic for AddItemWindow.xaml
    /// </summary>
    public partial class AddItemWindow
    {
        readonly AddItem _data = new AddItem();
        List<En> _listOfItems;
        FNA.FilePair _selectedItem;
        private readonly C.Item _importedConfig;

        public AddItemWindow()
        {
            AlwaysLoad();
        }

        private void AlwaysLoad()
        {
            _listOfItems = Functions.Data.GetItemsDatabase();
            DataContext = _data;
            InitializeComponent();
            CB_BuySellSelector.SelectedIndex = 0;
            CB_QuantityType.SelectedIndex = 0;
            PopulateWithDefaults();
        }

        public AddItemWindow(GridItem item)
        {
            AlwaysLoad();

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
                    if (_data.ItemList.All(x => x.UrlName != item.UrlName))
                    {
                        _data.ItemList.Add(item);
                    }
                }
            }
        }

        private void Search_ForItem_MouseUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ItemTextbox.Text))
            {
                if (ItemTextbox.Text.Length > 1)
                {
                    int maxDisplay = 6;
                    foreach (var item in _listOfItems.OrderBy(x => Utils.LevenshteinDistance(x.Name, ItemTextbox.Text)))
                    {
                        if (item.Name.ToLowerInvariant().Contains(ItemTextbox.Text.ToLowerInvariant()))
                        {
                            if (_data.ItemList.All(w => w != item) && _data.ItemList.Count < maxDisplay)
                            {
                                _data.ItemList.Add(item);
                            }
                        }
                        else
                        {
                            _data.ItemList.Remove(item);
                        }
                    }
                }
            }
            else
            {
                _data.ItemList.Clear();
                PopulateWithDefaults();
            }

            if (_data.ItemList.Count > 0)
            {
                ItemTextbox.IsDropDownOpen = true;
            }
        }

        private void ItemTextbox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text))
            {
            }
        }


        private void ItemTextbox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox) sender;
            if (cb != null)
            {
                var thisSelection = (En) cb.SelectedItem;
                if (thisSelection != null)
                {
                    BackgroundWorker bw = new BackgroundWorker
                    {
                        WorkerReportsProgress = true
                    };

                    bw.DoWork += delegate { _selectedItem = FNA.GetFilePair(thisSelection.UrlName); };
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
                        Image_Item.Source = new BitmapImage(new Uri(Path.Combine(Functions.PathToTemp(),
                            Path.GetFileName(_selectedItem.FileName))));
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
                String text = (String) e.DataObject.GetData(typeof(String));
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

        private async void TrySave_Click(object sender, RoutedEventArgs e)
        {
            List<string> issues = new List<string>();
            if (ItemTextbox.SelectedIndex == -1)
            {
                issues.Add("Select an item.");
            }
            if (_selectedItem != null && _selectedItem.ItemHasRanks)
            {
                if (string.IsNullOrEmpty(Text_RankMin.Text))
                {
                    issues.Add("Include a minimum rank you would like to search for.");
                }

                if (string.IsNullOrEmpty(Text_RankMax.Text))
                {
                    issues.Add("Include a maximum rank you would like to search for.");
                }
            }
            if (string.IsNullOrEmpty(Text_Price.Text))
            {
                issues.Add("Include a price.");
            }
            switch ((CB_QuantityType.SelectedItem as ComboBoxItem).Content.ToString())
            {
                case "Any":
                    break;
                case "At least":
                    if (string.IsNullOrEmpty(Quantity_Selector_Single.Text))
                    {
                        issues.Add("Include a minimum quantity to search for.");
                    }
                    break;
                case "At most":
                    if (string.IsNullOrEmpty(Quantity_Selector_Single.Text))
                    {
                        issues.Add("Include a maximum quantity to search for.");
                    }
                    break;
                case "Between":
                    if (string.IsNullOrEmpty(Quantity_Selector_Single.Text))
                    {
                        issues.Add("Include a maximum quantity to search for.");
                    }
                    if (string.IsNullOrEmpty(Quantity_Selector_Single.Text))
                    {
                        issues.Add("Include a minimum quantity to search for.");
                    }
                    break;
            }

            if (issues.Count != 0)
            {
                StackPanel stackPanel = new StackPanel {Margin = new Thickness(4, 4, 4, 4)};

                Button b = new Button
                {
                    Command = DialogHost.CloseDialogCommand,
                    Content = "Close"
                };
                TextBlock te = new TextBlock
                {
                    Text = "Please fix the following issues:",
                    Padding = new Thickness(20, 20, 20, 10),
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.IndianRed
                };
                stackPanel.Children.Add(te);
                foreach (var issue in issues)
                {
                    TextBlock t = new TextBlock
                    {
                        Text = issue,
                        Padding = new Thickness(20, 10, 20, 10)
                    };
                    stackPanel.Children.Add(t);
                }

                stackPanel.Children.Add(b);
                await DialogHost.Show(stackPanel);
            }
            else
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
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            CB_BuySellSelector.SelectedIndex = 0;
            ItemTextbox.SelectedIndex = -1;
            Text_Price.Text = string.Empty;
            CB_QuantityType.SelectedIndex = 0;
            Quantity_Selector_Single.Text = string.Empty;
            Text_QuantityMin.Text = string.Empty;
            Text_QuantityMax.Text = string.Empty;
            Text_RankMax.Text = string.Empty;
            Text_RankMin.Text = string.Empty;
            Image_Item.MouseUp -= ClickImage;
            Image_Item.Source = new BitmapImage();

        }
    }
}