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
    public class Pall : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public List<string> Permissions
        {
            get
            {
                return new List<string>() {
                    "captflag.pall"
                };
            }
        }
        public string Name = "pall";

        public string Help => "Remove every player from specified rocket group.";

        public string Syntax => "remove <GroupName>";

        public List<string> Aliases => new List<string> { "pallremove", "premoveall" };

        string IRocketCommand.Name => "pall";

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length >= 1)
            {
                if (command[0].ToString().Equals("remove"))
                {
                    Rocket.API.Serialisation.RocketPermissionsGroup grup = R.Permissions.GetGroup(command[1]);
                    if (grup != null)
                    {
                        foreach (var p in Provider.clients)
                        {
                            UnturnedPlayer pl = Rocket.Unturned.Player.UnturnedPlayer.FromSteamPlayer(p);
                            R.Permissions.RemovePlayerFromGroup(grup.Id, pl);
                        }
                        UnturnedChat.Say(caller, "Removed all players from "+command[1] + "!");
                    }
                    else
                    {
                        UnturnedChat.Say(caller, command[1]+" is not an existing RocketMod group.");
                    }
                }
            }
            else
            {
                UnturnedChat.Say(caller, "/pall remove <GroupName>");
            }
        }
    }
}
