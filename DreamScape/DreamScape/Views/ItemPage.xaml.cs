using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using DreamScape.Model;
using DreamScape.Data;
using Microsoft.EntityFrameworkCore;
using DreamScape.Helpers;

namespace DreamScape.Views
{
    public sealed partial class ItemPage : Page
    {
        private List<Item> allItems;
        private List<Item> filteredItems;
        MainWindow mainWindow;
        public ItemPage(MainWindow mainWindow)
        {
            this.InitializeComponent();
            this.mainWindow = mainWindow;
            UserHelper userHelper = new UserHelper();
            userHelper.IsUserLoggedIn(MainWindow.LoggedInUser, mainWindow);
            LoadDropdowns();
            LoadItems();

            if (!userHelper.IsUserAdmin(MainWindow.LoggedInUser))
            {
                // Hide add button for non-admin users
                AddNewItemButton.Visibility = Visibility.Collapsed;

                // Handle the ContainerContentChanging event of the ListView to hide the buttons within the ListView items
                InventoryListView.ContainerContentChanging += InventoryListView_ContainerContentChanging;
            }
        }

        private void InventoryListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var container = args.ItemContainer as ListViewItem;
            if (container != null)
            {
                var assignButton = FindControl<Button>(container, "AssignButton");
                var editButton = FindControl<Button>(container, "EditButton");
                if (assignButton != null)
                {
                    assignButton.Visibility = Visibility.Collapsed;
                }
                if (editButton != null)
                {
                    editButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        private T FindControl<T>(DependencyObject parent, string controlName) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T control && ((FrameworkElement)child).Name == controlName)
                {
                    return control;
                }
                var result = FindControl<T>(child, controlName);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
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
                    allItems = db.Items
                                 .Include(item => item.Type)
                                 .Include(item => item.Rarity)
                                 .ToList();

                    filteredItems = new List<Item>(allItems);
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
                    filteredItems = filteredItems.OrderBy(inventory => inventory.Name).ToList();
                    break;
                case "Type":
                    filteredItems = filteredItems.OrderBy(inventory => inventory.Type.Name).ToList();
                    break;
                case "Stats":
                    filteredItems = filteredItems.OrderBy(inventory => inventory.Power + inventory.Speed + inventory.Durability).ToList();
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
                filteredItems = filteredItems.Where(inventory => inventory.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (typeFilter != "All" && !string.IsNullOrEmpty(typeFilter))
            {
                filteredItems = filteredItems.Where(inventory => inventory.Type.Name == typeFilter).ToList();
            }

            if (rarityFilter != "All" && !string.IsNullOrEmpty(rarityFilter))
            {
                filteredItems = filteredItems.Where(inventory => inventory.Rarity.Name == rarityFilter).ToList();
            }

            InventoryListView.ItemsSource = filteredItems;
        }

        private void ToEdit_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Item inventoryItem = button.DataContext as Item;

            if (inventoryItem != null)
            {
                int itemId = inventoryItem.Id;
                mainWindow.ToEditItems(itemId);
            }
        }

        private void ToAddItem_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.toAddItem();
        }

        private void ToAssign_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Item inventoryItem = button.DataContext as Item;

            if (inventoryItem != null)
            {
                int itemId = inventoryItem.Id;
                mainWindow.ToAssignItem(itemId);
            }
        }

        private void InventoryListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Item clickedItem)
            {
                int itemId = clickedItem.Id;
                mainWindow.ToSpecificItem(itemId);
            }
        }

    }
}
