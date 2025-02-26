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
using DreamScape.Helpers;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfilePage : Page
    {
        MainWindow mainWindow;
        public ProfilePage(MainWindow mainWindow)
        {
            this.InitializeComponent();
            this.mainWindow = mainWindow;
            UserHelper userHelper = new UserHelper();
            userHelper.IsUserLoggedIn(MainWindow.LoggedInUser, mainWindow);
            UserNameText.Text = "UserName: " + MainWindow.LoggedInUser.UserName;
            EmailText.Text = "Email: " + MainWindow.LoggedInUser.Email;
        }

        private void Edit_click(object sender, RoutedEventArgs e)
        {
            mainWindow.ToEditProfile();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.LoggedInUser = null;
            mainWindow.ToLogin();
        }
    }
}
