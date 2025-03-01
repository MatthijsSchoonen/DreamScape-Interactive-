using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using DreamScape.Data;
using DreamScape.Model;
using DreamScape.Helpers;

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
            LoadDropdowns();
            LoadItems();
            UserHelper userHelper = new UserHelper();
            userHelper.IsUserLoggedIn(MainWindow.LoggedInUser, mainWindow);
        }

     

        private void LoadDropdowns()
        {
            using (var db = new AppDbContext())
            {
                var types = db.Types.ToList();
                var rarities = db.Rarities.ToList();

                FilterComboBox.ItemsSource = new List<string> { "All" }.Concat(types.Select(t => t.Name)).ToList();
                RarityFilterComboBox.ItemsSource = new List<string> { "All" }.Concat(rarities.Select(r => r.Name)).ToList();
            }
        }

        private void LoadItems()
        {
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
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
            ApplyFilters();
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void NameFilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string typeFilter = FilterComboBox.SelectedItem as string;
            string rarityFilter = RarityFilterComboBox.SelectedItem as string;
            string nameFilter = NameFilterTextBox.Text;

            filteredItems = allItems;

            if (!string.IsNullOrEmpty(nameFilter))
            {
                filteredItems = filteredItems.Where(inventory => inventory.Item.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (typeFilter != "All" && !string.IsNullOrEmpty(typeFilter))
            {
                filteredItems = filteredItems.Where(inventory => inventory.Item.Type.Name == typeFilter).ToList();
            }

            if (rarityFilter != "All" && !string.IsNullOrEmpty(rarityFilter))
            {
                filteredItems = filteredItems.Where(inventory => inventory.Item.Rarity.Name == rarityFilter).ToList();
            }

            InventoryListView.ItemsSource = filteredItems;
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
                Console.WriteLine($"Error toggling trade status: {ex.Message}");
            }
        }
    }
}
