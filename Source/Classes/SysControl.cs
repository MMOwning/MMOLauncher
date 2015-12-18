using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MMOLauncher.Classes
{
    class IsUserAdministrator
    {
        public bool Check()
        {
            //bool value to hold our return value
            bool isAdmin;
            try
            {
                //get the currently logged in user
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

    }
}
