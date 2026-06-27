using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.Logging;

namespace hahaha
{
    public static partial class ha
    {
        // ---------------------------------------------------------------
        //
        // ---------------------------------------------------------------
        public static Form? Form_Main = null;

        public static ILogger? Log_Form_Main = null;
        public static hahaha_setting_box? Setting = null;
    }

    // ---------------------------------------------------------------
    //
    // ---------------------------------------------------------------

    public static partial class app
    {
        // ---------------------------------------------------------------
        //
        // ---------------------------------------------------------------
        public static string Name_Application = "base";
        public static string Name_Title = "°òÂ¦¬[ºc";
    }

    // ---------------------------------------------------------------
    //
    // ---------------------------------------------------------------

    public static partial class setting
    {
        // ---------------------------------------------------------------
        //
        // ---------------------------------------------------------------

    }

    // ---------------------------------------------------------------
    //
    // ---------------------------------------------------------------

    public static partial class layout
    {
        // ---------------------------------------------------------------
        //
        // ---------------------------------------------------------------

    }



    public static partial class hahaha
    {
        // ---------------------------------------------------------------
        //
        // ---------------------------------------------------------------
        public static form_auto_run_multiple? Form_Main_ = null;


        // ---------------------------------------------------------------
        //
        // ---------------------------------------------------------------
        public static hahahalib.hahaha_log Log_ = new hahahalib.hahaha_log();
        public static hahahalib.hahaha_json Json_ = new hahahalib.hahaha_json();


        // ---------------------------------------------------------------
        //
        // ---------------------------------------------------------------
        public static hahaha_setting_box? Setting_Box_ = null;
        // ---------------------------------------------------------------
        //
        // ---------------------------------------------------------------

        // ---------------------------------------------------------------
        //
        // ---------------------------------------------------------------





        // ---------------------------------------------------------------
        // ¥D­n
        // ---------------------------------------------------------------
        public static int Initial_Environment()
        {
            Setting_Box_ = new hahaha_setting_box();

            if (Setting_Box_.Load_All() != 0)
            {
                Setting_Box_.Save_All();
            }

            ha.Setting = Setting_Box_;

  

            

            return 0;

        }


        


        public static int Initial()
        {
            return 0;
        }

        public static int Initial_UI()
        {
            return 0;
        }

        public static int Close()
        {
            hahaha.Log_.Close();

            return 0;
        }

        

    }    

}