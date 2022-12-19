using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mono.Cecil.Mdb;
using System.Linq;

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
				case "save":
					if (args.Length < 2)
						throw new Exception("缺少参数1：命名");
					if (NameCollection.Contains(args[1]))
						throw new UsageException("命名重复：删除或更换命名");
					if (ToggleLoader.LoadedToggles.ContainsKey(args[1]))
                        throw new UsageException("命名请勿与魂石效果选项名重复：删除或更换命名");
                    List<string> SavedCustom = new List<string>();
					NameCollection.Add(args[1]);
					SavedCustom.Add(args[1]);
					foreach (string toggle in ToggleLoader.LoadedToggles.Keys)
						if (player.GetToggleValue(toggle))
							SavedCustom.Add(toggle);
                    CustomSettings.Add(SavedCustom);
                    if (!Main.dedServ)
					{	
                        CustomSettingsPath.Put("CustomSettings", CustomSettings);
                        CustomSettingsPath.Put("NameCollection", NameCollection);
                        CustomSettingsPath.Save();
                    }
                    break;
				case "remove":
					break;
				case "clear":
					CustomSettings.Clear();
					NameCollection.Clear();
					if (!Main.dedServ)
						CustomSettingsPath.Save();
					break;
				case "all":
                    if (CustomSettings.Count == 0)
					{
						Main.NewText("无保存预设");
						break;
					}
					foreach (List<string> list in CustomSettings)
					{
						Main.NewText($"{list[0]}：{list}"); // WIP
                    }
					break;
				case "names":
					Main.NewText(NameCollection); // WIP
					break;
				case "load":
					break;
				default:
					throw new UsageException("参数0错误");
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
                        foreach (List<string> list in CustomSettings)
                        {
                            foreach (string name in list)
                                Mod.Logger.Warn(name);
                        }
						Mod.Logger.Info("//");
                    }
                    settingsList.Clear();
				}
                settingsList.Add(val);
            }
            CustomSettings.Add(settingsList);

			Mod.Logger.Error(CustomSettings[0][0]);
			foreach (List<string> list in CustomSettings)
			{
				Mod.Logger.Error(list.Count);
                foreach (string name in list)
                    Mod.Logger.Error(name);
            }

            CustomSettingsPath.Put("CustomSettings", CustomSettings);
            CustomSettingsPath.Put("NameCollection", NameCollection);
            CustomSettingsPath.Save();
        }

		public override void Unload()
		{
			CustomSettings = null;
			NameCollection = null;
			CustomSettingsPath = null;
		}

		void GetNamesFromList(List<string> list)
		{

		} // WIP （先在这画个饼.jpg 

		public List<List<string>> CustomSettings;
		public List<string> NameCollection;

		public static Preferences CustomSettingsPath;
		internal static string path = Path.Combine(Main.SavePath, "ModConfigs", "FurgosNohitHelper_CustomSettings.json");
		//internal static List<string> AllKeys;
	}
}