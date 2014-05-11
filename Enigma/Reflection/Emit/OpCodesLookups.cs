using System.Collections.Generic;
using System.Reflection.Emit;

namespace Enigma.Reflection.Emit
{
    public static class OpCodesLookups
    {
        public static Dictionary<int, OpCode> LoadArg = new Dictionary<int, OpCode> {
            {0, OpCodes.Ldarg_0},
            {1, OpCodes.Ldarg_1},
            {2, OpCodes.Ldarg_2},
            {3, OpCodes.Ldarg_3},
        };

        public static Dictionary<int, OpCode> LoadInt32 = new Dictionary<int, OpCode> {
            {-1, OpCodes.Ldc_I4_M1},
            {0, OpCodes.Ldc_I4_0},
            {1, OpCodes.Ldc_I4_1},
            {2, OpCodes.Ldc_I4_2},
            {3, OpCodes.Ldc_I4_3},
            {4, OpCodes.Ldc_I4_4},
            {5, OpCodes.Ldc_I4_5},
            {6, OpCodes.Ldc_I4_6},
            {7, OpCodes.Ldc_I4_7},
            {8, OpCodes.Ldc_I4_8},
        };

        public static Dictionary<int, OpCode> SetLocal = new Dictionary<int, OpCode> {
            {0, OpCodes.Stloc_0},
            {1, OpCodes.Stloc_1},
            {2, OpCodes.Stloc_2},
            {3, OpCodes.Stloc_3},
        };

        public static Dictionary<int, OpCode> GetLocal = new Dictionary<int, OpCode> {
            {0, OpCodes.Ldloc_0},
            {1, OpCodes.Ldloc_1},
            {2, OpCodes.Ldloc_2},
            {3, OpCodes.Ldloc_3},
        };
    }
}