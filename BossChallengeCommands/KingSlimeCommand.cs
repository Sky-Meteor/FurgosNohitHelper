using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace FurgosNohitHelper.BossChallengeCommands
{
    public class KingSlimeCommand : BaseBossCommand
    {
        public override string Cmd => "ks";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            EasyRegisterAction(caller, input, args, ref SavedInventory, ref SavedEquipments);
        }

        public override void OnLoad()
        {
            EasyLoadInventoryAndEquipments(ref SavedInventory, ref SavedEquipments);
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