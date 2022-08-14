using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LobaRealtime.Classes.Module
{
    public class Player
    {
        public string name { get; set; }
        public int avatarIndex { get; set; }
        public int status { get; set; }
    }

    public class ModuleClass
    {
        public string senderName { get; set; }
        public int senderAvatarIndex { get; set; }
        public string roomName { get; set; }
        public string roomPassword { get; set; }
        public bool isTournament { get; set; }
        public int maxPlayers { get; set; }
        public int duration { get; set; }
        public bool useJokerInSequence { get; set; }
        public bool useJokerInSet { get; set; }
        public List<int> honorCardsValues { get; set; }
        public List<Player> players { get; set; }
        public string JSON { get; set; }
    }
}
