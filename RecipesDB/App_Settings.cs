using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace RecipesDB
{
    public class App_Settings
    {
        public int SettingId { get; set; }
        public string SettingKey { get; set; } = null!;
        public string SettingValue { get; set; } = null!;
        public string Description { get; set; } = null!;

        public App_Settings() { }

        public App_Settings(int settingid,
                            string settingkey,
                            string settingvalue,
                            string description)
        {
            SettingId = settingid;
            SettingKey = settingkey;
            SettingValue = settingvalue;
            Description = description;
        }
    }
}
