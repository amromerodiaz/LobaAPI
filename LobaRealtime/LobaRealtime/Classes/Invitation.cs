using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LobaRealtime.Classes
{
    [Serializable]
    public class Invitation
    {
        public string senderName;
        public int senderAvatarIndex;

        public string roomName;
        public string roomPassword;

        public string matchType;
        public int maxPlayers;
        public int duration;
        public bool useJokerInSequence;
        public bool useJokerInSet;
        public int[] honorCardsValues;
    }
}
