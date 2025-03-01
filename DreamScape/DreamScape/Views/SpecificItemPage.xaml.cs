using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using DreamScape.Model;
using DreamScape.Helpers;
using DreamScape.Data;
using Microsoft.EntityFrameworkCore;
using DreamScape.Helpers;
using Microsoft.UI.Xaml;
namespace DreamScape.Views
{
    public sealed partial class SpecificItemPage : Page
    {
        MainWindow mainWindow;
        int ItemId;

        public SpecificItemPage(MainWindow mainWindow, int id)
        {
            this.InitializeComponent();
            this.mainWindow = mainWindow;
            this.ItemId = id;
            UserHelper userHelper = new UserHelper();
            userHelper.IsUserLoggedIn(MainWindow.LoggedInUser, mainWindow);

            LoadUsersWithSpecificItem();
        }

        private void LoadUsersWithSpecificItem()
        {
            using (var dbContext = new AppDbContext())
            {
                var usersWithItemForTrade = dbContext.Inventories
                    .Include(i => i.User)
                    .Include(i => i.Item)
                    .Where(i => i.ItemId == this.ItemId && i.IsForTrade)
                    .ToList();

                var usersWithItem = dbContext.Inventories
                    .Include(i => i.User)
                    .Include(i => i.Item)
                    .Where(i => i.ItemId == this.ItemId )
                    .ToList();

                UsersListView.ItemsSource = usersWithItemForTrade;
                PlayerCountTextBlock.Text = $"Number of players with this item: {usersWithItem.Count}";
                UserHelper userHelper = new();
                if (!userHelper.IsUserAdmin(MainWindow.LoggedInUser))
                {
                    PlayerCountTextBlock.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void UsersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Inventory clickedInventory)
            {
                mainWindow.ToCreateTrade(clickedInventory.ItemId, clickedInventory.UserId);
            }
        }

    }
}
