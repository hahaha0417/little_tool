using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace hahaha
{
    internal static class CpuTopologyDetector
    {
        public static string[] GetLogicalCpuLabels()
        {
            int logicalCpuCount = Environment.ProcessorCount;
            string[] labels = new string[logicalCpuCount];

            for (int i = 0; i < logicalCpuCount; i++)
            {
                labels[i] = $"CPU {i}";
            }

            try
            {
                Dictionary<int, string> coreTypes = ReadCoreTypes();
                for (int i = 0; i < logicalCpuCount; i++)
                {
                    if (coreTypes.TryGetValue(i, out string? coreType) &&
                        (coreType == "P Core" || coreType == "E Core"))
                    {
                        labels[i] = $"CPU {i} ({coreType})";
                    }
                }
            }
            catch
            {
                // Fall back to plain CPU numbering when topology details are unavailable.
            }

            return labels;
        }

        private static Dictionary<int, string> ReadCoreTypes()
        {
            int length = 0;
            GetLogicalProcessorInformationEx(LOGICAL_PROCESSOR_RELATIONSHIP.RelationProcessorCore, IntPtr.Zero, ref length);

            int error = Marshal.GetLastWin32Error();
            if (length <= 0 || error != ERROR_INSUFFICIENT_BUFFER)
            {
                throw new Win32Exception(error);
            }

            IntPtr buffer = Marshal.AllocHGlobal(length);
            try
            {
                if (!GetLogicalProcessorInformationEx(LOGICAL_PROCESSOR_RELATIONSHIP.RelationProcessorCore, buffer, ref length))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                var logicalCpuEfficiency = new Dictionary<int, byte>();
                IntPtr current = buffer;
                IntPtr end = IntPtr.Add(buffer, length);

                while (current.ToInt64() < end.ToInt64())
                {
                    SYSTEM_LOGICAL_PROCESSOR_INFORMATION_EX header =
                        Marshal.PtrToStructure<SYSTEM_LOGICAL_PROCESSOR_INFORMATION_EX>(current);

                    if (header.Relationship == LOGICAL_PROCESSOR_RELATIONSHIP.RelationProcessorCore)
                    {
                        PROCESSOR_RELATIONSHIP processor =
                            Marshal.PtrToStructure<PROCESSOR_RELATIONSHIP>(IntPtr.Add(current, PROCESSOR_RELATIONSHIP_OFFSET));
                        int groupOffset = PROCESSOR_RELATIONSHIP_OFFSET + PROCESSOR_RELATIONSHIP_GROUPMASK_OFFSET;

                        for (int i = 0; i < processor.GroupCount; i++)
                        {
                            GROUP_AFFINITY affinity =
                                Marshal.PtrToStructure<GROUP_AFFINITY>(IntPtr.Add(current, groupOffset + (i * Marshal.SizeOf<GROUP_AFFINITY>())));

                            if (affinity.Group != 0)
                            {
                                continue;
                            }

                            ulong mask = affinity.Mask.ToUInt64();
                            for (int bit = 0; bit < 64; bit++)
                            {
                                if ((mask & (1UL << bit)) != 0)
                                {
                                    logicalCpuEfficiency[bit] = processor.EfficiencyClass;
                                }
                            }
                        }
                    }

                    current = IntPtr.Add(current, header.Size);
                }

                return ConvertEfficiencyClassesToLabels(logicalCpuEfficiency);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private static Dictionary<int, string> ConvertEfficiencyClassesToLabels(Dictionary<int, byte> logicalCpuEfficiency)
        {
            var result = new Dictionary<int, string>();
            if (logicalCpuEfficiency.Count == 0)
            {
                return result;
            }

            var distinctClasses = new HashSet<byte>(logicalCpuEfficiency.Values);
            if (distinctClasses.Count <= 1)
            {
                return result;
            }

            byte minClass = byte.MaxValue;
            byte maxClass = byte.MinValue;

            foreach (byte efficiencyClass in distinctClasses)
            {
                if (efficiencyClass < minClass)
                {
                    minClass = efficiencyClass;
                }

                if (efficiencyClass > maxClass)
                {
                    maxClass = efficiencyClass;
                }
            }

            if (minClass == maxClass)
            {
                return result;
            }

            foreach ((int logicalCpu, byte efficiencyClass) in logicalCpuEfficiency)
            {
                if (efficiencyClass == maxClass)
                {
                    result[logicalCpu] = "P Core";
                }
                else if (efficiencyClass == minClass)
                {
                    result[logicalCpu] = "E Core";
                }
            }

            return result;
        }

        private const int ERROR_INSUFFICIENT_BUFFER = 122;
        private static readonly int PROCESSOR_RELATIONSHIP_OFFSET =
            Marshal.OffsetOf<SYSTEM_LOGICAL_PROCESSOR_INFORMATION_EX_PROCESSOR>("Processor").ToInt32();
        private static readonly int PROCESSOR_RELATIONSHIP_GROUPMASK_OFFSET =
            Marshal.OffsetOf<PROCESSOR_RELATIONSHIP>("GroupMask").ToInt32();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetLogicalProcessorInformationEx(
            LOGICAL_PROCESSOR_RELATIONSHIP relationshipType,
            IntPtr buffer,
            ref int returnedLength);

        private enum LOGICAL_PROCESSOR_RELATIONSHIP
        {
            RelationProcessorCore = 0
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_LOGICAL_PROCESSOR_INFORMATION_EX
        {
            public LOGICAL_PROCESSOR_RELATIONSHIP Relationship;
            public int Size;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_LOGICAL_PROCESSOR_INFORMATION_EX_PROCESSOR
        {
            public LOGICAL_PROCESSOR_RELATIONSHIP Relationship;
            public int Size;
            public PROCESSOR_RELATIONSHIP Processor;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESSOR_RELATIONSHIP
        {
            public byte Flags;
            public byte EfficiencyClass;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] Reserved;

            public ushort GroupCount;
            public GROUP_AFFINITY GroupMask;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct GROUP_AFFINITY
        {
            public UIntPtr Mask;
            public ushort Group;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public ushort[] Reserved;
        }
    }
}
