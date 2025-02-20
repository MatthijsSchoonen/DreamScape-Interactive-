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
using DreamScape.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PasswordResetPage : Page
    {
        MainWindow mainWindow;
        public PasswordResetPage(MainWindow mainWindow)
        {
            this.InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private async void SendEmail_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = "";
            string email = EmailText.Text;
            EmailHelper emailHelper = new EmailHelper();

            if (string.IsNullOrEmpty(email) || !emailHelper.IsValidEmail(email))
            {
                ErrorMessage.Text = "Not a valid Email";
                return;
            }

            using (AppDbContext db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    ErrorMessage.Text = "Email does not exist";
                    return;
                }

                // Generate a random code
                string resetCode = Guid.NewGuid().ToString();

                // Save the reset code to the database
                PasswordReset passwordReset = new PasswordReset
                {
                    UserId = user.Id,
                    Code = resetCode
                };
                db.PasswordResets.Add(passwordReset);
                db.SaveChanges();

                // Send the reset code to the user's email
                await emailHelper.SendPasswordReset(email, resetCode);

                ErrorMessage.Text = "Password reset email sent";
                EmailPanel.Visibility = Visibility.Collapsed;
                ValidateCode.Visibility = Visibility.Visible;
            }
        }


        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ToLogin();
        }

        private void VerifyCode_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage1.Text = "";
            string enteredCode = code.Text;

            using (AppDbContext db = new AppDbContext())
            {
                var passwordReset = db.PasswordResets.FirstOrDefault(pr => pr.Code == enteredCode);
                if (passwordReset == null)
                {
                    ErrorMessage1.Text = "Invalid code";
                    return;
                }

                // Code is valid, show the reset password panel
                ValidateCode.Visibility = Visibility.Collapsed;
                ResetPassword.Visibility = Visibility.Visible;
            }
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage2.Text = "";
            string newPassword = password.Password;
            string confirmNewPassword = confirmPassword.Password;

            if (newPassword != confirmNewPassword)
            {
                ErrorMessage.Text = "Passwords do not match";
                return;
            }

            AppDbContext db = new AppDbContext();

            var passwordReset = db.PasswordResets.FirstOrDefault(pr => pr.Code == code.Text);
            if (passwordReset == null)
            {
                ErrorMessage2.Text = "Invalid code";
                return;
            }

            if (!IsValidPassword(newPassword))
            {
                ErrorMessage2.Text = "Password must be at least 8 characters long, contain at least one capital letter, and one number.";
                return;
            }

            var user = db.Users.FirstOrDefault(u => u.Id == passwordReset.UserId);
            if (user == null)
            {
                ErrorMessage2.Text = "User not found";
                return;
            }

            // Update the user's password
            user.Password = SecureHasher.Hash(newPassword);
            db.Users.Update(user);
            db.PasswordResets.Remove(passwordReset);
            db.SaveChanges();

            ErrorMessage2.Text = "Password has been reset";
            mainWindow.ToLogin();

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
    }
}
