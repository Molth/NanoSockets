﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NanoSockets
{
    public static class XxHash32
    {
        public static int Hash(ReadOnlySpan<byte> buffer, uint seed = 0)
        {
            uint num1 = 0;
            uint num2 = 0;
            uint num3 = 0;
            uint num4 = 0;
            uint num5 = 0;
            uint num6 = 0;
            uint num7 = 0;
            uint num8 = 0;
            ref var local1 = ref MemoryMarshal.GetReference(buffer);
#if NET6_0_OR_GREATER
            ref var local2 = ref Unsafe.Add(ref local1, buffer.Length);
#else
            ref var local2 = ref Unsafe.Add(ref local1, (nint)buffer.Length);
#endif
            if (buffer.Length >= 16)
            {
                num1 = (uint)((int)seed - 1640531535 - 2048144777);
                num2 = seed + 2246822519U;
                num3 = seed;
                num4 = seed - 2654435761U;
                const nint elementOffset1 = 16;
                for (ref var local3 = ref Unsafe.Subtract(ref local2, Unsafe.ByteOffset(ref local1, ref local2) % elementOffset1); Unsafe.IsAddressLessThan(ref local1, ref local3); local1 = ref Unsafe.Add(ref local1, elementOffset1))
                {
                    var num9 = num1 + Unsafe.ReadUnaligned<uint>(ref local1) * 2246822519U;
                    num1 = (uint)((((int)num9 << 13) | (int)(num9 >> 19)) * -1640531535);
                    var num10 = num2 + Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref local1, 4)) * 2246822519U;
                    num2 = (uint)((((int)num10 << 13) | (int)(num10 >> 19)) * -1640531535);
                    var num11 = num3 + Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref local1, 8)) * 2246822519U;
                    num3 = (uint)((((int)num11 << 13) | (int)(num11 >> 19)) * -1640531535);
                    var num12 = num4 + Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref local1, 12)) * 2246822519U;
                    num4 = (uint)((((int)num12 << 13) | (int)(num12 >> 19)) * -1640531535);
                    num8 += 4U;
                }
            }

            const nint elementOffset2 = 4;
            for (; Unsafe.ByteOffset(ref local1, ref local2) >= elementOffset2; local1 = ref Unsafe.Add(ref local1, elementOffset2))
            {
                var num13 = (uint)Unsafe.ReadUnaligned<int>(ref local1);
                var num14 = num8++;
                switch (num14 % 4U)
                {
                    case 0:
                        num5 = num13;
                        break;
                    case 1:
                        num6 = num13;
                        break;
                    case 2:
                        num7 = num13;
                        break;
                    default:
                        if (num14 == 3U)
                        {
                            num1 = (uint)((int)seed - 1640531535 - 2048144777);
                            num2 = seed + 2246822519U;
                            num3 = seed;
                            num4 = seed - 2654435761U;
                        }

                        var num15 = num1 + num5 * 2246822519U;
                        num1 = (uint)((((int)num15 << 13) | (int)(num15 >> 19)) * -1640531535);
                        var num16 = num2 + num6 * 2246822519U;
                        num2 = (uint)((((int)num16 << 13) | (int)(num16 >> 19)) * -1640531535);
                        var num17 = num3 + num7 * 2246822519U;
                        num3 = (uint)((((int)num17 << 13) | (int)(num17 >> 19)) * -1640531535);
                        var num18 = num4 + num13 * 2246822519U;
                        num4 = (uint)((((int)num18 << 13) | (int)(num18 >> 19)) * -1640531535);
                        break;
                }
            }

            for (; Unsafe.IsAddressLessThan(ref local1, ref local2); local1 = ref Unsafe.Add(ref local1, 1))
            {
                uint num19 = local1;
                var num20 = num8++;
                switch (num20 % 4U)
                {
                    case 0:
                        num5 = num19;
                        break;
                    case 1:
                        num6 = num19;
                        break;
                    case 2:
                        num7 = num19;
                        break;
                    default:
                        if (num20 == 3U)
                        {
                            num1 = (uint)((int)seed - 1640531535 - 2048144777);
                            num2 = seed + 2246822519U;
                            num3 = seed;
                            num4 = seed - 2654435761U;
                        }

                        var num21 = num1 + num5 * 2246822519U;
                        num1 = (uint)((((int)num21 << 13) | (int)(num21 >> 19)) * -1640531535);
                        var num22 = num2 + num6 * 2246822519U;
                        num2 = (uint)((((int)num22 << 13) | (int)(num22 >> 19)) * -1640531535);
                        var num23 = num3 + num7 * 2246822519U;
                        num3 = (uint)((((int)num23 << 13) | (int)(num23 >> 19)) * -1640531535);
                        var num24 = num4 + num19 * 2246822519U;
                        num4 = (uint)((((int)num24 << 13) | (int)(num24 >> 19)) * -1640531535);
                        break;
                }
            }

            var num25 = num8;
            var num26 = num25 % 4U;
            var num27 = (uint)((num25 < 4U ? (int)seed + 374761393 : (((int)num1 << 1) | (int)(num1 >> 31)) + (((int)num2 << 7) | (int)(num2 >> 25)) + (((int)num3 << 12) | (int)(num3 >> 20)) + (((int)num4 << 18) | (int)(num4 >> 14))) + (int)num25 * 4);
            if (num26 > 0U)
            {
                var num28 = num27 + num5 * 3266489917U;
                num27 = (uint)((((int)num28 << 17) | (int)(num28 >> 15)) * 668265263);
                if (num26 > 1U)
                {
                    var num29 = num27 + num6 * 3266489917U;
                    num27 = (uint)((((int)num29 << 17) | (int)(num29 >> 15)) * 668265263);
                    if (num26 > 2U)
                    {
                        var num30 = num27 + num7 * 3266489917U;
                        num27 = (uint)((((int)num30 << 17) | (int)(num30 >> 15)) * 668265263);
                    }
                }
            }

            var num31 = (int)num27;
#if NET7_0_OR_GREATER
            var num32 = (num31 ^ (num31 >>> 15)) * -2048144777;
            var num33 = (num32 ^ (num32 >>> 13)) * -1028477379;
            return num33 ^ (num33 >>> 16);
#else
            var num32 = (num31 ^ (int)((uint)num31 >> 15)) * -2048144777;
            var num33 = (num32 ^ (int)((uint)num32 >> 13)) * -1028477379;
            return num33 ^ (int)((uint)num33 >> 16);
#endif
        }
    }
}