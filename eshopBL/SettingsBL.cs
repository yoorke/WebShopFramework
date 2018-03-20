using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopDL;

namespace eshopBL
{
    public class SettingsBL
    {
        public Settings GetSettings()
        {
            return new SettingsDL().GetSettings();
        }

        public int SaveSettings(Settings settings)
        {
            return new SettingsDL().SaveSettings(settings);
        }
    }
}
