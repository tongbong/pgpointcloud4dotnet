using Ionic.Zlib;
using System;
using System.Collections.Generic;
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
                        throw new InvalidOperationException();
                    case DimensionalCompression.Deflate:
                        HandleDeflateCompressedDate(dimension, sizeOfCompressedData);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
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
