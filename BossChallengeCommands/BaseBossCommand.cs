using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.IO;
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
        public static Dictionary<int, Tuple<int, Tuple<int, string>>> SaveInventory(Player player)
        {
            Dictionary<int, Tuple<int, Tuple<int, string>>> retVal = new Dictionary<int, Tuple<int, Tuple<int, string>>>();
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                int type = item.type;
                string internalName = "TerrariaItem";
                if (type >= Main.maxItemTypes)
                    internalName = item.ModItem.Name;
                retVal.Add(i, new Tuple<int, Tuple<int, string>>(type, new Tuple<int, string>(item.stack, internalName)));
            }
            return retVal;
        }
        protected void SaveIfNotServ()
        {
            if (!Main.dedServ)
                BossSavePath.Save();
        }
        #endregion
        public virtual void OnLoad()
        {
            
        }
        public virtual void OnUnload()
        {
            
        }
        public sealed override void Load()
        {
            BossSavePath = new Preferences(path);
            BossSavePath.Load();
            OnLoad();
            SaveIfNotServ();
        }
        public sealed override void Unload()
        {
            SaveIfNotServ();
            OnUnload();
            BossSavePath = null;
        }

        public Preferences BossSavePath;

        internal static string path = Path.Combine(Main.SavePath, "ModConfigs", "FurgosNohitHelper_BossCustoms.json");
    }
}
