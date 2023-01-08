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
        public static Dictionary<int, Tuple<string, int>> SaveInventory(Player player)
        {
            Dictionary<int, Tuple<string, int>> retVal = new Dictionary<int, Tuple<string, int>>();
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                int type = item.type;
                string registerString = type.ToString();
                if (!IsVanillaItem(type))
                {
                    ModItem modItem = ModContent.GetModItem(type);
                    registerString = $"{modItem.Mod.Name}/{modItem.Name}";
                }
                retVal.Add(i, new Tuple<string, int>(registerString, item.stack));
            }
            return retVal;
        }
        public static void LoadInventory(Player player, Dictionary<int, Tuple<string, int>> savedInventory)
        {
            foreach (KeyValuePair<int, Tuple<string, int>> itemInfo in savedInventory)
            {
                int index = itemInfo.Key;
                string typeOrItemName = itemInfo.Value.Item1;
                int stack = itemInfo.Value.Item2;
                Item item = player.inventory[index];
                if (int.TryParse(typeOrItemName, out int type))
                {
                    item.type = type;
                    item.stack = stack;
                }
                else
                {
                    string[] modNamePair = typeOrItemName.Split("/");
                    string modName = modNamePair[0];
                    if (ModLoader.TryGetMod(modName, out Mod mod))
                    {
                        string itemName = modNamePair[1];
                        if (mod.TryFind(itemName, out ModItem modItem))
                        {
                            item.type = modItem.Type;
                            item.stack = stack;
                        }
                        else
                        {
                            Main.NewText($"找不到原位于物品栏第{index + 1}位的物品：{itemName}（所属Mod：{modName}），跳过加载此物品");
                            continue;
                        }
                    }
                    else
                    {
                        Main.NewText($"原位于物品栏第{index + 1}位的物品所属的Mod：{modName}未被加载，跳过加载此物品");
                        continue;
                    }
                }
            }
        }
        public static bool IsVanillaItem(int type) => type < Main.maxItemTypes;
        public static bool IsVanillaItem(Item item) => item.type < Main.maxItemTypes;
        protected void SaveIfNotServ()
        {
            if (!Main.dedServ)
                BossSavePath.Save();
        }
        protected void SaveIfNotServ(string name, object value)
        {
            BossSavePath.Put(name, value);
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

        protected Preferences BossSavePath;

        internal static string path = Path.Combine(Main.SavePath, "ModConfigs", "FurgosNohitHelper_BossCustoms.json");
    }
}
