using FargowiltasSouls.Toggler;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using Terraria.Localization;

namespace FurgosNohitHelper
{
    public class FurgoCommand : ModCommand
    {
        public override string Command => "furgo";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Player player = caller.Player;
            if (args.Length < 1)
                throw new UsageException("缺少参数");

            switch (args[0])
            {
                #region toggle
                case "toggle":
                    switch (args[1])
                    {
                        case "save":
                            if (args.Length < 3)
                                throw new Exception("缺少参数：命名");
                            if (NameCollection.Contains(args[2]))
                                throw new UsageException("命名重复：删除或更换命名");
                            if (ToggleLoader.LoadedToggles.ContainsKey(args[2]))
                                throw new UsageException("命名请勿与魂石效果选项名重复：更换命名");
                            List<string> SavedCustom = new List<string>();
                            NameCollection.Add(args[2]);
                            SavedCustom.Add(args[2]);
                            foreach (string toggle in ToggleLoader.LoadedToggles.Keys)
                                if (player.GetToggleValue(toggle))
                                    SavedCustom.Add(toggle);
                            CustomSettings.Add(SavedCustom);
                            SaveIfNotServ();
                            break;
                        case "remove":
                            if (args.Length < 3)
                                throw new Exception("缺少参数：名称");
                            if (!NameCollection.Contains(args[2]))
                                throw new UsageException($"参数{args[2]}错误：不存在名称");
                            foreach (List<string> list in CustomSettings)
                            {
                                if (list[0] == args[2])
                                {
                                    CustomSettings.Remove(list);
                                    NameCollection.Remove(args[2]);
                                    SaveIfNotServ();
                                    break;
                                }
                            }
                            break;
                        case "clear":
                            CustomSettings.Clear();
                            NameCollection.Clear();
                            SaveIfNotServ();
                            break;
                        case "all":
                            if (CustomSettings.Count == 0)
                            {
                                Main.NewText("无保存预设");
                                break;
                            }
                            foreach (List<string> list in CustomSettings)
                            {
                                Main.NewText("\n");
                                Main.NewText(GetNamesFromList(list));
                            }
                            break;
                        case "names":
                            Main.NewText(GetNamesFromList(NameCollection));
                            break;
                        case "load":
                            if (args.Length < 3)
                                throw new Exception("缺少参数：名称");
                            if (!NameCollection.Contains(args[2]))
                                throw new UsageException($"参数{args[2]}错误：不存在名称");
                            foreach (List<string> list in CustomSettings)
                            {
                                if (list[0] == args[2])
                                {
                                    SetAllToggles(player, false);
                                    for (int i = 1; i < list.Count; i++)
                                        player.SetToggleValue(list[i], true);
                                    Main.NewText($"已加载预设：{GetNamesFromList(list)}");
                                    break;
                                }
                            }
                            break;
                        default:
                            throw new UsageException($"参数{args[1]}错误：不存在指令");
                    }
                    break;
                #endregion
                case "station":
                    break;
                default:
                    throw new UsageException($"参数{args[0]}错误：不存在指令");
            }
        }

        public override void Load()
        {
            TextReader file = File.OpenText(path);
            JsonReader reader = new JsonTextReader(file);
            JsonSerializer jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings() { Formatting = Formatting.Indented });
            var DeserializeVar = jsonSerializer.Deserialize<Dictionary<string, JArray>>(reader);

            CustomSettingsPath = new Preferences(path);

            NameCollection = new List<string>();
            CustomSettings = new List<List<string>>();

            foreach (var value in DeserializeVar["NameCollection"].Values())
            {
                string val = value.ToString();
                NameCollection.Add(val);
            }

            List<string> settingsList = new List<string>();
            foreach (var value in DeserializeVar["CustomSettings"].Values())
            {
                string val = value.ToString();
                
                if (NameCollection.Contains(val))
                {
                    if (settingsList.Count > 0)
                    {
                        CustomSettings.Add(settingsList.ToList());
                    }
                    settingsList.Clear();
                }
                settingsList.Add(val);
            }
            CustomSettings.Add(settingsList);

