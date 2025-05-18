using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;

namespace CaptFlag.Commands
{
    class CaptheFlag : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public List<string> Permissions
        {
            get
            {
                return new List<string>() {
                    "captflag.captflag"
                };
            }
        }
        public string Name = "captflag";
        public string Help => "Start Capture The Flag.";
        public string Syntax => "";
        public List<string> Aliases => new List<string> { "capturetheflag", "captheflag" };
        string IRocketCommand.Name => "captflag";

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (isOn)
            {
                isOn = false;
                ItemManager.onTakeItemRequested -= Pick;
                UnturnedChat.Say(caller, "Ended!");
            }
            else
            {
                isOn = true;
                ItemManager.onTakeItemRequested += Pick;
                UnturnedChat.Say(caller, "Started!");
            }
        }
        bool isOn=false;
        public UnturnedPlayer Leader;
        private void Pick(Player player, byte x, byte y, uint instanceID, byte to_x, byte to_y, byte to_rot, byte to_page, ItemData itemData, ref bool shouldAllow)
        {
            if (instanceID == CaptFlag.Instance.Configuration.Instance.FlagID)
            {
                Leader = UnturnedPlayer.FromPlayer(player);
                UnturnedChat.Say(Leader.DisplayName+" got the flag!");
            }
        }
    }
}
