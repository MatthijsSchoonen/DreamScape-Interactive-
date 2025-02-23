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
using Windows.Foundation.Diagnostics;
using DreamScape.Helpers;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private MainWindow mainWindow;
        private int failedLoginAttempts = 0;
        private const int MaxFailedLoginAttempts = 3;

        public LoginPage(MainWindow mainWindow)
        {
            this.InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = "";
            string email = EmailText.Text;
            string password = PasswordText.Password;

            AppDbContext db = new AppDbContext();
            User user = db.Users.FirstOrDefault(u => u.Email == email.ToLower());
            //validate password
            if (user != null && SecureHasher.Verify(password, user.Password))
            {
                MainWindow.LoggedInUser = user;
                ErrorMessage.Text = "Login Successful.";
                failedLoginAttempts = 0; // Reset the counter on successful login
                mainWindow.ToInventory();
                return;
            }

            if (user == null)
            {
                ErrorMessage.Text = "Invalid Email or password.";
                return;
            }

            failedLoginAttempts++;
            if (failedLoginAttempts >= MaxFailedLoginAttempts)
            {
                LoginButton.IsEnabled = false;
                ErrorMessage.Text = "Too many failed login attempts. Please try again later.";


                //To send email to user that someone is trying to login to your account
                EmailHelper emailHelper = new EmailHelper();
                await emailHelper.SendWarningEmail(user.Email);
            }
            else
            {
                ErrorMessage.Text = "Invalid Email or password.";
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ToRegister();
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ToPasswordReset();
        }
    }
}
