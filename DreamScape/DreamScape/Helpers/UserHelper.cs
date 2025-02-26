using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DreamScape.Model;

namespace DreamScape.Helpers
{
    class UserHelper
    {
        public  void IsUserLoggedIn(User user, MainWindow mainWindow)
        {
            if ( user != null)
            {
                return;
            }

            mainWindow.ToLogin();
        }

        public  bool IsUserAdmin(User user)
        {
            if (user.RoleId == 1)
            {
                return true;
            }
            return false;
        }
    }
}
