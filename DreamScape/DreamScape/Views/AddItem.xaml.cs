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
using DreamScape.Data;
using DreamScape.Model;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddItem : Page
    {

        MainWindow mainWindow;
        private Item item;
        public AddItem(MainWindow mainWindow)
        {
            this.InitializeComponent();
            this.mainWindow = mainWindow;
            LoadDropdowns();
        }

  

        private void LoadDropdowns()
        {
            using (var db = new AppDbContext())
            {
                TypeComboBox.ItemsSource = db.Types.ToList();
                RarityComboBox.ItemsSource = db.Rarities.ToList();
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PowerTextBox.Text) ||
                string.IsNullOrWhiteSpace(SpeedTextBox.Text) ||
                string.IsNullOrWhiteSpace(DurabilityTextBox.Text) ||
                string.IsNullOrWhiteSpace(MagicTextBox.Text) ||
                TypeComboBox.SelectedValue == null ||
                RarityComboBox.SelectedValue == null)
            {
                // Display an error message to the user
                ErrorMessageText.Text = "All fields are required.";
                return;
            }

            if (!int.TryParse(PowerTextBox.Text, out int power) ||
                !int.TryParse(SpeedTextBox.Text, out int speed) ||
                !int.TryParse(DurabilityTextBox.Text, out int durability) ||
                power < 0 || power > 100 ||
                speed < 0 || speed > 100 ||
                durability < 0 || durability > 100)
            {
                // Display an error message to the user
                ErrorMessageText.Text = "Power, Durability, and Speed must be between 0 and 100.";
                return;
            }

            var newItem = new Item
            {
                Name = NameTextBox.Text,
                Power = power,
                Speed = speed,
                Durability = durability,
                Magic = MagicTextBox.Text,
                TypeId = (int)TypeComboBox.SelectedValue,
                RarityId = (int)RarityComboBox.SelectedValue
            };

            using (var db = new AppDbContext())
            {
                db.Items.Add(newItem);
                db.SaveChanges();
            }

            await AddItemDialog.ShowAsync();
        }


        private void Back_click(object sender, RoutedEventArgs e)
        {
            mainWindow.ToItems();
        }

        private void AddItemDialog_CloseButtonClick(object sender, RoutedEventArgs e)
        {
            // Close the dialog
            AddItemDialog.Hide();

            // Navigate to login page
            mainWindow.ToItems();
        }
    }
}
