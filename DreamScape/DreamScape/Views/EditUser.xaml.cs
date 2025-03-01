using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DreamScape.Data;
using DreamScape.Helpers;
using DreamScape.Model;

namespace DreamScape.Views
{
    public sealed partial class EditUser : Page
    {
        MainWindow mainWindow;
        private int userId;

        public EditUser(MainWindow mainWindow, int userId)
        {
            this.InitializeComponent();
            this.mainWindow = mainWindow;
            this.userId = userId;
            LoadUserData();
        }

        private void LoadUserData()
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    UsernameTextBox.Text = user.UserName;
                    EmailTextBox.Text = user.Email;
                }
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                ErrorMessageText.Text = "All fields are required.";
                return;
            }

            EmailHelper emailHelper = new EmailHelper();

            if (!emailHelper.IsValidEmail(EmailTextBox.Text))
            {
                ErrorMessageText.Text = "Invalid email address.";
                return;
            }

            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    if (db.Users.Any(u => u.Email == EmailTextBox.Text && u.Id != userId))
                    {
                        ErrorMessageText.Text = "Email is already in use.";
                        return;
                    }

                    user.UserName = UsernameTextBox.Text;
                    user.Email = EmailTextBox.Text;

                    db.SaveChanges();

                    await EditUserDialog.ShowAsync();
                }
            }
        }

        private void Back_click(object sender, RoutedEventArgs e)
        {
            mainWindow.toUsers();
        }

        private void EditUserDialog_CloseButtonClick(object sender, RoutedEventArgs e)
        {
            EditUserDialog.Hide();
            mainWindow.toUsers();
        }
    }
}
