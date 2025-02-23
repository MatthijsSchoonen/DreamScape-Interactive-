using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using DreamScape.Data;
using DreamScape.Model;

namespace DreamScape.Views
{
    public sealed partial class InventoryPage : Page
    {
        private List<Inventory> allItems;
        private List<Inventory> filteredItems;
        MainWindow mainWindow;

        public InventoryPage(MainWindow mainWindow)
        {
            this.InitializeComponent();
            this.mainWindow = mainWindow;
            LoadItems();
        }

        private void LoadItems()
        {
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    // Load items from the database for the logged-in user
                    int id = MainWindow.LoggedInUser.Id;
                    allItems = db.Inventories
                                 .Where(inventory => inventory.UserId == id)
                                 .Include(inventory => inventory.Item)
                                 .ThenInclude(item => item.Type)
                                 .Include(inventory => inventory.Item)
                                 .ThenInclude(item => item.Rarity)
                                 .ToList();

                    filteredItems = new List<Inventory>(allItems);
                    InventoryListView.ItemsSource = filteredItems;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error, show a message to the user)
                Console.WriteLine($"Error loading items: {ex.Message}");
            }
        }


        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string sort = (SortComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            switch (sort)
            {
                case "Name":
                    filteredItems = filteredItems.OrderBy(inventory => inventory.Item.Name).ToList();
                    break;
                case "Type":
                    filteredItems = filteredItems.OrderBy(inventory => inventory.Item.Type.Name).ToList();
                    break;
                case "Stats":
                    filteredItems = filteredItems.OrderBy(inventory => inventory.Item.Power + inventory.Item.Speed + inventory.Item.Durability).ToList();
                    break;
            }
            InventoryListView.ItemsSource = filteredItems;
        }
        private void RarityFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string rarityFilter = (RarityFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string typeFilter = (FilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string rarityFilter = (RarityFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            filteredItems = allItems;

            if (typeFilter != "All")
            {
                filteredItems = filteredItems.Where(inventory => inventory.Item.Type.Name == typeFilter).ToList();
            }

            if (rarityFilter != "All")
            {
                filteredItems = filteredItems.Where(inventory => inventory.Item.Rarity.Name == rarityFilter).ToList();
            }

            InventoryListView.ItemsSource = filteredItems;
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ToggleTrade_Click(object sender, RoutedEventArgs e)
        {
            Inventory selectedInventory = (sender as Button).DataContext as Inventory;
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    var inventoryItem = db.Inventories
                                          .Include(i => i.Item)
                                          .FirstOrDefault(i => i.Id == selectedInventory.Id && i.UserId == MainWindow.LoggedInUser.Id);
                    if (inventoryItem != null)
                    {
                        inventoryItem.IsForTrade = !inventoryItem.IsForTrade;
                        db.SaveChanges();
                        LoadItems(); // Reload items to reflect the change
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error, show a message to the user)
                Console.WriteLine($"Error toggling trade status: {ex.Message}");
            }
        }
    }
}
