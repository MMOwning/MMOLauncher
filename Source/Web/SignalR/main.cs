using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace MMOLauncher.Web.SignalR
{
    public class MyHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }
    }

}