            CustomSettingsPath.Put("CustomSettings", CustomSettings);
            CustomSettingsPath.Put("NameCollection", NameCollection);
            SaveIfNotServ();
        }

        public override void Unload()
        {
            CustomSettings = null;
            NameCollection = null;
            CustomSettingsPath = null;
        }

        public List<List<string>> CustomSettings;
        public List<string> NameCollection;

        public Preferences CustomSettingsPath;
        internal static string path = Path.Combine(Main.SavePath, "ModConfigs", "FurgosNohitHelper_CustomSettings.json");

        #region Utils
        public static void SetAllToggles(Player player, bool set)
        {
            foreach (string key in ToggleLoader.LoadedToggles.Keys)
            {
                player.SetToggleValue(key, set);
            }
        }
        public static string GetNamesFromList(List<string> list)
        {
            if (list.Count == 0)
                return "";
            string output = $"{list[0]}：";
            int count = 0;
            for (int i = 1; i < list.Count; i++)
            {
                if (count >= ModContent.GetInstance<FNHConfig>().CommandTogglePerLine)
                {
                    output += "\n";
                    count = 0;
                }
                string text = LocalizationLoader.GetOrCreateTranslation(ModLoader.GetMod("FargowiltasSouls"), $"{list[i]}Config").GetTranslation(Language.ActiveCulture);
                string key = text[text.IndexOf("{")..(text.IndexOf("}") + 1)];
                text = text.Replace(key, Language.GetTextValue(key[2..(key.Length - 1)]));
                output += $"{text}，";
                count += 1;
            }
            return output[..(output.Length - 1)];
        }
        void SaveIfNotServ()
        {
            if (!Main.dedServ)
            {
                CustomSettingsPath.Save();
            }
        }
        #endregion
        #region EnchColors
        public readonly static Dictionary<string, string> EnchColors = new()
        {
            { "BorealConfig", "8B7464" },
            { "MahoganyConfig", "b56c64" },
            { "EbonConfig", "645a8d" },
            { "ShadeConfig", "586876" },
            { "ShadeOnHitConfig", "586876" },
            { "PalmConfig", "b78d56" },
            { "PearlConfig", "ad9a5f" },

            { "AdamantiteConfig", "dd557d" },
            { "CobaltConfig", "3da4c4" },
            { "AncientCobaltConfig", "354c74" },
            { "MythrilConfig", "9dd290" },
            { "OrichalcumConfig", "eb3291" },
            { "PalladiumConfig", "f5ac28" },
            { "PalladiumOrbConfig", "f5ac28" },
            { "TitaniumConfig", "828c88" },

            { "CopperConfig", "d56617" },
            { "IronMConfig", "988e83" },
            { "SilverSConfig", "b4b4cc" },
            { "TinConfig", "a28b4e" },
            { "TungstenConfig", "b0d2b2" },
            { "TungstenProjConfig", "b0d2b2" },
            { "ObsidianConfig", "453e73" },

            { "GladiatorConfig", "9c924e" },
            { "GoldConfig", "e7b21c" },
            { "GoldToPiggyConfig", "e7b21c" },
            { "HuntressConfig", "7ac04c" },
            { "RedRidingRainConfig", "c01b3c" },
            { "ValhallaConfig", "93651e" },
            { "SquirePanicConfig", "948f8c" },

            { "BeeConfig", "FEF625" },
            { "BeetleConfig", "6D5C85" },
            { "CactusConfig", "799e1d" },
            { "PumpkinConfig", "e3651c" },
            { "SpiderConfig", "6d4e45" },
            { "TurtleConfig", "f89c5c" },

            { "ChlorophyteConfig", "248900" },
            { "CrimsonConfig", "C8364B" },
            { "RainConfig", "ffec00" },
            { "RainInnerTubeConfig", "ffec00" },
            { "FrostConfig", "7abdb9" },
            { "JungleConfig", "71971f" },
            { "JungleDashConfig", "71971f" },
            { "MoltenConfig", "c12b2b" },
            { "MoltenEConfig", "c12b2b" },
            { "ShroomiteConfig", "008cf4" },
            { "ShroomiteShroomConfig", "008cf4" },

            { "DarkArtConfig", "9b5cb0" },
            { "ApprenticeConfig", "5d86a6" },
            { "NecroConfig", "565643" },
            { "NecroGloveConfig", "565643" },
            { "ShadowConfig", "42356f" },
            { "AncientShadowConfig", "42356f" },
            { "MonkConfig", "920520" },
            { "ShinobiDashConfig", "935b18" },
            { "ShinobiConfig", "935b18" },
            { "SpookyConfig", "644e74" },
            { "NinjaSpeedConfig", "565643" },
            { "CrystalDashConfig", "249dcf" },

            { "FossilConfig", "8c5c3b" },
            { "ForbiddenConfig", "e7b21c" },
            { "HallowDodgeConfig", "968564" },
            { "HallowedConfig", "968564" },
            { "HallowSConfig", "968564" },
            { "SpectreConfig", "accdfc" },
            { "TikiConfig", "56A52B" },

            { "MeteorConfig", "5f4752" },
            { "NebulaConfig", "fe7ee5" },
            { "SolarConfig", "fe9e23" },
            { "SolarFlareConfig", "fe9e23" },
            { "StardustConfig", "00aeee" },
            { "VortexSConfig", "00f2aa" },
            { "VortexVConfig", "00f2aa" }
        };
        #endregion
    }
}