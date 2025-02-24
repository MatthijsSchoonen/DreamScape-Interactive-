using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DreamScape.Data;
using DreamScape.Model;

namespace DreamScape.Views
{
    public sealed partial class EditItem : Page
    {
        MainWindow mainWindow;
        private int itemId;
        private Item item;

        public EditItem(MainWindow mainWindow, int itemId)
        {
            this.InitializeComponent();
            this.mainWindow = mainWindow;
            this.itemId = itemId;
            LoadDropdowns();
            LoadItem();
        }

        private void LoadItem()
        {
            using (var db = new AppDbContext())
            {
                item = db.Items.FirstOrDefault(i => i.Id == itemId);
                if (item != null)
                {
                    NameTextBox.Text = item.Name;
                    TypeComboBox.SelectedValue = item.TypeId;
                    RarityComboBox.SelectedValue = item.RarityId;
                    PowerTextBox.Text = item.Power.ToString();
                    SpeedTextBox.Text = item.Speed.ToString();
                    DurabilityTextBox.Text = item.Durability.ToString();
                    MagicTextBox.Text = item.Magic;
                }
            }
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
                ErrorMessageText.Text = "Power Durability and Speed must be between 0 and 100";
                return;
            }

            var db = new AppDbContext();

                var itemToUpdate = db.Items.FirstOrDefault(i => i.Id == itemId);
                if (itemToUpdate != null)
                {
                    itemToUpdate.Name = NameTextBox.Text;
                    itemToUpdate.Power = power;
                    itemToUpdate.Speed = speed;
                    itemToUpdate.Durability = durability;
                    itemToUpdate.Magic = MagicTextBox.Text;

                    itemToUpdate.TypeId = (int)TypeComboBox.SelectedValue;
                    itemToUpdate.RarityId = (int)RarityComboBox.SelectedValue;

                    db.SaveChanges();
                }

            await UpdateItemDialog.ShowAsync();
        }

        private void Back_click(object sender, RoutedEventArgs e)
        {
            mainWindow.ToItems();
        }

        private void UpdateItemDialog_CloseButtonClick(object sender, RoutedEventArgs e)
        {
            // Close the dialog
            UpdateItemDialog.Hide();

            // Navigate to login page
            mainWindow.ToItems();
        }
    }
}
