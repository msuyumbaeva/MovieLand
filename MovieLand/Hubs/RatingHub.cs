using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Hubs
{
    [Authorize]
    public class RatingHub : Hub
    {
        public async Task SendMessage(string movieId, string movieValue) {
            var user = Context.User.Identity.Name;
            await Clients.All.SendAsync("ReceiveMessage", user, movieId, movieValue);
        }
    }
}
