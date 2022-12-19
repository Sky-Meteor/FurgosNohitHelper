using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

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
					foreach (List<string> list in CustomSettings)
						if (list[0] == args[1])
							throw new UsageException("命名重复：删除或更换命名");
					List<string> SavedCustom = new List<string>();
					SavedCustom.Add(args[1]);
					foreach (string toggle in ToggleLoader.LoadedToggles.Keys)
						if (player.GetToggleValue(toggle))
							SavedCustom.Add(toggle);
                    CustomSettings.Add(SavedCustom);
                    if (!Main.dedServ)
					{	
                        CustomSettingsPath.Put("CustomSettings", CustomSettings);
                        CustomSettingsPath.Save();
                    }
                    break;
				case "remove":
					break;
				case "clear":
					CustomSettings.Clear();
					break;
				case "all":
					if (CustomSettings.Count == 0)
					{
						Main.NewText("无保存预设");
						break;
					}
					foreach (List<string> list in CustomSettings)
					{
						Main.NewText($"{list[0]}：{list}");

                    }
					/*foreach (string key in AllKeys)
					{
						Main.NewText($"{key}：{CustomSettingsPath.Get<List<string>>(key, null)}");
					}*/
					break;
				case "load":
					break;
				default:
					throw new UsageException("参数0错误");
			}
		}

		public override void Load()
		{
			CustomSettings = new List<List<string>>();
            CustomSettingsPath = new Preferences(path);
			//AllKeys = CustomSettingsPath.GetAllKeys();

			CustomSettings = CustomSettingsPath.Get("CustomSettings", new List<List<string>>());
        }

		public override void Unload()
		{
			CustomSettings = null;
			CustomSettingsPath = null;
		}

		void GetNamesFromList(List<string> list)
		{

		} // WIP （先在这画个饼.jpg 

		public List<List<string>> CustomSettings;

		public static Preferences CustomSettingsPath;
		internal static string path = Path.Combine(Main.SavePath, "ModConfigs", "FurgosNohitHelper_CustomSettings.json");
		//internal static List<string> AllKeys;
	}
}