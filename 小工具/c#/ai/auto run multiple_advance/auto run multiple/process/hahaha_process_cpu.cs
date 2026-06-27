using System.Diagnostics;

namespace hahaha
{
    internal static class hahaha_process_cpu
    {
        public static string[] Priority_Options { get; } =
        {
            nameof(ProcessPriorityClass.Idle),
            nameof(ProcessPriorityClass.BelowNormal),
            nameof(ProcessPriorityClass.Normal),
            nameof(ProcessPriorityClass.AboveNormal),
            nameof(ProcessPriorityClass.High),
            nameof(ProcessPriorityClass.RealTime),
        };

        public static int Max_Affinity_Bits => Math.Min(Environment.ProcessorCount, IntPtr.Size * 8 - 1);

        public static long Build_Default_Affinity_Mask()
        {
            long mask = 0;
            for (int i = 0; i < Max_Affinity_Bits; i++)
            {
                mask |= 1L << i;
            }

            return mask;
        }

        public static long Normalize_Affinity_Mask(long mask)
        {
            long defaultMask = Build_Default_Affinity_Mask();
            if (mask <= 0)
            {
                return defaultMask;
            }

            return mask & defaultMask;
        }

        public static bool Try_Parse_Priority(string? value, out ProcessPriorityClass priority)
        {
            return Enum.TryParse(value, true, out priority);
        }

        public static int Apply_Process_Settings(hahaha_setting_system_item item)
        {
            if (item.Process == null)
            {
                return -1;
            }

            try
            {
                if (item.Process.HasExited)
                {
                    return -1;
                }

                item.Process.Refresh();

                if (Try_Parse_Priority(item.Cpu_Priority, out var priority))
                {
                    item.Process.PriorityClass = priority;
                }

                long affinityMask = Normalize_Affinity_Mask(item.Cpu_Affinity_Mask);
                item.Process.ProcessorAffinity = (IntPtr)affinityMask;

                return 0;
            }
            catch
            {
                return -1;
            }
        }
    }
}
