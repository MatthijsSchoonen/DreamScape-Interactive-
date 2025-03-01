using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DreamScape.Model;
using DreamScape.Data;
using Microsoft.EntityFrameworkCore;

namespace DreamScape.Views
{
    public sealed partial class UserPage : Page
    {
        private List<User> allUsers;
        MainWindow mainWindow;

        public UserPage(MainWindow mainWindow)
        {
            this.InitializeComponent();
            this.mainWindow = mainWindow;
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    allUsers = db.Users
                                 .Include(user => user.Role)
                                 .ToList();

                    UserListView.ItemsSource = allUsers;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading users: {ex.Message}");
            }
        }

        private void ToAddUser_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ToCreateUser();
        }

        private void ToEdit_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            User user = button.DataContext as User;

            if (user != null)
            {
                int userId = user.Id;
                mainWindow.ToEditUser(userId);
            }
        }

        private void ToDelete_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            User user = button.DataContext as User;

            if (user != null)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.Users.Remove(user);
                    dbContext.SaveChanges();
                }

                LoadUsers();
            }
        }
    }
}
