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

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Player player = caller.Player;
            //ClearInventoryAndEquipments(player);
            SavedItems = SaveInventory(player);
            BossSavePath.Put("ks", SavedItems);
            SaveIfNotServ();
            foreach (var i in SavedItems)
            {
                Main.NewText($"{i.Key} {i.Value}");
            }
        }
        public override void OnLoad()
        {
            SavedItems = new Dictionary<int, Tuple<int, Tuple<int, string>>>();
            SavedItems = BossSavePath.Get<Dictionary<int, Tuple<int, Tuple<int, string>>>>("ks", new());
            BossSavePath.Put("ks", SavedItems);
        }
        public override void OnUnload()
        {
            SavedItems = null;
        }

        Dictionary<int, Tuple<int, Tuple<int, string>>> SavedItems;
    }
}
