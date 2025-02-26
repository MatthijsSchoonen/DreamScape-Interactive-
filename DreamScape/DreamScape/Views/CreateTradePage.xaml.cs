using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DreamScape.Model;
using DreamScape.Data;
using System.Linq;
using System.Collections.ObjectModel;

namespace DreamScape.Views
{
    public sealed partial class CreateTradePage : Page
    {
        MainWindow mainWindow;
        int ItemId;
        int sellerId;

        public CreateTradePage(MainWindow mainWindow, int itemId, int sellerId)
        {
            this.InitializeComponent();
            this.mainWindow = mainWindow;
            this.ItemId = itemId;
            this.sellerId = sellerId;
            LoadItems();
        }

        private void LoadItems()
        {
            using (var dbContext = new AppDbContext())
            {
                // Load the trade item
                var tradeItem = dbContext.Items.FirstOrDefault(i => i.Id == this.ItemId);
                if (tradeItem != null)
                {
                    TradeItemTextBlock.Text = tradeItem.Name;
                }

                // Load the logged-in user's inventory items for sell
                var sellItems = dbContext.Inventories
                    .Where(i => i.UserId == MainWindow.LoggedInUser.Id)
                    .Select(i => i.Item)
                    .ToList();
                SellItemsListView.ItemsSource = sellItems;
            }
        }

        private void CreateTrade_Click(object sender, RoutedEventArgs e)
        {
            var selectedSellItems = SellItemsListView.SelectedItems.Cast<Item>().ToList();

            if (selectedSellItems.Any())
            {
                using (var dbContext = new AppDbContext())
                {
                    var trade = new Trade
                    {
                        SellerId = sellerId,
                        BuyerId = MainWindow.LoggedInUser.Id,
                        SellItemId = new ObservableCollection<int>(selectedSellItems.Select(i => i.Id)),
                        TradeItemId = new ObservableCollection<int> { ItemId },
                        StatusId = 1 // Assuming 1 is the status for "Pending"
                    };

                    dbContext.Trades.Add(trade);
                    dbContext.SaveChanges();
                }

                mainWindow.ToTrades();
            }
        }
    }
}
