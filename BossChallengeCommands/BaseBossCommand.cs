using Ionic.Zlib;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

        public sealed override string Usage => $"/{Cmd} save保存配置，/{Cmd} load加载配置";

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
        public static Dictionary<int, string> SaveEquipments(Player player)
        {
            Dictionary<int, string> retVal = new Dictionary<int, string>();
            int armorCount = player.armor.Length;
            int miscEquipCount = player.miscEquips.Length;
            for (int i = 0; i < armorCount + miscEquipCount; i++)
            {
                Item item = new Item();
                if (IsArmorOtherwiseMisc(i))
                    item = player.armor[RealIndexOfArmorOrMisc(i)];
                else
                    item = player.miscEquips[RealIndexOfArmorOrMisc(i)];
                int type = item.type;
                string registerString = type.ToString();
                if (!IsVanillaItem(type))
                {
                    ModItem modItem = ModContent.GetModItem(type);
                    registerString = $"{modItem.Mod.Name}/{modItem.Name}";
                }
                retVal.Add(i, registerString);
            }
            return retVal;

            bool IsArmorOtherwiseMisc(int index) => index < armorCount;
            int RealIndexOfArmorOrMisc(int index) => IsArmorOtherwiseMisc(index) ? index : index - armorCount;
        }
        public static void LoadInventory(Player player, Dictionary<int, Tuple<string, int>> savedInventory)
        {
            foreach (KeyValuePair<int, Tuple<string, int>> itemInfo in savedInventory)
            {
                int index = itemInfo.Key;
                string typeOrItemName = itemInfo.Value.Item1;
                int stack = itemInfo.Value.Item2;
                if (int.TryParse(typeOrItemName, out int type))
                {
                    if (type == 0)
                    {
                        player.inventory[index].TurnToAir();
                        continue;
                    }
                    player.inventory[index] = new Item(type, stack);
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
                            player.inventory[index] = new Item(modItem.Type, stack);
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
        public static void LoadEquipments(Player player, Dictionary<int, string> savedEquipments)
        {
            int armorCount = player.armor.Length;

            foreach (KeyValuePair<int, string> itemInfo in savedEquipments)
            {
                int index = itemInfo.Key;
                string typeOrItemName = itemInfo.Value;
                Item item = new Item();
                if (int.TryParse(typeOrItemName, out int type))
                {
                    if (type == 0)
                    {
                        if (IsArmorOtherwiseMisc(index))
                            player.armor[RealIndexOfArmorOrMisc(index)].TurnToAir();
                        else
                            player.miscEquips[RealIndexOfArmorOrMisc(index)].TurnToAir();
                        continue;
                    }
                    LoadArmorOrMisc(new Item(type), index);
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
                            LoadArmorOrMisc(new Item(modItem.Type), index);
                        }
                        else
                        {
                            Main.NewText($"找不到原位于{(IsArmorOtherwiseMisc(index) ? (IsArmorOtherwiseVanities(index) ? "装备" : "装饰") : "杂项类饰品")}栏第{RealIndexOfArmorOrMisc(index) + 1}位的物品：{itemName}（所属Mod：{modName}），跳过加载此物品");
                            continue;
                        }
                    }
                    else
                    {
                        Main.NewText($"原位于{(IsArmorOtherwiseMisc(index) ? (IsArmorOtherwiseVanities(index) ? "装备" : "装饰") : "杂项类饰品")}栏第{RealIndexOfArmorOrMisc(index) + 1}位的物品所属的Mod：{modName}未被加载，跳过加载此物品");
                        continue;
                    }
                }
            }

            bool IsArmorOtherwiseVanities(int index) => index < armorCount / 2;
            bool IsArmorOtherwiseMisc(int index) => index < armorCount;
            int RealIndexOfArmorOrMisc(int index) => IsArmorOtherwiseMisc(index) ? index : index - armorCount;
            void LoadArmorOrMisc(Item item, int index)
            {
                if (IsArmorOtherwiseMisc(index))
                    player.armor[RealIndexOfArmorOrMisc(index)] = item;
                else
                    player.miscEquips[RealIndexOfArmorOrMisc(index)] = item;
            }
        }
        public static bool IsVanillaItem(int type) => type < Main.maxItemTypes;
        public static bool IsVanillaItem(Item item) => item.type < Main.maxItemTypes;
        protected void EasyLoadInventoryAndEquipments(ref Dictionary<string, Dictionary<int, Tuple<string, int>>> SavedInventory, ref Dictionary<string, Dictionary<int, string>> SavedEquipments)
        {
            SavedInventory[Cmd] = BossSavePath.Get<Dictionary<int, Tuple<string, int>>>($"{Cmd}/inv", new());
            BossSavePath.Put($"{Cmd}/inv", SavedInventory[Cmd]);
            SavedEquipments[Cmd] = BossSavePath.Get<Dictionary<int, string>>($"{Cmd}/equip", new());
            BossSavePath.Put($"{Cmd}/equip", SavedEquipments[Cmd]);
        }
        protected void EasyRegisterAction(CommandCaller caller, string input, string[] args, ref Dictionary<string, Dictionary<int, Tuple<string, int>>> SavedInventory, ref Dictionary<string, Dictionary<int, string>> SavedEquipments)
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
                    SavedInventory[Cmd] = SaveInventory(player);
                    SaveIfNotServ($"{Cmd}/inv", SavedInventory[Cmd]);
                    SavedEquipments[Cmd] = SaveEquipments(player);
                    SaveIfNotServ($"{Cmd}/equip", SavedEquipments[Cmd]);
                    Main.NewText("保存成功");
                    break;
                case "load":
                    LoadInventory(player, SavedInventory[Cmd]);
                    LoadEquipments(player, SavedEquipments[Cmd]);
                    Main.NewText("加载成功");
                    break;
                default:
                    Main.NewText(Usage);
                    return;
            }
        }
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
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            EasyRegisterAction(caller, input, args, ref SavedInventory, ref SavedEquipments);
        }

        public virtual void OnLoad()
        {
            SavedInventory = new Dictionary<string, Dictionary<int, Tuple<string, int>>>();
            SavedEquipments = new Dictionary<string, Dictionary<int, string>>();
            EasyLoadInventoryAndEquipments(ref SavedInventory, ref SavedEquipments);
        }
        public virtual void OnUnload()
        {
            SavedInventory = null;
            SavedEquipments = null;
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

        Dictionary<string, Dictionary<int, Tuple<string, int>>> SavedInventory;
        Dictionary<string, Dictionary<int, string>> SavedEquipments;

        protected Preferences BossSavePath;

        internal static string path = Path.Combine(Main.SavePath, "ModConfigs", "FurgosNohitHelper_BossCustoms.json");
    }
}
