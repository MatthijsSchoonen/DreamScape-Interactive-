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
        public static User LoggedInUser = new User();
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
            Frame.Content = new LoginPage(this);
        }

        public void ToRegister()
        {
            Frame.Content = new Register(this);
        }

    }
}
