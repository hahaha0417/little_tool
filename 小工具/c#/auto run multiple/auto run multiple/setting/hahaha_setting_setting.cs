using System.Text.Json.Serialization;

namespace hahaha
{
    public class hahaha_setting_system_class
    {

        public string Name { get; set; } = "";
        public List<hahaha_setting_system_item> Items { get; set; } = new List<hahaha_setting_system_item>();


        public hahaha_setting_system_class()
        {
        }

    }

    public class hahaha_setting_system_item
    {
        public bool Auto_Reload { get; set; } = false;
        public string Name { get; set; } = "";
        public string Command { get; set; } = "";
        public string Parameter { get; set; } = "";

        [JsonIgnore]
        public string Status { get; set; } = "";

        [JsonIgnore]
        public System.Diagnostics.Process Process = null!;

        [JsonIgnore]
        public bool Running = false;


        public hahaha_setting_system_item()
        {
        }

    }

    public class hahaha_setting_setting
    {
   
        public string Name_Class { get; set; } = "setting";
        public string Name_File { get; set; } = "setting.json";
        public List<hahaha_setting_system_class> Items { get; set; } = new List<hahaha_setting_system_class>();


        public hahaha_setting_setting()
        {
        }

        
    }
}