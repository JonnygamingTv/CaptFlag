using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Steamworks;
using System;

namespace CaptFlag.Commands
{
    class Vehicleslow : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public List<string> Permissions
        {
            get
            {
                return new List<string>() {
                    "captflag.slowvehicles"
                };
            }
        }
        public string Name = "vehicleslow";
        public string Help => "Enable minigame vehicle handling.";
        public string Syntax => "";
        public List<string> Aliases => new List<string> { "slowvehicle", "shootslower" };
        string IRocketCommand.Name => "vehicleslow";
        bool on = false;
        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (on)
            {
                SDG.Unturned.VehicleManager.onDamageVehicleRequested -= DmgVReqHandler;
                VehicleManager.onDamageTireRequested -= DmgTires;
                on = false;
                UnturnedChat.Say(player, "disabled");
            }
            else
            {
                SDG.Unturned.VehicleManager.onDamageVehicleRequested += DmgVReqHandler;
                VehicleManager.onDamageTireRequested += DmgTires;
                on = true;
                UnturnedChat.Say(player, "enabled");
            }
        }
        public void DmgVReqHandler(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            var r = new Random();
            ushort which = ushort.Parse(r.Next(1, pendingTotalDamage).ToString());
            vehicle.askBurnFuel(which);
            ushort res = (ushort)(vehicle.fuel - which);
            vehicle.tellFuel(res);
            //shouldAllow = false;
        }
        List<List<object>> vehiclus = new List<List<object>>();
        List<InteractableVehicle> vehlist = new List<InteractableVehicle>();
        List<int> tireTimes = new List<int>();
        public void DmgTires(CSteamID instigatorSteamID, InteractableVehicle vehicle, int tireIndex, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            List<object> gg = new List<object>();
            if (!vehlist.Contains(vehicle))
            {
                gg.Add(vehicle);
                for (int i = 0; i < vehicle.tires.Length; i++)
                {
                    gg.Add(0);
                }
                vehiclus.Add(gg);
                vehlist.Add(vehicle);
            }
            else
            {
                gg = vehiclus[vehlist.IndexOf(vehicle)];
            }
            int count = System.Int32.Parse(gg[tireIndex+1].ToString()) + 1;
            gg[tireIndex+1] = count;
            if(count<=1)shouldAllow = false;
        }
    }
}
