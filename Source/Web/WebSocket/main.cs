using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Owin.WebSocket;

namespace MMOLauncher.Web.WebSocket
{
    public delegate void ClientConnectedEventHandler(WebSocketConnection sender, EventArgs e);
    public class MyWebSocket : WebSocketConnection
    {
        public event ClientConnectedEventHandler ClientConnected;

        public List<WebSocketConnection> Connections { get; private set; }

        public override Task OnMessageReceived(ArraySegment<byte> message, WebSocketMessageType type)
        {
            //Handle the message from the client

            //Example of JSON serialization with the client
            var json = Encoding.UTF8.GetString(message.Array, message.Offset, message.Count);

            var toSend = Encoding.UTF8.GetBytes(json);

            //Echo the message back to the client as text
            Console.WriteLine(toSend);

            SendText(Encoding.UTF8.GetBytes("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"), true);
            return SendText(toSend, true);
        }







        public override void OnOpen()
        {
            Console.WriteLine("WS Connections opend");
            SendText(Encoding.UTF8.GetBytes("YYYYYYYYYYYY"), true);
            /*Connections = new List<WebSocketConnection>();

            foreach (WebSocketState ws in Connections)
            {
                
            }*/
        }

        public override void OnClose(WebSocketCloseStatus? closeStatus, string closeStatusDescription)
        {
            Console.WriteLine("WS Connections closed");
        }
        //public override bool Authenticate(IOwinRequest request){return true;}
    }

}
