using System;
using Terraria;
using Terraria.ModLoader;

namespace FurgosNohitHelper.BossChallengeCommands
{
    public abstract class BaseBossCommand : ModCommand
    {
        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<FNHConfig>().EnableBossChallengeCommand;

        public override CommandType Type => CommandType.Chat;

        public abstract string Cmd { get; }

        public override string Command => Cmd;

        #region Utils
        public static void ClearInventory(Player player)
        {
            foreach (Item item in player.inventory)
            {
                if (!item.IsAir)
                    item.TurnToAir();
            }
        }
        public static void ClearEquipments(Player player, bool clearVanities = false)
        {
            int armorCount = 8 + player.GetAmountOfExtraAccessorySlotsToShow();
            if (clearVanities)
                armorCount *= 2;
            for (int i = 0; i < armorCount; i++)
            {
                Item item = player.armor[i];
                if (!item.IsAir)
                    item.TurnToAir();
            }
            foreach (Item item in player.miscEquips)
            {
                if (!item.IsAir)
                    item.TurnToAir();
            }
        }
        public static void ClearInventoryAndEquipments(Player player, bool clearVanities = false)
        {
            ClearInventory(player);
            ClearEquipments(player, clearVanities);
        }
        #endregion
    }
}
