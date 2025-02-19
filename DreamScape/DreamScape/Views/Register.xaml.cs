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
using DreamScape.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Register : Page
    {
        MainWindow mainWindow;
        public Register(MainWindow mainWindow)
        {
            this.InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void NavigateToLogin_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ToLogin();
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = "";
            string userName = UserNameText.Text;
            string email = EmailText.Text.ToLower();
            string password = PasswordText.Password;
            string confirmPassWord = PasswordConfirmText.Password;

            // Validate fields
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassWord))
            {
                ErrorMessage.Text = "All fields are required.";
                return;
            }

            if (!IsValidEmail(email))
            {
                ErrorMessage.Text = "Invalid email format.";
                return;
            }

            if (password != confirmPassWord)
            {
                ErrorMessage.Text = "Passwords do not match.";
                return;
            }

            if (!IsValidPassword(password))
            {
                ErrorMessage.Text = "Password must be at least 8 characters long, contain at least one capital letter, and one number.";
                return;
            }

            // Check if username/email already exists in the database
            AppDbContext db = new AppDbContext();
            User existingUser = db.Users.FirstOrDefault(u => u.UserName == userName);
            if (existingUser != null)
            {
                ErrorMessage.Text = "Username already is taken.";
                return;
            }

            existingUser = db.Users.FirstOrDefault(u => u.Email == email);
            if (existingUser != null)
            {
                ErrorMessage.Text = "Email already is taken.";
                return;
            }

            // Create new user
            User newUser = new User
            {
                UserName = userName,
                Email = email,
                Password = SecureHasher.Hash(password),
                RoleId = 2
            };

            db.Users.Add(newUser);
            await db.SaveChangesAsync();

            // Send mail to user if account created successfully
            EmailHelper emailHelper = new EmailHelper();
            await emailHelper.ConfirmAccountCreation(email);

            // Show account created dialog
            await AccountCreatedDialog.ShowAsync();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPassword(string password)
        {
            if (password.Length < 8)
                return false;

            if (!password.Any(char.IsUpper))
                return false;

            if (!password.Any(char.IsDigit))
                return false;

            return true;
        }

        private void AccountCreatedDialog_CloseButtonClick(object sender, RoutedEventArgs e)
        {
            // Close the dialog
            AccountCreatedDialog.Hide();

            // Navigate to login page
            mainWindow.ToLogin();
        }


    }
}
