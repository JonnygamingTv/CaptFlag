using Rocket.Core.Plugins;
using System.Collections.Generic;

namespace CaptFlag
{
    public class CaptFlag : RocketPlugin<CaptFlagConfig>
    {
        public static CaptFlag Instance { get; private set; }
        List<Rocket.Unturned.Player.UnturnedPlayer> Players=new List<Rocket.Unturned.Player.UnturnedPlayer>();
        List<int> Regions = new List<int>();
        List<UnityEngine.Vector3> LastPos = new List<UnityEngine.Vector3>();
        bool aa = false;
        bool pa = false;
        bool ba = false;
        protected override void Load()
        {
            Instance = this;
            Rocket.Core.Logging.Logger.Log("Capture The Flag!");
            Rocket.Unturned.U.Events.OnPlayerDisconnected += PLeav;
            if (Configuration.Instance.arenA.Count > 0) arenaListening();
            if (Configuration.Instance.flags.Count > 0)
            {
                PVPListening();
                barrListen();
            }
        }
        protected override void Unload()
        {
            Rocket.Core.Logging.Logger.Log("Time to die lol");
            Rocket.Unturned.U.Events.OnPlayerDisconnected -= PLeav;
        }
        public void PLeav(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            int x = Players.IndexOf(player);
            if (x != -1)
            {
                Players.Remove(player);
                Regions.RemoveAt(x);
                LastPos.RemoveAt(x);
            }
        }
        public void arenaListening()
        {
            if (!aa) { Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerUpdatePosition += PlayerUpdatePosition; aa = true; }
        }
        public void PVPListening()
        {
            if (!pa) { Rocket.Unturned.Events.UnturnedEvents.OnPlayerDamaged += OnPlayerDamaged; Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerUpdateHealth += OnPlayerHealth; pa = true; }
        }
        public void barrListen()
        {
            if (!ba) { SDG.Unturned.BarricadeManager.onDeployBarricadeRequested += DBRH; ba = true; }
        }
        private void PlayerUpdatePosition(Rocket.Unturned.Player.UnturnedPlayer player, UnityEngine.Vector3 position)
        {
            int x = Players.IndexOf(player);
            if (x == -1) { x = Players.Count; Players.Add(player);Regions.Add(-1); LastPos.Add(player.Position); }
            bool any = false;
            for (int i = 0; i < Configuration.Instance.arenA.Count; i++)
            {
                if (Configuration.Instance.arenA[i].Count > 1)
                {
                    if (player.Position.x>Configuration.Instance.arenA[i][0][0]&& player.Position.y > Configuration.Instance.arenA[i][0][1] && player.Position.z > Configuration.Instance.arenA[i][0][2]&&player.Position.x<Configuration.Instance.arenA[i][1][0]&& player.Position.y < Configuration.Instance.arenA[i][1][1]&& player.Position.z<Configuration.Instance.arenA[i][1][2])
                    {
                        if (Configuration.Instance.flags[i].Contains("entry")) {
                            //player.Position.Set(lastPos[x].x, lastPos[x].y, lastPos[x].z);
                            if (UnityEngine.Vector3.Distance(LastPos[x], player.Position) > Configuration.Instance.allowMove) return;
                            UnityEngine.Vector3 newPos = new UnityEngine.Vector3();
                            newPos.Set(LastPos[x].x, LastPos[x].y, LastPos[x].z);
                            player.Teleport(newPos,player.Rotation);
                        }
                        else if (Configuration.Instance.flags[i].Contains("tp")) {
                            UnityEngine.Vector3 newPos = new UnityEngine.Vector3();
                            int b = Configuration.Instance.flags[i].IndexOf("tp");
                            newPos.Set(float.Parse(Configuration.Instance.flags[b+1].ToString()), float.Parse(Configuration.Instance.flags[b + 2].ToString()), float.Parse(Configuration.Instance.flags[b + 3].ToString()));
                            player.Teleport(newPos, player.Rotation);
                        } 
                        else if (Configuration.Instance.flags[i].Contains("entrygroup"))
                        {
                            int b = Configuration.Instance.flags[i].IndexOf("entrygroup");
                            Rocket.Core.R.Permissions.AddPlayerToGroup(Rocket.Core.R.Permissions.GetGroup(Configuration.Instance.flags[i][b+1]).Id, player);
                        }
                        else if (Configuration.Instance.flags[i].Contains("entryrgroup"))
                        {
                            int b = Configuration.Instance.flags[i].IndexOf("entryrgroup");
                            Rocket.Core.R.Permissions.RemovePlayerFromGroup(Rocket.Core.R.Permissions.GetGroup(Configuration.Instance.flags[i][b + 1]).Id, player);
                        }
                        else 
                        {
                            Regions[x] = i;
                            any = true;
                            break;
                        }
                    }
                }
            }
            if (!any && Regions[x]!=-1)
            {
                if (Configuration.Instance.flags[Regions[x]].Contains("exitgroup"))
                {
                    int b = Configuration.Instance.flags[Regions[x]].IndexOf("exitgroup");
                    Rocket.Core.R.Permissions.AddPlayerToGroup(Rocket.Core.R.Permissions.GetGroup(Configuration.Instance.flags[Regions[x]][b + 1]).Id, player);
                }
                else if (Configuration.Instance.flags[Regions[x]].Contains("exitrgroup"))
                {
                    int b = Configuration.Instance.flags[Regions[x]].IndexOf("exitrgroup");
                    Rocket.Core.R.Permissions.RemovePlayerFromGroup(Rocket.Core.R.Permissions.GetGroup(Configuration.Instance.flags[Regions[x]][b + 1]).Id, player);
                }else
                if (Configuration.Instance.flags[Regions[x]].Contains("exit"))
                {
                    //player.Position.Set(lastPos[x].x, lastPos[x].y, lastPos[x].z);
                    if (UnityEngine.Vector3.Distance(LastPos[x], player.Position) > Configuration.Instance.allowMove) return;
                    UnityEngine.Vector3 newPos = new UnityEngine.Vector3();
                    newPos.Set(LastPos[x].x, LastPos[x].y, LastPos[x].z);
                    player.Teleport(newPos, player.Rotation);
                    return;
                }
                Regions[x] = -1;
            }
            LastPos[x] = player.Position;
        }
        private void OnPlayerDamaged(Rocket.Unturned.Player.UnturnedPlayer player, ref SDG.Unturned.EDeathCause cause, ref SDG.Unturned.ELimb limb, ref Rocket.Unturned.Player.UnturnedPlayer killer, ref UnityEngine.Vector3 direction, ref float damage, ref float times, ref bool canDamage)
        {
            int x = Players.IndexOf(player);
            if (x == -1 && killer != null) x = Players.IndexOf(killer);
            if (x != -1) {
                if (Regions[x] != -1)
                {
                    if (Configuration.Instance.flags[Regions[x]].Contains("pvp"))
                    {
                        damage = 0;
                        canDamage = false;
                        if(killer!=null) killer.Player.equipment.dequip();
                    }
                }
            }
        }
        private void DBRH(SDG.Unturned.Barricade barricade, SDG.Unturned.ItemBarricadeAsset asset, UnityEngine.Transform hit, ref UnityEngine.Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            for (int i = 0; i < Configuration.Instance.arenA.Count; i++)
            {
                if (Configuration.Instance.arenA[i].Count > 1&& Configuration.Instance.flags[i].Contains("build"))
                {
                    for (int ii = 0; ii < Configuration.Instance.arenA[i].Count; ii += 2)
                    {
                        if (point.x > Configuration.Instance.arenA[i][ii][0] && point.y > Configuration.Instance.arenA[i][ii][1] && point.z > Configuration.Instance.arenA[i][ii][2] && point.x < Configuration.Instance.arenA[i][ii + 1][0] && point.y < Configuration.Instance.arenA[i][ii + 1][1] && point.z < Configuration.Instance.arenA[i][ii + 1][2])
                        {
                            shouldAllow = false;
                            break;
                        }
                    }
                }
            }
        }
        private void OnPlayerHealth(Rocket.Unturned.Player.UnturnedPlayer player, byte health)
        {
            int x = Players.IndexOf(player);
            if (x != -1)
            {
                if (Regions[x] != -1)
                {
                    if (Configuration.Instance.flags[Regions[x]].Contains("pvp"))
                    {
                        player.Heal(100);
                    }
                }
            }
        }
    }
}
