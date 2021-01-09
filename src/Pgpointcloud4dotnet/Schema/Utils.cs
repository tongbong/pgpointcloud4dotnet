

using System;
using System.Runtime.InteropServices;

namespace Pgpointcloud4dotnet.Schema
{
    internal static class Utils
    {

        static internal int GetDimensionSize(dimensionType dimension)
        {
            switch (dimension.interpretation)
            {
                case interpretationType.@float:
                    {
                        return 4;
                    }

                case interpretationType.@double:
                    {
                        return 8;
                    }

                case interpretationType.int8_t:
                    {
                        return 1;
                    }

                case interpretationType.int16_t:
                    {
                        return 2;
                    }

                case interpretationType.int32_t:
                    {
                        return 4;
                    }

                case interpretationType.int64_t:
                    {
                        return 8;
                    }

                case interpretationType.uint8_t:
                    {
                        return 1;
                    }

                case interpretationType.uint16_t:
                    {
                        return 2;
                    }

                case interpretationType.uint32_t:
                    {
                        return 4;
                    }

                case interpretationType.uint64_t:
                    {
                        return 8;
                    }

                default:
                    throw new InvalidOperationException("Type " + dimension.interpretation + " is not supported");
            }   
        }

        internal static T Read<T>(byte[] data, int pointIndex, int dimensionSize)
            where T : struct
        {
            Span<byte> dimensionValueAsBytes = new Span<byte>(data, pointIndex, dimensionSize);
            return MemoryMarshal.Read<T>(dimensionValueAsBytes);
        }
    }
}
