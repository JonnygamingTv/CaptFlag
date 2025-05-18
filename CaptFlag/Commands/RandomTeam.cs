using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using Steamworks;
using System.Collections.Generic;
using Rocket.Unturned.Chat;
using Rocket.Core;

namespace CaptFlag.Commands
{
    public class RandomTeam : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public List<string> Permissions
        {
            get
            {
                return new List<string>() {
                    "captflag.random"
                };
            }
        }
        public string Name = "randomteam";

        public string Help => "Put every player into a random team.";

        public string Syntax => "<team1, team2, team3..>";

        public List<string> Aliases => new List<string> { "randomizeteam", "randomizer" };

        string IRocketCommand.Name => "randomteam";

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length >= 1) {
                Boolean makegroups = false;
                if (command[0].ToString().Substring(0, 5).Equals("group"))
                {
                    makegroups = true;
                }
                List<string> teams = new List<string>();
                List<int> teamM = new List<int>();
                List<GroupInfo> uteams = new List<GroupInfo>();
                List<Rocket.API.Serialisation.RocketPermissionsGroup> teamgrups = new List<Rocket.API.Serialisation.RocketPermissionsGroup>();
                bool prefetch = false;
                if (Provider.clients.Count > command.Length) prefetch = true;
                for (int i = 1; i < command.Length; i++)
                {
                    teams.Add(command[i]);
                    teamM.Add(0);
                    if(makegroups)uteams.Add(GroupManager.addGroup(GroupManager.generateUniqueGroupID(), i.ToString()));
                    if (prefetch)
                    {
                        teamgrups.Add(R.Permissions.GetGroup(command[i]));
                    }
                }
                foreach (var p in Provider.clients)
                {
                    int re = 0;
                    for(int t = 0; t < teamM.Count; t++)
                    {
                        if (teamM[t] > re) re = teamM[t];
                    }
                    List<string> availteams = new List<string>();
                    for (int t = 0; t < teams.Count; t++)
                    {
                        if (teamM[t] < re) availteams.Add(teams[t]);
                    }
                    if (availteams.Count == 0) availteams = teams;
                    var r = new Random();
                    int which = r.Next(0,availteams.Count);
                    teamM[teams.IndexOf(availteams[which])]++;
                    UnturnedPlayer pl = Rocket.Unturned.Player.UnturnedPlayer.FromSteamPlayer(p);
                    UnturnedChat.Say(pl, "Team: "+which.ToString());
                    Rocket.API.Serialisation.RocketPermissionsGroup grup;
                    if (prefetch) {
                        grup=teamgrups[teams.IndexOf(availteams[which])];
                    } else { grup = R.Permissions.GetGroup(availteams[which]); }
                    if(grup!=null) R.Permissions.AddPlayerToGroup(grup.Id, pl);
                    if (makegroups) {
                        p.player.quests.ServerAssignToGroup(uteams[teams.IndexOf(availteams[which])].groupID, (p.isAdmin? EPlayerGroupRank.ADMIN:EPlayerGroupRank.MEMBER), true);
                    }
                }
            }
            else
            {
                UnturnedChat.Say(caller, "/randomizer group [team1, team2, ...]");
            }
        }
    }
}
