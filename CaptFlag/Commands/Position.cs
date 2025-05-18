using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace CaptFlag.Commands
{
    class Position : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public List<string> Permissions
        {
            get
            {
                return new List<string>() {
                    "captflag.pos"
                };
            }
        }
        public string Name = "position";
        public string Help => "Get your position XYZ.";
        public string Syntax => "";
        public List<string> Aliases => new List<string> { "pos" };
        string IRocketCommand.Name => "position";
        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            UnturnedChat.Say(caller, player.Position.x + "," + player.Position.y + "," + player.Position.z+" ("+ player.Rotation + ")");
        }
    }
}
