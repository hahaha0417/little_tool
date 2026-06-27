namespace hahaha
{
    public class hahaha_setting_box
    {
        // -------------------------------------------------
        // system
        // -------------------------------------------------
        public hahaha_setting_system System = new hahaha_setting_system();
        // -------------------------------------------------
        // global
        // -------------------------------------------------
        public hahaha_setting_global Global = new hahaha_setting_global();
        // -------------------------------------------------
        // setting
        // -------------------------------------------------
        public hahaha_setting_setting Setting = new hahaha_setting_setting();
        // -------------------------------------------------
        //
        // -------------------------------------------------
        public hahaha_setting_box()
        {
        }

        public int Load_All()
        {
            bool result = true;
            // -------------------------------------------------
            // system
            // -------------------------------------------------
            result &= Load_System() == 0;
            // -------------------------------------------------
            // global
            // -------------------------------------------------
            result &= Load_Global() == 0;
            // -------------------------------------------------
            // setting
            // -------------------------------------------------
            result &= Load_Setting() == 0;
            // -------------------------------------------------
            // 
            // -------------------------------------------------

            return result ? 0 : -1;

        }

        public int Save_All()
        {
            // -------------------------------------------------
            // system
            // -------------------------------------------------
            Save_System();
            // -------------------------------------------------
            // global
            // -------------------------------------------------
            Save_Global();
            // -------------------------------------------------
            // setting
            // -------------------------------------------------
            Save_Setting();
            // -------------------------------------------------
            // 
            // -------------------------------------------------

            return 0;

        }

        public int Load_System()
        {
            int result = hahaha.Json_.Load($"{System.Name_Setting}/{System.Name_Class}/{System.Name_File}", ref System);

            return result;

        }

        public int Save_System()
        {
            hahaha.Json_.Save($"{System.Name_Setting}/{System.Name_Class}/{System.Name_File}", System);

            return 0;

        }

        public int Load_Global()
        {
            int result = hahaha.Json_.Load($"{System.Name_Setting}/{Global.Name_Class}/{Global.Name_File}", ref Global);

            return result;

        }

        public int Save_Global()
        {
            hahaha.Json_.Save($"{System.Name_Setting}/{Global.Name_Class}/{Global.Name_File}", Global);

            return 0;

        }

        public int Load_Setting()
        {
            int result = hahaha.Json_.Load($"{System.Name_Setting}/{Setting.Name_Class}/{Setting.Name_File}", ref Setting);

            return result;

        }

        public int Save_Setting()
        {
            hahaha.Json_.Save($"{System.Name_Setting}/{Setting.Name_Class}/{Setting.Name_File}", Setting);

            return 0;

        }
    }
}