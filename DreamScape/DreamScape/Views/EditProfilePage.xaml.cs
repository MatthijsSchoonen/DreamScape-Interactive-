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
using DreamScape.Helpers;
using DreamScape.Model; 

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditProfilePage : Page
    {
        MainWindow mainWindow;
        public EditProfilePage(MainWindow mainWindow)
        {
            this.InitializeComponent();
            this.mainWindow = mainWindow;
            UserNameText.Text = MainWindow.LoggedInUser.UserName;
            EmailText.Text = MainWindow.LoggedInUser.Email;
        }

        private void Back_click(object sender, RoutedEventArgs e)
        {
            mainWindow.ToProFile();
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = "";
            string userName = UserNameText.Text;
            string email = EmailText.Text.ToLower();
            string password = PasswordText.Password;
            string newPassword = NewPasswordText.Password;
            string confirmPassword = NewPasswordConfirmText.Password;
            EmailHelper emailHelper = new EmailHelper();

            // Validate fields
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email))
            {
                ErrorMessage.Text = "Username and Email fields are required.";
                return;
            }

            if (!emailHelper.IsValidEmail(email))
            {
                ErrorMessage.Text = "Invalid email format.";
                return;
            }

            // Check if username/email already exists in the database
            AppDbContext db = new AppDbContext();
            User existingUser = db.Users.FirstOrDefault(u => u.UserName == userName && u.Id != MainWindow.LoggedInUser.Id);
            if (existingUser != null)
            {
                ErrorMessage.Text = "Username is already taken.";
                return;
            }

            existingUser = db.Users.FirstOrDefault(u => u.Email == email && u.Id != MainWindow.LoggedInUser.Id);
            if (existingUser != null)
            {
                ErrorMessage.Text = "Email is already taken.";
                return;
            }

            // Update user email and username
            User user = db.Users.FirstOrDefault(u => u.Id == MainWindow.LoggedInUser.Id);
            if (user != null)
            {
                user.UserName = userName;
                user.Email = email;

                // If password update
                if (!string.IsNullOrEmpty(newPassword) || !string.IsNullOrEmpty(password) || !string.IsNullOrEmpty(confirmPassword))
                {
                    if (newPassword != confirmPassword)
                    {
                        ErrorMessage.Text = "Passwords do not match.";
                        return;
                    }

                    if (!IsValidPassword(newPassword))
                    {
                        ErrorMessage.Text = "Password must be at least 8 characters long, contain at least one capital letter, and one number.";
                        return;
                    }

                    // Validate current password
                    if (SecureHasher.Verify(password, user.Password))
                    {
                        // Update password
                        user.Password = SecureHasher.Hash(newPassword);
                    }
                    else
                    {
                        ErrorMessage.Text = "Current password is incorrect.";
                        return;
                    }
                }

                db.SaveChanges();
                MainWindow.LoggedInUser = user;
            }

            await UpdateProfileDialog.ShowAsync();
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

        private void UpdateProfileDialog_CloseButtonClick(object sender, RoutedEventArgs e)
        {
            // Close the dialog
            UpdateProfileDialog.Hide();

            // Navigate to login page
            mainWindow.ToProFile();
        }
    }
}
