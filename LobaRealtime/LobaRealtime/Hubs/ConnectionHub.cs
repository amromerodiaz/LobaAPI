using LobaRealtime.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
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

            Classes.Module.ModuleClass moduleClass = JsonConvert.DeserializeObject<Classes.Module.ModuleClass>(invitationJson);

            Console.WriteLine(invitationJson);
            Console.WriteLine(moduleClass.senderName);

            string logmsg = null;
            logmsg = moduleClass.senderName + " invite[ " + moduleClass.roomName + " ] to Players";
            for (int i = 0; i < moduleClass.players.Count; i++)
            {
                logmsg = logmsg + " [ " + moduleClass.players[i].name + " ] ";
            }
             Send_Receive_log(moduleClass.senderName, logmsg, moduleClass.roomName);

            logmsg = null;
            logmsg = "Player " + moduleClass.senderName + " has created a Room [ " + moduleClass.roomName + " ] ";
             Send_Receive_log(moduleClass.senderName, logmsg, moduleClass.roomName);

            if (moduleClass.isTournament)
            {
                logmsg = null;
                logmsg = "Player " + moduleClass.senderName + " starts a tournament [ " + moduleClass.roomName + " ] ";
                Send_Receive_log(moduleClass.senderName, logmsg, moduleClass.roomName);
            }
            else
            {
                logmsg = null;
                logmsg = "Player " + moduleClass.senderName + " starts a single game [ " + moduleClass.roomName + " ] ";
                Send_Receive_log(moduleClass.senderName, logmsg, moduleClass.roomName);
            }
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

        //This was add by me 
        public void AcceptInvitation(string name , string Sendername , string roomid , bool Accepted)
        {
            if (Accepted)
            {
                Console.WriteLine("ChangeOnlineStatus: " + name + ", " + Sendername + " " + roomid + " " + Accepted);
                string logmsg = "Player ["+ name +" ] accept invitation from [ "+ Sendername +" ]";
                Send_Receive_log(name, logmsg, roomid);
            }
            else
            {
                Console.WriteLine("ChangeOnlineStatus: " + name + ", " + Sendername + " " + roomid + " " + Accepted);
                string logmsg = "Player [" + name + " ] rejected the invitation from [ " + Sendername + " ]";
                Send_Receive_log(name, logmsg, roomid);
            }
         
        }

        public void Player_Leave_Room(string name , string roomid , bool host)
        {
            if(host)
            {
                Console.WriteLine("Player_Leave_Room" + name + " left the room" + roomid + " " + host);
                string logmsg = "Player(Host) [" + name + " ] left the room [ " + roomid + " ]";
                Send_Receive_log(name, logmsg, roomid);
            }
            else
            {
                Console.WriteLine("Player_Leave_Room" + name + " left the room" + roomid +" " + host);
                string logmsg = "Player [" + name + " ] left the room [ " + roomid + " ]";
                Send_Receive_log(name, logmsg, roomid);
            }
        }

        public void Player_playing_Locally(string name , bool isTournament)
        {
            if(isTournament)
            {
                Console.WriteLine("Player ["+ name + "] playing [Tournament] locally");
                string logmsg = "Player [" + name + "] playing [Tournament] locally";
                Send_Receive_log(name, logmsg, "locally");
            }
            else
            {
                Console.WriteLine("Player [" + name + "] playing [Single] locally");
                string logmsg = "Player [" + name + "] playing [Single] locally";
                Send_Receive_log(name, logmsg, "locally");
            }
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

            Post("Player Connected to Server", name);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var connectionID = Context.ConnectionId;
            Player player = usersManager.MakePlayerOffline(connectionID);
            NotifyRegisteredPlayersAboutOnlineStatus(player, false);
            FetchPlayersCount(null);

           Post("Player Disconnected to Server", player.name);
           // remove(user);
            return base.OnDisconnectedAsync(exception);
        }

        public void Post(string logmsg, string name)
        {
            string connectionstring;
          //   connectionstring = "SERVER=localhost;PORT=3306;DATABASE=iniviation;UID=root;PASSWORD=Paritosh#5";
            connectionstring = "SERVER=localhost;PORT=50281;DATABASE=invitations;UID=azure;PASSWORD=6#vWHD_$";
            MySqlConnection cnn = new MySqlConnection(connectionstring);
            cnn.Open();

            if (cnn.State == System.Data.ConnectionState.Open)
            {
                Debug.WriteLine("0-0-0-0-0-0-0-0>-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
            }
            
            //Console.Write("lOLOLOLOLOLOL");
            //string query = "select * from logs;";
            string query = "INSERT INTO log (Log_msg, Player_Name) VALUES (@msg, @playerid);";
            MySqlCommand cmd = new MySqlCommand(query, cnn);

            cmd.Parameters.AddWithValue("@msg", logmsg);
            cmd.Parameters.AddWithValue("@playerid", name);

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                //Debug.WriteLine(reader["LogId"]);
                Debug.WriteLine(reader["Log_msg"]);
                Debug.WriteLine(reader["Player_Name"]);
                //Debug.WriteLine(reader["LogTime"]);
                //Console.WriteLine(reader["LogId"]);

            }
            cnn.Close();
        }

        public void Send_Receive_log(string sender , string logmsg , string roomid)
        {
            string connectionstring;
            connectionstring = "SERVER=localhost;PORT=50281;DATABASE=invitations;UID=azure;PASSWORD=6#vWHD_$";
            // connectionstring = "SERVER=localhost;PORT=3306;DATABASE=iniviation;UID=root;PASSWORD=Paritosh#5";
            MySqlConnection cnn = new MySqlConnection(connectionstring);
            cnn.Open();

            if (cnn.State == System.Data.ConnectionState.Open)
            {
                Debug.WriteLine("0-0-0-0-0-0-0-0>-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
            }
            //Console.Write("lOLOLOLOLOLOL");
            //string query = "select * from logs;";
            string query = "INSERT INTO invitationlog (Log_msg,Sender_Name,RoomID) VALUES (@msg, @sender,@RoomID);";
            MySqlCommand cmd = new MySqlCommand(query, cnn);

            cmd.Parameters.AddWithValue("@msg", logmsg);
            cmd.Parameters.AddWithValue("@sender", sender);
            cmd.Parameters.AddWithValue("@RoomID", roomid);

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                //Debug.WriteLine(reader["LogId"]);
                Debug.WriteLine(reader["Log_msg"]);
                Debug.WriteLine(reader["Sender_Name"]);
                //Debug.WriteLine(reader["LogTime"]);
                //Console.WriteLine(reader["LogId"]);

            }

            cnn.Close();
        }


    }


    

}
