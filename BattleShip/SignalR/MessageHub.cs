using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleShip.SignalR
{
    public class MessageHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task LobbyAccepted(string message)
        {
            await Clients.All.SendAsync("LobbyResponse", message);
        }
    }
}
