using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace MMOwningLauncher.Web.WebSocket
{


    public class Electron : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            var name = Context.QueryString["name"];
            Send(!name.IsNullOrEmpty() ? String.Format("\"{0}\" to {1}", e.Data, name) : e.Data);
        }
    }


    public class Echo : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            var name = Context.QueryString["name"];
            Send(!name.IsNullOrEmpty() ? String.Format("\"{0}\" to {1}", e.Data, name) : e.Data);
        }
    }




    public class Chat : WebSocketBehavior
    {
        private string _name;
        private static int _number = 0;
        private string _prefix;

        public Chat()
          : this(null)
        {
        }

        public Chat(string prefix)
        {
            _prefix = !prefix.IsNullOrEmpty() ? prefix : "anon#";
        }


        private string getName()
        {
            var name = Context.QueryString["name"];
            return !name.IsNullOrEmpty() ? name : _prefix + getNumber();
        }

        private static int getNumber()
        {
            return Interlocked.Increment(ref _number);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Sessions.Broadcast(String.Format("{0} got logged off...", _name));
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Sessions.Broadcast(String.Format("{0}: {1}", _name, e.Data));
        }

        protected override void OnOpen()
        {
            _name = getName();
        }
    }




}
