using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DreamScape.Model;
using DreamScape.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace DreamScape.Views
{
    public sealed partial class TradesPage : Page
    {
        MainWindow mainWindow;

        public TradesPage(MainWindow mainWindow)
        {
            this.InitializeComponent();
            this.mainWindow = mainWindow;
            LoadTrades();
            LoadStatuses();
        }

        private void LoadTrades()
        {
            using (var dbContext = new AppDbContext())
            {
                var trades = dbContext.Trades
                    .Include(t => t.Seller)
                    .Include(t => t.Buyer)
                    .Include(t => t.status)
                    .ToList();

                // Retrieve all item IDs
                var sellItemIds = trades.SelectMany(t => t.SellItemId).ToList();
                var tradeItemIds = trades.SelectMany(t => t.TradeItemId).ToList();
                var allItemIds = sellItemIds.Concat(tradeItemIds).Distinct().ToList();

                // Retrieve items by IDs
                var items = dbContext.Items
                    .Include(i => i.Type)
                    .Include(i => i.Rarity)
                    .Where(i => allItemIds.Contains(i.Id))
                    .ToList();

                // Populate SellItem and TradeItem collections
                foreach (var trade in trades)
                {
                    trade.SellItem = new ObservableCollection<Item>(items.Where(i => trade.SellItemId.Contains(i.Id)));
                    trade.TradeItem = new ObservableCollection<Item>(items.Where(i => trade.TradeItemId.Contains(i.Id)));
                }

                TradesListView.ItemsSource = trades;
            }
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedStatus = StatusFilterComboBox.SelectedItem as Status;
            if (selectedStatus != null)
            {
                using (var dbContext = new AppDbContext())
                {
                    var trades = dbContext.Trades
                        .Include(t => t.Seller)
                        .Include(t => t.Buyer)
                        .Include(t => t.status)
                        .Where(t => (t.SellerId == MainWindow.LoggedInUser.Id || t.BuyerId == MainWindow.LoggedInUser.Id) && t.StatusId == selectedStatus.Id)
                        .ToList();

                    // Retrieve all item IDs
                    var sellItemIds = trades.SelectMany(t => t.SellItemId).ToList();
                    var tradeItemIds = trades.SelectMany(t => t.TradeItemId).ToList();
                    var allItemIds = sellItemIds.Concat(tradeItemIds).Distinct().ToList();

                    // Retrieve items by IDs
                    var items = dbContext.Items
                        .Include(i => i.Type)
                        .Include(i => i.Rarity)
                        .Where(i => allItemIds.Contains(i.Id))
                        .ToList();

                    // Populate SellItem and TradeItem collections
                    foreach (var trade in trades)
                    {
                        trade.SellItem = new ObservableCollection<Item>(items.Where(i => trade.SellItemId.Contains(i.Id)));
                        trade.TradeItem = new ObservableCollection<Item>(items.Where(i => trade.TradeItemId.Contains(i.Id)));
                    }

                    TradesListView.ItemsSource = trades;
                }
            }
        }



        private void LoadStatuses()
        {
            using (var dbContext = new AppDbContext())
            {
                var statuses = dbContext.Statuses.ToList();
                StatusFilterComboBox.ItemsSource = statuses;
            }
        }



        private void DeleteTrade_Click(object sender, RoutedEventArgs e)
        {
            // Only continue if logged in user == buyer
            var button = sender as Button;
            var trade = button.DataContext as Trade;

            if (trade != null && trade.BuyerId == MainWindow.LoggedInUser.Id)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.Trades.Remove(trade);
                    dbContext.SaveChanges();
                }

                LoadTrades();
            }
        }

        private void AcceptTrade_Click(object sender, RoutedEventArgs e)
        {
            // Only continue if logged in user == seller and trade status is pending
            var button = sender as Button;
            var trade = button.DataContext as Trade;

            if (trade != null && trade.SellerId == MainWindow.LoggedInUser.Id && trade.StatusId == 1) // Assuming 1 is the pending status
            {
                using (var dbContext = new AppDbContext())
                {
                    // Remove sell items from seller's inventory and add trade items to seller's inventory
                    foreach (var sellItemId in trade.SellItemId)
                    {
                        var sellerInventoryItem = dbContext.Inventories.FirstOrDefault(i => i.UserId == trade.SellerId && i.ItemId == sellItemId);
                        if (sellerInventoryItem != null)
                        {
                            dbContext.Inventories.Remove(sellerInventoryItem);
                        }
                    }

                    foreach (var tradeItemId in trade.TradeItemId)
                    {
                        var sellerInventoryItem = dbContext.Inventories.FirstOrDefault(i => i.UserId == trade.SellerId && i.ItemId == tradeItemId);
                        if (sellerInventoryItem == null)
                        {
                            dbContext.Inventories.Add(new Inventory { UserId = trade.SellerId, ItemId = tradeItemId, Count = 1, IsForTrade = false });
                        }
                        else
                        {
                            sellerInventoryItem.Count += 1;
                        }
                    }

                    // Remove trade items from buyer's inventory and add sell items to buyer's inventory
                    foreach (var tradeItemId in trade.TradeItemId)
                    {
                        var buyerInventoryItem = dbContext.Inventories.FirstOrDefault(i => i.UserId == trade.BuyerId && i.ItemId == tradeItemId);
                        if (buyerInventoryItem != null)
                        {
                            dbContext.Inventories.Remove(buyerInventoryItem);
                        }
                    }

                    foreach (var sellItemId in trade.SellItemId)
                    {
                        var buyerInventoryItem = dbContext.Inventories.FirstOrDefault(i => i.UserId == trade.BuyerId && i.ItemId == sellItemId);
                        if (buyerInventoryItem == null)
                        {
                            dbContext.Inventories.Add(new Inventory { UserId = trade.BuyerId, ItemId = sellItemId, Count = 1, IsForTrade = false });
                        }
                        else
                        {
                            buyerInventoryItem.Count += 1;
                        }
                    }

                    // Update trade status
                    dbContext.Trades.Attach(trade);
                    trade.StatusId = 3;
                    dbContext.Entry(trade).Property(t => t.StatusId).IsModified = true;
                    dbContext.SaveChanges();
                }

                LoadTrades();
            }
        }

        private void DeclineTrade_Click(object sender, RoutedEventArgs e)
        {
            // Only continue if logged in user == seller and trade status is pending
            var button = sender as Button;
            var trade = button.DataContext as Trade;

            if (trade != null && trade.SellerId == MainWindow.LoggedInUser.Id && trade.StatusId == 1) // Assuming 1 is the pending status
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.Trades.Attach(trade);
                    trade.StatusId = 2;
                    dbContext.Entry(trade).Property(t => t.StatusId).IsModified = true;
                    dbContext.SaveChanges();
                }

                LoadTrades();
            }
        }
    }
}