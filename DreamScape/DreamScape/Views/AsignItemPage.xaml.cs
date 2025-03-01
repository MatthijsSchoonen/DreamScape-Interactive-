using System;
using System.Collections.Generic;
using System.Linq;
using DreamScape.Data;
using DreamScape.Model;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.EntityFrameworkCore;
using DreamScape.Helpers;

namespace DreamScape.Views
{
    public sealed partial class AsignItemPage : Page
    {
        MainWindow mainWindow;
        private int ItemId;

        public AsignItemPage(MainWindow mainWindow, int id)
        {
            this.InitializeComponent();
            this.mainWindow = mainWindow;
            UserHelper userHelper = new UserHelper();
            userHelper.IsUserLoggedIn(MainWindow.LoggedInUser, mainWindow);
            this.ItemId = id;
            LoadItemName();
            LoadUsers();
        }

        private async void LoadItemName()
        {
            using (var context = new AppDbContext())
            {
                var item = await context.Items.FindAsync(ItemId);
                if (item != null)
                {
                    ItemNameTextBlock.Text = $"Assigning Item: {item.Name}";
                }
            }
        }

        private async void LoadUsers()
        {
            using (var context = new AppDbContext())
            {
                var users = await context.Users.ToListAsync();
                UserComboBox.ItemsSource = users;
                UserComboBox.DisplayMemberPath = "UserName";
                UserComboBox.SelectedValuePath = "Id";
            }
        }

        private async void AssignItemButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessageText.Text = "";
            var selectedUser = (User)UserComboBox.SelectedItem;
            if (selectedUser != null)
            {
                using (var context = new AppDbContext())
                {
                    var inventory = new Inventory
                    {
                        UserId = selectedUser.Id,
                        ItemId = this.ItemId,
                        Count = 1,
                        IsForTrade = false
                    };

                    context.Inventories.Add(inventory);
                    await context.SaveChangesAsync();

                    await AsignItemDialog.ShowAsync();
                }
            }
            else
            {
                ErrorMessageText.Text = "No User Selected";
            }
        }

        private void Back_click(object sender, RoutedEventArgs e)
        {
            mainWindow.ToItems();
        }

        private void AsignItemDialog_CloseButtonClick(object sender, RoutedEventArgs e)
        {
            // Close the dialog
            AsignItemDialog.Hide();

            // Navigate to login page
            mainWindow.ToItems();
        }
    }
}
