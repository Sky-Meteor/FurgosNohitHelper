using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace FurgosNohitHelper
{
    public class FNHConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("指令相关选项")]
        [Label("加载魂石选项提示每行显示条数")]
        [DefaultValue(9)]
        public int CommandTogglePerLine;
    }
}
