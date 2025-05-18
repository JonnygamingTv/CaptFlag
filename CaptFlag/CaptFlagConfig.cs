using Rocket.API;
using System.Collections.Generic;

namespace CaptFlag
{
    public class CaptFlagConfig : IRocketPluginConfiguration
    {
        public List<string> arenas;
        public List<List<List<float>>> arenA;
        public List<List<string>> flags;
        public bool NoCosmetics;
        public ushort FlagID;
        public float allowMove;
        public void LoadDefaults()
        {
            arenas = new List<string>();
            arenA = new List<List<List<float>>>();
            flags = new List<List<string>>();
            NoCosmetics = false;
            FlagID = 0;
            allowMove = 15f;
        }
    }
}
