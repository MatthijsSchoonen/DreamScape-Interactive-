using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DreamScape.Data;
using DreamScape.Helpers;
using DreamScape.Model;

namespace DreamScape.Views
{
    public sealed partial class CreateUser : Page
    {
        MainWindow mainWindow;

        public CreateUser(MainWindow mainWindow)
        {
            this.InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private async void Create_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) 
                )
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
                if (db.Users.Any(u => u.Email == EmailTextBox.Text))
                {
                    ErrorMessageText.Text = "Email is already in use.";
                    return;
                }

                string hashedPassword = SecureHasher.Hash(Guid.NewGuid().ToString());


                var newUser = new User
                {
                    UserName = UsernameTextBox.Text,
                    Email = EmailTextBox.Text,
                    Password = hashedPassword,
                    RoleId = 2
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                await emailHelper.ConfirmAccountCreation(newUser.Email);

                await CreateUserDialog.ShowAsync();
            }
        }

        private void Back_click(object sender, RoutedEventArgs e)
        {
            mainWindow.ToLogin();
        }

        private void CreateUserDialog_CloseButtonClick(object sender, RoutedEventArgs e)
        {
            CreateUserDialog.Hide();
            mainWindow.toUsers();
        }
    }
}
