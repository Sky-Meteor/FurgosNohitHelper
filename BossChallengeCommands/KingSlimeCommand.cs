using System;
using Terraria;
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
        }
    }
}
