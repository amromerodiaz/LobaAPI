using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LobaRealtime.Classes
{
    public class PlayersManager
    {
        readonly List<Player> players = new List<Player>();

        public PlayersManager()
        {
            System.Diagnostics.Debug.WriteLine("Player Manager Created");
        }

        public List<Player> GetOnlinePlayers()
        {
            return players.FindAll(player => player.showOnline);
        }

        public void PrintPlayers()
        {
            foreach (var player in players)
            {
                System.Diagnostics.Debug.WriteLine("-------------------------");
                player.Print();
            }
        }

        public string GetConnectionID(string name)
        {
            int i = FindPlayerIndexByName(name);
            return i != -1 ? players[i].connectionID : string.Empty;
        }

        public Player TogglePlayerOnlineStatus(string name, bool status)
        {
            int index = FindPlayerIndexByName(name);
            System.Diagnostics.Debug.WriteLine("Player found: " + (index != -1));

            if (index == -1) return null;
            players[index].showOnline = status;

            return players[index];
        }

        public Player MakePlayerOnline(string connectionID, string name, string avatarIndex, bool showOnline, string level)
        {
            System.Diagnostics.Debug.WriteLine("Make online");
            if (FindPlayerIndexByCID(connectionID) != -1) return null;
            if (connectionID != null && name != null && avatarIndex != null)
            {
                System.Diagnostics.Debug.WriteLine("connectionID: " + connectionID);
                Player newPlayer = new Player(connectionID, name, avatarIndex, showOnline, level);
                players.Add(newPlayer);

                return newPlayer;
            }

            return null;
        }

        public Player MakePlayerOffline(string connectionID)
        {
            System.Diagnostics.Debug.WriteLine("Make offline: " + connectionID);
            int index = FindPlayerIndexByCID(connectionID);
            if (index == -1) return null;
            Player player = players[index];
            players.RemoveAt(index);

            return player;
        }

        /// <summary>
        /// Player with name will now get notifications from server when a player
        /// becomes online or offline
        /// </summary>
        /// <param name="name"></param>
        /// <param name="register"></param>
        public string Register(string name, bool register)
        {
            int index = FindPlayerIndexByName(name);
            if (index == -1) return null;
            System.Diagnostics.Debug.WriteLine("Registered");
            players[index].NotifyOnPlayerUpdate = register;
            return players[index].connectionID;
        }

        public List<string> GetPlayersToNotify()
        {
            List<string> registeredPlayers = new List<string>();

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].NotifyOnPlayerUpdate)
                {
                    registeredPlayers.Add(players[i].connectionID);
                }
            }

            return registeredPlayers;
        }

        int FindPlayerIndexByCID(string connectionID)
        {
            return players.FindIndex(user => user.connectionID == connectionID);
        }

        public int FindPlayerIndexByName(string name)
        {
            return players.FindIndex(user => user.name == name);
        }

        public string FindConnectionIDByName(string name)
        {
            return players[players.FindIndex(user => user.name == name)].connectionID;
        }

        public string FindPlayerNameByCID(string connectionID)
        {
            return players[players.FindIndex(user => user.connectionID == connectionID)].name;
        }
    }
}
