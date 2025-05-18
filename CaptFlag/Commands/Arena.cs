using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace CaptFlag.Commands
{
    class Arena : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public List<string> Permissions
        {
            get
            {
                return new List<string>() {
                    "captflag.arena"
                };
            }
        }
        public string Name = "arena";
        public string Help => "Make a region for an arena.";
        public string Syntax => "";
        public List<string> Aliases => new List<string> { "addarena", "createarena" };
        string IRocketCommand.Name => "arena";

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length > 0)
            {
                switch (command[0])
                {
                    case "create":
                        {
                            if (command.Length > 1)
                            {
                                int x = CaptFlag.Instance.Configuration.Instance.arenas.IndexOf(command[1]);
                                if (x == -1)
                                {
                                    CaptFlag.Instance.Configuration.Instance.arenas.Add(command[1]);
                                    CaptFlag.Instance.Configuration.Instance.arenA.Add(new List<List<float>>());
                                    CaptFlag.Instance.Configuration.Instance.flags.Add(new List<string>());
                                    UnturnedChat.Say(caller, "Created " + command[1]);
                                }
                            }
                            else
                            {
                                UnturnedChat.Say(caller, "/arena create Test");
                            }
                            break;
                        }
                    case "delete":
                        {
                            if (command.Length > 1)
                            {
                                int x = CaptFlag.Instance.Configuration.Instance.arenas.IndexOf(command[1]);
                                if (x > -1)
                                {
                                    CaptFlag.Instance.Configuration.Instance.arenA.RemoveAt(x);
                                    CaptFlag.Instance.Configuration.Instance.flags.RemoveAt(x);
                                    CaptFlag.Instance.Configuration.Instance.arenas.Remove(command[1]);
                                }
                                UnturnedChat.Say(caller, "Removed " + command[1]);
                            }
                            else
                            {
                                UnturnedChat.Say(caller, "/arena delete Test");
                            }
                            break;
                        }
                    case "point":
                        {
                            if (command.Length > 1)
                            {
                                int x = CaptFlag.Instance.Configuration.Instance.arenas.IndexOf(command[1]);
                                if (x > -1)
                                {
                                    if (command.Length == 3)
                                    {
                                        if (command[2] == "clear")
                                        {
                                            CaptFlag.Instance.Configuration.Instance.arenA[x].Clear();
                                            UnturnedChat.Say(caller, "Cleared!");
                                            return;
                                        }
                                        if (int.TryParse(command[2], out int r)) {
                                            List<float> gg = new List<float>() { player.Position.x - r, player.Position.y - r, player.Position.z - r };
                                            CaptFlag.Instance.Configuration.Instance.arenA[x].Add(gg);
                                            gg = new List<float>() { player.Position.x + r, player.Position.y + r, player.Position.z + r };
                                            CaptFlag.Instance.Configuration.Instance.arenA[x].Add(gg);
                                            UnturnedChat.Say(caller, "Added points around you in radius " + command[2]);
                                        }
                                    }
                                    else
                                    {
                                        List<float> gg = new List<float>();
                                        gg.Add(player.Position.x);
                                        gg.Add(player.Position.y);
                                        gg.Add(player.Position.z);
                                        CaptFlag.Instance.Configuration.Instance.arenA[x].Add(gg);
                                        UnturnedChat.Say(caller, "Added point to " + command[1]);
                                    }
                                }
                                CaptFlag.Instance.arenaListening();
                            }
                            else
                            {
                                UnturnedChat.Say(caller, "/arena point Test [radius]");
                            }
                            break;
                        }
                    case "flag":
                        {
                            if (command.Length > 3)
                            {
                                int x = CaptFlag.Instance.Configuration.Instance.arenas.IndexOf(command[1]);
                                if (x > -1) {
                                    if (command[3].ToLower().Contains("of") || command[3].ToLower() == "false") {
                                        if (command[2].ToLower() == "tp")
                                        {
                                            int b = CaptFlag.Instance.Configuration.Instance.flags[x].IndexOf("tp");
                                            if (b != -1)
                                            {
                                                CaptFlag.Instance.Configuration.Instance.flags[x].RemoveAt(b);
                                                CaptFlag.Instance.Configuration.Instance.flags[x].RemoveAt(b);
                                                CaptFlag.Instance.Configuration.Instance.flags[x].RemoveAt(b);
                                                CaptFlag.Instance.Configuration.Instance.flags[x].RemoveAt(b);
                                                UnturnedChat.Say(caller, "Disabled teleport.");
                                            }
                                            else
                                            {
                                                UnturnedChat.Say(caller, "Teleport is not enabled.");
                                            }
                                        }
                                        else if (command[2].ToLower() == "entrygroup"|| command[2].ToLower() == "entryrgroup"|| command[2].ToLower() == "exitgroup"|| command[2].ToLower() == "exitrgroup")
                                        {
                                            int b = CaptFlag.Instance.Configuration.Instance.flags[x].IndexOf(command[2].ToLower());
                                            if (b != -1)
                                            {
                                                CaptFlag.Instance.Configuration.Instance.flags[x].RemoveAt(b);
                                                CaptFlag.Instance.Configuration.Instance.flags[x].RemoveAt(b);
                                                UnturnedChat.Say(caller, "Disabled "+ command[2].ToLower() + ".");
                                            }
                                            else
                                            {
                                                UnturnedChat.Say(caller, "No "+ command[2].ToLower() + " was set.");
                                            }
                                        }
                                        else
                                        {
                                            CaptFlag.Instance.Configuration.Instance.flags[x].Add(command[2]);
                                            UnturnedChat.Say(caller, "Denying " + command[2]);
                                        }
                                    }
                                    else {
                                        if (command[2].ToLower() == "tp")
                                        {
                                            float[] nP = new float[] { player.Position.x, player.Position.y, player.Position.z };
                                            for (int i = 3; i < command.Length && i < 6; i++) if (float.TryParse(command[i], out float ps)) nP[i - 3] = ps;
                                            int b = CaptFlag.Instance.Configuration.Instance.flags[x].IndexOf("tp");
                                            if (b != -1)
                                            {
                                                for(int i=0;i<3;i++) CaptFlag.Instance.Configuration.Instance.flags[x][b+i]=nP[i].ToString();
                                                UnturnedChat.Say(caller, "Edited teleport.");
                                            }
                                            else
                                            {
                                                CaptFlag.Instance.Configuration.Instance.flags[x].Add("tp");
                                                for (int i = 0; i < 3; i++) CaptFlag.Instance.Configuration.Instance.flags[x].Add(nP[i].ToString());
                                                UnturnedChat.Say(caller, "Enabled teleport.");
                                            }
                                        }else if (command[2].ToLower() == "entrygroup"|| command[2].ToLower() == "entryrgroup"|| command[2].ToLower() == "exitgroup"|| command[2].ToLower() == "exitrgroup")
                                        {
                                            int b = CaptFlag.Instance.Configuration.Instance.flags[x].IndexOf(command[2].ToLower());
                                            if (b != -1)
                                            {
                                                CaptFlag.Instance.Configuration.Instance.flags[x][b+1] = command[3];
                                                UnturnedChat.Say(caller, "Set "+ command[2].ToLower() + " to "+command[3]+".");
                                            }
                                            else
                                            {
                                                CaptFlag.Instance.Configuration.Instance.flags[x].Add(command[2].ToLower());
                                                CaptFlag.Instance.Configuration.Instance.flags[x].Add(command[3]);
                                                UnturnedChat.Say(caller, "Enabled and set "+ command[2].ToLower() + " to " + command[3]+".");
                                            }
                                        }
                                        else
                                        {
                                            CaptFlag.Instance.Configuration.Instance.flags[x].Remove(command[2]);
                                            UnturnedChat.Say(caller, "Allowing " + command[2]);
                                        }
                                    }
                                    CaptFlag.Instance.Configuration.Save();
                                    if(command[2].ToLower()=="pvp")CaptFlag.Instance.PVPListening();
                                    if(command[2].ToLower()=="build")CaptFlag.Instance.barrListen();
                                    CaptFlag.Instance.arenaListening();

                                }
                                else { UnturnedChat.Say(caller, "No such arena."); }
                            }
                            else
                            {
                                UnturnedChat.Say(caller, "/arena flag Test PVP false");
                            }
                                break;
                        }
                    case "flags":
                        if (command.Length == 2)
                        {
                            int x = CaptFlag.Instance.Configuration.Instance.arenas.IndexOf(command[1]);
                            if (x != -1)
                            {
                                string listArns = CaptFlag.Instance.Configuration.Instance.arenas[x]+": ";
                                for (int i = 0; i < CaptFlag.Instance.Configuration.Instance.flags[x].Count; i++)
                                {
                                    listArns += ", "+ CaptFlag.Instance.Configuration.Instance.flags[x][i];
                                }
                                UnturnedChat.Say(caller, listArns);
                                break;
                            }
                        }
                        UnturnedChat.Say(caller, "pvp, exit, entry, equip, gest, build, tp <pos>, entrygroup <group>, entryrgroup <group>, exitgroup <group>, exitrgroup <group>");
                        break;
                    case "list":
                        {
                            if (command.Length < 2)
                            {
                                string listArns="Arenas: "+CaptFlag.Instance.Configuration.Instance.arenas.Count;
                                for(int i=0;i< CaptFlag.Instance.Configuration.Instance.arenas.Count; i++)
                                {
                                    listArns += ", "+CaptFlag.Instance.Configuration.Instance.arenas[i];
                                }
                                UnturnedChat.Say(caller, listArns);
                            }
                            else
                            {
                                int x = CaptFlag.Instance.Configuration.Instance.arenas.IndexOf(command[1]);
                                if (x > -1)
                                {
                                    string listArns = "Points: "+CaptFlag.Instance.Configuration.Instance.arenA[x].Count;
                                    for(int i=0;i< CaptFlag.Instance.Configuration.Instance.arenA[x].Count; i++)
                                    {
                                        listArns += ", (";
                                        for (int ii = 0; ii < CaptFlag.Instance.Configuration.Instance.arenA[x][i].Count; ii++)
                                        {
                                            listArns += (ii>0?",":"")+CaptFlag.Instance.Configuration.Instance.arenA[x][i][ii];
                                        }
                                        listArns += ")";
                                    }
                                    UnturnedChat.Say(caller, listArns);
                                }
                                else
                                {
                                    UnturnedChat.Say(caller, "That region is not setup.");
                                }
                                }
                            break;
                        }
                    case "save":
                        CaptFlag.Instance.Configuration.Save();
                        break;
                }
            }
            else
            {
                UnturnedChat.Say(caller, "/arena create Test");
            }
        }
    }
}
