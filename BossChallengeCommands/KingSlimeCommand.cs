using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

namespace FurgosNohitHelper.BossChallengeCommands
{
    public class KingSlimeCommand : BaseBossCommand
    {
        public override string Cmd => "ks";

        public override string Usage => "/ks save保存配置，/ks load加载配置";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Player player = caller.Player;
            if (args.Length < 1)
            {
                Main.NewText(Usage);
                return;
            }
            switch (args[0])
            {
                case "save":
                    SavedItems = SaveInventory(player);
                    SaveIfNotServ("ks", SavedItems);
                    Main.NewText("保存成功");
                    break;
                case "load":
                    //ClearInventoryAndEquipments(player);
                    ClearInventory(player);
                    LoadInventory(player, SavedItems);
                    Main.NewText("加载成功");
                    break;
                default:
                    Main.NewText(Usage);
                    return;
            }
        }
        public override void OnLoad()
        {
            SavedItems = BossSavePath.Get<Dictionary<int, Tuple<string, int>>>("ks", new());
            BossSavePath.Put("ks", SavedItems);
        }
        public override void OnUnload()
        {
            SavedItems = null;
        }

        Dictionary<int, Tuple<string, int>> SavedItems;
    }
}
