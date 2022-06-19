using LobaRealtime.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Web;

namespace LobaRealtime.Hubs
{
    public class ConnectionHub : Hub
    {
        static readonly PlayersManager usersManager = new PlayersManager();

        #region Client Calls

        public void RegisterForOnlineNotification(string name, bool register)
        {
            usersManager.PrintPlayers();
            System.Diagnostics.Debug.WriteLine("RegisterForOnlineNotification: " + name + ", " + register);
            string ID = usersManager.Register(name, register);
            GetOnlinePlayers(null, ID);
        }

        public void GetOnlinePlayers(string name, string ID)
        {
            if (ID == null)
                ID = usersManager.FindConnectionIDByName(name);

            Clients.Client(ID).SendAsync("RegisterResponse", JsonConvert.SerializeObject(usersManager.GetOnlinePlayers()));
        }

        public void FetchPlayersCount(string name)
        {
            if (name != null)
            {
                string ID = usersManager.FindConnectionIDByName(name);
                Clients.Client(ID).SendAsync("PlayersCountFetched", usersManager.GetOnlinePlayers().Count);
            }
            else
                Clients.All.SendAsync("PlayersCountFetched", usersManager.GetOnlinePlayers().Count);
        }

        public void NotifyRegisteredPlayersAboutOnlineStatus(Player player, bool online)
        {
            if (player == null) return;
            List<string> connectionIDs = usersManager.GetPlayersToNotify();
            Clients.Clients(connectionIDs).SendAsync("PlayerStatus", JsonConvert.SerializeObject(player), online);
        }

        public void InvitePlayers(string connectionIDs, string invitationJson)
        {
            List<string> CIDs = JsonConvert.DeserializeObject<List<string>>(connectionIDs);
            Clients.Clients(CIDs).SendAsync("InvitationReceived", invitationJson);
        }

        public void ChangeOnlineStatus(string name, bool newStatus)
        {
            System.Diagnostics.Debug.WriteLine("ChangeOnlineStatus: " + name + ", " + newStatus);
            Player player = usersManager.TogglePlayerOnlineStatus(name, newStatus);
            NotifyRegisteredPlayersAboutOnlineStatus(player, newStatus);
            FetchPlayersCount(null);
        }

        #endregion

        public void Send(string methodName, List<string> connectionIDs = null, object arg1 = null)
        {
            if (connectionIDs == null)
                Clients.All.SendAsync(methodName, arg1);
            else
                Clients.Clients(connectionIDs).SendAsync(methodName, arg1);
        }

        public override Task OnConnectedAsync()
        {
            string connectionID = Context.ConnectionId;
            HttpContext httpContext = Context.GetHttpContext();
            string querry = httpContext.Request.QueryString.Value;
            NameValueCollection querryCollection = HttpUtility.ParseQueryString(querry);

            string name = querryCollection["name"];
            string avatarIndex = querryCollection["avatarIndex"];
            string level = querryCollection["level"];

            Player newPlayer = usersManager.MakePlayerOnline(connectionID, name, avatarIndex, true, level);
            NotifyRegisteredPlayersAboutOnlineStatus(newPlayer, true);
            FetchPlayersCount(null);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var connectionID = Context.ConnectionId;
            Player player = usersManager.MakePlayerOffline(connectionID);
            NotifyRegisteredPlayersAboutOnlineStatus(player, false);
            FetchPlayersCount(null);


            // remove(user);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
