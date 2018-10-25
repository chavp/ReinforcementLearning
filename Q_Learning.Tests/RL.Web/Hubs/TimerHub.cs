
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RL.Web.Hubs
{
    public class TimerHub : Hub
    {
        public async Task UpdateTime()
        {
            await Clients.All.SendAsync("ReceiveMessage", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }
    }
}
