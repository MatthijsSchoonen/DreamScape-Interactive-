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
using DreamScape.Views;
using DreamScape.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public static User? LoggedInUser = null;
        public MainWindow()
        {
            this.InitializeComponent();
            AppDbContext context = new AppDbContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            ToLogin();
        }

        public void ToLogin()
        {
            UserNav.Visibility = Visibility.Collapsed;
            TopBar.Visibility = Visibility.Collapsed;
            Frame.Content = new LoginPage(this);
        }

        public void ToRegister()
        {
            TopBar.Visibility = Visibility.Collapsed;
            Frame.Content = new Register(this);
        }

        public void ToPasswordReset()
        {
            TopBar.Visibility = Visibility.Collapsed;
            Frame.Content = new PasswordResetPage(this);
        }


        public void ToInventory()
        {
            UserHelper userHelper = new UserHelper();
            if (userHelper.IsUserAdmin(LoggedInUser)) { 
                UserNav.Visibility= Visibility.Visible;
            }
            TopBar.Visibility = Visibility.Visible;
            Frame.Content = new InventoryPage(this);
        }

        public void ToProFile()
        {
            TopBar.Visibility = Visibility.Visible;
            Frame.Content = new ProfilePage(this);
        }

        public void ToEditProfile()
        {
            TopBar.Visibility = Visibility.Visible;
            Frame.Content = new EditProfilePage(this);
        }

        public void ToItems()
        {
            TopBar.Visibility = Visibility.Visible;
            Frame.Content = new ItemPage(this);
        }

        public void ToEditItems(int id)
        {
            TopBar.Visibility = Visibility.Visible;
            Frame.Content = new EditItem(this, id);
        }

        public void toAddItem()
        {
            TopBar.Visibility = Visibility.Visible;
            Frame.Content = new AddItem(this);
        }

        public void ToAssignItem(int id)
        {
            TopBar.Visibility = Visibility.Visible;
            Frame.Content = new AsignItemPage(this, id);
        }

        public void ToSpecificItem(int id)
        {
            TopBar.Visibility = Visibility.Visible;
            Frame.Content = new SpecificItemPage(this, id);
        }

        public void ToCreateTrade(int itemId, int userId)
        {
            Frame.Content = new CreateTradePage(this, itemId, userId);
        }

        public void ToTrades()
        {
            Frame.Content = new TradesPage(this);
        }
        
        public void toUsers()
        {
            Frame.Content = new UserPage(this);
        }

        public void ToCreateUser()
        {
            Frame.Content = new CreateUser(this);
        }

        public void ToEditUser(int id)
        {
            Frame.Content = new EditUser(this, id);
        }


        private void HeaderNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // Switch case for navigating between the pages
            switch (args.SelectedItemContainer.Content.ToString())
            {
                case "Inventory":
                    ToInventory();
                    break;
                case "Items":
                    ToItems();
                    break;
                case "Trades":
                    ToTrades();
                    break;
                case "Users":
                    toUsers();
                    break;
                default:
                    ToLogin();
                    break;
            }
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            ToProFile();
        }


    }
}
