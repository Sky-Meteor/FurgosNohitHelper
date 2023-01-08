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
                    SavedInventory = SaveInventory(player);
                    SaveIfNotServ("ks/inv", SavedInventory);
                    SavedEquipments = SaveEquipments(player);
                    SaveIfNotServ("ks/equip", SavedEquipments);
                    Main.NewText("保存成功");
                    break;
                case "load":
                    LoadInventory(player, SavedInventory);
                    LoadEquipments(player, SavedEquipments);
                    Main.NewText("加载成功");
                    break;
                default:
                    Main.NewText(Usage);
                    return;
            }
        }
        public override void OnLoad()
        {
            SavedInventory = BossSavePath.Get<Dictionary<int, Tuple<string, int>>>("ks/inv", new());
            BossSavePath.Put("ks/inv", SavedInventory);
            SavedEquipments = BossSavePath.Get<Dictionary<int, string>>("ks/equip", new());
            BossSavePath.Put("ks/equip", SavedEquipments);
        }
        public override void OnUnload()
        {
            SavedInventory = null;
            SavedEquipments = null;
        }

        Dictionary<int, Tuple<string, int>> SavedInventory;
        Dictionary<int, string> SavedEquipments;
    }
}
