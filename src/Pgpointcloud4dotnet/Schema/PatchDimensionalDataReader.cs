using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pgpointcloud4dotnet.Schema
{
    internal class PatchDimensionalDataReader : PatchDataReader
    {
        private readonly PatchHeaderReader _headerReader;

        private int index = 0;

        public PatchDimensionalDataReader(PatchHeaderReader headerReader)
        {
            _headerReader = headerReader;
            this.index = _headerReader.index;
        }

        public Patch Patch => _headerReader.Patch;

        internal override void FillPatch()
        {
            for (int i = 0; i < Patch.NumberOfPoints; i++)
            {
                Patch.AddPoint(new Point());
            }
            List<dimensionType> dimensions = _headerReader.Schema.dimension
                                                .OrderBy(x => Convert.ToInt32(x.position))
                                                .ToList();

            foreach (var dimension in dimensions)
            {
                // Read compression type
                DimensionalCompression compressionType = (DimensionalCompression)Utils.Read<byte>(_headerReader.Wkb, index, 1);
                index += 1;

                // Read size of compressed data
                uint sizeOfCompressedData = Utils.Read<uint>(_headerReader.Wkb, index, 4);
                index += 4;

                switch (compressionType)
                {
                    case DimensionalCompression.NoCompression:
                        throw new InvalidOperationException();
                    case DimensionalCompression.RunLengthCompression:
                        HandleRunLengthCompressedData(dimension);
                        break;
                    case DimensionalCompression.SignificantBitsRemoval:
                        HandleSigbitsCompressedData(dimension);
                        break;
                    case DimensionalCompression.Deflate:
                        HandleDeflateCompressedDate(dimension, sizeOfCompressedData);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        //pc_bytes_sigbits_decode_8
        private void HandleSigbitsCompressedData(dimensionType dimension)
        {
            int dimensionSize = Utils.GetDimensionSize(dimension);

            switch (dimension.interpretation)
            {
                case interpretationType.@float:
                    HandleSigbitsCompressed32bitsData(dimension, dimensionSize);
                    break;
                case interpretationType.@double:
                    HandleSigbitsCompressed64bitsData(dimension, dimensionSize);
                    break;
                case interpretationType.int8_t:
                    HandleSigbitsCompressed8bitsData(dimension, dimensionSize);
                    break;
                case interpretationType.int16_t:
                    HandleSigbitsCompressed16bitsData(dimension, dimensionSize);
                    break;
                case interpretationType.int32_t:
                    HandleSigbitsCompressed32bitsData(dimension, dimensionSize);
                    break;
                case interpretationType.int64_t:
                    HandleSigbitsCompressed64bitsData(dimension, dimensionSize);
                    break;
                case interpretationType.uint8_t:
                    HandleSigbitsCompressed8bitsData(dimension, dimensionSize);
                    break;
                case interpretationType.uint16_t:
                    HandleSigbitsCompressed16bitsData(dimension, dimensionSize);
                    break;
                case interpretationType.uint32_t:
                    HandleSigbitsCompressed32bitsData(dimension, dimensionSize);
                    break;
                case interpretationType.uint64_t:
                    HandleSigbitsCompressed64bitsData(dimension, dimensionSize);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        static private object ConvertBytesToProperType(byte valueToConvert, dimensionType dimension)
        {
            byte[] valueAsBytes = BitConverter.GetBytes(valueToConvert);
            switch (dimension.interpretation)
            {
                case interpretationType.int8_t:
                    return (sbyte)valueAsBytes[0];

                case interpretationType.uint8_t:
                    return valueAsBytes[0];

                default:
                    throw new InvalidOperationException("You should not be here");
            }
        }

        static private object ConvertBytesToProperType(ushort valueToConvert, dimensionType dimension)
        {
            byte[] valueAsBytes = BitConverter.GetBytes(valueToConvert);
            switch (dimension.interpretation)
            {
                case interpretationType.int16_t:
                    return BitConverter.ToInt16(valueAsBytes);

                case interpretationType.uint16_t:
                    return BitConverter.ToUInt16(valueAsBytes);

                default:
                    throw new InvalidOperationException("You should not be here");
            }
        }

        static private object ConvertBytesToProperType(uint valueToConvert, dimensionType dimension)
        {
            byte[] valueAsBytes = BitConverter.GetBytes(valueToConvert);
            switch (dimension.interpretation)
            {
                case interpretationType.@float:
                    return BitConverter.ToSingle(valueAsBytes);

                case interpretationType.int32_t:
                    return BitConverter.ToInt32(valueAsBytes);

                case interpretationType.uint32_t:
                    return BitConverter.ToUInt32(valueAsBytes);

                default:
                    throw new InvalidOperationException("You should not be here");
            }
        }

        static private object ConvertBytesToProperType(ulong valueToConvert, dimensionType dimension)
        {
            byte[] valueAsBytes = BitConverter.GetBytes(valueToConvert);
            switch (dimension.interpretation)
            {
                case interpretationType.@double:
                    return BitConverter.ToDouble(valueAsBytes);

                case interpretationType.int64_t:
                    return BitConverter.ToInt64(valueAsBytes);

                case interpretationType.uint64_t:
                    return BitConverter.ToUInt64(valueAsBytes);

                default:
                    throw new InvalidOperationException("You should not be here");
            }
        }

        private void HandleSigbitsCompressed64bitsData(dimensionType dimension, int dimensionSize)
        {
            int bitwidth = 64;
            int bit = bitwidth;

            /* How many unique bits? */
            ulong numberOfVariableBitsInThisDimension = Utils.Read<ulong>(_headerReader.Wkb, index, dimensionSize);
            index += dimensionSize;
            /* What is the shared bit value? */
            ulong theBitsThatAreSharedByEveryWordInThisDimension = Utils.Read<ulong>(_headerReader.Wkb, index, dimensionSize);
            index += dimensionSize;
            /* Calculate mask */
            ulong mask = (0xFFFFFFFFFFFFFFFF >> (bit - (int)numberOfVariableBitsInThisDimension));

            for (int i = 0; i < Patch.Points.Count; i++)
            {
                int shift = bit - (int)numberOfVariableBitsInThisDimension;
                ulong val = Utils.Read<ulong>(_headerReader.Wkb, index, dimensionSize);
                if (shift >= 0)
                {
                    val >>= shift;
                    val &= mask;
                    val |= theBitsThatAreSharedByEveryWordInThisDimension;

                    Patch.Points[i][dimension.name] = ConvertBytesToProperType(val, dimension);
                    bit -= (int)numberOfVariableBitsInThisDimension;
                    if (bit <= 0)
                    {
                        index += dimensionSize;
                        bit = bitwidth;
                    }
                }
                else
                {
                    int s = Math.Abs(shift);
                    val <<= s;
                    val &= mask;
                    val |= theBitsThatAreSharedByEveryWordInThisDimension;
                    index += dimensionSize;
                    bit = bitwidth;
                    ulong val2 = Utils.Read<ulong>(_headerReader.Wkb, index, dimensionSize);
                    shift = bit - s;
                    val2 >>= shift;
                    val2 &= mask;
                    bit -= s;
                    val |= val2;
                    Patch.Points[i][dimension.name] = ConvertBytesToProperType(val, dimension);
                }
            }
            index += dimensionSize;
        }

        private void HandleSigbitsCompressed32bitsData(dimensionType dimension, int dimensionSize)
        {
            int bitwidth = 32;
            int bit = bitwidth;

            /* How many unique bits? */
            uint numberOfVariableBitsInThisDimension = Utils.Read<uint>(_headerReader.Wkb, index, dimensionSize);
            index += dimensionSize;
            /* What is the shared bit value? */
            uint theBitsThatAreSharedByEveryWordInThisDimension = Utils.Read<uint>(_headerReader.Wkb, index, dimensionSize);
            index += dimensionSize;
            /* Calculate mask */
            uint mask = (0xFFFFFFFF >> (bit - (int)numberOfVariableBitsInThisDimension));

            for (int i = 0; i < Patch.Points.Count; i++)
            {
                int shift = bit - (int)numberOfVariableBitsInThisDimension;
                uint val = Utils.Read<uint>(_headerReader.Wkb, index, dimensionSize);
                if (shift >= 0)
                {
                    val >>= shift;
                    val &= mask;
                    val |= theBitsThatAreSharedByEveryWordInThisDimension;

                    Patch.Points[i][dimension.name] = ConvertBytesToProperType(val, dimension);
                    bit -= (int)numberOfVariableBitsInThisDimension;
                    if (bit <= 0)
                    {
                        index += dimensionSize;
                        bit = bitwidth;
                    }
                }
                else
                {
                    int s = Math.Abs(shift);
                    val <<= s;
                    val &= mask;
                    val |= theBitsThatAreSharedByEveryWordInThisDimension;
                    index += dimensionSize;
                    bit = bitwidth;
                    uint val2 = Utils.Read<uint>(_headerReader.Wkb, index, dimensionSize);
                    shift = bit - s;
                    val2 >>= shift;
                    val2 &= mask;
                    bit -= s;
                    val |= val2;
                    Patch.Points[i][dimension.name] = ConvertBytesToProperType(val, dimension);
                }
            }
            index += dimensionSize;
        }

        private void HandleSigbitsCompressed16bitsData(dimensionType dimension, int dimensionSize)
        {
            int bitwidth = 16;
            int bit = 16;

            /* How many unique bits? */
            ushort numberOfVariableBitsInThisDimension = Utils.Read<ushort>(_headerReader.Wkb, index, dimensionSize);
            index += dimensionSize;
            /* What is the shared bit value? */
            ushort theBitsThatAreSharedByEveryWordInThisDimension = Utils.Read<ushort>(_headerReader.Wkb, index, dimensionSize);
            index += dimensionSize;
            /* Calculate mask */
            ushort mask = (ushort)(0xFFFF >> (bit - numberOfVariableBitsInThisDimension));

            for (int i = 0; i < Patch.Points.Count; i++)
            {
                int shift = bit - numberOfVariableBitsInThisDimension;
                ushort val = Utils.Read<ushort>(_headerReader.Wkb, index, dimensionSize);
                if (shift >= 0)
                {
                    val >>= shift;
                    val &= mask;
                    val |= theBitsThatAreSharedByEveryWordInThisDimension;
                    Patch.Points[i][dimension.name] = ConvertBytesToProperType(val, dimension);
                    bit -= numberOfVariableBitsInThisDimension;
                    if (bit <= 0)
                    {
                        index += dimensionSize;
                        bit = bitwidth;
                    }
                }
                else
                {
                    int s = Math.Abs(shift);
                    val <<= s;
                    val &= mask;
                    val |= theBitsThatAreSharedByEveryWordInThisDimension;
                    index += dimensionSize;
                    bit = bitwidth;
                    ushort val2 = Utils.Read<ushort>(_headerReader.Wkb, index, dimensionSize);
                    shift = bit - s;
                    val2 >>= shift;
                    val2 &= mask;
                    bit -= s;
                    val |= val2;
                    Patch.Points[i][dimension.name] = ConvertBytesToProperType(val, dimension);
                }
            }
            index += dimensionSize;
        }

        private void HandleSigbitsCompressed8bitsData(dimensionType dimension, int dimensionSize)
        {
            int bitwidth = 8;
            int bit = 8;

            /* How many unique bits? */
            byte numberOfVariableBitsInThisDimension = Utils.Read<byte>(_headerReader.Wkb, index, dimensionSize);
            index += dimensionSize;
            /* What is the shared bit value? */
            byte theBitsThatAreSharedByEveryWordInThisDimension = Utils.Read<byte>(_headerReader.Wkb, index, dimensionSize);
            index += dimensionSize;
            /* Calculate mask */
            byte mask = (byte)(0xFF >> (bit - numberOfVariableBitsInThisDimension));

            for (int i = 0; i < Patch.Points.Count; i++)
            {
                int shift = bit - numberOfVariableBitsInThisDimension;
                byte val = Utils.Read<byte>(_headerReader.Wkb, index, dimensionSize);
                if (shift >= 0)
                {
                    val >>= shift;
                    val &= mask;
                    val |= theBitsThatAreSharedByEveryWordInThisDimension;
                    Patch.Points[i][dimension.name] = ConvertBytesToProperType(val, dimension);
                    bit -= numberOfVariableBitsInThisDimension;
                    if (bit <= 0)
                    {
                        index += dimensionSize;
                        bit = bitwidth;
                    }
                }
                else
                {
                    int s = Math.Abs(shift);
                    val <<= s;
                    val &= mask;
                    val |= theBitsThatAreSharedByEveryWordInThisDimension;
                    index += dimensionSize;
                    bit = bitwidth;
                    byte val2 = Utils.Read<byte>(_headerReader.Wkb, index, dimensionSize);
                    shift = bit - s;
                    val2 >>= shift;
                    val2 &= mask;
                    bit -= s;
                    val |= val2;
                    Patch.Points[i][dimension.name] = ConvertBytesToProperType(val, dimension);
                }
            }
            index += dimensionSize;
        }

        private void HandleRunLengthCompressedData(dimensionType dimension)
        {
            byte wordRepeats = Utils.Read<byte>(_headerReader.Wkb, index, 1);
            index += 1;

            int dimensionSize = Utils.GetDimensionSize(dimension);
            object dimensionValue = ReadDimensionValue(dimension, _headerReader.Wkb, dimensionSize, index);
            index += dimensionSize;

            if (dimensionValue != null)
            {
                for (int i = 0; i < wordRepeats; i++)
                {
                    Patch.Points[i][dimension.name] = dimensionValue;
                }
            }
        }

        //private object UncompressRunLengthDimensionValue(dimensionType dimension)
        //{
        //    object dimensionValue;
        //    int dimensionSize = Utils.GetDimensionSize(dimension);
        //    switch (dimension.interpretation)
        //    {
        //        case interpretationType.@float:
        //            dimensionValue = Utils.Read<float>(_headerReader.Wkb, index, dimensionSize);
        //            break;

        //        case interpretationType.@double:
        //            dimensionValue = Utils.Read<double>(_headerReader.Wkb, index, dimensionSize);
        //            break;

        //        case interpretationType.int8_t:
        //            dimensionValue = Utils.Read<sbyte>(_headerReader.Wkb, index, dimensionSize);
        //            break;

        //        case interpretationType.int16_t:
        //            dimensionValue = Utils.Read<short>(_headerReader.Wkb, index, dimensionSize);
        //            break;

        //        case interpretationType.int32_t:
        //            dimensionValue = Utils.Read<int>(_headerReader.Wkb, index, dimensionSize);
        //            break;

        //        case interpretationType.int64_t:
        //            dimensionValue = Utils.Read<long>(_headerReader.Wkb, index, dimensionSize);
        //            break;

        //        case interpretationType.uint8_t:
        //            dimensionValue = Utils.Read<byte>(_headerReader.Wkb, index, dimensionSize);
        //            break;

        //        case interpretationType.uint16_t:
        //            dimensionValue = Utils.Read<ushort>(_headerReader.Wkb, index, dimensionSize);
        //            break;

        //        case interpretationType.uint32_t:
        //            dimensionValue = Utils.Read<uint>(_headerReader.Wkb, index, dimensionSize);
        //            break;

        //        case interpretationType.uint64_t:
        //            dimensionValue = Utils.Read<ulong>(_headerReader.Wkb, index, dimensionSize);
        //            break;

        //        default:
        //            throw new InvalidOperationException("Type " + dimension.interpretation + " is not supported");
        //    }

        //    return dimensionValue;
        //}

        private void HandleDeflateCompressedDate(dimensionType dimension, uint sizeOfCompressedData)
        {
            Span<byte> compressedDimensionData = new Span<byte>(_headerReader.Wkb, index, (int)sizeOfCompressedData);
            index += (int)sizeOfCompressedData;

            byte[] uncompressedData = ZlibStream.UncompressBuffer(compressedDimensionData.ToArray());
            int dimensionSize = Utils.GetDimensionSize(dimension);

            int dataIndex = 0;
            for (int pointIndex = 0; pointIndex < Patch.NumberOfPoints; pointIndex++)
            {
                object dimensionValue = ReadDimensionValue(dimension, uncompressedData, dimensionSize, dataIndex);
                Patch.Points[pointIndex][dimension.name] = dimensionValue;
                dataIndex += dimensionSize;
            }
        }

        private static object ReadDimensionValue(dimensionType dimension, byte[] binaryData, int dimensionSize, int dataIndex)
        {
            object dimensionValue;
            switch (dimension.interpretation)
            {
                case interpretationType.@float:
                    dimensionValue = Utils.Read<float>(binaryData, dataIndex, dimensionSize);
                    break;

                case interpretationType.@double:
                    dimensionValue = Utils.Read<double>(binaryData, dataIndex, dimensionSize);
                    break;

                case interpretationType.int8_t:
                    dimensionValue = Utils.Read<sbyte>(binaryData, dataIndex, dimensionSize);
                    break;

                case interpretationType.int16_t:
                    dimensionValue = Utils.Read<short>(binaryData, dataIndex, dimensionSize);
                    break;

                case interpretationType.int32_t:
                    dimensionValue = Utils.Read<int>(binaryData, dataIndex, dimensionSize);
                    break;

                case interpretationType.int64_t:
                    dimensionValue = Utils.Read<long>(binaryData, dataIndex, dimensionSize);
                    break;

                case interpretationType.uint8_t:
                    dimensionValue = Utils.Read<byte>(binaryData, dataIndex, dimensionSize);
                    break;

                case interpretationType.uint16_t:
                    dimensionValue = Utils.Read<ushort>(binaryData, dataIndex, dimensionSize);
                    break;

                case interpretationType.uint32_t:
                    dimensionValue = Utils.Read<uint>(binaryData, dataIndex, dimensionSize);
                    break;

                case interpretationType.uint64_t:
                    dimensionValue = Utils.Read<ulong>(binaryData, dataIndex, dimensionSize);
                    break;

                default:
                    throw new InvalidOperationException("Type " + dimension.interpretation + " is not supported");
            }

            return dimensionValue;
        }
    }
}
