using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LobaRealtime.Classes
{
    [Serializable]
    public class Player
    {
        public string connectionID;
        public string name;
        public string avatarIndex;
        public bool NotifyOnPlayerUpdate;
        public bool showOnline = true;
        public string level;

        public Player(string connectionID, string name, string avatarIndex, bool showOnline, string level)
        {
            System.Diagnostics.Trace.TraceError($"lvl {level} {name}:{connectionID} joined with {avatarIndex}");
            this.connectionID = connectionID;
            this.name = name;
            this.avatarIndex = avatarIndex;
            this.showOnline = showOnline;
            this.level = level;
        }

        public void Print()
        {
            System.Diagnostics.Trace.TraceError($"{name}:{connectionID} joined with {avatarIndex}");
        }
    }
}
